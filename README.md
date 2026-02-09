# AG-UI Demo Workspace

A demo application showcasing the **AG-UI (Agent-User Interface) protocol** with **CopilotKit** — featuring a .NET 9 backend and Next.js frontend with 4 AI agent scenarios powered by Azure OpenAI.

This is **NOT** 4 separate servers. It is a single .NET backend exposing 4 AG-UI endpoints, consumed by a single Next.js frontend through CopilotKit.

---

## Architecture

```
                        User (Browser)
                             |
                             v
┌─────────────────────────────────────────────────────────┐
│  Next.js Frontend (port 3000)                           │
│                                                         │
│  ┌───────────────────────────────────────────────────┐  │
│  │ React Pages                                       │  │
│  │  /time-tracker    → <CopilotKit agent="time_tracker">│
│  │  /calendar        → <CopilotKit agent="calendar">   │
│  │  /knowledge-base  → <CopilotKit agent="knowledge_base">│
│  │  /security-issues → <CopilotKit agent="security_issues">│
│  │                                                   │  │
│  │  Each page wraps <CopilotChat> for the chat UI    │  │
│  └───────────────────────┬───────────────────────────┘  │
│                          │                              │
│  ┌───────────────────────▼───────────────────────────┐  │
│  │ API Route: /api/copilotkit                        │  │
│  │  CopilotRuntime + ExperimentalEmptyAdapter        │  │
│  │  Routes to 4 HttpAgent instances (from @ag-ui/client)│
│  └───────────────────────┬───────────────────────────┘  │
└──────────────────────────┼──────────────────────────────┘
                           │
                  HTTP POST + SSE Response
                  (AG-UI Protocol over SSE)
                           │
┌──────────────────────────▼──────────────────────────────┐
│  .NET 9 Backend (port 5018)                             │
│                                                         │
│  ┌───────────────────────────────────────────────────┐  │
│  │ ASP.NET Core Pipeline                             │  │
│  │  builder.Services.AddAGUI()                       │  │
│  │  app.MapAGUI("/time_tracker",    agent)            │  │
│  │  app.MapAGUI("/calendar",        agent)            │  │
│  │  app.MapAGUI("/knowledge_base",  agent)            │  │
│  │  app.MapAGUI("/security_issues", agent)            │  │
│  └───────────────────────┬───────────────────────────┘  │
│                          │                              │
│  ┌───────────────────────▼───────────────────────────┐  │
│  │ ChatClientAgent (per endpoint)                    │  │
│  │  - System prompt (Instructions)                   │  │
│  │  - Domain tools (AIFunctionFactory.Create)        │  │
│  │  - Source-gen JSON (McpAguiSerializerContext)      │  │
│  └───────────────────────┬───────────────────────────┘  │
│                          │                              │
│  ┌───────────────────────▼───────────────────────────┐  │
│  │ Azure OpenAI (GPT-4.1-mini)                       │  │
│  │  - Receives user message + tool definitions       │  │
│  │  - Returns tool calls or text completions         │  │
│  └───────────────────────────────────────────────────┘  │
│                                                         │
│  ┌───────────────────────────────────────────────────┐  │
│  │ In-Memory Data Stores (Singletons)                │  │
│  │  TimeEntryStore   → 30 days of seed entries       │  │
│  │  CalendarStore    → Recurring + scattered events  │  │
│  │  KnowledgeArticleStore → 21 seeded articles       │  │
│  │  SecurityIssueStore    → 24 seeded issues         │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

---

## How the AG-UI Protocol Works

AG-UI (Agent-User Interface) is an **event-driven protocol** that standardizes how AI agents communicate with frontends. It uses **Server-Sent Events (SSE)** over HTTP as the transport layer.

### Request-Response Flow

1. **User sends a message** in the CopilotChat UI
2. **CopilotKit** packages the message and sends a POST to `/api/copilotkit`
3. The **CopilotRuntime** routes to the correct `HttpAgent` based on the `agent` name
4. The `HttpAgent` sends an HTTP POST to the .NET backend endpoint (e.g., `http://localhost:5018/time_tracker`)
5. The **MapAGUI middleware** receives the request and invokes the `ChatClientAgent`
6. The agent sends the user message + tool definitions to **Azure OpenAI**
7. Azure OpenAI responds with either a **tool call** or a **text completion**
8. The AG-UI middleware converts the response into a **stream of SSE events**
9. Events flow back through the pipeline to the **CopilotChat UI**, which renders them in real-time

### SSE Event Stream

Every agent response is a stream of typed JSON events, each prefixed with `data:` and separated by double newlines. A typical interaction looks like this:

