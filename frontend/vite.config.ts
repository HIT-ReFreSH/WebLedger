import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/Ledger': {
        target: 'http://localhost:5143',
        changeOrigin: true,
      },
      '/Config': {
        target: 'http://localhost:5143',
        changeOrigin: true,
      },
    },
  },
})
