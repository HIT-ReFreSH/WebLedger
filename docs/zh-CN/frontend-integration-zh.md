# WebLedger 前端集成指南

本文档介绍如何将前端应用程序与 WebLedger 后端 API 集成。

## API 基础

### 基础 URL
- **开发环境**: `http://localhost:5000`
- **生产环境**: `https://your-domain.com`

所有 API 端点都以 `/api` 为前缀。

### 身份验证

WebLedger 使用基于 JWT（JSON Web Tokens）的身份验证。

#### 1. 登录获取令牌

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "your-username",
  "password": "your-password"
}
```

成功响应：
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": {
    "id": 1,
    "username": "your-username"
  }
}
```

#### 2. 在请求中使用令牌

```http
GET /api/ledger/entries
Authorization: Bearer <your-jwt-token>
```

## React 集成示例

### 项目设置

```bash
# 创建新的 React 应用（TypeScript 模板）
npx create-react-app my-ledger-app --template typescript
cd my-ledger-app

# 安装必要的依赖
npm install axios
npm install @mui/material @emotion/react @emotion/styled
npm install @mui/icons-material
```

### 创建 API 服务

`src/services/api.ts`:
```typescript
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000';

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
});

// 请求拦截器：添加认证令牌
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('auth_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 响应拦截器：处理错误
    return Promise.reject(error);
  }
);

// API 方法
export const authAPI = {
  login: (username: string, password: string) =>
    api.post('/api/auth/login', { username, password }),
  },
};

export const ledgerAPI = {
  getEntries: (params?: { startDate?: string; endDate?: string; category?: string }) =>
    api.get('/api/ledger/entries', { params }),
  
  createEntry: (entry: {
    date: string;
    description: string;
    amount: number;
    category: string;
    notes?: string;
  }) => api.post('/api/ledger/entries', entry),
  
  getCategories: () => api.get('/api/ledger/categories'),
  
  getSummary: (params?: { startDate?: string; endDate?: string }) =>
    api.get('/api/ledger/summary', { params }),
};

export default api;
```

### 登录组件示例

`src/components/Login.tsx`:
```typescript
import React, { useState } from 'react';
import { TextField, Button, Box, Alert } from '@mui/material';
import { authAPI } from '../services/api';

const Login: React.FC<{ onLoginSuccess: () => void }> = ({ onLoginSuccess }) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await authAPI.login(username, password);
      localStorage.setItem('auth_token', response.data.token);
      onLoginSuccess();
    } catch (err) {
      setError('登录失败，请检查用户名和密码');
    }
  };

  return (
    <Box component="form" onSubmit={handleSubmit} sx={{ maxWidth: 400, mx: 'auto', mt: 4 }}>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <TextField
        fullWidth
        label="用户名"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        margin="normal"
        required
      />
      <TextField
        fullWidth
        label="密码"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        margin="normal"
        required
      />
      <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
        登录
      </Button>
    </Box>
  );
};

export default Login;
```

### 账目列表组件

`src/components/LedgerList.tsx`:
```typescript
import React, { useState, useEffect } from 'react';
import {
  Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, Paper, Typography
} from '@mui/material';
import { ledgerAPI } from '../services/api';

interface LedgerEntry {
  id: number;
  date: string;
  description: string;
  amount: number;
  category: string;
  balance: number;
}

const LedgerList: React.FC = () => {
  const [entries, setEntries] = useState<LedgerEntry[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchEntries();
  }, []);

  const fetchEntries = async () => {
    try {
      const response = await ledgerAPI.getEntries();
      setEntries(response.data);
    } catch (error) {
      console.error('获取账目失败:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <Typography>加载中...</Typography>;

  return (
    <TableContainer component={Paper}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>日期</TableCell>
            <TableCell>描述</TableCell>
            <TableCell>类别</TableCell>
            <TableCell align="right">金额</TableCell>
            <TableCell align="right">余额</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {entries.map((entry) => (
            <TableRow key={entry.id}>
              <TableCell>{new Date(entry.date).toLocaleDateString()}</TableCell>
              <TableCell>{entry.description}</TableCell>
              <TableCell>{entry.category}</TableCell>
              <TableCell align="right" sx={{ color: entry.amount >= 0 ? 'green' : 'red' }}>
                {entry.amount >= 0 ? '+' : ''}{entry.amount.toFixed(2)}
              </TableCell>
              <TableCell align="right">{entry.balance.toFixed(2)}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
  
  logout: () => {
    </TableContainer>
  );
};

export default LedgerList;
```

## Vue.js 集成示例

### Vue 3 + Composition API

```javascript
// src/composables/useLedgerApi.js
import { ref } from 'vue';
import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000';

    localStorage.removeItem('auth_token');
api.interceptors.response.use(
      window.location.href = '/login';
export function useLedgerApi() {
  const api = axios.create({
    baseURL: API_BASE_URL,
  });

  // 设置请求拦截器添加令牌
  api.interceptors.request.use(config => {
    const token = localStorage.getItem('auth_token');
    if (token) {
    }
  (response) => response,
  (error) => {
      config.headers.Authorization = `Bearer ${token}`;
    if (error.response?.status === 401) {
    }
    return config;

  });

  const entries = ref([]);
  const loading = ref(false);

  const fetchEntries = async () => {
    loading.value = true;
    try {
      const response = await api.get('/api/ledger/entries');
      entries.value = response.data;
    } catch (error) {
      console.error('获取数据失败:', error);
    } finally {
      loading.value = false;
    }
  };

  return {
    entries,
    loading,
    fetchEntries,
  };
}
```

## 环境变量配置

在 React/Vue 项目根目录创建 `.env` 文件：

```env
REACT_APP_API_URL=http://localhost:5000
REACT_APP_APP_NAME=WebLedger Frontend
```

## 安全最佳实践

1. **令牌存储**: 使用 `localStorage` 或 `sessionStorage` 存储 JWT 令牌
2. **HTTPS**: 生产环境始终使用 HTTPS
3. **输入验证**: 前端和后端都要验证用户输入
4. **错误处理**: 优雅地处理 API 错误
5. **加载状态**: 显示加载指示器

## 测试 API 连接

使用以下命令测试 API 是否可用：

```bash
# 测试健康检查端点
curl http://localhost:5000/health

# 测试 API 端点（需要认证）
curl -H "Authorization: Bearer YOUR_TOKEN" http://localhost:5000/api/ledger/entries
```

## 故障排除

### CORS 问题
如果遇到 CORS 错误，确保后端已正确配置 CORS：

```csharp
// 在 .NET 后端 Program.cs 中
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
```

### 连接超时
- 检查后端服务是否正在运行
- 验证端口号是否正确
- 检查防火墙设置


## 更多资源

- [React 官方文档](https://reactjs.org/docs/getting-started.html)
- [Vue.js 官方文档](https://vuejs.org/guide/introduction.html)
- [Axios 文档](https://axios-http.com/docs/intro)
- [Material-UI 组件库](https://mui.com/material-ui/getting-started/)

如需更多帮助，请查阅 [WebLedger GitHub 仓库](https://github.com/HIT-ReFreSH/WebLedger) 或提交 Issue。

---