```
data: {"type":"RUN_STARTED","threadId":"","runId":""}

data: {"type":"TOOL_CALL_START","toolCallId":"call_abc","toolCallName":"get_security_summary"}
data: {"type":"TOOL_CALL_ARGS","toolCallId":"call_abc","delta":"{}"}
data: {"type":"TOOL_CALL_END","toolCallId":"call_abc"}
data: {"type":"TOOL_CALL_RESULT","toolCallId":"call_abc","content":"...JSON data...","role":"tool"}

data: {"type":"TEXT_MESSAGE_START","messageId":"msg_1","role":"assistant"}
data: {"type":"TEXT_MESSAGE_CONTENT","messageId":"msg_1","delta":"Here"}
data: {"type":"TEXT_MESSAGE_CONTENT","messageId":"msg_1","delta":" is"}
data: {"type":"TEXT_MESSAGE_CONTENT","messageId":"msg_1","delta":" your"}
data: {"type":"TEXT_MESSAGE_CONTENT","messageId":"msg_1","delta":" summary"}
data: {"type":"TEXT_MESSAGE_END","messageId":"msg_1"}

data: {"type":"RUN_FINISHED","threadId":"","runId":""}
```

### Event Types Used in This Demo

| Category | Events | Purpose |
|----------|--------|---------|
| **Lifecycle** | `RUN_STARTED`, `RUN_FINISHED` | Bracket the beginning and end of an agent run |
| **Tool Calls** | `TOOL_CALL_START`, `TOOL_CALL_ARGS`, `TOOL_CALL_END`, `TOOL_CALL_RESULT` | LLM invokes a domain tool, receives the result, and uses it to formulate a response |
| **Text Messages** | `TEXT_MESSAGE_START`, `TEXT_MESSAGE_CONTENT`, `TEXT_MESSAGE_END` | Stream text tokens from the LLM to the UI in real-time |

### Event Protocol Rules

Events must follow a strict sequencing protocol:

```
RUN_STARTED
  -> TOOL_CALL_START -> TOOL_CALL_ARGS* -> TOOL_CALL_END -> TOOL_CALL_RESULT
  -> TEXT_MESSAGE_START -> TEXT_MESSAGE_CONTENT* -> TEXT_MESSAGE_END
RUN_FINISHED
```

The AG-UI client SDK validates this sequence. Invalid orderings (e.g., `TEXT_MESSAGE_CONTENT` without a preceding `TEXT_MESSAGE_START`) are caught and reported.

### Why SSE Over WebSockets

| Aspect | SSE | WebSockets |
|--------|-----|------------|
| Direction | Server to Client (unidirectional) | Bidirectional |
| Protocol | Standard HTTP/1.1 | Upgrade handshake |
| Reconnection | Built-in auto-reconnect | Manual |
| Load balancers | Works out of the box | Requires sticky sessions |
| Complexity | Simple text protocol | Binary framing |

SSE is a natural fit for the agent-user pattern: the user sends a single request (POST), and the agent streams back a response as a series of events.

---

## How the Backend Works

### Key Technologies

| Package | Purpose |
|---------|---------|
| `Microsoft.Agents.AI.Hosting.AGUI.AspNetCore` | AG-UI middleware — `AddAGUI()` and `MapAGUI()` |
| `Microsoft.Agents.AI` | `ChatClientAgent` and `ChatClientAgentOptions` |
| `Microsoft.Extensions.AI` | `AIFunctionFactory.Create()` for tool registration |
| `Azure.AI.OpenAI` | Azure OpenAI client |
| `DotNetEnv` | Load `.env` file for credentials |

### Agent Creation Pattern

Each agent is created via `AgentFactory` using this pattern:

```csharp
// 1. Get the domain store (singleton with seed data)
var store = sp.GetRequiredService<TimeEntryStore>();

// 2. Get the Azure OpenAI chat client
var chatClient = azureOpenAIClient.GetChatClient(deploymentName);

// 3. Create domain tools from store methods
var tools = new List<AITool>
{
    AIFunctionFactory.Create(store.LogTime, "log_time",
        "Log time against a project for a given date.",
        McpAguiSerializerContext.Default.Options),
    // ... more tools
};

// 4. Create the agent with system prompt + tools
return chatClient.CreateAIAgent(new ChatClientAgentOptions
{
    Instructions = "You are a time tracking assistant...",
    ChatOptions = new ChatOptions { Tools = tools }
});
```

### Source-Generated JSON Serialization

All DTOs used in tool parameters and return values must be registered in `McpAguiSerializerContext` for .NET's source-generated JSON serializer. This includes primitive types used as tool parameters:

```csharp
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(int?))]      // For optional tool parameters
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTime?))]
[JsonSerializable(typeof(TimeEntry))]
[JsonSerializable(typeof(List<TimeEntry>))]
// ... all DTOs, enums, and List<T> variants
internal sealed partial class McpAguiSerializerContext : JsonSerializerContext;
```

### Endpoint Mapping

