# WebLedger 前端管理系统

这是 WebLedger 的现代化 React 管理后台，提供完整的财务数据管理功能。

## 技术栈

- **React 18+** - 现代化的前端框架
- **TypeScript** - 类型安全的开发体验
- **Vite** - 快速的构建工具
- **pnpm** - 高效的包管理器
- **TanStack Router** - 类型安全的路由管理
- **TanStack Query** - 强大的数据获取和缓存
- **Ant Design 5** - 企业级 UI 组件库
- **Zod** - TypeScript 优先的数据验证

## 功能特性

### 已实现功能

- ✅ **仪表盘概览**
  - 最近 30 天的收入/支出统计
  - 净额、收入、支出三大核心指标
  - 按分类统计 Top 10（带占比进度条）
  - 最常使用类型 Top 5
  - 平均每日收入/支出快速统计

- ✅ **条目列表**
  - 时间范围筛选（支持日期时间选择器）
  - 收入/支出方向筛选
  - 按分类筛选
  - 实时查询统计（查询结果、收入、支出）
  - 排序功能（时间、金额）
  - 条目删除（带二次确认）
  - 分页和每页数量调整

- ✅ **新建条目**
  - 表单验证（使用 Zod）
  - 金额输入（支持正负数）
  - 日期时间选择器
  - 类型输入（带字符计数）
  - 分类下拉选择
  - 描述文本域（带字符计数）
  - 快速填充功能（收入/支出示例）
  - 最近分类列表展示
  - 操作提示说明

- ✅ **分类管理**
  - 新增/更新分类
  - 分类列表（带分页）
  - 层级关系显示（父分类）
  - 分类删除（带二次确认）
  - 操作提示说明

- ✅ **类型管理**
  - 类型统计总览（总数、使用次数、最常使用）
  - 详细的收入/支出分析
  - 默认分类显示
  - 自动创建说明

- ✅ **响应式设计**
  - 完全适配移动端、平板、桌面
  - 侧边栏可折叠
  - 表格横向滚动
  - 弹性布局

- ✅ **用户体验优化**
  - 加载状态提示
  - 错误提示
  - 操作成功反馈
  - 图标美化
  - 色彩区分（收入绿色、支出红色）
  - 面包屑导航
  - 固定顶栏
  - 页脚信息

## 快速开始

### 前置要求

- Node.js 18+
- pnpm 8+（推荐）或 npm/yarn

### 安装依赖

```bash
# 推荐使用 pnpm
pnpm install

# 或使用 npm
npm install
```

### 开发环境运行

1. 确保后端服务已启动（默认运行在 http://localhost:5143）

2. 启动前端开发服务器：

```bash
pnpm dev
```

3. 打开浏览器访问 http://localhost:5173

### 构建生产版本

```bash
pnpm build
```

构建产物将生成在 `dist` 目录。

### 预览生产版本

```bash
pnpm preview
```

## 配置说明

### 环境变量

项目支持以下环境变量配置：

- `.env.development` - 开发环境配置
- `.env.production` - 生产环境配置
- `.env.local` - 本地覆盖配置（不会被提交到 Git）

主要配置项：

```env
# 后端 API 基础地址
VITE_API_BASE_URL=http://localhost:5143
```

### Vite 代理配置

开发环境使用 Vite 代理，配置在 `vite.config.ts`：

```typescript
server: {
  proxy: {
    '/Ledger': { target: 'http://localhost:5143' },
    '/Config': { target: 'http://localhost:5143' },
  },
}
```

## 访问控制

如果后端启用了访问控制，需要配置访问凭证：

1. 在页面顶部的 "访问控制" 输入框中输入：
   - `wl-access`: 访问名称
   - `wl-secret`: 访问密钥

2. 配置后将自动保存到 localStorage，后续请求自动携带

3. 初次使用时，如果数据库中没有访问记录，系统允许所有请求（初始化模式）

## 项目结构

```
frontend/
├── public/          # 静态资源
├── src/
│   ├── lib/         # 工具库
│   │   ├── api.ts   # API 调用封装
│   │   └── types.ts # TypeScript 类型定义
│   ├── pages/       # 页面组件
│   │   ├── Dashboard.tsx   # 仪表盘
│   │   ├── Entries.tsx     # 条目列表
│   │   ├── NewEntry.tsx    # 新建条目
│   │   ├── Categories.tsx  # 分类管理
│   │   └── Types.tsx       # 类型管理
│   ├── AppLayout.tsx   # 应用布局
│   ├── router.tsx      # 路由配置
│   ├── main.tsx        # 应用入口
│   └── index.css       # 全局样式
├── .env.development    # 开发环境变量
├── .env.production     # 生产环境变量
├── .env.example        # 环境变量示例
├── package.json        # 项目依赖
├── tsconfig.json       # TypeScript 配置
├── vite.config.ts      # Vite 配置
└── README.md           # 项目说明
```

## 开发指南

### 添加新页面

1. 在 `src/pages/` 创建页面组件
2. 在 `src/router.tsx` 注册路由
3. 在 `src/AppLayout.tsx` 添加菜单项

### API 调用

使用 `src/lib/api.ts` 中封装的 API 方法：

```typescript
import { api } from '../lib/api'
import { useQuery, useMutation } from '@tanstack/react-query'

// 查询数据
const { data, isLoading, error } = useQuery({
  queryKey: ['categories'],
  queryFn: api.getCategories,
})

// 修改数据
const mutation = useMutation({
  mutationFn: api.insertEntry,
  onSuccess: () => { /* 成功回调 */ },
})
```

### 类型定义

所有数据类型定义在 `src/lib/types.ts`，确保类型安全。

## 常见问题

### 1. 网络请求失败

- 检查后端服务是否启动
- 检查 API 地址配置是否正确
- 检查访问控制凭证是否正确

### 2. 构建失败

- 运行 `pnpm install` 重新安装依赖
- 检查 Node.js 版本是否符合要求
- 清除缓存后重试：`rm -rf node_modules pnpm-lock.yaml && pnpm install`

### 3. 样式异常

- 确保已导入 Ant Design 的 reset 样式
- 检查 `src/main.tsx` 中是否包含 `import 'antd/dist/reset.css'`

## 贡献指南

欢迎提交 Issue 和 Pull Request！

提交 PR 时请确保：

1. 代码遵循项目的 TypeScript 规范
2. 所有组件都有适当的类型定义
3. UI 变更包含相关截图
4. 测试通过且无 ESLint 错误

## 许可证

本项目遵循 MIT 许可证。

