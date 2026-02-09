using System.Text.Json;
using McpAguiServer.Shared;

namespace McpAguiServer.Calendar;

internal sealed class CalendarStore
{
    private readonly List<CalendarEvent> _events = [];
    private static readonly JsonSerializerOptions s_jsonOptions = new() { WriteIndented = false };

    private static readonly string[] Attendees =
        ["Alex Rivera", "Morgan Chen", "Jamie Patel", "Taylor Kim", "Casey Brooks"];

    private static readonly string[] Locations = ["Zoom", "Room 3B", "Teams", "Room 101"];

    public CalendarStore()
    {
        SeedData();
    }

    private void SeedData()
    {
        var rng = new Random(123);
        var today = DateTime.Today;
        var start = today.AddMonths(-1);
        var end = today.AddMonths(1);
        var projects = ProjectSeed.Projects;

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                continue;

            // Daily Standup (recurring)
            _events.Add(new CalendarEvent
            {
                Id = Guid.NewGuid(),
                Title = "Daily Standup",
                Start = date.AddHours(9),
                End = date.AddHours(9).AddMinutes(15),
                Type = EventType.Meeting,
                Location = "Zoom",
                Attendees = [.. Attendees],
                IsRecurring = true,
            });

            // Lunch Break (recurring)
            _events.Add(new CalendarEvent
            {
                Id = Guid.NewGuid(),
                Title = "Lunch Break",
                Start = date.AddHours(12),
                End = date.AddHours(13),
                Type = EventType.FocusBlock,
                IsRecurring = true,
            });

            // Team Retrospective on Fridays
            if (date.DayOfWeek == DayOfWeek.Friday)
            {
                _events.Add(new CalendarEvent
                {
                    Id = Guid.NewGuid(),
                    Title = "Team Retrospective",
                    Start = date.AddHours(15),
                    End = date.AddHours(16),
                    Type = EventType.Meeting,
                    Location = "Room 3B",
                    Attendees = [.. Attendees],
                    IsRecurring = true,
                });
            }

            // Scattered events (2-3 per week, roughly one every 2-3 days)
            if (rng.NextDouble() < 0.45)
            {
                var project = projects[rng.Next(projects.Count)];
                AddScatteredEvent(date, project, rng);
            }
            if (rng.NextDouble() < 0.25)
            {
                var project = projects[rng.Next(projects.Count)];
                AddScatteredEvent(date, project, rng);
            }
        }
    }

    private void AddScatteredEvent(DateTime date, Project project, Random rng)
    {
        int eventKind = rng.Next(10);
        if (eventKind < 5)
        {
            // Meeting
            string[] meetingTitles =
                ["Sprint Planning", "Code Review Session", "Stakeholder Demo", "Architecture Review", "1:1 with Tech Lead", "All Hands"];
            int hour = rng.Next(0, 3) switch { 0 => 10, 1 => 14, _ => 16 };
            int durationMinutes = rng.Next(0, 2) == 0 ? 60 : 30;
            int attendeeCount = rng.Next(2, 6);
            var attendees = Attendees.OrderBy(_ => rng.Next()).Take(attendeeCount).ToList();

            _events.Add(new CalendarEvent
            {
                Id = Guid.NewGuid(),
                Title = $"{meetingTitles[rng.Next(meetingTitles.Length)]} — {project.Name}",
                Start = date.AddHours(hour),
                End = date.AddHours(hour).AddMinutes(durationMinutes),
                Type = EventType.Meeting,
                ProjectCode = project.Code,
                ProjectName = project.Name,
                Location = Locations[rng.Next(Locations.Length)],
                Description = $"Team sync for {project.Name}",
                Attendees = attendees,
            });
        }
        else if (eventKind < 8)
        {
            // Focus Block
            string[] focusTitles = ["Deep Work — Development", "Documentation Sprint", "Bug Triage"];
            int hour = rng.Next(0, 2) == 0 ? 10 : 14;
            int durationHours = focusTitles[rng.Next(focusTitles.Length)] == "Bug Triage" ? 1 : rng.Next(2, 4);

            _events.Add(new CalendarEvent
            {
                Id = Guid.NewGuid(),
                Title = $"{focusTitles[rng.Next(focusTitles.Length)]} — {project.Name}",
                Start = date.AddHours(hour),
                End = date.AddHours(hour + durationHours),
                Type = EventType.FocusBlock,
                ProjectCode = project.Code,
                ProjectName = project.Name,
                Description = $"Focus time for {project.Name}",
            });
        }
        else
        {
            // Deadline
            string[] deadlineTitles =
                ["Sprint Deadline", "Release v2.1", "Security Audit Due", "Quarterly Review Prep"];

            _events.Add(new CalendarEvent
            {
                Id = Guid.NewGuid(),
                Title = $"{deadlineTitles[rng.Next(deadlineTitles.Length)]} — {project.Name}",
                Start = date.AddHours(17),
                End = date.AddHours(18),
                Type = EventType.Deadline,
                ProjectCode = project.Code,
                ProjectName = project.Name,
                Description = $"Deadline for {project.Name}",
            });
        }
    }

    public string GetScheduleForDay(string? date = null)
    {
        var targetDate = date is not null ? DateTime.Parse(date) : DateTime.Today;
        var events = _events
            .Where(e => e.Start.Date == targetDate.Date)
            .OrderBy(e => e.Start)
            .ToList();

        var result = new
        {
            date = targetDate.ToString("yyyy-MM-dd"),
            events,
            summary = new
            {
                totalEvents = events.Count,
                meetings = events.Count(e => e.Type == EventType.Meeting),
                focusBlocks = events.Count(e => e.Type == EventType.FocusBlock),
                deadlines = events.Count(e => e.Type == EventType.Deadline),
                busyHours = events.Sum(e => (e.End - e.Start).TotalHours),
            },
        };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetScheduleForWeek(string? date = null)
    {
        var targetDate = date is not null ? DateTime.Parse(date) : DateTime.Today;
        // Find Monday of the week
        int daysToMonday = ((int)targetDate.DayOfWeek - 1 + 7) % 7;
        var monday = targetDate.AddDays(-daysToMonday);
        var friday = monday.AddDays(4);

        var events = _events
            .Where(e => e.Start.Date >= monday.Date && e.Start.Date <= friday.Date)
            .OrderBy(e => e.Start)
            .ToList();

        var days = Enumerable.Range(0, 5)
            .Select(i => monday.AddDays(i))
            .Select(d => new
            {
                date = d.ToString("yyyy-MM-dd"),
                dayOfWeek = d.DayOfWeek.ToString(),
                events = events.Where(e => e.Start.Date == d.Date).ToList(),
            })
            .ToList();

        var result = new
        {
            weekOf = monday.ToString("yyyy-MM-dd"),
            days,
            weeklySummary = new
            {
                totalEvents = events.Count,
                meetings = events.Count(e => e.Type == EventType.Meeting),
                focusBlocks = events.Count(e => e.Type == EventType.FocusBlock),
                deadlines = events.Count(e => e.Type == EventType.Deadline),
            },
        };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetScheduleForMonth(int year, int month)
    {
        var firstDay = new DateTime(year, month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var events = _events
            .Where(e => e.Start.Date >= firstDay && e.Start.Date <= lastDay)
            .ToList();

        var dailyCounts = events
            .GroupBy(e => e.Start.Date)
            .Select(g => new
            {
                date = g.Key.ToString("yyyy-MM-dd"),
                eventCount = g.Count(),
                meetings = g.Count(e => e.Type == EventType.Meeting),
                focusBlocks = g.Count(e => e.Type == EventType.FocusBlock),
                deadlines = g.Count(e => e.Type == EventType.Deadline),
            })
            .OrderBy(d => d.date)
            .ToList();

        var busiestDay = dailyCounts.OrderByDescending(d => d.eventCount).FirstOrDefault();

        var result = new
        {
            month = $"{year}-{month:D2}",
            dailyCounts,
            summary = new
            {
                totalEvents = events.Count,
                totalDaysWithEvents = dailyCounts.Count,
                busiestDay = busiestDay is not null ? new { busiestDay.date, busiestDay.eventCount } : null,
            },
        };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetUpcomingDeadlines(int? days = null)
    {
        int lookAhead = days ?? 30;
        var now = DateTime.Today;
        var cutoff = now.AddDays(lookAhead);

        var deadlines = _events
            .Where(e => e.Type == EventType.Deadline && e.Start.Date >= now && e.Start.Date <= cutoff)
            .OrderBy(e => e.Start)
            .Select(e => new
            {
                e.Id,
                e.Title,
                date = e.Start.ToString("yyyy-MM-dd"),
                e.ProjectCode,
                e.ProjectName,
                daysUntil = (e.Start.Date - now).Days,
            })
            .ToList();

        var result = new { deadlines, totalCount = deadlines.Count, lookAheadDays = lookAhead };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetFreeSlots(string date, int? minDurationMinutes = null)
    {
        int minMinutes = minDurationMinutes ?? 30;
        var targetDate = DateTime.Parse(date);
        var workStart = targetDate.AddHours(8);
        var workEnd = targetDate.AddHours(18);

        var events = _events
            .Where(e => e.Start.Date == targetDate.Date)
            .OrderBy(e => e.Start)
            .ToList();

        var freeSlots = new List<object>();
        var currentStart = workStart;

        foreach (var evt in events)
        {
            if (evt.Start > currentStart)
            {
                var durationMinutes = (evt.Start - currentStart).TotalMinutes;
                if (durationMinutes >= minMinutes)
                {
                    freeSlots.Add(new
                    {
                        start = currentStart.ToString("HH:mm"),
                        end = evt.Start.ToString("HH:mm"),
                        durationMinutes,
                    });
                }
            }
            if (evt.End > currentStart)
                currentStart = evt.End;
        }

        if (currentStart < workEnd)
        {
            var durationMinutes = (workEnd - currentStart).TotalMinutes;
            if (durationMinutes >= minMinutes)
            {
                freeSlots.Add(new
                {
                    start = currentStart.ToString("HH:mm"),
                    end = workEnd.ToString("HH:mm"),
                    durationMinutes,
                });
            }
        }

        var result = new
        {
            date,
            freeSlots,
            totalFreeSlots = freeSlots.Count,
            minDurationMinutes = minMinutes,
        };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }
}
