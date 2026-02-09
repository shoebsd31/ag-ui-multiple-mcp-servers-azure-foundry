import Link from "next/link";

const agents = [
  {
    href: "/time-tracker",
    emoji: "⏰",
    title: "Time Tracker",
    description:
      "Log and track working hours across 4 projects. 8-hour daily cap with project time analysis.",
    color: "bg-blue-50 border-blue-200 hover:border-blue-400 hover:shadow-lg hover:shadow-blue-100",
    iconBg: "bg-blue-100",
  },
  {
    href: "/calendar",
    emoji: "📅",
    title: "Calendar",
    description:
      "Query your schedule by day, week, or month. Find free time slots and upcoming deadlines.",
    color: "bg-green-50 border-green-200 hover:border-green-400 hover:shadow-lg hover:shadow-green-100",
    iconBg: "bg-green-100",
  },
  {
    href: "/knowledge-base",
    emoji: "📚",
    title: "Knowledge Base",
    description:
      "Search ServiceNow-style knowledge articles. Browse by project, category, or keyword.",
    color: "bg-purple-50 border-purple-200 hover:border-purple-400 hover:shadow-lg hover:shadow-purple-100",
    iconBg: "bg-purple-100",
  },
  {
    href: "/security-issues",
    emoji: "🛡️",
    title: "Security Issues",
    description:
      "Review security findings across projects. Risk scores, severity analysis, and remediation priorities.",
    color: "bg-red-50 border-red-200 hover:border-red-400 hover:shadow-lg hover:shadow-red-100",
    iconBg: "bg-red-100",
  },
];

export default function HomePage() {
  return (
    <div className="max-w-4xl mx-auto py-12 px-6">
      <div className="text-center mb-10">
        <h1 className="text-3xl font-bold text-gray-900">
          🚀 AG-UI Demo Workspace
        </h1>
        <p className="text-gray-600 mt-2 text-lg">
          4 AI agent scenarios powered by AG-UI protocol with CopilotKit ✨
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {agents.map((agent) => (
          <Link
            key={agent.href}
            href={agent.href}
            className={`block p-6 rounded-xl border-2 transition-all duration-200 ${agent.color}`}
          >
            <div className="flex items-center gap-3 mb-2">
              <span className={`text-2xl p-2 rounded-lg ${agent.iconBg}`}>
                {agent.emoji}
              </span>
              <h2 className="text-xl font-semibold text-gray-900">
                {agent.title}
              </h2>
            </div>
            <p className="text-gray-600 mt-2">{agent.description}</p>
          </Link>
        ))}
      </div>

      <div className="mt-10 text-center text-sm text-gray-500">
        <p>
          🖥️ .NET 9 backend on port 5018 | ⚡ Next.js frontend on port 3000
        </p>
        <p className="mt-1">
          Each agent wraps Azure OpenAI with domain-specific tools via the AG-UI
          protocol
        </p>
      </div>
    </div>
  );
}
