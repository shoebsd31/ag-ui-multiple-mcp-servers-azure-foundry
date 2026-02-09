using System.Text.Json;

namespace McpAguiServer.SecurityIssues;

internal sealed class SecurityIssueStore
{
    private readonly List<SecurityIssue> _issues = [];
    private static readonly JsonSerializerOptions s_jsonOptions = new() { WriteIndented = false };

    public SecurityIssueStore()
    {
        SeedData();
    }

    private void SeedData()
    {
        var now = DateTime.UtcNow;

        // Project Alpha (CRM)
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0001", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "SQL Injection vulnerability in customer search endpoint",
            Description = "The customer search API endpoint concatenates user input directly into SQL queries without parameterization. An attacker could extract or modify database contents by injecting malicious SQL through the search field.",
            Severity = Severity.Critical, Status = IssueStatus.InProgress,
            ReportedDate = now.AddDays(-45), ReportedBy = "Penetration Test",
            AssignedTo = "Alex Rivera", AffectedComponent = "Customer Search API",
            Recommendation = "Replace string concatenation with parameterized queries. Apply input validation and implement an ORM layer for all database operations.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0002", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "Insecure direct object reference in user profile endpoint",
            Description = "The /api/users/{id}/profile endpoint allows any authenticated user to access other users' profiles by changing the ID parameter. No authorization check verifies the requesting user has permission to view the target profile.",
            Severity = Severity.High, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-38), ReportedBy = "Code Review",
            AssignedTo = null, AffectedComponent = "User Profile Service",
            Recommendation = "Implement object-level authorization checks. Verify the authenticated user has the required role or relationship to access the requested profile.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0003", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "Missing rate limiting on login endpoint",
            Description = "The /api/auth/login endpoint has no rate limiting, allowing unlimited authentication attempts. This makes brute-force and credential stuffing attacks feasible.",
            Severity = Severity.Medium, Status = IssueStatus.Resolved,
            ReportedDate = now.AddDays(-60), ResolvedDate = now.AddDays(-30),
            ReportedBy = "OWASP ZAP Scan", AssignedTo = "Morgan Chen",
            AffectedComponent = "Authentication Service",
            Recommendation = "Implement rate limiting (max 5 attempts per minute per IP) and account lockout after 10 failed attempts.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0004", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "Sensitive data exposure in API error responses",
            Description = "API error responses include stack traces, internal class names, and database connection strings in development and production environments alike.",
            Severity = Severity.Medium, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-32), ReportedBy = "Static Analysis (Semgrep)",
            AssignedTo = null, AffectedComponent = "REST API Layer",
            Recommendation = "Configure environment-specific error handling. Production errors should return generic messages with correlation IDs only.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0005", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "Outdated jQuery library with known XSS vulnerabilities",
            Description = "The frontend dashboard uses jQuery 2.1.4 which has multiple known XSS vulnerabilities (CVE-2015-9251, CVE-2019-11358).",
            Severity = Severity.Low, Status = IssueStatus.Dismissed,
            ReportedDate = now.AddDays(-75), ResolvedDate = now.AddDays(-70),
            ReportedBy = "Dependency Scanner", AssignedTo = null,
            AffectedComponent = "Frontend Dashboard",
            Recommendation = "Upgrade jQuery to 3.7+ or migrate to a modern framework. Dismissed: Dashboard is being replaced in Q2.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0006", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "Weak password policy allows short passwords",
            Description = "The password policy accepts passwords as short as 4 characters with no complexity requirements. This makes user accounts vulnerable to brute-force attacks.",
            Severity = Severity.Medium, Status = IssueStatus.InProgress,
            ReportedDate = now.AddDays(-28), ReportedBy = "Code Review",
            AssignedTo = "Jamie Patel", AffectedComponent = "Authentication Service",
            Recommendation = "Enforce minimum 12 characters, require uppercase, lowercase, numbers, and special characters. Implement password strength meter.",
        });

        // Project Nexus (API Gateway)
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0007", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "JWT token not validated for expiration in gateway middleware",
            Description = "The API gateway middleware extracts JWT claims but does not validate the 'exp' claim. Expired tokens are accepted as valid, allowing indefinite access after token issuance.",
            Severity = Severity.Critical, Status = IssueStatus.Resolved,
            ReportedDate = now.AddDays(-55), ResolvedDate = now.AddDays(-40),
            ReportedBy = "Penetration Test", AssignedTo = "Taylor Kim",
            AffectedComponent = "API Gateway Core",
            Recommendation = "Add explicit expiration validation in the JWT middleware. Set maximum token lifetime to 1 hour with refresh token rotation.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0008", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "CORS misconfiguration allows wildcard origins",
            Description = "The API gateway CORS policy is configured with Access-Control-Allow-Origin: * combined with Access-Control-Allow-Credentials: true, which is a security anti-pattern.",
            Severity = Severity.High, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-35), ReportedBy = "OWASP ZAP Scan",
            AssignedTo = null, AffectedComponent = "API Gateway Core",
            Recommendation = "Replace wildcard origin with an explicit allowlist of trusted domains. Remove Allow-Credentials when using wildcard origins.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0009", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "API keys transmitted in URL query parameters",
            Description = "The client SDK sends API keys as query parameters (?api_key=...) instead of in headers. Query parameters are logged in server access logs, proxy logs, and browser history.",
            Severity = Severity.High, Status = IssueStatus.InProgress,
            ReportedDate = now.AddDays(-42), ReportedBy = "Code Review",
            AssignedTo = "Casey Brooks", AffectedComponent = "Client SDK",
            Recommendation = "Move API keys to the Authorization header. Update all client SDK versions and deprecate query parameter authentication.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0010", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "Missing TLS certificate validation for upstream services",
            Description = "The gateway's HTTP client disables TLS certificate validation for upstream service calls, making it vulnerable to man-in-the-middle attacks.",
            Severity = Severity.Medium, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-25), ReportedBy = "Static Analysis (Semgrep)",
            AssignedTo = null, AffectedComponent = "Service Mesh",
            Recommendation = "Enable TLS certificate validation. Use internal CA certificates for service-to-service communication within the cluster.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0011", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "Verbose error messages expose internal service topology",
            Description = "Error responses from the gateway include upstream service hostnames, ports, and internal network paths, leaking infrastructure details to external consumers.",
            Severity = Severity.Low, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-20), ReportedBy = "Bug Bounty Program",
            AssignedTo = null, AffectedComponent = "Error Handling Middleware",
            Recommendation = "Sanitize error responses to remove internal hostnames and network paths. Return only generic error messages with correlation IDs.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0012", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "Insufficient logging for authentication failures",
            Description = "Failed authentication attempts are not logged with sufficient detail (IP address, user agent, timestamp) for forensic analysis and anomaly detection.",
            Severity = Severity.Medium, Status = IssueStatus.Resolved,
            ReportedDate = now.AddDays(-50), ResolvedDate = now.AddDays(-35),
            ReportedBy = "Static Analysis (Semgrep)", AssignedTo = "Alex Rivera",
            AffectedComponent = "Audit Logging Service",
            Recommendation = "Log all authentication events with IP, user agent, timestamp, and outcome. Feed logs into SIEM for anomaly detection.",
        });

        // Project Orbit (Mobile App)
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0013", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Unencrypted local storage of authentication tokens",
            Description = "Authentication tokens are stored in plain text in AsyncStorage on both iOS and Android. Any app with device access or a rooted device can read these tokens.",
            Severity = Severity.High, Status = IssueStatus.InProgress,
            ReportedDate = now.AddDays(-40), ReportedBy = "Penetration Test",
            AssignedTo = "Morgan Chen", AffectedComponent = "Mobile Auth Module",
            Recommendation = "Use iOS Keychain and Android Keystore for secure token storage. Encrypt tokens at rest using platform-provided encryption APIs.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0014", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Certificate pinning not implemented for API calls",
            Description = "The mobile app does not implement certificate pinning, making it vulnerable to man-in-the-middle attacks using proxy tools or compromised CAs.",
            Severity = Severity.High, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-33), ReportedBy = "Penetration Test",
            AssignedTo = null, AffectedComponent = "Network Layer",
            Recommendation = "Implement certificate pinning for all API endpoints. Pin the intermediate CA certificate and implement a backup pin for certificate rotation.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0015", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Deep link scheme allows URL hijacking",
            Description = "The custom URL scheme (orbit://) can be claimed by any malicious app on the device, potentially intercepting authentication callbacks and sensitive deep links.",
            Severity = Severity.Medium, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-27), ReportedBy = "Code Review",
            AssignedTo = null, AffectedComponent = "Deep Link Handler",
            Recommendation = "Migrate from custom URL schemes to Universal Links (iOS) and App Links (Android) which are verified against the domain.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0016", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Biometric authentication bypass on rooted devices",
            Description = "The biometric authentication check can be bypassed on rooted/jailbroken devices by hooking into the authentication callback and forcing a success response.",
            Severity = Severity.Medium, Status = IssueStatus.Resolved,
            ReportedDate = now.AddDays(-58), ResolvedDate = now.AddDays(-32),
            ReportedBy = "Penetration Test", AssignedTo = "Jamie Patel",
            AffectedComponent = "Biometric Auth Module",
            Recommendation = "Add root/jailbreak detection. Require server-side verification for biometric auth. Use hardware-backed biometric APIs.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0017", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Screenshots not disabled on sensitive screens",
            Description = "Users can take screenshots of sensitive screens (account details, payment info) which may be synced to cloud services or accessed by other apps.",
            Severity = Severity.Low, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-18), ReportedBy = "Code Review",
            AssignedTo = null, AffectedComponent = "Screen Security",
            Recommendation = "Set FLAG_SECURE on Android and use UIScreen.isCaptured notifications on iOS for sensitive screens.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0018", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Third-party analytics SDK collecting excessive user data",
            Description = "The analytics SDK collects device identifiers, location data, and contact list hashes beyond what is disclosed in the privacy policy.",
            Severity = Severity.Medium, Status = IssueStatus.InProgress,
            ReportedDate = now.AddDays(-22), ReportedBy = "Static Analysis (Semgrep)",
            AssignedTo = "Taylor Kim", AffectedComponent = "Analytics Integration",
            Recommendation = "Audit SDK permissions and data collection. Disable unnecessary data collection APIs. Update privacy policy to reflect actual data practices.",
        });

        // Project Vault (Data Warehouse)
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0019", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Unencrypted PII data in ETL staging tables",
            Description = "Personal identifiable information (names, email addresses, phone numbers) is stored in plain text in ETL staging tables during the nightly transformation process.",
            Severity = Severity.Critical, Status = IssueStatus.InProgress,
            ReportedDate = now.AddDays(-48), ReportedBy = "Static Analysis (Semgrep)",
            AssignedTo = "Casey Brooks", AffectedComponent = "ETL Pipeline",
            Recommendation = "Encrypt PII columns in staging tables using column-level encryption. Implement data masking for non-production environments.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0020", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Overly permissive IAM roles for data lake access",
            Description = "The ETL service account has full read/write access to all data lake containers, violating the principle of least privilege. A compromised service account could access all organizational data.",
            Severity = Severity.High, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-36), ReportedBy = "Code Review",
            AssignedTo = null, AffectedComponent = "Cloud Infrastructure",
            Recommendation = "Implement granular IAM policies scoped to specific containers and operations. Use separate service accounts per pipeline.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0021", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Missing audit trail for data export operations",
            Description = "Data export operations (CSV downloads, API bulk exports) are not logged, making it impossible to track data exfiltration or comply with data access audit requirements.",
            Severity = Severity.Medium, Status = IssueStatus.Resolved,
            ReportedDate = now.AddDays(-52), ResolvedDate = now.AddDays(-28),
            ReportedBy = "Bug Bounty Program", AssignedTo = "Alex Rivera",
            AffectedComponent = "Data Export Service",
            Recommendation = "Log all export operations with user identity, export scope, row count, and timestamp. Send alerts for exports exceeding threshold size.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0022", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Hardcoded database credentials in configuration files",
            Description = "Production database connection strings with embedded credentials are stored in appsettings.json and checked into the source repository.",
            Severity = Severity.High, Status = IssueStatus.Resolved,
            ReportedDate = now.AddDays(-65), ResolvedDate = now.AddDays(-50),
            ReportedBy = "Dependency Scanner", AssignedTo = "Morgan Chen",
            AffectedComponent = "Configuration Management",
            Recommendation = "Migrate credentials to Azure Key Vault. Use managed identity for database authentication. Rotate compromised credentials immediately.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0023", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Backup files stored without encryption in S3 bucket",
            Description = "Database backup files are stored in an S3 bucket without server-side encryption enabled. The bucket policy allows read access from multiple AWS accounts.",
            Severity = Severity.Medium, Status = IssueStatus.Open,
            ReportedDate = now.AddDays(-30), ReportedBy = "OWASP ZAP Scan",
            AssignedTo = null, AffectedComponent = "Backup Service",
            Recommendation = "Enable S3 server-side encryption (SSE-KMS). Restrict bucket policy to the backup service account only. Enable bucket versioning and access logging.",
        });
        _issues.Add(new SecurityIssue
        {
            Id = "SEC-0024", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Column-level encryption missing for SSN and financial data",
            Description = "Social security numbers and financial account numbers are stored in plain text in the data warehouse dimension tables, accessible to all analysts with read access.",
            Severity = Severity.Low, Status = IssueStatus.InProgress,
            ReportedDate = now.AddDays(-15), ReportedBy = "Code Review",
            AssignedTo = "Jamie Patel", AffectedComponent = "Data Encryption Layer",
            Recommendation = "Implement column-level encryption using Always Encrypted or application-level encryption. Create separate decryption roles for authorized personnel only.",
        });
    }

    public string GetSecurityIssues(string? projectCode = null, string? severity = null, string? status = null)
    {
        var filtered = _issues.AsEnumerable();

        if (projectCode is not null)
            filtered = filtered.Where(i => i.ProjectCode.Equals(projectCode, StringComparison.OrdinalIgnoreCase));
        if (severity is not null && Enum.TryParse<Severity>(severity, ignoreCase: true, out var sev))
            filtered = filtered.Where(i => i.Severity == sev);
        if (status is not null && Enum.TryParse<IssueStatus>(status, ignoreCase: true, out var stat))
            filtered = filtered.Where(i => i.Status == stat);

        var issues = filtered.OrderBy(i => i.Severity).ThenByDescending(i => i.ReportedDate).ToList();

        var result = new { issues, totalCount = issues.Count };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetSecurityIssueDetail(string issueId)
    {
        var issue = _issues.FirstOrDefault(i => i.Id.Equals(issueId, StringComparison.OrdinalIgnoreCase));
        if (issue is null)
            return $"Error: Issue '{issueId}' not found.";

        var detail = new
        {
            issue.Id, issue.ProjectCode, issue.ProjectName, issue.Title, issue.Description,
            severity = issue.Severity.ToString(), status = issue.Status.ToString(),
            issue.ReportedDate, issue.ResolvedDate, issue.ReportedBy, issue.AssignedTo,
            issue.AffectedComponent, issue.Recommendation,
            daysSinceReported = (DateTime.UtcNow - issue.ReportedDate).Days,
        };
        return JsonSerializer.Serialize(detail, s_jsonOptions);
    }

    public string GetSecuritySummary(string? projectCode = null)
    {
        var filtered = _issues.AsEnumerable();
        if (projectCode is not null)
            filtered = filtered.Where(i => i.ProjectCode.Equals(projectCode, StringComparison.OrdinalIgnoreCase));

        var list = filtered.ToList();

        var bySeverity = Enum.GetValues<Severity>()
            .Select(s => new { severity = s.ToString(), count = list.Count(i => i.Severity == s) })
            .ToList();

        var byStatus = Enum.GetValues<IssueStatus>()
            .Select(s => new { status = s.ToString(), count = list.Count(i => i.Status == s) })
            .ToList();

        var byProject = list.GroupBy(i => new { i.ProjectCode, i.ProjectName })
            .Select(g => new
            {
                g.Key.ProjectCode, g.Key.ProjectName,
                total = g.Count(),
                open = g.Count(i => i.Status == IssueStatus.Open),
                inProgress = g.Count(i => i.Status == IssueStatus.InProgress),
                resolved = g.Count(i => i.Status == IssueStatus.Resolved),
                critical = g.Count(i => i.Severity == Severity.Critical),
                high = g.Count(i => i.Severity == Severity.High),
            })
            .OrderByDescending(p => p.critical).ThenByDescending(p => p.high)
            .ToList();

        // Risk score
        var openOrInProgress = list.Where(i => i.Status is IssueStatus.Open or IssueStatus.InProgress).ToList();
        string riskScore = openOrInProgress.Any(i => i.Severity == Severity.Critical) ? "Critical"
            : openOrInProgress.Any(i => i.Severity == Severity.High) ? "High"
            : openOrInProgress.Any(i => i.Severity == Severity.Medium) ? "Medium"
            : "Low";

        var result = new
        {
            totalIssues = list.Count,
            riskScore,
            bySeverity,
            byStatus,
            byProject,
        };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetIssuesByProject(string projectCode)
    {
        var issues = _issues
            .Where(i => i.ProjectCode.Equals(projectCode, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.Severity)
            .ThenByDescending(i => i.ReportedDate)
            .ToList();

        var stats = new
        {
            total = issues.Count,
            critical = issues.Count(i => i.Severity == Severity.Critical),
            high = issues.Count(i => i.Severity == Severity.High),
            medium = issues.Count(i => i.Severity == Severity.Medium),
            low = issues.Count(i => i.Severity == Severity.Low),
            open = issues.Count(i => i.Status == IssueStatus.Open),
            inProgress = issues.Count(i => i.Status == IssueStatus.InProgress),
            resolved = issues.Count(i => i.Status == IssueStatus.Resolved),
        };

        var result = new { projectCode, issues, statistics = stats };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetCriticalAndHighIssues()
    {
        var issues = _issues
            .Where(i => i.Severity is Severity.Critical or Severity.High
                     && i.Status is IssueStatus.Open or IssueStatus.InProgress)
            .OrderBy(i => i.Severity)
            .ThenByDescending(i => i.ReportedDate)
            .Select(i => new
            {
                i.Id, i.ProjectCode, i.ProjectName, i.Title,
                severity = i.Severity.ToString(), status = i.Status.ToString(),
                i.AffectedComponent, i.AssignedTo,
                daysSinceReported = (DateTime.UtcNow - i.ReportedDate).Days,
            })
            .ToList();

        var result = new
        {
            issues,
            totalCount = issues.Count,
            criticalCount = issues.Count(i => i.severity == "Critical"),
            highCount = issues.Count(i => i.severity == "High"),
        };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }
}
