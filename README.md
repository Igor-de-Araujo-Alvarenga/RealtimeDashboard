# Real-time Sales Dashboard

A live sales monitoring dashboard built with **.NET 10**, **SignalR**, and **React**. Sales events are generated server-side and pushed to all connected clients in real time — no polling.

## Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 10 Minimal API |
| Real-time | SignalR |
| Background | .NET `BackgroundService` |
| Frontend | React 18 + Recharts |
| Bundler | Vite |
| Container | Docker + Docker Compose |

## How it works

1. `SalesBroadcastService` runs as a `BackgroundService`, generating a fake sale every 600ms–2s
2. Each sale is pushed to all connected clients via SignalR (`SaleOccurred` + `SnapshotUpdated` events)
3. The React frontend connects to the SignalR hub using `@microsoft/signalr`
4. Charts and KPIs update live with no page refresh

## Get started

### With Docker (recommended)

```bash
docker compose up --build
```

Then open [http://localhost:5173](http://localhost:5173).

### Without Docker

**API:**
```bash
cd api
dotnet run
```

**Frontend** (separate terminal):
```bash
cd frontend
npm install
npm run dev
```

Then open [http://localhost:5173](http://localhost:5173).

## Project structure

```
├── api/
│   ├── src/
│   │   ├── Hubs/          SignalR hub
│   │   ├── Models/        Record types
│   │   └── Services/      Data generator + broadcast service
│   └── Program.cs
├── frontend/
│   └── src/
│       ├── components/    KpiCard, SalesFeed
│       ├── hooks/         useDashboard (SignalR connection)
│       └── App.jsx
└── docker-compose.yml
```

## SignalR events

| Event | Payload | Description |
|---|---|---|
| `SaleOccurred` | `SaleEvent` | A single new sale |
| `SnapshotUpdated` | `DashboardSnapshot` | Full aggregated state |


