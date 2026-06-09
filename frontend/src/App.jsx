import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell } from 'recharts'
import { useDashboard } from './hooks/useDashboard'
import { KpiCard } from './components/KpiCard'
import { SalesFeed } from './components/SalesFeed'
import { AnomalyPanel } from './components/AnomalyPanel'

const REGION_COLORS = ['#00ff87', '#00d4ff', '#ff6b35', '#bf5af2', '#ffd60a']

function fmt(n) {
  if (n >= 1_000_000) return `$${(n / 1_000_000).toFixed(1)}M`
  if (n >= 1_000) return `$${(n / 1_000).toFixed(1)}K`
  return `$${n}`
}

export default function App() {
  const { snapshot, recentSales, alerts, connected, pulse } = useDashboard()

  return (
    <div className="app">
      <header className="header">
        <div className="header__brand">
          <span className="header__logo">◈</span>
          <span className="header__title">SALES OPS</span>
        </div>
        <div className="header__meta">
          {alerts.length > 0 && (
            <span className="header__alert-count">
              ⚠ {alerts.length} alert{alerts.length > 1 ? 's' : ''}
            </span>
          )}
          <span className={`status-dot ${connected ? 'status-dot--on' : 'status-dot--off'}`} />
          <span className="status-label">{connected ? 'LIVE' : 'CONNECTING'}</span>
        </div>
      </header>

      <main className="main">
        <div className={`kpi-row ${pulse ? 'kpi-row--pulse' : ''}`}>
          <KpiCard label="Total Revenue" value={snapshot ? fmt(snapshot.totalRevenue) : '—'} sub="all time" />
          <KpiCard label="Total Orders" value={snapshot ? snapshot.totalOrders.toLocaleString() : '—'} sub="processed" />
          <KpiCard label="Avg Order" value={snapshot ? fmt(snapshot.avgOrderValue) : '—'} sub="per transaction" />
          <KpiCard label="Top Product" value={snapshot?.topProduct ?? '—'} sub="by revenue" />
          <KpiCard label="Top Region" value={snapshot?.topRegion ?? '—'} sub="by revenue" />
        </div>

        <div className="charts-row">
          <div className="chart-panel">
            <h3 className="panel-title">Revenue by Region</h3>
            {snapshot?.regionStats?.length > 0 ? (
              <ResponsiveContainer width="100%" height={220}>
                <BarChart data={snapshot.regionStats} margin={{ top: 8, right: 8, left: 0, bottom: 40 }}>
                  <XAxis dataKey="region" tick={{ fill: '#888', fontSize: 11, fontFamily: 'DM Mono' }} angle={-25} textAnchor="end" interval={0} />
                  <YAxis tick={{ fill: '#888', fontSize: 11, fontFamily: 'DM Mono' }} tickFormatter={v => `$${(v / 1000).toFixed(0)}k`} />
                  <Tooltip contentStyle={{ background: '#111', border: '1px solid #333', borderRadius: 8, fontFamily: 'DM Mono', fontSize: 12 }} formatter={v => [fmt(v), 'Revenue']} />
                  <Bar dataKey="revenue" radius={[4, 4, 0, 0]}>
                    {snapshot.regionStats.map((_, i) => <Cell key={i} fill={REGION_COLORS[i % REGION_COLORS.length]} />)}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            ) : <div className="chart-empty">Waiting for data...</div>}
          </div>

          <div className="chart-panel">
            <h3 className="panel-title">Revenue by Product</h3>
            {snapshot?.productStats?.length > 0 ? (
              <ResponsiveContainer width="100%" height={220}>
                <BarChart data={snapshot.productStats} layout="vertical" margin={{ top: 8, right: 24, left: 8, bottom: 8 }}>
                  <XAxis type="number" tick={{ fill: '#888', fontSize: 11, fontFamily: 'DM Mono' }} tickFormatter={v => `$${(v / 1000).toFixed(0)}k`} />
                  <YAxis type="category" dataKey="product" tick={{ fill: '#ccc', fontSize: 11, fontFamily: 'DM Mono' }} width={110} />
                  <Tooltip contentStyle={{ background: '#111', border: '1px solid #333', borderRadius: 8, fontFamily: 'DM Mono', fontSize: 12 }} formatter={v => [fmt(v), 'Revenue']} />
                  <Bar dataKey="revenue" fill="#00ff87" radius={[0, 4, 4, 0]} />
                </BarChart>
              </ResponsiveContainer>
            ) : <div className="chart-empty">Waiting for data...</div>}
          </div>

          <SalesFeed sales={recentSales} />
        </div>

        <AnomalyPanel alerts={alerts} />
      </main>
    </div>
  )
}
