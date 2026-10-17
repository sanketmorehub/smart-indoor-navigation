# Smart Indoor Navigation System

A multi-tenant SaaS platform for "Google Maps inside buildings": hospitals, hotels, colleges, malls,
offices, government buildings, airports and more. Visitors scan a QR code (their **current location**),
search a **destination**, and get a shortest-path route drawn on an SVG floor map, with no app install.

> Status: **Phase 1-2 complete** - repository foundation, domain model, MySQL schema with
> tenant isolation, demo seed (ABC Hospital as data only), isolation tests.

## Repository layout

```
smart-indoor-navigation/
├── backend/                      ASP.NET Core (.NET 9) modular monolith
│   ├── SmartNav.slnx
│   ├── src/
│   │   ├── SmartNav.Api/             HTTP host, middleware, (later) endpoints
│   │   ├── SmartNav.Application/     Use cases, DTOs, abstractions (ITenantContext...)
│   │   ├── SmartNav.Domain/          Entities & enums - no dependencies
│   │   ├── SmartNav.Infrastructure/  EF Core (MySQL), tenant filters, interceptor, seeding
│   │   ├── SmartNav.Navigation/      Pure routing library (Dijkstra -> A*) - Phase 7
│   │   └── SmartNav.Shared/          IDs (UUIDv7), clock, common types
│   └── tests/SmartNav.Infrastructure.Tests/   Tenant-isolation tests
├── apps/                         (Phase 9/11) visitor + admin React apps
├── packages/                     (Phase 9/10) map-renderer, api-client, ui
├── database/schema/              Generated MySQL DDL (see below)
├── docs/adr/                     Architecture decision records
├── docker-compose.yml            MySQL 8 + MinIO for local dev
└── .github/workflows/backend-ci.yml
```

## Quick start (local)

Prerequisites: .NET 9 SDK, Docker, `dotnet tool install --global dotnet-ef --version 9.0.9`

```bash
docker compose up -d mysql

cd backend
dotnet build
dotnet test                                   # 9 tenant-isolation / QR token tests

# 1) Create the initial EF migration (generated locally, then commit it)
dotnet ef migrations add InitialSchema -p src/SmartNav.Infrastructure -s src/SmartNav.Infrastructure -o Persistence/Migrations

# (optional) Export readable MySQL DDL for review
dotnet ef migrations script -p src/SmartNav.Infrastructure -s src/SmartNav.Infrastructure -o ../database/schema/001_initial_schema.sql

# 2) Apply schema + seed demo tenant. The demo owner password comes from user-secrets, never from git.
dotnet user-secrets init --project src/SmartNav.Api
dotnet user-secrets set "Seed:DemoOwnerPassword" "<choose-a-strong-password>" --project src/SmartNav.Api
dotnet run --project src/SmartNav.Api -- migrate-and-seed
# Logs print the 4 demo QR URLs: /q/{token}

dotnet run --project src/SmartNav.Api          # GET http://localhost:5062/health
```

## Tenant isolation (how it is enforced)

1. **Tenant context is server-side only** (`ITenantContext`): from the JWT `tid` claim for admins,
   from the QR token for visitors. Never from request bodies/routes.
2. **EF Core global query filters** on every `ITenantOwned` entity - fail-closed (no tenant => no rows).
3. **`AuditableTenantInterceptor`** stamps `TenantId` on insert, blocks forged/foreign TenantIds and any
   TenantId change.
4. **Composite foreign keys** - e.g. `Floors(TenantId, BuildingId) -> Buildings(TenantId, Id)` and
   `NavigationEdges(BuildingId, FromNodeId) -> NavigationNodes(BuildingId, Id)`. The database rejects
   cross-tenant / cross-building links even if application code is wrong.
5. **Tests** (`TenantIsolationTests`) prove cross-tenant reads return nothing (-> HTTP 404).

## Data model (Phase 2)

Tenants, TenantBranding, TenantSettings, Users, Roles, Permissions, RolePermissions,
TenantMemberships, RefreshTokens, FileAssets, Buildings, BuildingSettings, Floors, FloorMaps,
LocationCategories, Locations, VerticalConnectors, NavigationNodes, NavigationEdges,
GraphSnapshots, QrCodes, QrScanEvents, AuditLogs.

See `docs/adr/0001-architecture-baseline.md` for the decisions behind it.

## Demo data (seed, not code)

ABC Hospital -> Main Building -> Ground (Reception, Emergency, Medical Store, Lift A, Stairs 1) and
First (Cardiology, Neurology, Lift A, Stairs 1). 14 nodes, 14 edges (incl. a lift edge and a
non-accessible stair edge), 4 QR codes: Reception, Cardiology, Lift A Ground, Lift A First.

## Roadmap

| Phase | Scope | Status |
|---|---|---|
| 1 | Repo, solution skeleton, docker-compose, CI | done |
| 2 | Domain, EF Core schema, tenant isolation, seed, tests | done |
| 3 | API foundation: ProblemDetails, validation, logging, OpenAPI | next |
| 4 | Auth: JWT + refresh rotation, switch-tenant, permission policies | |
| 5 | Buildings/floors/maps (SVG sanitizer + storage)/locations | |
| 6 | Graph editing, validation, publish snapshots | |
| 7 | Navigation engine: Dijkstra, profiles, instructions | |
| 8 | QR resolution, visitor context, PNG/SVG rendering | |
| 9-10 | Visitor web app + SVG route overlay | |
| 11 | Admin web app + simple graph editor | |
| 12-13 | Hardening, E2E tests, deployment | |
