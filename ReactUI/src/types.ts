export interface AuthResponse {
  token: string;
  username: string;
  email: string;
}

export interface Stock {
  id: number;
  symbol: string;
  name: string;
  currentPrice: number;
  previousClose: number;
  change: number;
  changePercent: number;
}

export interface PortfolioHolding {
  symbol: string;
  name: string;
  quantity: number;
  averageCost: number;
  currentPrice: number;
  marketValue: number;
  profitLoss: number;
  profitLossPercent: number;
}

export interface Portfolio {
  cashBalance: number;
  portfolioValue: number;
  totalValue: number;
  todaysProfitLoss: number;
  totalProfitLoss: number;
  holdings: PortfolioHolding[];
}

export interface Transaction {
  id: number;
  type: string;
  symbol: string;
  stockName: string;
  quantity: number;
  pricePerShare: number;
  totalAmount: number;
  createdAt: string;
}

export interface OrderResponse {
  orderId: number;
  type: string;
  symbol: string;
  quantity: number;
  pricePerShare: number;
  totalAmount: number;
  status: string;
}
