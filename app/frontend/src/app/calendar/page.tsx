"use client";

import { ChatLayout } from "@/components/chat-layout";

export default function CalendarPage() {
  return (
    <ChatLayout
      agentName="calendar"
      title="Calendar"
      emoji="📅"
      description="Check your schedule, find free slots, and view upcoming deadlines 🗓️"
      initialMessage="📅 Hi! I'm your calendar assistant. I can show you today's schedule, find free time slots, check upcoming deadlines, or give you a weekly overview. What would you like to know?"
      placeholder="e.g., What's on my schedule today?"
      suggestions={[
        { title: "🗓️ Today's schedule", message: "What's on my schedule for today?" },
        { title: "🟢 Find free slots", message: "Find me available time slots for today" },
        { title: "🎯 Deadlines", message: "What upcoming deadlines do I have in the next 7 days?" },
      ]}
    />
  );
}
