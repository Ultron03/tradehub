import { useState, type FormEvent } from 'react';
import type { Stock } from '../types';

interface TradeModalProps {
  stock: Stock;
  mode: 'buy' | 'sell';
  onClose: () => void;
  onSubmit: (quantity: number) => Promise<void>;
}

export default function TradeModal({ stock, mode, onClose, onSubmit }: TradeModalProps) {
  const [quantity, setQuantity] = useState(1);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const total = stock.currentPrice * quantity;

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await onSubmit(quantity);
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Order failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal" onClick={(e) => e.stopPropagation()}>
        <h2>{mode === 'buy' ? 'Buy' : 'Sell'} {stock.symbol}</h2>
        <p className="modal-subtitle">{stock.name} @ ₹{stock.currentPrice.toFixed(2)}</p>
        <form onSubmit={handleSubmit}>
          <label>
            Quantity
            <input
              type="number"
              min={1}
              value={quantity}
              onChange={(e) => setQuantity(Math.max(1, Number(e.target.value)))}
              required
            />
          </label>
          <p className="total">Total: ₹{total.toLocaleString('en-IN', { minimumFractionDigits: 2 })}</p>
          {error && <p className="error">{error}</p>}
          <div className="modal-actions">
            <button type="button" className="btn-secondary" onClick={onClose}>Cancel</button>
            <button type="submit" className={mode === 'buy' ? 'btn-buy' : 'btn-sell'} disabled={loading}>
              {loading ? 'Processing...' : `Confirm ${mode === 'buy' ? 'Buy' : 'Sell'}`}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
