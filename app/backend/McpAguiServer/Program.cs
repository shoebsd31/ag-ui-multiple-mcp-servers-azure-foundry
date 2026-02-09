using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using McpAguiServer;
using McpAguiServer.Calendar;
using McpAguiServer.KnowledgeBase;
using McpAguiServer.SecurityIssues;
using McpAguiServer.TimeTracker;

var builder = WebApplication.CreateBuilder(args);

// Load .env from the app root (two levels up from McpAguiServer/)
DotNetEnv.Env.Load(Path.Combine(builder.Environment.ContentRootPath, "..", "..", ".env"));

// Register stores as singletons
builder.Services.AddSingleton<TimeEntryStore>();
builder.Services.AddSingleton<CalendarStore>();
builder.Services.AddSingleton<KnowledgeArticleStore>();
builder.Services.AddSingleton<SecurityIssueStore>();

builder.Services.AddAGUI();

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();

// Initialize the agent factory
AgentFactory.Initialize();

// Map all AG-UI endpoints
var serviceProvider = app.Services;
app.MapAGUI("/time_tracker", AgentFactory.CreateTimeTracker(serviceProvider));
app.MapAGUI("/calendar", AgentFactory.CreateCalendar(serviceProvider));
app.MapAGUI("/knowledge_base", AgentFactory.CreateKnowledgeBase(serviceProvider));
app.MapAGUI("/security_issues", AgentFactory.CreateSecurityIssues(serviceProvider));

Console.WriteLine("AG-UI endpoints mapped: /time_tracker, /calendar, /knowledge_base, /security_issues");
Console.WriteLine("Server is ready.");

app.Run();
