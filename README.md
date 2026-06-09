# Real-time Sales Dashboard with ML Anomaly Detection

A live sales monitoring dashboard built with **.NET 10**, **SignalR**, **ML.NET**, and **React**. Sales events are generated server-side, pushed to all clients in real time, and continuously analyzed by an **in-process ML model** that detects anomalous transactions.

## Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 10 Minimal API |
| Real-time | SignalR |
| ML | ML.NET 3 — IidSpikeDetector (time series) |
| Background | .NET `BackgroundService` |
| Frontend | React 18 + Recharts |
| Container | Docker + Docker Compose |

## How it works

1. `SalesBroadcastService` generates a sale every 600ms–2s via `SalesDataService`
2. Each sale is pushed to all clients via SignalR (`SaleOccurred`, `SnapshotUpdated`)
3. `AnomalyDetectionService` feeds each sale amount into ML.NET's **IidSpikeDetector**
4. After 30 samples, the model starts detecting statistical spikes in sale values
5. When an anomaly is found, an `AnomalyDetected` event is broadcast with severity, score, and a human-readable reason
6. The React frontend shows alerts in real time in the Anomaly Detection panel

## ML model details

- **Algorithm**: IID Spike Detection (Independently and Identically Distributed)
- **Input**: sale amount over a sliding window
- **Output**: anomaly flag, p-value, and anomaly score
- **Confidence**: 95%
- **Warm-up**: 30 sales before the model starts evaluating
- **Severity**: `Critical` when p-value < 0.01, `Warning` otherwise
- Runs **entirely in-process** — no external API, no cloud dependency

## Get started

### With Docker
```bash
docker compose up --build
```
Open [http://localhost:5173](http://localhost:5173). Wait ~30 seconds for the ML model to warm up.

### Without Docker
```bash
# Terminal 1
cd api && dotnet run

# Terminal 2
cd frontend && npm install && npm run dev
```

## Project structure

```
├── api/
│   ├── src/
│   │   ├── Hubs/       SignalR hub
│   │   ├── ML/         AnomalyDetectionService (ML.NET)
│   │   ├── Models/     Records + ML input/output types
│   │   └── Services/   SalesDataService + SalesBroadcastService
│   └── Program.cs
├── frontend/
│   └── src/
│       ├── components/ KpiCard, SalesFeed, AnomalyPanel
│       ├── hooks/      useDashboard (SignalR + anomaly events)
│       └── App.jsx
└── docker-compose.yml
```

## SignalR events

| Event | Payload | Description |
|---|---|---|
| `SaleOccurred` | `SaleEvent` | New sale |
| `SnapshotUpdated` | `DashboardSnapshot` | Aggregated state |
| `AnomalyDetected` | `AnomalyAlert` | ML-detected anomaly with score and reason |
