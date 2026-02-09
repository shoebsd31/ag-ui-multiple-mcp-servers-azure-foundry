"use client";

import React from "react";
import { CopilotKit } from "@copilotkit/react-core";
import { CopilotChat } from "@copilotkit/react-ui";

interface SuggestionItem {
  title: string;
  message: string;
}

interface ChatLayoutProps {
  agentName: string;
  initialMessage: string;
  title: string;
  description: string;
  emoji?: string;
  suggestions?: SuggestionItem[];
  placeholder?: string;
}

export function ChatLayout({
  agentName,
  initialMessage,
  title,
  description,
  emoji,
  suggestions,
  placeholder = "Type a message...",
}: ChatLayoutProps) {
  return (
    <CopilotKit
      runtimeUrl="/api/copilotkit"
      showDevConsole={false}
      agent={agentName}
    >
      <div className="flex flex-col items-center justify-center min-h-[calc(100vh-57px)] p-6">
        <div className="mb-4 text-center">
          <h1 className="text-2xl font-bold text-gray-900">
            {emoji && <span className="mr-2">{emoji}</span>}
            {title}
          </h1>
          <p className="text-gray-600 mt-1 max-w-lg">{description}</p>
        </div>
        <div className="w-full max-w-2xl h-[70vh] rounded-xl shadow-xl overflow-hidden border border-gray-200/50 bg-white">
          <CopilotChat
            className="h-full"
            labels={{
              initial: initialMessage,
              title: `${emoji || ""} ${title}`.trim(),
              placeholder: placeholder,
            }}
            suggestions={suggestions}
          />
        </div>
      </div>
    </CopilotKit>
  );
}
