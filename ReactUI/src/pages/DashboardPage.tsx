import { useCallback, useEffect, useState } from 'react';
import { api } from '../api';
import { useAuth } from '../context/AuthContext';
import type { Portfolio, Stock, Transaction } from '../types';
import TradeModal from '../components/TradeModal';

function formatCurrency(value: number) {
  return `₹${value.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
}

function formatPnL(value: number) {
  const prefix = value >= 0 ? '+' : '';
  return `${prefix}${formatCurrency(value)}`;
}

export default function DashboardPage() {
  const { user, logout } = useAuth();
  const [stocks, setStocks] = useState<Stock[]>([]);
  const [portfolio, setPortfolio] = useState<Portfolio | null>(null);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [tradeTarget, setTradeTarget] = useState<{ stock: Stock; mode: 'buy' | 'sell' } | null>(null);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState<'stocks' | 'holdings' | 'history'>('stocks');

  const refresh = useCallback(async () => {
    try {
      const [stocksData, portfolioData, txData] = await Promise.all([
        api.getStocks(),
        api.getPortfolio(),
        api.getTransactions(),
      ]);
      setStocks(stocksData);
      setPortfolio(portfolioData);
      setTransactions(txData);
      setError('');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load data');
    }
  }, []);

  useEffect(() => {
    refresh();
    const interval = setInterval(refresh, 3000);
    return () => clearInterval(interval);
  }, [refresh]);

  const handleTrade = async (quantity: number) => {
    if (!tradeTarget) return;
    if (tradeTarget.mode === 'buy') {
      await api.buy(tradeTarget.stock.id, quantity);
    } else {
      await api.sell(tradeTarget.stock.id, quantity);
    }
    await refresh();
  };

  return (
    <div className="dashboard">
      <header className="topbar">
        <div>
          <h1>TradeHub</h1>
          <span className="user-badge">Welcome, {user?.username}</span>
        </div>
        <button className="btn-secondary" onClick={logout}>Logout</button>
      </header>

      {error && <div className="banner-error">{error}</div>}

      {portfolio && (
        <section className="stats-grid">
          <div className="stat-card primary">
            <span className="stat-label">Portfolio Value</span>
            <span className="stat-value">{formatCurrency(portfolio.totalValue)}</span>
          </div>
          <div className="stat-card">
            <span className="stat-label">Cash Balance</span>
            <span className="stat-value">{formatCurrency(portfolio.cashBalance)}</span>
          </div>
          <div className="stat-card">
            <span className="stat-label">Holdings Value</span>
            <span className="stat-value">{formatCurrency(portfolio.portfolioValue)}</span>
          </div>
          <div className={`stat-card ${portfolio.todaysProfitLoss >= 0 ? 'positive' : 'negative'}`}>
            <span className="stat-label">Today&apos;s P/L</span>
            <span className="stat-value">{formatPnL(portfolio.todaysProfitLoss)}</span>
          </div>
          <div className={`stat-card ${portfolio.totalProfitLoss >= 0 ? 'positive' : 'negative'}`}>
            <span className="stat-label">Total P/L</span>
            <span className="stat-value">{formatPnL(portfolio.totalProfitLoss)}</span>
          </div>
        </section>
      )}

      <nav className="tabs">
        <button className={activeTab === 'stocks' ? 'active' : ''} onClick={() => setActiveTab('stocks')}>Stocks</button>
        <button className={activeTab === 'holdings' ? 'active' : ''} onClick={() => setActiveTab('holdings')}>Holdings</button>
        <button className={activeTab === 'history' ? 'active' : ''} onClick={() => setActiveTab('history')}>History</button>
      </nav>

      {activeTab === 'stocks' && (
        <section className="panel">
          <h2>Available Stocks</h2>
          <div className="stock-grid">
            {stocks.map((stock) => (
              <article key={stock.id} className="stock-card">
                <div className="stock-header">
                  <div>
                    <h3>{stock.symbol}</h3>
                    <p>{stock.name}</p>
                  </div>
                  <span className={`change ${stock.change >= 0 ? 'up' : 'down'}`}>
                    {stock.change >= 0 ? '▲' : '▼'} {Math.abs(stock.changePercent).toFixed(2)}%
                  </span>
                </div>
                <p className="price">₹{stock.currentPrice.toFixed(2)}</p>
                <div className="stock-actions">
                  <button className="btn-buy" onClick={() => setTradeTarget({ stock, mode: 'buy' })}>Buy</button>
                  <button className="btn-sell" onClick={() => setTradeTarget({ stock, mode: 'sell' })}>Sell</button>
                </div>
              </article>
            ))}
          </div>
        </section>
      )}

      {activeTab === 'holdings' && portfolio && (
        <section className="panel">
          <h2>Your Holdings</h2>
          {portfolio.holdings.length === 0 ? (
            <p className="empty">No holdings yet. Buy some stocks to get started!</p>
          ) : (
            <table className="data-table">
              <thead>
                <tr>
                  <th>Stock</th>
                  <th>Qty</th>
                  <th>Avg Cost</th>
                  <th>Current</th>
                  <th>Value</th>
                  <th>P/L</th>
                </tr>
              </thead>
              <tbody>
                {portfolio.holdings.map((h) => (
                  <tr key={h.symbol}>
                    <td><strong>{h.symbol}</strong><br /><small>{h.name}</small></td>
                    <td>{h.quantity}</td>
                    <td>₹{h.averageCost.toFixed(2)}</td>
                    <td>₹{h.currentPrice.toFixed(2)}</td>
                    <td>{formatCurrency(h.marketValue)}</td>
                    <td className={h.profitLoss >= 0 ? 'up' : 'down'}>
                      {formatPnL(h.profitLoss)} ({h.profitLossPercent.toFixed(1)}%)
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </section>
      )}

      {activeTab === 'history' && (
        <section className="panel">
          <h2>Transaction History</h2>
          {transactions.length === 0 ? (
            <p className="empty">No transactions yet.</p>
          ) : (
            <table className="data-table">
              <thead>
                <tr>
                  <th>Date</th>
                  <th>Type</th>
                  <th>Stock</th>
                  <th>Qty</th>
                  <th>Price</th>
                  <th>Total</th>
                </tr>
              </thead>
              <tbody>
                {transactions.map((tx) => (
                  <tr key={tx.id}>
                    <td>{new Date(tx.createdAt).toLocaleString()}</td>
                    <td><span className={`badge ${tx.type.toLowerCase()}`}>{tx.type}</span></td>
                    <td><strong>{tx.symbol}</strong></td>
                    <td>{tx.quantity}</td>
                    <td>₹{tx.pricePerShare.toFixed(2)}</td>
                    <td>{formatCurrency(tx.totalAmount)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </section>
      )}

      {tradeTarget && (
        <TradeModal
          stock={tradeTarget.stock}
          mode={tradeTarget.mode}
          onClose={() => setTradeTarget(null)}
          onSubmit={handleTrade}
        />
      )}
    </div>
  );
}
