import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/login': 'http://localhost:5000',
      '/register': 'http://localhost:5000',
      '/stocks': 'http://localhost:5000',
      '/orders': 'http://localhost:5000',
      '/portfolio': 'http://localhost:5000',
      '/transactions': 'http://localhost:5000',
    },
  },
})
