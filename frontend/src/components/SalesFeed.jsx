export function SalesFeed({ sales }) {
  return (
    <div className="feed-panel">
      <h3 className="panel-title">Live Feed</h3>
      <div className="feed-list">
        {sales.map((sale, i) => (
          <div key={sale.id} className={`feed-item ${i === 0 ? 'feed-item--new' : ''}`}>
            <div className="feed-item__left">
              <span className="feed-product">{sale.product}</span>
              <span className="feed-region">{sale.region}</span>
            </div>
            <div className="feed-item__right">
              <span className="feed-amount">${sale.amount.toLocaleString()}</span>
              <span className="feed-qty">×{sale.quantity}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
