using System.Text.Json;

namespace McpAguiServer.KnowledgeBase;

internal sealed class KnowledgeArticleStore
{
    private readonly List<KnowledgeArticle> _articles = [];
    private static readonly JsonSerializerOptions s_jsonOptions = new() { WriteIndented = false };

    public KnowledgeArticleStore()
    {
        SeedData();
    }

    private void SeedData()
    {
        var rng = new Random(99);
        var now = DateTime.UtcNow;

        // Project Alpha (CRM)
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0001", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "Resolving OAuth 2.0 Token Refresh Failures in CRM Module",
            Category = "Troubleshooting", Tags = ["auth", "oauth", "token", "crm"],
            Author = "Alex Rivera", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-80), LastUpdated = now.AddDays(-5),
            Content = "OAuth 2.0 token refresh failures in the CRM module typically occur when the refresh token has expired or the token endpoint URL has changed. The CRM module uses a background service to refresh tokens 5 minutes before expiration.\n\nTo resolve this issue, first check the token endpoint configuration in appsettings.json. Ensure the 'TokenEndpoint' URL matches the identity provider's current endpoint. Next, verify that the refresh token lifetime hasn't been reduced on the identity provider side.\n\nIf tokens are still failing to refresh, enable debug logging for the 'CRM.Auth' namespace and check for specific error codes. Common error codes include 'invalid_grant' (refresh token expired) and 'invalid_client' (client credentials changed). In most cases, re-authenticating the service account resolves the issue.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0002", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "How to Configure LDAP Integration for User Sync",
            Category = "How-To", Tags = ["ldap", "user-sync", "configuration"],
            Author = "Morgan Chen", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-70), LastUpdated = now.AddDays(-12),
            Content = "LDAP integration allows the CRM to synchronize user accounts from Active Directory. This guide covers the configuration steps for setting up bi-directional sync.\n\nStep 1: Navigate to Admin > Integrations > LDAP Configuration. Enter the LDAP server URL (ldaps://your-dc.domain.com:636), bind DN, and credentials. Step 2: Configure the search base DN to target the correct OU. Step 3: Map LDAP attributes to CRM user fields (sAMAccountName → username, mail → email, displayName → fullName).\n\nSchedule sync frequency (recommended: every 15 minutes for active directories with < 10,000 users). Enable 'Delta Sync' to process only changes since the last sync. Monitor sync logs under Admin > Logs > LDAP Sync for any attribute mapping errors.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0003", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "CRM Database Migration Best Practices — PostgreSQL to Azure SQL",
            Category = "Best Practice", Tags = ["database", "migration", "azure-sql"],
            Author = "Jamie Patel", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-60), LastUpdated = now.AddDays(-8),
            Content = "Migrating the CRM database from PostgreSQL to Azure SQL requires careful planning to handle data type differences and query syntax changes. This article outlines best practices from our successful migration.\n\nFirst, use Azure Database Migration Service (DMS) for schema conversion. Key type mappings: PostgreSQL 'text' → Azure SQL 'nvarchar(max)', 'jsonb' → 'nvarchar(max)' with JSON functions, 'serial' → 'int identity'. Second, convert all PostgreSQL-specific functions (e.g., 'now()' → 'GETUTCDATE()', array operations → JSON arrays).\n\nPerformance considerations: Azure SQL uses clustered indexes differently than PostgreSQL. Review all table schemas and add appropriate clustered indexes. Test with production-scale data before cutover. Use Azure SQL's query store to identify regression queries after migration.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0004", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "Architecture Overview — Alpha CRM Event-Driven Design",
            Category = "Architecture", Tags = ["architecture", "event-driven", "microservices"],
            Author = "Taylor Kim", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-90), LastUpdated = now.AddDays(-15),
            Content = "The Alpha CRM uses an event-driven architecture built on Azure Service Bus. Domain events flow through topic subscriptions, enabling loose coupling between modules.\n\nCore event flow: User actions trigger commands → Command handlers validate and persist → Domain events published to Service Bus → Subscribers process events asynchronously. Key topics: 'customer-events', 'order-events', 'notification-events'. Each module owns its data store (Customer module → CosmosDB, Order module → Azure SQL, Analytics → Data Lake).\n\nThe architecture supports eventual consistency. All cross-module reads use materialized views refreshed via event handlers. Saga orchestration handles multi-step processes like order fulfillment. The event store retains all events for 90 days, enabling replay for debugging and new projection creation.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0005", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "Deploying Alpha CRM to Kubernetes — Step by Step",
            Category = "Deployment", Tags = ["kubernetes", "deployment", "helm"],
            Author = "Casey Brooks", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-50), LastUpdated = now.AddDays(-3),
            Content = "This guide covers deploying the Alpha CRM application to an AKS (Azure Kubernetes Service) cluster using Helm charts.\n\nPrerequisites: AKS cluster provisioned, kubectl configured, Helm 3.x installed, ACR (Azure Container Registry) with CRM images pushed. Step 1: Add the CRM Helm repository and update. Step 2: Create a values-production.yaml with environment-specific overrides (replicas, resource limits, ingress host). Step 3: Install the chart with 'helm install alpha-crm ./charts/alpha-crm -f values-production.yaml -n crm-prod'.\n\nPost-deployment verification: Check pod status with 'kubectl get pods -n crm-prod'. Verify health endpoints respond on /healthz. Run smoke tests via the CI pipeline. Monitor resource usage in Azure Monitor for the first 24 hours after deployment.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0006", ProjectCode = "ALPHA", ProjectName = "Project Alpha",
            Title = "Troubleshooting Slow Queries in Customer Search Module",
            Category = "Troubleshooting", Tags = ["performance", "sql", "search", "indexing"],
            Author = "Alex Rivera", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-40), LastUpdated = now.AddDays(-2),
            Content = "Slow customer search queries (>2s response time) are typically caused by missing indexes or parameter sniffing issues in the search stored procedure.\n\nDiagnosis: Run the slow query with SET STATISTICS IO ON to identify table scans. Check the execution plan for missing index recommendations. Common culprits: the 'CustomerName' column lacks a full-text index, the 'LastActivityDate' column needs a filtered index for active customers only.\n\nResolution: Add the recommended indexes using the provided index creation scripts. For parameter sniffing, add OPTION (RECOMPILE) to the search procedure or use OPTIMIZE FOR UNKNOWN. After applying indexes, verify improved performance with the included benchmark queries. Expected improvement: search response time should drop to < 200ms for 95th percentile.",
        });

        // Project Nexus (API Gateway)
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0007", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "API Gateway Rate Limiting — Configuration and Tuning",
            Category = "How-To", Tags = ["rate-limiting", "api-gateway", "configuration"],
            Author = "Morgan Chen", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-75), LastUpdated = now.AddDays(-10),
            Content = "Rate limiting in the Nexus API Gateway uses a sliding window algorithm with Redis-backed counters. This guide covers configuration for different client tiers.\n\nConfiguration is defined in gateway-config.yaml under the 'rateLimiting' section. Default limits: Free tier (100 req/min, 1000 req/hour), Standard tier (500 req/min, 10000 req/hour), Premium tier (2000 req/min, unlimited hourly). Rate limit headers (X-RateLimit-Remaining, X-RateLimit-Reset) are automatically added to all responses.\n\nTuning tips: Monitor Redis memory usage as counters scale with unique client IDs. Use the 'burstAllowance' setting (default: 1.5x) to permit short traffic spikes without triggering limits. For webhook endpoints that receive batch events, configure endpoint-specific overrides with higher limits.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0008", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "Resolving 502 Bad Gateway Errors in Nexus Load Balancer",
            Category = "Troubleshooting", Tags = ["502", "load-balancer", "nginx"],
            Author = "Jamie Patel", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-65), LastUpdated = now.AddDays(-7),
            Content = "502 Bad Gateway errors in the Nexus load balancer indicate that Nginx cannot reach upstream services. This is the most common production issue reported for the API gateway.\n\nImmediate triage: Check upstream service health with 'kubectl get pods -n nexus'. Look for pods in CrashLoopBackOff or OOMKilled state. Check Nginx error logs for 'upstream timed out' or 'no live upstreams' messages. Common causes: (1) upstream service crashed due to memory pressure, (2) health check endpoint returns 5xx, (3) Nginx upstream timeout too short for slow endpoints.\n\nResolution: Increase proxy_read_timeout for known slow endpoints (e.g., report generation). Add circuit breaker configuration to prevent cascade failures. Ensure readiness probes match actual service startup time. Monitor with Prometheus alerts on 5xx rate exceeding 1% over 5 minutes.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0009", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "Microservices Circuit Breaker Pattern — Implementation Guide",
            Category = "Best Practice", Tags = ["circuit-breaker", "resilience", "polly"],
            Author = "Taylor Kim", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-55), LastUpdated = now.AddDays(-14),
            Content = "The circuit breaker pattern prevents cascade failures when downstream services are unhealthy. Nexus uses Polly for .NET services and resilience4j for Java services.\n\nConfiguration: Set failure threshold to 5 consecutive failures, break duration to 30 seconds, and half-open max attempts to 3. Define separate circuit breaker policies per downstream service. Log all state transitions (Closed → Open → HalfOpen → Closed) for monitoring.\n\nImplementation: Register Polly policies in DI container and apply via HttpClientFactory. For critical paths (authentication, payment), use a more aggressive threshold (3 failures, 60s break). For non-critical paths (analytics, recommendations), use lenient settings (10 failures, 15s break). Always provide fallback responses when the circuit is open.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0010", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "Nexus API Versioning Strategy and URL Schema",
            Category = "Architecture", Tags = ["api-versioning", "rest", "url-schema"],
            Author = "Casey Brooks", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-85), LastUpdated = now.AddDays(-20),
            Content = "Nexus uses URL-based API versioning with a structured schema that supports parallel version deployment and gradual deprecation.\n\nURL format: /api/v{major}/{service}/{resource}. Example: /api/v2/users/profile. Breaking changes require a new major version. Non-breaking additions (new optional fields, new endpoints) are added to the current version. The gateway routes to version-specific service instances using Kubernetes service selectors.\n\nDeprecation policy: Announce deprecation 6 months before removal. Add 'Sunset' and 'Deprecation' headers to deprecated version responses. Maintain at most 2 major versions simultaneously. Use API analytics to track adoption of new versions before removing old ones.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0011", ProjectCode = "NEXUS", ProjectName = "Project Nexus",
            Title = "Deploying Nexus Services with Blue-Green Strategy",
            Category = "Deployment", Tags = ["blue-green", "deployment", "zero-downtime"],
            Author = "Alex Rivera", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-45), LastUpdated = now.AddDays(-6),
            Content = "Blue-green deployment for Nexus services ensures zero-downtime releases by maintaining two identical production environments.\n\nProcess: (1) Deploy new version to 'green' environment while 'blue' serves traffic. (2) Run automated smoke tests against green. (3) Switch the load balancer to point to green. (4) Monitor error rates for 15 minutes. (5) If stable, decommission blue. If errors spike, switch back to blue immediately.\n\nAKS implementation: Use Kubernetes services with label selectors (version: blue/green). The switch is a single label change on the service selector. Database migrations must be backward-compatible — both blue and green must work with the same schema version during the transition window.",
        });

        // Project Orbit (Mobile App)
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0012", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Fixing Push Notification Delivery Failures on iOS",
            Category = "Troubleshooting", Tags = ["push-notifications", "ios", "apns"],
            Author = "Morgan Chen", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-72), LastUpdated = now.AddDays(-9),
            Content = "Push notification delivery failures on iOS typically stem from expired APNs certificates, invalid device tokens, or payload size exceeding the 4KB limit.\n\nDiagnosis: Check the APNs response codes in the notification service logs. Common codes: 410 (device token no longer active — user uninstalled), 403 (certificate expired or mismatched bundle ID), 413 (payload too large). The notification service retries failed deliveries 3 times with exponential backoff.\n\nResolution: For certificate issues, generate a new APNs certificate in Apple Developer Portal and update the server configuration. For invalid tokens, implement token cleanup by processing APNs feedback service data daily. For payload size, compress the notification payload by removing unnecessary custom data and using 'content-available' for silent notifications with background fetch.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0013", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "How to Implement Offline-First Data Sync in Orbit",
            Category = "How-To", Tags = ["offline", "sync", "sqlite", "mobile"],
            Author = "Jamie Patel", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-62), LastUpdated = now.AddDays(-11),
            Content = "Orbit's offline-first architecture uses SQLite for local storage and a conflict-resolution sync protocol for bi-directional data synchronization.\n\nLocal storage: All user data is stored in SQLite with a 'syncStatus' column (synced, pending, conflicted). The sync engine tracks changes using a local change log table that records insert/update/delete operations with timestamps.\n\nSync protocol: On connectivity change or periodic trigger (every 5 minutes when online), the sync engine pushes local changes to the server and pulls remote changes. Conflict resolution uses 'last-write-wins' by default, with manual resolution UI for critical data (e.g., customer records). The server maintains a vector clock per entity to detect concurrent modifications.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0014", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Mobile App Performance Optimization — Reducing Bundle Size",
            Category = "Best Practice", Tags = ["performance", "bundle-size", "lazy-loading"],
            Author = "Taylor Kim", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-52), LastUpdated = now.AddDays(-4),
            Content = "Orbit's mobile app bundle size was reduced from 45MB to 28MB through a combination of code splitting, asset optimization, and dependency auditing.\n\nCode splitting: Implement lazy loading for feature modules using React Navigation's lazy() option. Only the core module (auth, navigation, home) loads at startup. Feature modules load on first access. Asset optimization: Convert all PNG assets to WebP (40% size reduction). Use vector graphics (SVG) for icons instead of icon fonts. Remove unused font weights.\n\nDependency audit: Replace moment.js (320KB) with date-fns (tree-shakeable, 12KB for used functions). Remove lodash in favor of native Array/Object methods. Use the metro bundle analyzer to identify large dependencies and evaluate alternatives.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0015", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Orbit App Architecture — MVVM with Clean Architecture",
            Category = "Architecture", Tags = ["mvvm", "clean-architecture", "mobile"],
            Author = "Casey Brooks", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-88), LastUpdated = now.AddDays(-18),
            Content = "Orbit follows MVVM (Model-View-ViewModel) combined with Clean Architecture principles to maintain separation of concerns and testability.\n\nLayer structure: Presentation (Views + ViewModels) → Domain (Use Cases + Entities) → Data (Repositories + Data Sources). The domain layer has zero dependencies on frameworks or libraries. ViewModels expose state via observable streams and handle user interactions by invoking use cases.\n\nDependency injection uses InversifyJS with module-scoped containers. Each feature module registers its own dependencies. Cross-cutting concerns (logging, analytics, error reporting) are handled via decorators applied at the DI level. This architecture enables testing each layer independently — ViewModels with mock use cases, use cases with mock repositories.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0016", ProjectCode = "ORBIT", ProjectName = "Project Orbit",
            Title = "Configuring CI/CD Pipeline for Multi-Platform Mobile Builds",
            Category = "Deployment", Tags = ["ci-cd", "mobile", "android", "ios"],
            Author = "Alex Rivera", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-42), LastUpdated = now.AddDays(-1),
            Content = "The Orbit CI/CD pipeline uses Azure DevOps with platform-specific build agents for iOS (macOS) and Android (Linux) builds.\n\nPipeline stages: (1) Lint & unit tests (both platforms, Linux agent). (2) iOS build (macOS agent with Xcode 15, code signing via Fastlane match). (3) Android build (Linux agent with Gradle, signing via secure files). (4) UI tests (device farm with 10 device configurations). (5) Deploy to TestFlight (iOS) and Internal Testing track (Android).\n\nBranch strategy: Feature branches trigger stages 1-2. Main branch triggers full pipeline including deployment. Release branches trigger production deployment to App Store Connect and Google Play Console. Build caching reduces pipeline time from 45min to 20min by caching CocoaPods, Gradle dependencies, and node_modules.",
        });

        // Project Vault (Data Warehouse)
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0017", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Troubleshooting ETL Pipeline Failures in Nightly Jobs",
            Category = "Troubleshooting", Tags = ["etl", "pipeline", "data-warehouse"],
            Author = "Morgan Chen", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-68), LastUpdated = now.AddDays(-6),
            Content = "Nightly ETL pipeline failures are most commonly caused by source system schema changes, data volume spikes, or timeout issues in the transformation phase.\n\nTriage steps: Check the Airflow DAG run logs for the failed task. Identify the specific stage: Extract (source connectivity), Transform (data quality), or Load (target capacity). Common failures: (1) Source API returns 429 (rate limited) — increase retry count and add jitter. (2) NULL values in required fields — add data quality checks before transformation. (3) Load timeout — batch the insert operations.\n\nPrevention: Implement schema drift detection that compares source schemas nightly before ETL runs. Set up data volume alerts when incoming data exceeds 2x the 30-day average. Add circuit breakers to skip non-critical data sources when they fail, allowing the rest of the pipeline to complete.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0018", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "How to Add a New Data Source to the Vault Ingestion Layer",
            Category = "How-To", Tags = ["data-source", "ingestion", "configuration"],
            Author = "Jamie Patel", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-58), LastUpdated = now.AddDays(-13),
            Content = "Adding a new data source to Vault involves configuring the ingestion connector, defining the schema mapping, and scheduling the extraction job.\n\nStep 1: Create a connector configuration in /config/sources/. Specify connection type (REST API, JDBC, file-based), authentication, and extraction endpoint. Step 2: Define the schema mapping in /config/mappings/ — map source fields to Vault's canonical schema. Include data type conversions and default values. Step 3: Create an Airflow DAG using the ingestion template. Configure schedule (cron expression), retry policy, and SLA.\n\nTesting: Use the 'dry-run' mode to validate extraction without loading data. Check the staging table for correct data types and values. Run the full pipeline with a date range of 1 day to verify end-to-end flow. Add monitoring alerts for the new source in Grafana.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0019", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Data Partitioning Strategies for Large-Scale Analytics",
            Category = "Best Practice", Tags = ["partitioning", "performance", "analytics"],
            Author = "Taylor Kim", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-48), LastUpdated = now.AddDays(-10),
            Content = "Effective data partitioning is critical for query performance in Vault's multi-terabyte data warehouse. This article covers partitioning strategies for different table types.\n\nFact tables: Partition by date (monthly for tables > 1TB, daily for high-velocity tables). Use range partitioning with automatic partition creation. Archive partitions older than 2 years to cold storage (Azure Blob). Dimension tables: Generally small enough to avoid partitioning. Use hash distribution for large dimensions (> 10M rows).\n\nQuery optimization: Partition pruning eliminates scanning irrelevant partitions — always include the partition key in WHERE clauses. For cross-partition queries, use materialized views pre-aggregated by common dimensions. Monitor partition skew using the partition statistics dashboard — rebalance if any partition exceeds 3x the average size.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0020", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Vault Data Architecture — Star Schema Design",
            Category = "Architecture", Tags = ["star-schema", "dimensional-modeling", "warehouse"],
            Author = "Casey Brooks", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-82), LastUpdated = now.AddDays(-16),
            Content = "Vault uses a star schema design optimized for analytical queries. The schema consists of central fact tables surrounded by dimension tables.\n\nFact tables: FactSales (grain: individual transaction), FactUserActivity (grain: user action event), FactSystemMetrics (grain: 1-minute interval). Dimension tables: DimCustomer, DimProduct, DimDate, DimGeography, DimChannel. Each dimension includes a surrogate key (integer) and natural key (source system ID) with Type 2 SCD (Slowly Changing Dimensions) for tracking historical changes.\n\nNaming conventions: Fact tables prefixed with 'Fact', dimensions with 'Dim'. Foreign keys in fact tables use the pattern '{DimensionName}Key'. Date keys use YYYYMMDD integer format for efficient partition pruning. All monetary values stored in a common currency (USD) with the original currency and exchange rate preserved in separate columns.",
        });
        _articles.Add(new KnowledgeArticle
        {
            Id = "KB0021", ProjectCode = "VAULT", ProjectName = "Project Vault",
            Title = "Deploying Vault ETL Updates with Zero-Downtime Migrations",
            Category = "Deployment", Tags = ["deployment", "migration", "etl", "zero-downtime"],
            Author = "Alex Rivera", ViewCount = rng.Next(50, 501),
            CreatedDate = now.AddDays(-38), LastUpdated = now.AddDays(-2),
            Content = "Deploying ETL changes to the Vault data warehouse requires zero-downtime strategies to avoid disrupting scheduled jobs and downstream consumers.\n\nMigration strategy: Use expand-contract pattern for schema changes. Phase 1 (Expand): Add new columns/tables alongside existing ones. Update ETL to populate both old and new structures. Phase 2 (Migrate): Switch consumers to read from new structures. Phase 3 (Contract): Remove old structures after all consumers have migrated.\n\nDeployment checklist: (1) Run migration scripts in staging environment. (2) Verify ETL compatibility with both old and new schemas. (3) Deploy during the maintenance window (02:00-04:00 UTC) between nightly and morning jobs. (4) Monitor data quality dashboards for 24 hours post-deployment. (5) Keep rollback scripts ready — they should reverse Phase 1 expand operations.",
        });
    }

    public string SearchArticles(string query, string? projectCode = null, string? category = null)
    {
        var queryLower = query.ToLowerInvariant();
        var filtered = _articles.AsEnumerable();

        if (projectCode is not null)
            filtered = filtered.Where(a => a.ProjectCode.Equals(projectCode, StringComparison.OrdinalIgnoreCase));
        if (category is not null)
            filtered = filtered.Where(a => a.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        // Score: title match > tag match > content match
        var scored = filtered.Select(a =>
        {
            int score = 0;
            if (a.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) score += 10;
            if (a.Tags.Any(t => t.Contains(queryLower))) score += 5;
            if (a.Content.Contains(query, StringComparison.OrdinalIgnoreCase)) score += 1;
            return new { Article = a, Score = score };
        })
        .Where(x => x.Score > 0)
        .OrderByDescending(x => x.Score)
        .Select(x => new
        {
            x.Article.Id,
            x.Article.Title,
            x.Article.ProjectCode,
            x.Article.ProjectName,
            x.Article.Category,
            x.Article.Tags,
            x.Article.Author,
            x.Article.ViewCount,
            preview = x.Article.Content.Length > 200 ? x.Article.Content[..200] + "..." : x.Article.Content,
        })
        .ToList();

        var result = new { articles = scored, totalResults = scored.Count, query };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetArticle(string articleId)
    {
        var article = _articles.FirstOrDefault(a => a.Id.Equals(articleId, StringComparison.OrdinalIgnoreCase));
        if (article is null)
            return $"Error: Article '{articleId}' not found.";

        return JsonSerializer.Serialize(article, s_jsonOptions);
    }

    public string ListArticlesByProject(string projectCode)
    {
        var articles = _articles
            .Where(a => a.ProjectCode.Equals(projectCode, StringComparison.OrdinalIgnoreCase))
            .Select(a => new
            {
                a.Id, a.Title, a.Category, a.Tags, a.Author, a.ViewCount,
                lastUpdated = a.LastUpdated.ToString("yyyy-MM-dd"),
            })
            .ToList();

        var result = new { projectCode, articles, totalCount = articles.Count };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string ListArticlesByCategory(string category, string? projectCode = null)
    {
        var filtered = _articles.Where(a => a.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        if (projectCode is not null)
            filtered = filtered.Where(a => a.ProjectCode.Equals(projectCode, StringComparison.OrdinalIgnoreCase));

        var articles = filtered
            .Select(a => new
            {
                a.Id, a.Title, a.ProjectCode, a.ProjectName, a.Tags, a.Author, a.ViewCount,
                lastUpdated = a.LastUpdated.ToString("yyyy-MM-dd"),
            })
            .ToList();

        var result = new { category, projectCode, articles, totalCount = articles.Count };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }

    public string GetPopularArticles(int? count = null)
    {
        int take = Math.Min(count ?? 5, 10);
        var articles = _articles
            .OrderByDescending(a => a.ViewCount)
            .Take(take)
            .Select(a => new
            {
                a.Id, a.Title, a.ProjectCode, a.ProjectName, a.Category, a.ViewCount,
                lastUpdated = a.LastUpdated.ToString("yyyy-MM-dd"),
            })
            .ToList();

        var result = new { articles, totalCount = articles.Count };
        return JsonSerializer.Serialize(result, s_jsonOptions);
    }
}
