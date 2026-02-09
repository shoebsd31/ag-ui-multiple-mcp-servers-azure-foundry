"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

const navItems = [
  { href: "/", label: "Home", emoji: "🏠" },
  { href: "/time-tracker", label: "Time Tracker", emoji: "⏰" },
  { href: "/calendar", label: "Calendar", emoji: "📅" },
  { href: "/knowledge-base", label: "Knowledge Base", emoji: "📚" },
  { href: "/security-issues", label: "Security Issues", emoji: "🛡️" },
];

export function Nav() {
  const pathname = usePathname();

  return (
    <nav className="bg-white border-b border-gray-200 px-6 py-3 shadow-sm">
      <div className="max-w-7xl mx-auto flex items-center gap-6">
        <Link href="/" className="font-bold text-lg text-gray-900 flex items-center gap-2">
          <span>🚀</span>
          <span>AG-UI Demo</span>
        </Link>
        <div className="flex gap-1">
          {navItems.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className={`px-3 py-1.5 rounded-md text-sm font-medium transition-all duration-200 ${
                pathname === item.href
                  ? "bg-indigo-100 text-indigo-700 shadow-sm"
                  : "text-gray-600 hover:text-gray-900 hover:bg-gray-100"
              }`}
            >
              <span className="mr-1">{item.emoji}</span>
              {item.label}
            </Link>
          ))}
        </div>
      </div>
    </nav>
  );
}
