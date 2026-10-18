# Smart Indoor Navigation

SmartNav is a browser-based indoor navigation MVP. Visitors set their current location by
scanning a location QR code (or selecting a checkpoint), search the published hospital
destinations, and receive a shortest-time route, floor-by-floor map overlay, distance, walking
estimate, and step-by-step directions. The visitor app uses React/Vite, the API uses ASP.NET
Core/.NET 9, and persistent data uses MySQL with EF Core.

## Run the complete local demo

Prerequisites: Docker Desktop with Compose v2.

From the repository root:

```powershell
docker compose up --build
```

The API waits for MySQL, applies EF migrations, idempotently seeds and publishes the Aura Hospital
demo graph, then starts the visitor website. The illustrative directory includes 12 levels
(Basement 2, Basement 1, Ground, and Floors 1–9) and searchable destinations based on the supplied
hospital service list. Open:

- Visitor app: <http://localhost:8080>
- API health: <http://localhost:5062/health>
- API readiness (includes MySQL): <http://localhost:5062/health/ready>
- Development API docs: <http://localhost:5062/swagger>

To open the visitor app on another device connected to the same Wi-Fi, find this computer's
private IPv4 address with `ipconfig`. The visitor container listens on all host interfaces;
allow Docker Desktop through Windows Defender Firewall on **Private networks** if prompted.
For plain HTTP (navigation only), open `http://<computer-ipv4>:8080` on the other device.

Browser location and camera permissions require a secure context. For a temporary HTTPS link on
your Wi-Fi, create a root `.env` file with `LAN_HOST=<computer-ipv4>` and start the local
certificate proxy:

```powershell
docker compose --profile lan-https up --build -d
docker compose exec lan-https cat /data/caddy/pki/authorities/local/root.crt
```

Open `https://<computer-ipv4>:8443` on the other device. Install and trust the printed Caddy
root certificate on that device, then reload the page and grant location/camera permission.
The local CA is private to this demo and is not a public website certificate. Do not use this
setup as a public internet deployment.

On the visitor page, **Use my phone location** requests browser GPS and displays the live
coordinates and reported accuracy. Location stays in that browser and is not sent to the API;
opening the optional Google Maps link explicitly shares those coordinates with Google. GPS is a
best-effort aid outdoors; indoor accuracy and floor detection are not reliable without
a surveyed georeferenced map and indoor positioning equipment. Route calculation continues from
the selected QR checkpoint so the app does not invent a location on its schematic indoor map.

To inspect QR tokens for placing the demo QR codes, read the API logs:

```powershell
docker compose logs api
```

The API logs the demo checkpoint paths as `/q/<token>`; encode the full deployed HTTPS URL,
such as `https://your-domain.example/q/<token>`, when making a physical QR label. Opening that
link directly sets the visitor's starting location. The visitor app also accepts a full QR URL or
token through **Scan a location QR code**. Camera scanning requires browser camera permission;
the app must be served from HTTPS on a deployed site (localhost is allowed for local testing).

For local Vite development, start MySQL/API first and then run `npm ci` and `npm run dev` from
`apps/`. The Vite development server proxies `/api` to `http://127.0.0.1:5062`.

## Visitor API

All visitor endpoints read **published graph snapshots**, not admin draft data. Tenant scope is
resolved server-side from the building slug or opaque QR token.

| Method | Path | Purpose |
|---|---|---|
| `GET` | `/api/public/buildings/{tenantSlug}` | Published floors, searchable destinations, map graph, and active manual-start checkpoints |
| `GET` | `/api/public/qr/{token}` | Validate a QR token, record its scan, and resolve the current floor/node |
| `POST` | `/api/public/buildings/{tenantSlug}/routes` | Calculate a route from an active checkpoint to a public destination |
| `GET` | `/health` | Process liveness |
| `GET` | `/health/ready` | Database readiness |

Route request:

```json
{
  "startNodeId": "GUID from a listed checkpoint or resolved QR",
  "destinationLocationId": "GUID from the building's public locations",
  "accessibleOnly": true
}
```

The route response includes ordered node IDs, map-coordinate legs, distance, estimated time,
and individual floor-aware instructions. Routing weights minimize estimated travel time, with
lift wait/floor costs; accessible mode excludes inaccessible nodes, stairs, and restricted links.

## Deployment

The repository includes production-build containers: `backend/Dockerfile` for ASP.NET Core and
`apps/Dockerfile` for the static React visitor app. The visitor Nginx config serves the SPA and
proxies `/api` and `/health` to the API service on the same origin. A hosting provider or ingress
must terminate TLS; camera scanning will not work on a public plain-HTTP origin.

Before public deployment:

1. Use a managed/private MySQL instance, a unique strong database password, encrypted backups,
   and a least-privilege application account. Do not expose MySQL publicly.
2. Set `ConnectionStrings__Default` as a hosting secret. Never use the development values in
   `docker-compose.yml` for a public deployment.
3. Set `Cors__AllowedOrigins__0` (and any additional indexed origins) to the exact HTTPS visitor
   origin when the browser app and API are hosted separately. The API refuses to start in
   Production without an explicit CORS allow-list.
4. Set `AllowedHosts` to the API hostname, deploy behind HTTPS, and keep Swagger disabled outside
   Development.
5. Build/publish the API and frontend containers, run database migrations as a deployment step,
   and run the idempotent demo seeder only for a demo installation. Set `VITE_API_BASE_URL` at
   frontend build time only when the API is not reverse-proxied at the same origin.
6. Replace the illustrative Aura Hospital floor assignments, nodes, and distances with an approved
   building survey. The generated floor graph is a navigable demo layout, not the hospital's actual
   floor map and not an approved emergency or accessibility map.

Example API configuration is provided by environment variables rather than a committed
connection-string secret. Copy `apps/.env.example` to configure the frontend build when needed.

## Current scope

The **visitor navigation MVP** is connected end-to-end to the seeded Aura Hospital database and
published Dijkstra graph. The database/domain model is multi-tenant and includes admin-oriented
entities, but an authenticated admin console, owner login/authorization, building/floor/map CRUD,
map-file storage and QR print-management screens are not implemented yet. Those need secure
identity and verified building content and must not be replaced with unauthenticated write APIs.
The visitor app offers optional browser GPS for best-effort live coordinates when a device grants
permission. GPS is not mapped onto indoor floor coordinates, cannot reliably identify a floor, and
does not replace QR checkpoints. The built-in help assistant uses local rules and hospital directory
data; it is not a hosted generative-AI service. BLE/Wi-Fi/UWB indoor positioning, voice guidance,
and emergency dispatch are also out of scope. The demo's existing `abc-hospital` tenant slug is
retained for QR/API link compatibility.

## Repository layout

```text
backend/
  src/SmartNav.Api/                 ASP.NET Core visitor API and OpenAPI
  src/SmartNav.Application/         Application abstractions
  src/SmartNav.Domain/              Tenant, building, location, QR, and graph entities
  src/SmartNav.Infrastructure/       EF Core MySQL persistence, seeding, migrations
  src/SmartNav.Navigation/           Dijkstra router and published graph types
  tests/SmartNav.Infrastructure.Tests/ Tenant-isolation and routing tests
apps/
  src/                              React visitor web app
  Dockerfile                        Production static-web build
docker-compose.yml                 Local MySQL, API, and visitor stack
```
