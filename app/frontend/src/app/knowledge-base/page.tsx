"use client";

import { ChatLayout } from "@/components/chat-layout";

export default function KnowledgeBasePage() {
  return (
    <ChatLayout
      agentName="knowledge_base"
      title="Knowledge Base"
      emoji="📚"
      description="Search and browse ServiceNow-style knowledge articles across all projects 🔍"
      initialMessage="📚 Hi! I'm your knowledge base assistant. I can help you find articles by keyword, browse by project or category, or show you the most popular articles. What are you looking for?"
      placeholder="e.g., Search for API documentation..."
      suggestions={[
        { title: "⭐ Popular articles", message: "Show me the most popular knowledge articles" },
        { title: "📂 Browse by project", message: "List all articles for Project Alpha" },
        { title: "🔍 Search", message: "Search for articles about deployment" },
      ]}
    />
  );
}
