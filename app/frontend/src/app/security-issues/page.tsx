"use client";

import { ChatLayout } from "@/components/chat-layout";

export default function SecurityIssuesPage() {
  return (
    <ChatLayout
      agentName="security_issues"
      title="Security Issues"
      emoji="🛡️"
      description="Review security findings, risk scores, and remediation priorities across projects 🔒"
      initialMessage="🛡️ Hi! I'm your security analyst assistant. I can help you review security findings, check risk scores, find critical vulnerabilities, and prioritize remediation. What would you like to know?"
      placeholder="e.g., Show me all critical security issues..."
      suggestions={[
        { title: "🚨 Critical issues", message: "Show me all critical and high severity open issues" },
        { title: "📊 Security summary", message: "Give me an overall security posture summary with risk scores" },
        { title: "📂 By project", message: "Show security issues for Project Alpha" },
      ]}
    />
  );
}
