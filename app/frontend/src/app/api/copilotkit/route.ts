import {
  CopilotRuntime,
  ExperimentalEmptyAdapter,
  copilotRuntimeNextJSAppRouterEndpoint,
} from "@copilotkit/runtime";
import { HttpAgent } from "@ag-ui/client";
import { NextRequest } from "next/server";

const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5018";

function createAgents(): Record<string, HttpAgent> {
  return {
    time_tracker: new HttpAgent({ url: `${BACKEND_URL}/time_tracker` }),
    calendar: new HttpAgent({ url: `${BACKEND_URL}/calendar` }),
    knowledge_base: new HttpAgent({ url: `${BACKEND_URL}/knowledge_base` }),
    security_issues: new HttpAgent({ url: `${BACKEND_URL}/security_issues` }),
  };
}

export async function POST(request: NextRequest) {
  const agents = createAgents();
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const runtime = new CopilotRuntime({ agents: agents as any });
  const { handleRequest } = copilotRuntimeNextJSAppRouterEndpoint({
    runtime,
    serviceAdapter: new ExperimentalEmptyAdapter(),
    endpoint: "/api/copilotkit",
  });
  return handleRequest(request);
}
