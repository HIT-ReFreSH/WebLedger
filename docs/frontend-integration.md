# Frontend Integration Guide

This guide will help you build a modern frontend application (React or Vue) with TypeScript to connect to the WebLedger backend.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Quick Start with React + TypeScript](#quick-start-with-react--typescript)
- [Quick Start with Vue + TypeScript](#quick-start-with-vue--typescript)
- [Authentication Setup](#authentication-setup)
- [API Client Implementation](#api-client-implementation)
- [Example Usage](#example-usage)

## Prerequisites

- Node.js 18+ and npm/yarn/pnpm
- WebLedger backend running (see [Getting Started](./getting-started.md))
- Access credentials (access and secret)

## Quick Start with React + TypeScript

### 1. Create React App with Vite

```bash
npm create vite@latest my-ledger-app -- --template react-ts
cd my-ledger-app
npm install
```

### 2. Install Dependencies

```bash
npm install axios
# Optional: for state management
npm install zustand
# Optional: for routing
npm install react-router-dom
```

### 3. Create API Client

Create `src/api/client.ts`:

```typescript
import axios, { AxiosInstance } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5143';
const ACCESS_KEY = import.meta.env.VITE_WL_ACCESS || 'root';
const SECRET_KEY = import.meta.env.VITE_WL_SECRET || '';

export const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'wl-access': ACCESS_KEY,
    'wl-secret': SECRET_KEY,
  },
});

// Request interceptor for logging
apiClient.interceptors.request.use(
  (config) => {
    console.log('Request:', config.method?.toUpperCase(), config.url);
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);
```

### 4. Create Type Definitions

Create `src/types/ledger.ts`:

```typescript
export interface Entry {
  id?: string;
  type: string;
  category: string;
  amount: number;
  givenTime: string;
  description?: string;
}

export interface Category {
  name: string;
  parent?: string;
  description?: string;
}

export interface SelectOption {
  startTime?: string;
  endTime?: string;
  categories?: string[];
  types?: string[];
  limit?: number;
  offset?: number;
}

export interface LedgerType {
  name: string;
  defaultCategory: string;
  description?: string;
}
```

### 5. Create API Service

Create `src/api/ledgerService.ts`:

```typescript
import { apiClient } from './client';
import { Entry, Category, SelectOption, LedgerType } from '../types/ledger';

export const ledgerService = {
  // Entry operations
  async createEntry(entry: Entry): Promise<string> {
    const response = await apiClient.post<string>('/ledger/entry', entry);
    return response.data;
  },

  async deleteEntry(id: string): Promise<void> {
    await apiClient.delete(`/ledger/entry?id=${id}`);
  },

  async selectEntries(option: SelectOption): Promise<Entry[]> {
    const response = await apiClient.post<Entry[]>('/ledger/select', option);
    return response.data;
  },

  // Category operations
  async addOrUpdateCategory(category: Category): Promise<void> {
    await apiClient.put('/ledger/category', category);
  },

  async deleteCategory(categoryName: string): Promise<void> {
    await apiClient.delete(`/ledger/category?category=${categoryName}`);
  },

  async listCategories(): Promise<Category[]> {
    const response = await apiClient.get<Category[]>('/ledger/categories');
    return response.data;
  },

  // Type operations
  async addOrUpdateType(type: LedgerType): Promise<void> {
    await apiClient.put('/ledger/type', type);
  },

  async listTypes(): Promise<LedgerType[]> {
    const response = await apiClient.get<LedgerType[]>('/ledger/types');
    return response.data;
  },
};
```

### 6. Environment Variables

Create `.env.local`:

```env
VITE_API_URL=http://localhost:5143
VITE_WL_ACCESS=root
VITE_WL_SECRET=your-secret-key-here
```

### 7. Example Component

Create `src/components/EntryForm.tsx`:

```typescript
import React, { useState } from 'react';
import { ledgerService } from '../api/ledgerService';
import { Entry } from '../types/ledger';

export const EntryForm: React.FC = () => {
  const [formData, setFormData] = useState<Partial<Entry>>({
    type: '',
    category: '',
    amount: 0,
    givenTime: new Date().toISOString(),
    description: '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const id = await ledgerService.createEntry(formData as Entry);
      console.log('Created entry:', id);
      alert('Entry created successfully!');
      // Reset form
      setFormData({
        type: '',
        category: '',
        amount: 0,
        givenTime: new Date().toISOString(),
        description: '',
      });
    } catch (error) {
      console.error('Failed to create entry:', error);
      alert('Failed to create entry');
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label>Type:</label>
        <input
          type="text"
          value={formData.type}
          onChange={(e) => setFormData({ ...formData, type: e.target.value })}
          required
        />
      </div>
      <div>
        <label>Category:</label>
        <input
          type="text"
          value={formData.category}
          onChange={(e) => setFormData({ ...formData, category: e.target.value })}
          required
        />
      </div>
      <div>
        <label>Amount:</label>
        <input
          type="number"
          value={formData.amount}
          onChange={(e) => setFormData({ ...formData, amount: parseFloat(e.target.value) })}
          required
        />
      </div>
      <div>
        <label>Date:</label>
        <input
          type="datetime-local"
          value={formData.givenTime?.slice(0, 16)}
          onChange={(e) => setFormData({ ...formData, givenTime: new Date(e.target.value).toISOString() })}
          required
        />
      </div>
      <div>
        <label>Description:</label>
        <textarea
          value={formData.description}
          onChange={(e) => setFormData({ ...formData, description: e.target.value })}
        />
      </div>
      <button type="submit">Create Entry</button>
    </form>
  );
};
```

### 8. Run the Development Server

```bash
npm run dev
```

Your React app should now be running on `http://localhost:5173`.

## Quick Start with Vue + TypeScript

### 1. Create Vue App with Vite

```bash
npm create vite@latest my-ledger-app -- --template vue-ts
cd my-ledger-app
npm install
```

### 2. Install Dependencies

```bash
npm install axios
# Optional: for state management
npm install pinia
# Optional: for routing
npm install vue-router
```

### 3. Create API Client

Create `src/api/client.ts`:

```typescript
import axios, { AxiosInstance } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5143';
const ACCESS_KEY = import.meta.env.VITE_WL_ACCESS || 'root';
const SECRET_KEY = import.meta.env.VITE_WL_SECRET || '';

export const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'wl-access': ACCESS_KEY,
    'wl-secret': SECRET_KEY,
  },
});

// Request interceptor
apiClient.interceptors.request.use(
  (config) => {
    console.log('Request:', config.method?.toUpperCase(), config.url);
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);
```

### 4. Create Type Definitions

Create `src/types/ledger.ts` (same as React version above).

### 5. Create API Service

Create `src/api/ledgerService.ts` (same as React version above).

### 6. Environment Variables

Create `.env.local`:

```env
VITE_API_URL=http://localhost:5143
VITE_WL_ACCESS=root
VITE_WL_SECRET=your-secret-key-here
```

### 7. Example Component

Create `src/components/EntryForm.vue`:

```vue
<template>
  <form @submit.prevent="handleSubmit">
    <div>
      <label>Type:</label>
      <input v-model="formData.type" type="text" required />
    </div>
    <div>
      <label>Category:</label>
      <input v-model="formData.category" type="text" required />
    </div>
    <div>
      <label>Amount:</label>
      <input v-model.number="formData.amount" type="number" required />
    </div>
    <div>
      <label>Date:</label>
      <input v-model="formData.givenTime" type="datetime-local" required />
    </div>
    <div>
      <label>Description:</label>
      <textarea v-model="formData.description" />
    </div>
    <button type="submit">Create Entry</button>
  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ledgerService } from '../api/ledgerService';
import type { Entry } from '../types/ledger';

const formData = ref<Partial<Entry>>({
  type: '',
  category: '',
  amount: 0,
  givenTime: new Date().toISOString().slice(0, 16),
  description: '',
});

const handleSubmit = async () => {
  try {
    const entry: Entry = {
      ...formData.value,
      givenTime: new Date(formData.value.givenTime!).toISOString(),
    } as Entry;

    const id = await ledgerService.createEntry(entry);
    console.log('Created entry:', id);
    alert('Entry created successfully!');

    // Reset form
    formData.value = {
      type: '',
      category: '',
      amount: 0,
      givenTime: new Date().toISOString().slice(0, 16),
      description: '',
    };
  } catch (error) {
    console.error('Failed to create entry:', error);
    alert('Failed to create entry');
  }
};
</script>
```

### 8. Run the Development Server

```bash
npm run dev
```

Your Vue app should now be running on `http://localhost:5173`.

## Authentication Setup

### Storing Credentials Securely

**Development:**
- Use `.env.local` for development (already shown above)
- Never commit `.env.local` to version control

**Production:**
- Use environment variables from your hosting platform
- For browser apps, consider implementing a login flow where credentials are stored in secure HTTP-only cookies
- Alternatively, implement a backend-for-frontend (BFF) pattern to avoid exposing credentials in the browser

### Dynamic Authentication

If you want users to enter credentials:

```typescript
// src/api/client.ts
export function setAuthCredentials(access: string, secret: string) {
  apiClient.defaults.headers['wl-access'] = access;
  apiClient.defaults.headers['wl-secret'] = secret;
}

// Usage in login component
import { setAuthCredentials } from './api/client';

function handleLogin(access: string, secret: string) {
  setAuthCredentials(access, secret);
  // Store in localStorage/sessionStorage if needed
  localStorage.setItem('wl-access', access);
  localStorage.setItem('wl-secret', secret);
}
```

## Example Usage

### Fetching and Displaying Entries

**React:**
```typescript
import { useEffect, useState } from 'react';
import { ledgerService } from '../api/ledgerService';
import { Entry } from '../types/ledger';

export const EntryList: React.FC = () => {
  const [entries, setEntries] = useState<Entry[]>([]);

  useEffect(() => {
    loadEntries();
  }, []);

  const loadEntries = async () => {
    try {
      const data = await ledgerService.selectEntries({
        limit: 100,
        offset: 0,
      });
      setEntries(data);
    } catch (error) {
      console.error('Failed to load entries:', error);
    }
  };

  return (
    <div>
      <h2>Recent Entries</h2>
      {entries.map((entry) => (
        <div key={entry.id}>
          <strong>{entry.type}</strong> - {entry.category}: ${entry.amount}
        </div>
      ))}
    </div>
  );
};
```

**Vue:**
```vue
<template>
  <div>
    <h2>Recent Entries</h2>
    <div v-for="entry in entries" :key="entry.id">
      <strong>{{ entry.type }}</strong> - {{ entry.category }}: ${{ entry.amount }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ledgerService } from '../api/ledgerService';
import type { Entry } from '../types/ledger';

const entries = ref<Entry[]>([]);

const loadEntries = async () => {
  try {
    const data = await ledgerService.selectEntries({
      limit: 100,
      offset: 0,
    });
    entries.value = data;
  } catch (error) {
    console.error('Failed to load entries:', error);
  }
};

onMounted(loadEntries);
</script>
```

## CORS Configuration

If you encounter CORS issues during development, you may need to configure the backend.

Add to `web/Program.cs` (before `var app = builder.Build();`):

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
```

And after `var app = builder.Build();`:

```csharp
app.UseCors("AllowFrontend");
```

## Project Structure Recommendation

```
my-ledger-app/
├── src/
│   ├── api/
│   │   ├── client.ts          # Axios instance configuration
│   │   └── ledgerService.ts   # API methods
│   ├── types/
│   │   └── ledger.ts          # TypeScript type definitions
│   ├── components/
│   │   ├── EntryForm.tsx/vue  # Create entry form
│   │   └── EntryList.tsx/vue  # Display entries
│   ├── pages/                 # Route pages (optional)
│   ├── stores/                # State management (optional)
│   └── App.tsx/vue
├── .env.local                 # Local environment variables
└── package.json
```

## Next Steps

- Explore the Swagger UI at `http://localhost:5143/swagger` for all available endpoints
- Implement error handling and loading states
- Add state management (Redux/Zustand for React, Pinia for Vue)
- Implement routing for multi-page applications
- Add form validation
- Create reusable components for categories, types, and views

## Troubleshooting

**CORS Errors:**
- Ensure the backend has CORS configured for your frontend origin
- Check that the frontend URL matches the allowed origins

**401 Unauthorized:**
- Verify your `wl-access` and `wl-secret` headers are correct
- Check that credentials are properly set in environment variables

**Network Errors:**
- Ensure the backend is running
- Verify the `VITE_API_URL` points to the correct backend address

For more backend setup details, see [Getting Started Guide](./getting-started.md).
