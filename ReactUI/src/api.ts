import type { AuthResponse, OrderResponse, Portfolio, Stock, Transaction } from './types';

const API_BASE = '';

async function request<T>(url: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem('token');
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string>),
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE}${url}`, { ...options, headers });

  if (!response.ok) {
    const body = await response.json().catch(() => ({ error: 'Request failed' }));
    throw new Error(body.error || 'Request failed');
  }

  return response.json();
}

export const api = {
  register: (username: string, email: string, password: string) =>
    request<AuthResponse>('/register', {
      method: 'POST',
      body: JSON.stringify({ username, email, password }),
    }),

  login: (email: string, password: string) =>
    request<AuthResponse>('/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    }),

  getStocks: () => request<Stock[]>('/stocks'),

  getStock: (id: number) => request<Stock>(`/stocks/${id}`),

  buy: (stockId: number, quantity: number) =>
    request<OrderResponse>('/orders/buy', {
      method: 'POST',
      body: JSON.stringify({ stockId, quantity }),
    }),

  sell: (stockId: number, quantity: number) =>
    request<OrderResponse>('/orders/sell', {
      method: 'POST',
      body: JSON.stringify({ stockId, quantity }),
    }),

  getPortfolio: () => request<Portfolio>('/portfolio'),

  getTransactions: () => request<Transaction[]>('/transactions'),
};