Each agent is mapped to an HTTP endpoint using `MapAGUI()`:

```csharp
app.MapAGUI("/time_tracker",    AgentFactory.CreateTimeTracker(serviceProvider));
app.MapAGUI("/calendar",        AgentFactory.CreateCalendar(serviceProvider));
app.MapAGUI("/knowledge_base",  AgentFactory.CreateKnowledgeBase(serviceProvider));
app.MapAGUI("/security_issues", AgentFactory.CreateSecurityIssues(serviceProvider));
```

When a POST request arrives at any of these endpoints, the AG-UI middleware:
1. Deserializes the incoming messages
2. Passes them to the `ChatClientAgent`
3. The agent invokes Azure OpenAI with the messages + tool definitions
4. If the LLM returns a tool call, the agent executes the tool and sends the result back to the LLM
5. The LLM's final text response is streamed as SSE events back to the caller

### Tool Methods

All tool methods return `string` (JSON-serialized data). The LLM interprets the raw JSON and formats it for the user. Each domain has 5 tools:

| Agent | Tools |
|-------|-------|
| **Time Tracker** | `log_time`, `get_time_entries`, `get_project_time_summary`, `get_time_for_project`, `get_daily_breakdown` |
| **Calendar** | `get_schedule_for_day`, `get_schedule_for_week`, `get_schedule_for_month`, `get_upcoming_deadlines`, `get_free_slots` |
| **Knowledge Base** | `search_knowledge_articles`, `get_article`, `list_articles_by_project`, `list_articles_by_category`, `get_popular_articles` |
| **Security Issues** | `get_security_issues`, `get_security_issue_detail`, `get_security_summary`, `get_issues_by_project`, `get_critical_and_high_issues` |

---

## How the Frontend Works

### Key Technologies

| Package | Purpose |
|---------|---------|
| `@copilotkit/react-core` | `CopilotKit` provider component |
| `@copilotkit/react-ui` | `CopilotChat` component with markdown rendering |
| `@copilotkit/runtime` | `CopilotRuntime` for the API route |
| `@ag-ui/client` | `HttpAgent` — AG-UI client that speaks SSE to the backend |
| `@ag-ui/core` | AG-UI event types and protocol definitions |

### Frontend-to-Backend Bridge

The API route (`/api/copilotkit`) acts as a bridge between CopilotKit and the .NET backend:

```typescript
// Create HttpAgent instances pointing to each backend endpoint
const agents = {
  time_tracker:    new HttpAgent({ url: "http://localhost:5018/time_tracker" }),
  calendar:        new HttpAgent({ url: "http://localhost:5018/calendar" }),
  knowledge_base:  new HttpAgent({ url: "http://localhost:5018/knowledge_base" }),
  security_issues: new HttpAgent({ url: "http://localhost:5018/security_issues" }),
};

// CopilotRuntime routes requests to the correct agent
const runtime = new CopilotRuntime({ agents });
```

### Agent Page Pattern

Each agent page follows the same pattern using a shared `ChatLayout` component:

```tsx
<CopilotKit runtimeUrl="/api/copilotkit" agent="time_tracker">
  <CopilotChat
    labels={{ initial: "Hi! I'm your time tracking assistant...", title: "⏰ Time Tracker" }}
    suggestions={[
      { title: "📊 Show summary", message: "Show me a summary of my time..." },
    ]}
  />
</CopilotKit>
```

The `agent` prop on `CopilotKit` tells the runtime which `HttpAgent` to use, which in turn determines which backend endpoint receives the request.

### How CopilotChat Renders AG-UI Events

CopilotChat processes the SSE event stream from the backend and renders it in real-time:

| SSE Event | CopilotChat Behavior |
|-----------|---------------------|
| `RUN_STARTED` | Shows loading indicator |
| `TOOL_CALL_START` | Indicates the agent is working (activity dots) |
| `TOOL_CALL_RESULT` | Tool result stored internally (not shown to user) |
| `TEXT_MESSAGE_START` | Creates a new assistant message bubble |
| `TEXT_MESSAGE_CONTENT` | Appends streamed text tokens with markdown rendering |
| `TEXT_MESSAGE_END` | Finalizes the message |
| `RUN_FINISHED` | Hides loading indicator |

---

## Agent Scenarios

### 1. Time Tracker (`/time-tracker`)
Log and track working hours across 4 projects with an 8-hour daily cap. Query time entries, view project summaries, and analyze daily breakdowns. Seed data includes 30 working days of entries with 3-5 entries per day.

### 2. Calendar (`/calendar`)
Read-only calendar assistant with schedule queries by day, week, or month. Find free time slots and view upcoming deadlines. Seed data includes recurring events (daily standup, lunch, Friday retro) and scattered meetings, focus blocks, and deadlines spanning 1 month before and after today.

