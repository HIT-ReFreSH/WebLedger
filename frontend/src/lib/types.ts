/**
 * Type Definitions for WebLedger API
 *
 * This module contains all TypeScript type definitions used throughout the application.
 * These types ensure type safety when interacting with the backend API.
 *
 * @module types
 */

/**
 * Represents a new ledger entry to be created
 *
 * @property amount - Transaction amount (positive for income, negative for expense)
 * @property givenTime - ISO 8601 datetime string of when the transaction occurred
 * @property type - Entry type identifier (e.g., "Salary", "Groceries")
 * @property category - Optional category name (if omitted, uses type's default)
 * @property description - Optional textual description of the entry
 */
export type Entry = {
  amount: number
  givenTime: string
  type: string
  category?: string | null
  description?: string | null
}

/**
 * Represents a stored ledger entry with system-generated ID
 *
 * @extends Entry
 * @property id - Unique identifier (UUID) assigned by the server
 */
export type RecordedEntry = Entry & {
  id: string
}

/**
 * Represents a hierarchical category for organizing entries
 *
 * @property name - Unique category name
 * @property superCategory - Optional parent category for hierarchy
 *
 * @example
 * { name: "Beverages", superCategory: "Food" }
 */
export type Category = {
  name: string
  superCategory?: string | null
}

/**
 * Options for querying entries with filters
 *
 * @property startTime - ISO 8601 datetime string for range start (inclusive)
 * @property endTime - ISO 8601 datetime string for range end (inclusive)
 * @property direction - Filter by income (true) or expense (false), null for all
 * @property category - Filter by category name, null for all categories
 */
export type SelectOption = {
  startTime: string
  endTime: string
  direction?: boolean | null
  category?: string | null
}

/**
 * Options for querying aggregated view data
 *
 * @property viewName - Name of the view to query
 * @property limit - Maximum number of results to return
 */
export type ViewQueryOption = {
  viewName: string
  limit: number
}

/**
 * Aggregated view query results
 *
 * @property raw - Raw entry data included in the view
 * @property byCategory - Sum of amounts grouped by category name
 * @property byTime - Sum of amounts grouped by time period
 */
export type ViewQueryResult = {
  raw: Entry[]
  byCategory: Record<string, number>
  byTime: Record<string, number>
}

/**
 * Credentials for login or registration
 *
 * @property name - Access name/username
 * @property key - Secret key/password
 */
export type LoginRequest = {
  name: string
  key: string
}