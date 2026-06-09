export function AnomalyPanel({ alerts }) {
  if (alerts.length === 0) return (
    <div className="anomaly-panel anomaly-panel--empty">
      <h3 className="panel-title">⬡ ML Anomaly Detection</h3>
      <div className="anomaly-waiting">
        <span className="anomaly-waiting__icon">◌</span>
        <span>Collecting samples — alerts will appear here once the model is ready (~30 sales)</span>
      </div>
    </div>
  )

  return (
    <div className="anomaly-panel">
      <h3 className="panel-title">⬡ ML Anomaly Detection</h3>
      <div className="anomaly-list">
        {alerts.map((alert, i) => (
          <div key={alert.id} className={`anomaly-item anomaly-item--${alert.severity.toLowerCase()} ${i === 0 ? 'anomaly-item--new' : ''}`}>
            <div className="anomaly-item__header">
              <span className={`anomaly-badge anomaly-badge--${alert.severity.toLowerCase()}`}>
                {alert.severity === 'Critical' ? '⚠ CRITICAL' : '△ WARNING'}
              </span>
              <span className="anomaly-score">score {alert.score.toFixed(3)}</span>
            </div>
            <p className="anomaly-reason">{alert.reason}</p>
            <div className="anomaly-meta">
              <span>{alert.region}</span>
              <span>${alert.amount.toLocaleString()}</span>
              <span>{new Date(alert.detectedAt).toLocaleTimeString()}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
