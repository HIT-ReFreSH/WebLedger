/**
 * Application Routing Configuration
 *
 * This module sets up the TanStack Router for the WebLedger application.
 * It defines all routes and their associated page components.
 *
 * @module router
 */

import { createRootRoute, createRoute, createRouter } from '@tanstack/react-router'
import { AppLayout } from './AppLayout'
import Dashboard from './pages/Dashboard'
import Entries from './pages/Entries'
import NewEntry from './pages/NewEntry'
import Categories from './pages/Categories'
import Types from './pages/Types'

/**
 * Root route that wraps all other routes with the AppLayout component
 */
const rootRoute = createRootRoute({
  component: AppLayout,
})

/**
 * Dashboard route - displays summary statistics and overview
 */
const dashboardRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/',
  component: Dashboard,
})

/**
 * Entries list route - shows all ledger entries with filters
 */
const entriesRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'entries',
  component: Entries,
})

/**
 * New entry form route - allows creating new ledger entries
 */
const newEntryRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'entry/new',
  component: NewEntry,
})

/**
 * Categories management route - CRUD operations for categories
 */
const categoriesRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'categories',
  component: Categories,
})

/**
 * Types overview route - read-only view of entry types
 */
const typesRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'types',
  component: Types,
})

/**
 * Route tree combining all application routes
 */
const routeTree = rootRoute.addChildren([
  dashboardRoute,
  entriesRoute,
  newEntryRoute,
  categoriesRoute,
  typesRoute,
])

/**
 * Main router instance exported for use in the application
 */
export const router = createRouter({ routeTree })

/**
 * Type augmentation for TanStack Router to provide type safety
 */
declare module '@tanstack/react-router' {
  interface Register { router: typeof router }
}