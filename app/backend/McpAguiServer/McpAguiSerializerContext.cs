using System.Text.Json.Serialization;
using McpAguiServer.Calendar;
using McpAguiServer.KnowledgeBase;
using McpAguiServer.SecurityIssues;
using McpAguiServer.Shared;
using McpAguiServer.TimeTracker;

namespace McpAguiServer;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTime?))]
[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(List<Project>))]
[JsonSerializable(typeof(TimeEntry))]
[JsonSerializable(typeof(List<TimeEntry>))]
[JsonSerializable(typeof(CalendarEvent))]
[JsonSerializable(typeof(List<CalendarEvent>))]
[JsonSerializable(typeof(EventType?))]
[JsonSerializable(typeof(KnowledgeArticle))]
[JsonSerializable(typeof(List<KnowledgeArticle>))]
[JsonSerializable(typeof(SecurityIssue))]
[JsonSerializable(typeof(List<SecurityIssue>))]
[JsonSerializable(typeof(Severity?))]
[JsonSerializable(typeof(IssueStatus?))]
internal sealed partial class McpAguiSerializerContext : JsonSerializerContext;
