# How to Run

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)
- [pnpm](https://pnpm.io/) (`npm install -g pnpm`)
- Azure OpenAI resource with a chat deployment (e.g., gpt-4.1-mini or gpt-4o)

## 1. Configure Environment

Copy the example environment file and fill in your Azure OpenAI credentials:

```bash
cp .env.example .env
```

Edit `.env`:
```env
AZURE_OPENAI_ENDPOINT=https://your-resource.cognitiveservices.azure.com/
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4.1-mini
AZURE_OPENAI_API_KEY=your-api-key-here
```

**Important:** `AZURE_OPENAI_ENDPOINT` must be the base URL only. Do NOT include `/openai/deployments/...`.

## 2. Run the .NET Backend

```bash
cd backend
dotnet restore McpAguiServer/McpAguiServer.csproj
dotnet run --project McpAguiServer --urls "http://localhost:5018"
```

The backend exposes 4 AG-UI endpoints:
- `POST http://localhost:5018/time_tracker`
- `POST http://localhost:5018/calendar`
- `POST http://localhost:5018/knowledge_base`
- `POST http://localhost:5018/security_issues`

## 3. Run the Next.js Frontend

In a separate terminal:

```bash
cd frontend
pnpm install
pnpm dev
```

On Windows PowerShell, if you need to set the backend URL:
```powershell
$env:BACKEND_URL="http://localhost:5018"; pnpm dev
```

On Bash/macOS:
```bash
BACKEND_URL="http://localhost:5018" pnpm dev
```

## 4. Open the App

Visit [http://localhost:3000](http://localhost:3000) and select an agent scenario:

- `/time-tracker` — Log and query time entries
- `/calendar` — Check schedules and find free slots
- `/knowledge-base` — Search knowledge articles
- `/security-issues` — Review security findings

## Testing with curl

Test the backend directly with SSE streaming:

```bash
curl -N -X POST http://localhost:5018/time_tracker \
  -H "Content-Type: application/json" \
  -d '{"messages":[{"role":"user","content":"How much time did I log this week?"}]}'
```

## Troubleshooting

| Problem | Solution |
|---------|----------|
| HTTP 404 from Azure OpenAI | Use base URL only for `AZURE_OPENAI_ENDPOINT` |
| HTTP 401 | Check API key or ensure RBAC role `Cognitive Services OpenAI User` |
| CORS errors | Backend CORS is configured for `http://localhost:3000` |
| Build fails "Could not copy apphost.exe" | Stop any running backend process first |
| Frontend build errors with CopilotKit | Ensure `serverExternalPackages` in `next.config.ts` |
