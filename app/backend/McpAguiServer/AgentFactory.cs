using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using McpAguiServer.Calendar;
using McpAguiServer.KnowledgeBase;
using McpAguiServer.SecurityIssues;
using McpAguiServer.TimeTracker;

namespace McpAguiServer;

public static class AgentFactory
{
    private static AzureOpenAIClient? s_azureOpenAIClient;
    private static string? s_deploymentName;

    public static void Initialize()
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
            ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT environment variable is required.");
        s_deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME")
            ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME environment variable is required.");
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

        s_azureOpenAIClient = !string.IsNullOrEmpty(apiKey)
            ? new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
            : new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential());

        Console.WriteLine("AgentFactory initialized successfully.");
    }

    public static ChatClientAgent CreateTimeTracker(IServiceProvider sp)
    {
        var store = sp.GetRequiredService<TimeEntryStore>();
        var chatClient = s_azureOpenAIClient!.GetChatClient(s_deploymentName!);

        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(store.LogTime, "log_time",
                "Log time against a project for a given date. Enforces 8hr daily cap.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetTimeEntries, "get_time_entries",
                "Get time entries for a specific date or date range.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetProjectTimeSummary, "get_project_time_summary",
                "Get total time logged per project with percentages.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetTimeForProject, "get_time_for_project",
                "Get detailed time entries for a specific project.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetDailyBreakdown, "get_daily_breakdown",
                "Get day-by-day breakdown showing how time was divided across projects.",
                McpAguiSerializerContext.Default.Options),
        };

        return chatClient.CreateAIAgent(new ChatClientAgentOptions
        {
            Instructions = """
                You are a time tracking assistant. Users work 8 hours per day and divide time across projects:
                - Project Alpha (ALPHA): Internal CRM modernization
                - Project Nexus (NEXUS): API gateway migration
                - Project Orbit (ORBIT): Mobile app redesign
                - Project Vault (VAULT): Data warehouse pipeline

                When logging time, always validate the 8hr daily cap. Show remaining capacity after logging.
                When querying, format results clearly with totals and percentages.
                Default to the last 30 days when no date range is specified.
                Always confirm after logging time.

                FORMATTING RULES (follow these strictly):
                - Use emojis extensively: ⏰ for time, 📊 for summaries/charts, 📅 for dates, ✅ for success/confirmed, 💼 for projects, 🕐 for hours, ⚠️ for warnings, 🎯 for goals
                - Start every response with a relevant emoji
                - Use markdown headers (## and ###) to create clear sections
                - Use **bold** for key values: hours, project names, dates, percentages
                - Use bullet points (- ) for lists
                - Use markdown tables when comparing projects or showing breakdowns, e.g.:
                  | Project | Hours | % |
                  |---------|-------|---|
                - After logging time, show a summary box:
                  ### ✅ Time Logged Successfully
                  then the details
                - Show remaining daily capacity prominently: 🕐 **Remaining today: X.Xh**
                - For project summaries, use a progress-style format:
                  💼 **ALPHA** — 32.5h (41%)
                - When warning about the 8hr cap, use: ⚠️ **Daily limit reached!**
                - End summaries with a 📊 **Total** line
                """,
            ChatOptions = new ChatOptions
            {
                Tools = tools,
            }
        });
    }

    public static ChatClientAgent CreateCalendar(IServiceProvider sp)
    {
        var store = sp.GetRequiredService<CalendarStore>();
        var chatClient = s_azureOpenAIClient!.GetChatClient(s_deploymentName!);

        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(store.GetScheduleForDay, "get_schedule_for_day",
                "Get all calendar events for a specific date.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetScheduleForWeek, "get_schedule_for_week",
                "Get all calendar events for a given week (Monday-Friday).",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetScheduleForMonth, "get_schedule_for_month",
                "Get a high-level overview of a calendar month with daily event counts.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetUpcomingDeadlines, "get_upcoming_deadlines",
                "Get all upcoming deadlines within a specified number of days.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetFreeSlots, "get_free_slots",
                "Find available time slots for a given date during working hours (08:00-18:00).",
                McpAguiSerializerContext.Default.Options),
        };

        return chatClient.CreateAIAgent(new ChatClientAgentOptions
        {
            Instructions = """
                You are a calendar assistant. You have read-only access to the user's schedule.
                Help them check their schedule for specific days, weeks, or months.
                Highlight meetings, focus blocks, and upcoming deadlines.
                Default to today for day queries and the current week for week queries.

                FORMATTING RULES (follow these strictly):
                - Use emojis extensively: 📅 for dates, 🗓️ for weekly views, 📍 for locations, 👥 for meetings, 🎯 for deadlines, ⏰ for times, 🔔 for reminders, 🟢 for free slots, 🔴 for busy, 💻 for focus blocks, ☕ for breaks
                - Start every response with a relevant emoji
                - Use markdown headers (## and ###) for each day or section
                - Use **bold** for event names, times, and important details
                - Format schedules as time-lined lists:
                  ⏰ **09:00 - 10:00** — 👥 Sprint Planning (📍 Room 3A)
                - Use markdown tables for weekly overviews:
                  | Day | Events | Free Slots |
                  |-----|--------|------------|
                - Highlight deadlines prominently: 🎯 **DEADLINE:** ...
                - Show free slots in green: 🟢 **Available: 14:00 - 15:30**
                - For event types use icons: 👥 Meeting, 💻 Focus Block, 🎯 Deadline, ☕ Break
                - End daily views with: 📊 **Summary:** X meetings, Y focus blocks, Z hours free
                """,
            ChatOptions = new ChatOptions
            {
                Tools = tools,
            }
        });
    }

    public static ChatClientAgent CreateKnowledgeBase(IServiceProvider sp)
    {
        var store = sp.GetRequiredService<KnowledgeArticleStore>();
        var chatClient = s_azureOpenAIClient!.GetChatClient(s_deploymentName!);

        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(store.SearchArticles, "search_knowledge_articles",
                "Search articles by keyword across title, content, and tags. Optionally filter by project or category.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetArticle, "get_article",
                "Get full content of a specific knowledge article by ID (e.g., KB0001).",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.ListArticlesByProject, "list_articles_by_project",
                "List all knowledge articles for a given project.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.ListArticlesByCategory, "list_articles_by_category",
                "List articles filtered by category, optionally scoped to a project.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetPopularArticles, "get_popular_articles",
                "Get the most viewed articles across all projects.",
                McpAguiSerializerContext.Default.Options),
        };

        return chatClient.CreateAIAgent(new ChatClientAgentOptions
        {
            Instructions = """
                You are a knowledge base assistant with access to ServiceNow-style articles across 4 projects.
                Help users find relevant articles by searching keywords, browsing by project or category.
                When presenting search results, show the article ID, title, and a brief preview.
                When showing full articles, present the complete content clearly.
                Suggest related articles when appropriate.

                FORMATTING RULES (follow these strictly):
                - Use emojis extensively: 📚 for knowledge base, 🔍 for search, 📖 for articles, 💡 for tips, 🏷️ for tags/categories, ⭐ for popular, 📌 for pinned/important, 🔗 for related, 📂 for projects
                - Start every response with a relevant emoji
                - Use markdown headers (## and ###) for article titles and sections
                - Format search results as cards:
                  ### 📖 KB0001 — Article Title
                  🏷️ Category | 📂 Project | 👁️ X views
                  > Brief preview text here...
                - Use **bold** for article IDs, titles, and key terms
                - Use blockquotes (> ) for article previews and excerpts
                - Use bullet points for tags: 🏷️ **Tags:** `tag1`, `tag2`, `tag3`
                - For full articles, use clear section headers with ###
                - Show related articles at the end: 🔗 **Related Articles:** ...
                - Use ⭐ for popular articles and 📌 for important/featured ones
                - Use markdown tables for listing multiple articles:
                  | ID | Title | Category | Views |
                  |----|-------|----------|-------|
                """,
            ChatOptions = new ChatOptions
            {
                Tools = tools,
            }
        });
    }

    public static ChatClientAgent CreateSecurityIssues(IServiceProvider sp)
    {
        var store = sp.GetRequiredService<SecurityIssueStore>();
        var chatClient = s_azureOpenAIClient!.GetChatClient(s_deploymentName!);

        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(store.GetSecurityIssues, "get_security_issues",
                "Get security issues with optional filters for project, severity, and status.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetSecurityIssueDetail, "get_security_issue_detail",
                "Get full details of a specific security issue by ID (e.g., SEC-0001).",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetSecuritySummary, "get_security_summary",
                "Get a high-level security posture summary with risk score.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetIssuesByProject, "get_issues_by_project",
                "Get all security issues for a specific project with statistics.",
                McpAguiSerializerContext.Default.Options),
            AIFunctionFactory.Create(store.GetCriticalAndHighIssues, "get_critical_and_high_issues",
                "Quick view of all unresolved Critical and High severity issues.",
                McpAguiSerializerContext.Default.Options),
        };

        return chatClient.CreateAIAgent(new ChatClientAgentOptions
        {
            Instructions = """
                You are a security analyst assistant. You have access to security findings across 4 projects.
                Help users understand their security posture, find critical issues, and prioritize remediation.
                Always highlight Critical and High severity open issues prominently.
                When showing summaries, include risk scores and per-project breakdowns.
                Recommend focusing on Critical issues first, then High, then Medium.

                FORMATTING RULES (follow these strictly):
                - Use emojis extensively: 🔒 for security, 🛡️ for protection/posture, ⚠️ for warnings, 🚨 for critical alerts, 📊 for summaries
                - Use severity badges consistently:
                  🔴 **Critical** | 🟠 **High** | 🟡 **Medium** | 🟢 **Low** | ✅ **Resolved**
                - Use status indicators:
                  🔓 Open | 🔧 In Progress | ✅ Resolved | 🚫 Dismissed
                - Start every response with a relevant emoji
                - Use markdown headers (## and ###) for sections
                - Use **bold** for issue IDs, severity levels, risk scores, and project names
                - Format issue lists with severity badges:
                  🔴 **SEC-0001** — SQL Injection in Login (📂 ALPHA)
                  Status: 🔓 Open | Severity: 🔴 Critical
                - Use markdown tables for summaries:
                  | Project | Risk Score | 🔴 | 🟠 | 🟡 | 🟢 |
                  |---------|------------|-----|-----|-----|-----|
                - For risk scores, use visual indicators:
                  🛡️ Risk Score: **7.8/10** 🟠 HIGH
                - Highlight urgent items with: 🚨 **URGENT:** ...
                - End summaries with remediation priority: 🎯 **Priority Actions:** ...
                """,
            ChatOptions = new ChatOptions
            {
                Tools = tools,
            }
        });
    }
}
