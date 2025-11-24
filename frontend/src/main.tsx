/**
 * Application Entry Point
 *
 * This is the main entry file that initializes the React application.
 * It sets up the query client for data fetching and the router for navigation.
 *
 * @module main
 */

import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RouterProvider } from '@tanstack/react-router'
import { ConfigProvider } from 'antd'
import zhCN from 'antd/locale/zh_CN'
import { router } from './router'

/**
 * TanStack Query client for managing server state
 * Handles caching, background updates, and request deduplication
 */
const queryClient = new QueryClient()

/**
 * Initialize and render the application
 *
 * - StrictMode: Enables additional development checks
 * - QueryClientProvider: Provides React Query functionality to all components
 * - ConfigProvider: Provides Ant Design theme and locale configuration
 * - RouterProvider: Handles client-side routing
 */
createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ConfigProvider
      locale={zhCN}
      theme={{
        token: {
          colorPrimary: '#1890ff',
          borderRadius: 6,
        },
      }}
    >
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={router} />
      </QueryClientProvider>
    </ConfigProvider>
  </StrictMode>,
)
