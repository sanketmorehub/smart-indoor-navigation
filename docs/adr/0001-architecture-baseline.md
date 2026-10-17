# ADR 0001 - Architecture baseline (approved 2026-10-17)

| # | Decision | Rationale |
|---|---|---|
| 1 | Modular monolith (ASP.NET Core, .NET 9) | One team, one DB; modules can be split later |
| 2 | Shared MySQL DB, shared schema, `TenantId` on every tenant-owned row | Cheapest, scales to thousands of tenants |
| 3 | Isolation in 5 layers: server-side tenant context, EF global filters (fail-closed), resource checks (404 not 403), composite FKs incl. TenantId/BuildingId, automated isolation tests | MySQL has no row-level security |
| 4 | UUIDv7 primary keys as `BINARY(16)` | Non-enumerable IDs (IDOR defence) + ordered inserts |
| 5 | Enums persisted as VARCHAR; location categories configurable per tenant | No ALTER TABLE for new types; nothing hospital-specific |
| 6 | Navigation graph is the source of truth; SVG is visual only | Routing never parses SVG |
| 7 | Edge cost = time (seconds); distance shown to users | Fair lift-vs-stairs comparison |
| 8 | Draft tables + immutable published `GraphSnapshots` | Half-edited graphs never go live; instant rollback |
| 9 | QR = opaque 128-bit base62 token, URL `/q/{token}` | No IDs exposed; survives slug renames |
| 10 | Uploaded SVGs sanitized (allowlist), served from a cookie-less domain, rendered via `<image>` | Defence in depth against SVG XSS |
| 11 | Monorepo: `backend/`, `apps/visitor`, `apps/admin`, `packages/*` | Shared map renderer + generated API client |
