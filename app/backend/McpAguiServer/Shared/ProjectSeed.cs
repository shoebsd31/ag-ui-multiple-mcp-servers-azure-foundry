namespace McpAguiServer.Shared;

internal static class ProjectSeed
{
    public static readonly List<Project> Projects =
    [
        new() { Code = "ALPHA", Name = "Project Alpha", Description = "Internal CRM modernization platform" },
        new() { Code = "NEXUS", Name = "Project Nexus", Description = "API gateway and microservices migration" },
        new() { Code = "ORBIT", Name = "Project Orbit", Description = "Customer-facing mobile app redesign" },
        new() { Code = "VAULT", Name = "Project Vault", Description = "Data warehouse and analytics pipeline" },
    ];

    public static Project? GetByCode(string code) =>
        Projects.FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
}
