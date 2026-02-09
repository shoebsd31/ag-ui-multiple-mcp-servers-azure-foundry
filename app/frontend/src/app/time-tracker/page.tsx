"use client";

import { ChatLayout } from "@/components/chat-layout";

export default function TimeTrackerPage() {
  return (
    <ChatLayout
      agentName="time_tracker"
      title="Time Tracker"
      emoji="⏰"
      description="Log and track working hours across projects (8hr daily cap) 💼"
      initialMessage="⏰ Hi! I'm your time tracking assistant. I can help you log hours, check your time entries, and analyze how you're spending your time across projects. What would you like to do?"
      placeholder="e.g., Log 3 hours on Project Alpha today..."
      suggestions={[
        { title: "📊 Show summary", message: "Show me a summary of my time across all projects for the last 30 days" },
        { title: "📅 Today's entries", message: "What time have I logged today?" },
        { title: "💼 Project breakdown", message: "Show me a daily breakdown of how I spent my time this week" },
      ]}
    />
  );
}
