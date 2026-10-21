import { createContext, useContext, useMemo, useState, type ReactNode } from 'react';
import type { AuthResponse } from '../types';

interface AuthContextValue {
  user: { username: string; email: string } | null;
  token: string | null;
  login: (response: AuthResponse) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem('token'));
  const [user, setUser] = useState<{ username: string; email: string } | null>(() => {
    const username = localStorage.getItem('username');
    const email = localStorage.getItem('email');
    return username && email ? { username, email } : null;
  });

  const value = useMemo(
    () => ({
      user,
      token,
      login: (response: AuthResponse) => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('username', response.username);
        localStorage.setItem('email', response.email);
        setToken(response.token);
        setUser({ username: response.username, email: response.email });
      },
      logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('username');
        localStorage.removeItem('email');
        setToken(null);
        setUser(null);
      },
    }),
    [token, user],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
}
