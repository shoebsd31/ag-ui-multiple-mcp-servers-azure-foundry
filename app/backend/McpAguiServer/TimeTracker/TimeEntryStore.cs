using System.Text.Json;
using McpAguiServer.Shared;

namespace McpAguiServer.TimeTracker;

internal sealed class TimeEntryStore
{
    private readonly List<TimeEntry> _entries = [];
    private static readonly JsonSerializerOptions s_jsonOptions = new() { WriteIndented = false };

    private static readonly string[] Descriptions =
    [
        "Sprint planning and backlog grooming",
        "Code review for authentication module",
        "API endpoint development for user service",
        "Bug fix for payment processing timeout",
        "Database migration script development",
        "UI component development for dashboard",
        "Integration testing with third-party services",
        "Documentation update for API endpoints",
        "Performance optimization for search queries",
        "Unit test coverage improvement",
        "Architecture design review session",
        "Deployment pipeline configuration",
        "Security vulnerability assessment",
        "Client meeting and requirements gathering",
        "Refactoring legacy data access layer",
    ];

    public TimeEntryStore()
    {
        SeedData();
    }

    private void SeedData()
    {
        var rng = new Random(42);
        var projects = ProjectSeed.Projects;
        // Weights: ALPHA 30%, NEXUS 25%, ORBIT 25%, VAULT 20%
        double[] weights = [0.30, 0.25, 0.25, 0.20];

        var today = DateTime.Today;
        var startDate = today.AddDays(-42); // go back ~6 weeks to get 30 working days

        int workingDays = 0;
        for (var date = startDate; workingDays < 30; date = date.AddDays(1))
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                continue;

            workingDays++;
            var dateStr = date.ToString("yyyy-MM-dd");

            int entryCount = rng.Next(3, 6); // 3-5 entries per day
            double remainingHours = 8.0;
            var usedProjects = new HashSet<int>();

            for (int i = 0; i < entryCount && remainingHours > 0; i++)
            {
                // Pick project based on weights
                int projectIdx;
                if (i < entryCount - 1)
                {
                    double roll = rng.NextDouble();
                    double cumulative = 0;
                    projectIdx = 0;
                    for (int p = 0; p < weights.Length; p++)
                    {
                        cumulative += weights[p];
                        if (roll <= cumulative) { projectIdx = p; break; }
                    }
                }
                else
                {
                    // Last entry: pick any project
                    projectIdx = rng.Next(projects.Count);
                }

                double maxHours = i == entryCount - 1 ? remainingHours : Math.Min(remainingHours, 4.0);
                double hours = Math.Round(Math.Max(0.5, Math.Min(maxHours, rng.Next(1, 7) * 0.5)) * 2) / 2;
                if (hours > remainingHours) hours = remainingHours;
                if (hours < 0.5) break;

                var project = projects[projectIdx];
                _entries.Add(new TimeEntry
                {
                    Id = Guid.NewGuid(),
                    ProjectCode = project.Code,
                    ProjectName = project.Name,
                    Date = dateStr,
                    Hours = hours,
                    Description = Descriptions[rng.Next(Descriptions.Length)],
                    CreatedAt = date.AddHours(8 + i),
                });

                remainingHours -= hours;
            }
        }
    }

    public string LogTime(string projectCode, string date, double hours, string description)
    {
        var project = ProjectSeed.GetByCode(projectCode);
        if (project is null)
            return $"Error: Unknown project code '{projectCode}'. Valid codes: ALPHA, NEXUS, ORBIT, VAULT.";

        if (hours < 0.5 || hours > 8.0)
            return "Error: Hours must be between 0.5 and 8.0.";

        if (hours % 0.5 != 0)
            return "Error: Hours must be in 0.5 increments.";

        double existingHours = _entries.Where(e => e.Date == date).Sum(e => e.Hours);
        double remaining = 8.0 - existingHours;

        if (existingHours + hours > 8.0)
            return $"Error: Cannot log {hours}h. Already logged {existingHours}h on {date}. Remaining capacity: {remaining}h.";

        var entry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            ProjectCode = project.Code,
            ProjectName = project.Name,
            Date = date,
            Hours = hours,
            Description = description,
            CreatedAt = DateTime.UtcNow,
        };
        _entries.Add(entry);

        double newRemaining = 8.0 - existingHours - hours;
        return $"Logged {hours}h to {project.Name} ({project.Code}) on {date}: \"{description}\". Daily total: {existingHours + hours}h / 8h. Remaining capacity: {newRemaining}h.";
    }

    public string GetTimeEntries(string startDate, string? endDate = null)
    {
        var end = endDate ?? startDate;
        var entries = _entries
            .Where(e => string.Compare(e.Date, startDate, StringComparison.Ordinal) >= 0
                     && string.Compare(e.Date, end, StringComparison.Ordinal) <= 0)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.CreatedAt)
            .ToList();

        double todayHours = _entries.Where(e => e.Date == DateTime.Today.ToString("yyyy-MM-dd")).Sum(e => e.Hours);

        var result = new
        {
            entries,
            totalHours = entries.Sum(e => e.Hours),
            entryCount = entries.Count,
            dateRange = new { start = startDate, end },
            remainingToday = 8.0 - todayHours,
        };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetProjectTimeSummary(string? startDate = null, string? endDate = null)
    {
        var filtered = _entries.AsEnumerable();
        if (startDate is not null)
            filtered = filtered.Where(e => string.Compare(e.Date, startDate, StringComparison.Ordinal) >= 0);
        if (endDate is not null)
            filtered = filtered.Where(e => string.Compare(e.Date, endDate, StringComparison.Ordinal) <= 0);

        var list = filtered.ToList();
        double total = list.Sum(e => e.Hours);

        var summaries = list
            .GroupBy(e => new { e.ProjectCode, e.ProjectName })
            .Select(g => new
            {
                g.Key.ProjectCode,
                g.Key.ProjectName,
                totalHours = g.Sum(e => e.Hours),
                percentage = total > 0 ? Math.Round(g.Sum(e => e.Hours) / total * 100, 1) : 0,
                entryCount = g.Count(),
            })
            .OrderByDescending(s => s.totalHours)
            .ToList();

        var result = new { summaries, totalHours = total, projectCount = summaries.Count };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetTimeForProject(string projectCode, string? startDate = null, string? endDate = null)
    {
        var project = ProjectSeed.GetByCode(projectCode);
        if (project is null)
            return $"Error: Unknown project code '{projectCode}'.";

        var filtered = _entries.Where(e => e.ProjectCode.Equals(projectCode, StringComparison.OrdinalIgnoreCase));
        if (startDate is not null)
            filtered = filtered.Where(e => string.Compare(e.Date, startDate, StringComparison.Ordinal) >= 0);
        if (endDate is not null)
            filtered = filtered.Where(e => string.Compare(e.Date, endDate, StringComparison.Ordinal) <= 0);

        var entries = filtered.OrderBy(e => e.Date).ThenBy(e => e.CreatedAt).ToList();
        var result = new
        {
            project = new { project.Code, project.Name },
            entries,
            totalHours = entries.Sum(e => e.Hours),
            entryCount = entries.Count,
        };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetDailyBreakdown(string startDate, string endDate)
    {
        var entries = _entries
            .Where(e => string.Compare(e.Date, startDate, StringComparison.Ordinal) >= 0
                     && string.Compare(e.Date, endDate, StringComparison.Ordinal) <= 0)
            .ToList();

        var days = entries
            .GroupBy(e => e.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                date = g.Key,
                totalHours = g.Sum(e => e.Hours),
                projects = g.GroupBy(e => e.ProjectCode)
                    .Select(pg => new { projectCode = pg.Key, hours = pg.Sum(e => e.Hours) })
                    .OrderByDescending(p => p.hours)
                    .ToList(),
            })
            .ToList();

        var result = new
        {
            days,
            dateRange = new { start = startDate, end = endDate },
            totalHours = entries.Sum(e => e.Hours),
            dayCount = days.Count,
        };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }
}