### 3. Knowledge Base (`/knowledge-base`)
ServiceNow-style knowledge article search across projects. Search by keyword, browse by project or category, and view popular articles. Seed data includes 21 articles (5-6 per project) across categories like How-To, Troubleshooting, Best Practice, Architecture, and Deployment.

### 4. Security Issues (`/security-issues`)
Security findings dashboard across projects. View issues by severity/status, get security posture summaries with risk scores, and prioritize remediation. Seed data includes 24 issues (6 per project) with severities from Critical to Low.

---

## Shared Seed Data (4 Projects)

| Code | Name | Description |
|------|------|-------------|
| ALPHA | Project Alpha | Internal CRM modernization platform |
| NEXUS | Project Nexus | API gateway and microservices migration |
| ORBIT | Project Orbit | Customer-facing mobile app redesign |
| VAULT | Project Vault | Data warehouse and analytics pipeline |

All data is in-memory and resets on backend restart.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend Runtime | .NET 9 |
| AG-UI Server SDK | Microsoft.Agents.AI.Hosting.AGUI.AspNetCore |
| Agent Framework | Microsoft.Agents.AI (ChatClientAgent) |
| Tool Creation | Microsoft.Extensions.AI (AIFunctionFactory) |
| LLM | Azure OpenAI (GPT-4.1-mini via Azure.AI.OpenAI) |
| Frontend | Next.js 15 + React 19 + TypeScript |
| AI UI | CopilotKit (@copilotkit/react-core, react-ui, runtime) |
| AG-UI Client | @ag-ui/client (HttpAgent), @ag-ui/core |
| Styling | Tailwind CSS 4 with custom CopilotKit overrides |

---

## Project Structure

```
app/
├── .env                                # Azure OpenAI credentials (not committed)
├── .env.example                        # Credentials template
├── run.bat                             # Start both services (Windows batch)
├── run.ps1                             # Start both services (PowerShell)
├── backend/
│   └── McpAguiServer/
│       ├── McpAguiServer.csproj        # .NET 9 project with NuGet packages
│       ├── Program.cs                  # Entry point, DI, CORS, MapAGUI endpoints
│       ├── AgentFactory.cs             # Azure OpenAI client, 4 agent factory methods
│       ├── McpAguiSerializerContext.cs  # Source-generated JSON for all DTOs
│       ├── Shared/
│       │   ├── Project.cs              # Project DTO (Code, Name, Description)
│       │   └── ProjectSeed.cs          # 4 shared projects (ALPHA, NEXUS, ORBIT, VAULT)
│       ├── TimeTracker/
│       │   ├── TimeEntry.cs            # DTO
│       │   └── TimeEntryStore.cs       # Seed data + 5 tools
│       ├── Calendar/
│       │   ├── EventType.cs            # Enum: Meeting, FocusBlock, Deadline
│       │   ├── CalendarEvent.cs        # DTO
│       │   └── CalendarStore.cs        # Seed data + 5 tools
│       ├── KnowledgeBase/
│       │   ├── KnowledgeArticle.cs     # DTO
│       │   └── KnowledgeArticleStore.cs # 21 articles + 5 tools
│       └── SecurityIssues/
│           ├── Severity.cs             # Enum: Critical, High, Medium, Low
│           ├── IssueStatus.cs          # Enum: Open, InProgress, Resolved, Dismissed
│           ├── SecurityIssue.cs        # DTO
│           └── SecurityIssueStore.cs   # 24 issues + 5 tools
└── frontend/
    ├── package.json                    # Dependencies (CopilotKit, AG-UI, Next.js)
    ├── next.config.ts                  # serverExternalPackages config
    ├── tsconfig.json
    ├── postcss.config.mjs              # Tailwind CSS setup
    └── src/
        ├── app/
        │   ├── globals.css             # Tailwind + CopilotKit CSS overrides
        │   ├── layout.tsx              # Root layout with Nav
        │   ├── page.tsx                # Home page with agent cards
        │   ├── api/copilotkit/
        │   │   └── route.ts            # CopilotRuntime bridge (HttpAgent → backend)
        │   ├── time-tracker/page.tsx    # Time Tracker agent page
        │   ├── calendar/page.tsx        # Calendar agent page
        │   ├── knowledge-base/page.tsx  # Knowledge Base agent page
        │   └── security-issues/page.tsx # Security Issues agent page
        └── components/
            ├── nav.tsx                 # Navigation bar with active state
            └── chat-layout.tsx         # Shared CopilotKit + CopilotChat wrapper
```

---

## Running the Project

See [howtorun.md](howtorun.md) for detailed setup and running instructions.

**Quick start:**

```bash
# 1. Copy and fill in credentials
cp .env.example .env

# 2. Run both services
.\run.ps1          # PowerShell
# or
run.bat            # Command Prompt
```

Then open http://localhost:3000 in your browser.
