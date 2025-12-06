# 前端集成指南

本指南将帮助你构建一个现代化的前端应用程序（React 或 Vue）并使用 TypeScript 连接到 WebLedger 后端。

## 目录

- [前置要求](#前置要求)
- [React + TypeScript 快速开始](#react--typescript-快速开始)
- [Vue + TypeScript 快速开始](#vue--typescript-快速开始)
- [认证设置](#认证设置)
- [API 客户端实现](#api-客户端实现)
- [使用示例](#使用示例)

## 前置要求

- Node.js 18+ 和 npm/yarn/pnpm
- WebLedger 后端正在运行（参见 [开始使用](./getting-started.md)）
- 访问凭证（access 和 secret）

## React + TypeScript 快速开始

### 1. 使用 Vite 创建 React 应用

```bash
npm create vite@latest my-ledger-app -- --template react-ts
cd my-ledger-app
npm install
```

### 2. 安装依赖

```bash
npm install axios
# 可选：用于状态管理
npm install zustand
# 可选：用于路由
npm install react-router-dom
```

### 3. 创建 API 客户端

创建 `src/api/client.ts`：

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

// 请求拦截器用于日志记录
apiClient.interceptors.request.use(
  (config) => {
    console.log('请求:', config.method?.toUpperCase(), config.url);
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 响应拦截器用于错误处理
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API 错误:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);
```

### 4. 创建类型定义

创建 `src/types/ledger.ts`：

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

### 5. 创建 API 服务

创建 `src/api/ledgerService.ts`：

```typescript
import { apiClient } from './client';
import { Entry, Category, SelectOption, LedgerType } from '../types/ledger';

export const ledgerService = {
  // 条目操作
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

  // 分类操作
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

  // 类型操作
  async addOrUpdateType(type: LedgerType): Promise<void> {
    await apiClient.put('/ledger/type', type);
  },

  async listTypes(): Promise<LedgerType[]> {
    const response = await apiClient.get<LedgerType[]>('/ledger/types');
    return response.data;
  },
};
```

### 6. 环境变量

创建 `.env.local`：

```env
VITE_API_URL=http://localhost:5143
VITE_WL_ACCESS=root
VITE_WL_SECRET=your-secret-key-here
```

### 7. 示例组件

创建 `src/components/EntryForm.tsx`：

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
      console.log('创建的条目:', id);
      alert('条目创建成功！');
      // 重置表单
      setFormData({
        type: '',
        category: '',
        amount: 0,
        givenTime: new Date().toISOString(),
        description: '',
      });
    } catch (error) {
      console.error('创建条目失败:', error);
      alert('创建条目失败');
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label>类型:</label>
        <input
          type="text"
          value={formData.type}
          onChange={(e) => setFormData({ ...formData, type: e.target.value })}
          required
        />
      </div>
      <div>
        <label>分类:</label>
        <input
          type="text"
          value={formData.category}
          onChange={(e) => setFormData({ ...formData, category: e.target.value })}
          required
        />
      </div>
      <div>
        <label>金额:</label>
        <input
          type="number"
          value={formData.amount}
          onChange={(e) => setFormData({ ...formData, amount: parseFloat(e.target.value) })}
          required
        />
      </div>
      <div>
        <label>日期:</label>
        <input
          type="datetime-local"
          value={formData.givenTime?.slice(0, 16)}
          onChange={(e) => setFormData({ ...formData, givenTime: new Date(e.target.value).toISOString() })}
          required
        />
      </div>
      <div>
        <label>描述:</label>
        <textarea
          value={formData.description}
          onChange={(e) => setFormData({ ...formData, description: e.target.value })}
        />
      </div>
      <button type="submit">创建条目</button>
    </form>
  );
};
```

### 8. 运行开发服务器

```bash
npm run dev
```

你的 React 应用现在应该在 `http://localhost:5173` 上运行。

## Vue + TypeScript 快速开始

### 1. 使用 Vite 创建 Vue 应用

```bash
npm create vite@latest my-ledger-app -- --template vue-ts
cd my-ledger-app
npm install
```

### 2. 安装依赖

```bash
npm install axios
# 可选：用于状态管理
npm install pinia
# 可选：用于路由
npm install vue-router
```

### 3. 创建 API 客户端

创建 `src/api/client.ts`：

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

// 请求拦截器
apiClient.interceptors.request.use(
  (config) => {
    console.log('请求:', config.method?.toUpperCase(), config.url);
    return config;
  },
  (error) => Promise.reject(error)
);

// 响应拦截器
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API 错误:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);
```

### 4. 创建类型定义

创建 `src/types/ledger.ts`（与 React 版本相同，见上文）。

### 5. 创建 API 服务

创建 `src/api/ledgerService.ts`（与 React 版本相同，见上文）。

### 6. 环境变量

创建 `.env.local`：

```env
VITE_API_URL=http://localhost:5143
VITE_WL_ACCESS=root
VITE_WL_SECRET=your-secret-key-here
```

### 7. 示例组件

创建 `src/components/EntryForm.vue`：

```vue
<template>
  <form @submit.prevent="handleSubmit">
    <div>
      <label>类型:</label>
      <input v-model="formData.type" type="text" required />
    </div>
    <div>
      <label>分类:</label>
      <input v-model="formData.category" type="text" required />
    </div>
    <div>
      <label>金额:</label>
      <input v-model.number="formData.amount" type="number" required />
    </div>
    <div>
      <label>日期:</label>
      <input v-model="formData.givenTime" type="datetime-local" required />
    </div>
    <div>
      <label>描述:</label>
      <textarea v-model="formData.description" />
    </div>
    <button type="submit">创建条目</button>
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
    console.log('创建的条目:', id);
    alert('条目创建成功！');
    // 重置表单
    formData.value = {
      type: '',
      category: '',
      amount: 0,
      givenTime: new Date().toISOString().slice(0, 16),
      description: '',
    };
  } catch (error) {
    console.error('创建条目失败:', error);
    alert('创建条目失败');
  }
};
</script>
```

### 8. 运行开发服务器

```bash
npm run dev
```

你的 Vue 应用现在应该在 `http://localhost:5173` 上运行。

## 认证设置

### 安全存储凭证

**开发环境：**
- 使用 `.env.local` 进行开发（如上文所示）
- 切勿将 `.env.local` 提交到版本控制

**生产环境：**
- 从你的托管平台使用环境变量
- 对于浏览器应用，考虑实现登录流程，将凭证存储在安全的 HTTP-only cookie 中
- 或者，实现后端即前端（BFF，Backend-for-Frontend）模式，避免在浏览器中暴露凭证

### 动态认证

如果你想让用户输入凭证：

```typescript
// src/api/client.ts
export function setAuthCredentials(access: string, secret: string) {
  apiClient.defaults.headers['wl-access'] = access;
  apiClient.defaults.headers['wl-secret'] = secret;
}

// 在登录组件中使用
import { setAuthCredentials } from './api/client';

function handleLogin(access: string, secret: string) {
  setAuthCredentials(access, secret);
  // 如果需要，存储在 localStorage/sessionStorage 中
  localStorage.setItem('wl-access', access);
  localStorage.setItem('wl-secret', secret);
}
```

## API 客户端实现

### 通用 API 客户端模式

以下是适用于任何前端框架的通用 API 客户端模式：

```typescript
// api/ledgerClient.ts
export class LedgerClient {
  private baseURL: string;
  private accessKey: string;
  private secretKey: string;

  constructor(config: {
    baseURL?: string;
    accessKey?: string;
    secretKey?: string;
  } = {}) {
    this.baseURL = config.baseURL || 'http://localhost:5143';
    this.accessKey = config.accessKey || '';
    this.secretKey = config.secretKey || '';
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseURL}${endpoint}`;

    const headers = new Headers(options.headers);
    headers.set('Content-Type', 'application/json');
    headers.set('wl-access', this.accessKey);
    headers.set('wl-secret', this.secretKey);

    const response = await fetch(url, {
      ...options,
      headers,
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${await response.text()}`);
    }

    return response.json();
  }

  async createEntry(entry: Entry): Promise<string> {
    return this.request<string>('/ledger/entry', {
      method: 'POST',
      body: JSON.stringify(entry),
    });
  }

  async selectEntries(options: SelectOption = {}): Promise<Entry[]> {
    const query = new URLSearchParams();
    Object.entries(options).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        if (Array.isArray(value)) {
          value.forEach(v => query.append(key, v.toString()));
        } else {
          query.append(key, value.toString());
        }
      }
    });

    const queryString = query.toString();
    const endpoint = queryString ? `/ledger/select?${queryString}` : '/ledger/select';

    return this.request<Entry[]>(endpoint, {
      method: 'GET',
    });
  }

  async listCategories(): Promise<Category[]> {
    return this.request<Category[]>('/ledger/categories');
  }

  async listTypes(): Promise<LedgerType[]> {
    return this.request<LedgerType[]>('/ledger/types');
  }

  setCredentials(access: string, secret: string) {
    this.accessKey = access;
    this.secretKey = secret;
  }
}

// 使用示例
const client = new LedgerClient();
client.setCredentials('your-access-key', 'your-secret-key');
```

## 使用示例

### 获取和显示条目

**React：**
```typescript
import { useEffect, useState } from 'react';
import { ledgerService } from '../api/ledgerService';
import { Entry } from '../types/ledger';

export const EntryList: React.FC = () => {
  const [entries, setEntries] = useState<Entry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadEntries();
  }, []);

  const loadEntries = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await ledgerService.selectEntries({
        limit: 100,
        offset: 0,
      });
      setEntries(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : '加载条目失败');
      console.error('加载条目失败:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>加载中...</div>;
  if (error) return <div>错误: {error}</div>;

  return (
    <div>
      <h2>最近条目</h2>
      <button onClick={loadEntries}>刷新</button>
      <ul>
        {entries.map((entry) => (
          <li key={entry.id}>
            <strong>{entry.type}</strong> - {entry.category}: ${entry.amount}
            <small>{new Date(entry.givenTime).toLocaleDateString()}</small>
            {entry.description && <p>{entry.description}</p>}
          </li>
        ))}
      </ul>
    </div>
  );
};
```

**Vue：**
```vue
<template>
  <div>
    <h2>最近条目</h2>
    <button @click="loadEntries">刷新</button>
    
    <div v-if="loading">加载中...</div>
    <div v-else-if="error" class="error">错误: {{ error }}</div>
    <div v-else>
      <ul>
        <li v-for="entry in entries" :key="entry.id">
          <strong>{{ entry.type }}</strong> - {{ entry.category }}: ${{ entry.amount }}
          <small>{{ formatDate(entry.givenTime) }}</small>
          <p v-if="entry.description">{{ entry.description }}</p>
        </li>
      </ul>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ledgerService } from '../api/ledgerService';
import type { Entry } from '../types/ledger';
const entries = ref<Entry[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const loadEntries = async () => {
  try {
    loading.value = true;
    error.value = null;
    const data = await ledgerService.selectEntries({
      limit: 100,
      offset: 0,
    });
    entries.value = data;
  } catch (err) {
    error.value = err instanceof Error ? err.message : '加载条目失败';
    console.error('加载条目失败:', err);
  } finally {
    loading.value = false;
  }
};
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString();
};
onMounted(loadEntries);
</script>
<style scoped>
.error {
  color: red;
}
ul {
  list-style: none;
  padding: 0;
}
li {
  border-bottom: 1px solid #eee;
  padding: 10px 0;
}
small {
  color: #666;
  margin-left: 10px;
}
</style>
```

## CORS 配置

如果在开发过程中遇到 CORS 问题，你可能需要配置后端。

在 `web/Program.cs` 中添加（在 `var app = builder.Build();` 之前）：

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

在 `var app = builder.Build();` 之后添加：

```csharp
app.UseCors("AllowFrontend");
```

## 项目结构推荐

```
my-ledger-app/
├── src/
│   ├── api/
│   │   ├── client.ts          # Axios 实例配置
│   │   └── ledgerService.ts   # API 方法
│   ├── types/
│   │   └── ledger.ts          # TypeScript 类型定义
│   ├── components/
│   │   ├── EntryForm.tsx/vue  # 创建条目表单
│   │   └── EntryList.tsx/vue  # 显示条目
│   ├── pages/                 # 路由页面（可选）
│   ├── stores/                # 状态管理（可选）
│   └── App.tsx/vue
├── .env.local                 # 本地环境变量
└── package.json
```

## 后续步骤

- 在 `http://localhost:5143/swagger` 浏览 Swagger UI，查看所有可用端点
- 实现错误处理和加载状态
- 添加状态管理（React 使用 Redux/Zustand，Vue 使用 Pinia）
- 为多页面应用程序实现路由
- 添加表单验证
- 为分类、类型和视图创建可重用组件

## 故障排除

**CORS 错误：**
- 确保后端已为你的前端来源配置了 CORS
- 检查前端 URL 是否与允许的来源匹配

**401 未授权：**
- 验证你的 `wl-access` 和 `wl-secret` 请求头是否正确
- 检查凭证是否在环境变量中正确设置

**网络错误：**
- 确保后端正在运行
- 验证 `VITE_API_URL` 是否指向正确的后端地址

有关更多后端设置详情，请参阅 [开始使用指南](./getting-started.md)。
```