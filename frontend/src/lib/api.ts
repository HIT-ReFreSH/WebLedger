/**
 * API Client for WebLedger Backend
 *
 * This module provides a typed API client for interacting with the WebLedger backend server.
 * All requests automatically include authentication headers (wl-access and wl-secret) from localStorage.
 *
 * @module api
 */

import type { Category, Entry, RecordedEntry, SelectOption, ViewQueryOption, ViewQueryResult, LoginRequest } from './types'

/**
 * Base URL for API requests, configurable via environment variable
 * Falls back to localhost:5143 if not set
 */
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string | undefined) ?? 'http://localhost:5143'

/**
 * Constructs HTTP headers with authentication credentials from localStorage
 *
 * @returns Headers object including Content-Type and optional auth headers
 */
function headers(): Record<string, string> {
  const h: Record<string, string> = { 'Content-Type': 'application/json' }
  const access = localStorage.getItem('wl-access')
  const secret = localStorage.getItem('wl-secret')
  if (access && secret) {
    h['wl-access'] = access
    h['wl-secret'] = secret
  }
  return h
}

/**
 * Generic response handler that throws on error or returns parsed JSON
 *
 * @template T - Expected response type
 * @param res - Fetch response object
 * @returns Parsed JSON response
 * @throws Error with server message or HTTP status code
 */
async function handle<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text()
    throw new Error(text || `HTTP ${res.status}`)
  }
  return res.json() as Promise<T>
}

/**
 * WebLedger API Client
 *
 * Provides methods for all backend operations including:
 * - Category management
 * - Entry CRUD operations
 * - Data querying and filtering
 * - Authentication
 */
export const api = {
  /**
   * Fetches all categories from the server
   *
   * @returns Promise resolving to array of categories
   */
  async getCategories(): Promise<Category[]> {
    const res = await fetch(`${API_BASE_URL}/Ledger/category`, { headers: headers() })
    return handle<Category[]>(res)
  },

  /**
   * Creates a new category or updates an existing one
   *
   * @param category - Category object with name and optional superCategory
   * @throws Error if operation fails
   */
  async addOrUpdateCategory(category: Category): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/Ledger/category`, {
      method: 'PUT',
      headers: headers(),
      body: JSON.stringify(category),
    })
    if (!res.ok) throw new Error(await res.text())
  },

  /**
   * Deletes a category by name
   *
   * @param name - Category name to delete
   * @throws Error if category is in use or doesn't exist
   */
  async removeCategory(name: string): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/Ledger/category?category=${encodeURIComponent(name)}`, {
      method: 'DELETE',
      headers: headers(),
    })
    if (!res.ok) throw new Error(await res.text())
  },

  /**
   * Queries entries with filters for time range, direction, and category
   *
   * @param option - Query options including startTime, endTime, direction, category
   * @returns Promise resolving to array of entries matching the criteria
   */
  async select(option: SelectOption): Promise<RecordedEntry[]> {
    const res = await fetch(`${API_BASE_URL}/Ledger/select`, {
      method: 'POST',
      headers: headers(),
      body: JSON.stringify(option),
    })
    return handle<RecordedEntry[]>(res)
  },

  /**
   * Creates a new ledger entry
   *
   * @param entry - Entry object with amount, time, type, optional category and description
   * @returns Promise resolving to the ID of the created entry
   * @throws Error if type is undefined and no category provided
   */
  async insertEntry(entry: Entry): Promise<string> {
    const res = await fetch(`${API_BASE_URL}/Ledger/entry`, {
      method: 'POST',
      headers: headers(),
      body: JSON.stringify(entry),
    })
    return handle<string>(res)
  },

  /**
   * Deletes an entry by ID
   *
   * @param id - UUID of the entry to delete
   * @throws Error if entry doesn't exist
   */
  async deleteEntry(id: string): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/Ledger/entry?id=${encodeURIComponent(id)}`, {
      method: 'DELETE',
      headers: headers(),
    })
    if (!res.ok) throw new Error(await res.text())
  },

  /**
   * Queries aggregated view data
   *
   * @param option - View query options including viewName and limit
   * @returns Promise resolving to aggregated results by category and time
   */
  async viewQuery(option: ViewQueryOption): Promise<ViewQueryResult> {
    const res = await fetch(`${API_BASE_URL}/Ledger/query`, {
      method: 'POST',
      headers: headers(),
      body: JSON.stringify(option),
    })
    return handle<ViewQueryResult>(res)
  },

  /**
   * Authenticates a user and retrieves access credentials
   *
   * @param body - Login request with name and key
   * @returns Promise resolving to response with code and message
   */
  async login(body: LoginRequest): Promise<{ code: number; message: string }> {
    const res = await fetch(`${API_BASE_URL}/Config/login`, {
      method: 'POST',
      headers: headers(),
      body: JSON.stringify(body),
    })
    return handle(res)
  },

  /**
   * Registers a new access credential
   *
   * @param body - Registration request with name and key
   * @returns Promise resolving to response with code, message, and optional secret key
   */
  async register(body: LoginRequest): Promise<{ code: number; message: string; key?: string }> {
    const res = await fetch(`${API_BASE_URL}/Config/register`, {
      method: 'POST',
      headers: headers(),
      body: JSON.stringify(body),
    })
    return handle(res)
  },
}