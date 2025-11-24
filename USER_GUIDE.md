# WebLedger 用户使用指南

本文档提供 WebLedger 项目从零开始的完整部署和使用指南。

## 📋 目录

- [系统架构](#系统架构)
- [环境要求](#环境要求)
- [快速开始](#快速开始)
- [详细部署步骤](#详细部署步骤)
  - [1. 数据库配置](#1-数据库配置)
  - [2. 后端部署](#2-后端部署)
  - [3. 前端部署](#3-前端部署)
- [系统使用](#系统使用)
- [常见问题](#常见问题)

---

## 系统架构

WebLedger 是一个现代化的财务管理系统，包含三个主要组件：

```
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│   前端      │ ───────>│   后端      │ ───────>│   MySQL     │
│  (React)    │  HTTP   │  (.NET 8)   │  EF Core│  数据库     │
│  Port:5173  │<────── │  Port:5143  │<────── │  Port:3306  │
└─────────────┘         └─────────────┘         └─────────────┘
```

- **前端**: React + TypeScript + Vite + Ant Design
- **后端**: ASP.NET Core Web API (.NET 8)
- **数据库**: MySQL 8.0+ with Entity Framework Core

---

## 环境要求

### 必需软件

| 软件 | 版本要求 | 下载地址 |
|------|---------|----------|
| **.NET SDK** | 8.0+ | [下载](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **Node.js** | 20.19+ or 22.12+ | [下载](https://nodejs.org/) |
| **pnpm** | 最新版 | [下载](https://pnpm.io/installation) |
| **MySQL** | 8.0+ | [下载](https://dev.mysql.com/downloads/mysql/) |

### 验证安装

```bash
# 检查 .NET SDK
dotnet --version
# 应显示: 8.0.xxx

# 检查 Node.js
node --version

# 检查 pnpm
pnpm --version
# 应显示: 9.x.x 或更高版本

# 检查 MySQL
mysql --version
```

**⚠️ 重要：** 本项目使用 **pnpm** 作为包管理器，请勿使用 npm 或 yarn。如果尚未安装 pnpm，请运行：

```bash
# 使用 npm 全局安装 pnpm
npm install -g pnpm

# 或使用官方脚本安装（推荐）
# Windows (PowerShell)
iwr https://get.pnpm.io/install.ps1 -useb | iex

# Linux/macOS
curl -fsSL https://get.pnpm.io/install.sh | sh -
```

---

## 快速开始

如果你只是想快速体验系统，按以下步骤操作：

### 1. 启动 MySQL 服务

**Windows:**
```bash
net start mysql
```

**Linux/macOS:**
```bash
# Ubuntu/Debian
sudo systemctl start mysql

# macOS
brew services start mysql
```

### 2. 创建数据库

```bash
mysql -u root -p
```

输入密码后，执行：
```sql
CREATE DATABASE WebLedger CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
exit;
```

### 3. 配置数据库连接

编辑 `web/appsettings.json`，修改数据库连接信息：

```json
{
  "ConnectionStrings": {
    "mysql": "database=WebLedger; server=localhost; port=3306; user=你的用户名; password=你的密码; Persist Security Info=False; Connect Timeout=300"
  }
}
```

**⚠️ 重要提示**: 请将 `user=你的用户名` 和 `password=你的密码` 替换为你的 MySQL 实际用户名和密码。

### 4. 启动后端

```bash
# 进入后端目录
cd web

# 应用数据库迁移（首次运行）
dotnet ef database update --project ../src

# 启动后端服务
dotnet run
```

等待看到：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5143
```

### 5. 启动前端

**打开新的终端窗口**，执行：

```bash
# 进入前端目录
cd frontend

# 安装依赖（首次运行）
pnpm install

# 启动前端服务
pnpm dev
```

等待看到：
```
  ➜  Local:   http://localhost:5173/
```

### 6. 访问系统

在浏览器中打开: **http://localhost:5173**

---

## 详细部署步骤

### 1. 数据库配置

#### 1.1 MySQL 安装（如果尚未安装）

**Windows:**
```bash
# 使用 Chocolatey 安装
choco install mysql

# 或从官网下载安装包
# https://dev.mysql.com/downloads/installer/
```

**Ubuntu/Debian:**
```bash
sudo apt update
sudo apt install mysql-server
sudo mysql_secure_installation
```

**macOS:**
```bash
brew install mysql
brew services start mysql
```

#### 1.2 创建数据库和用户

登录 MySQL：
```bash
mysql -u root -p
```

创建数据库：
```sql
CREATE DATABASE WebLedger CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

（可选）创建专用用户：
```sql
-- 创建用户
CREATE USER 'webledger'@'localhost' IDENTIFIED BY '你的强密码';

-- 授权
GRANT ALL PRIVILEGES ON WebLedger.* TO 'webledger'@'localhost';
FLUSH PRIVILEGES;

-- 退出
exit;
```

#### 1.3 配置连接字符串

编辑 `web/appsettings.json` 文件：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "mysql": "database=WebLedger; server=localhost; port=3306; user=你的用户名; password=你的密码; Persist Security Info=False; Connect Timeout=300"
  },
  "AllowedHosts": "*"
}
```

**连接字符串参数说明：**

| 参数 | 说明 | 示例 |
|------|------|------|
| `database` | 数据库名称 | `WebLedger` |
| `server` | MySQL 服务器地址 | `localhost` 或 `127.0.0.1` |
| `port` | MySQL 端口 | `3306`（默认） |
| `user` | 数据库用户名 | `root` 或 `webledger` |
| `password` | 数据库密码 | 你的 MySQL 密码 |

**⚠️ 安全建议：**
- 不要在生产环境使用 root 用户
- 使用强密码
- 不要将包含密码的配置文件提交到 Git

---

### 2. 后端部署

#### 2.1 还原依赖

```bash
# 在项目根目录执行
cd E:\学习\开源软件开发实践\OSSD25-Lab3-2023113301\WebLedger

# 还原 NuGet 包
dotnet restore

# 构建项目
dotnet build
```

#### 2.2 应用数据库迁移

```bash
# 进入 Web 项目目录
cd web

# 应用数据库迁移
dotnet ef database update --project ../src
```

**成功输出示例：**
```
Build succeeded.
Applying migration '20230102124705_init'.
Applying migration '20230106122729_ViewSystem'.
Done.
```

#### 2.3 启动后端服务

**开发模式：**
```bash
# 在 web 目录
dotnet run
```

**生产模式：**
```bash
# 发布应用
dotnet publish -c Release -o ./publish

# 运行发布版本
cd publish
dotnet WebLedger.dll
```

#### 2.4 验证后端

访问以下地址确认后端正常运行：

- **Swagger API 文档**: http://localhost:5143/swagger
- **测试端点**: http://localhost:5143/Ledger/category

如果看到 Swagger UI 或返回 `[]`（空数组），说明后端运行正常。

---

### 3. 前端部署

#### 3.1 安装 Node.js 和 pnpm

#### Windows

```bash
# 使用 Chocolatey
choco install nodejs

# 或者从官网下载安装
# https://nodejs.org/
```

#### Linux

```bash
# Ubuntu/Debian
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# CentOS/RHEL
curl -fsSL https://rpm.nodesource.com/setup_18.x | sudo bash -
sudo yum install -y nodejs
```

#### macOS

```bash
# 使用 Homebrew
brew install node
```

#### 3.2 安装依赖

```bash
# 进入前端目录
cd frontend

# 安装所有依赖
pnpm install
```

**关键依赖说明：**

| 依赖包 | 版本 | 说明 |
|--------|------|------|
| `react` | ^19.2.0 | React 核心框架 |
| `antd` | ^5.29.0 | UI 组件库 |
| `dayjs` | ^1.11.19 | 日期处理库 ⚠️ 必需 |
| `@tanstack/react-query` | ^5.90.10 | 数据请求管理 |
| `@tanstack/react-router` | ^1.136.8 | 路由管理 |

**⚠️ 重要：** `dayjs` 是 Ant Design DatePicker 的必需依赖，如果缺失会报错：`date4.isValid is not a function`

#### 3.2 启动开发服务器

```bash
# 在 frontend 目录
pnpm dev
```

**成功输出：**
```
  VITE v7.2.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
  ➜  press h + enter to show help
```

#### 3.3 构建生产版本（可选）

```bash
# 构建
pnpm build

# 预览构建结果
pnpm preview
```

构建后的文件在 `frontend/dist` 目录，可部署到任何静态文件服务器。

---

## 系统使用

### 启动顺序

按以下顺序启动各个服务：

1. **启动 MySQL** → 2. **启动后端** → 3. **启动前端**

### 访问地址

| 服务 | 地址 | 说明 |
|------|------|------|
| 前端界面 | http://localhost:5173 | 用户操作界面 |
| 后端 API | http://localhost:5143 | API 服务 |
| API 文档 | http://localhost:5143/swagger | Swagger UI |

### 首次使用

系统初始状态下，**无需任何认证即可访问**。顶栏的"访问控制"输入框可以暂时忽略。

#### 1. 创建分类（可选）

访问"分类管理"页面，创建一些分类，例如：
- 餐饮
- 交通
- 娱乐
- 工资
- 副业收入

#### 2. 添加条目

访问"新建条目"页面，填写表单：

| 字段 | 说明 | 示例 |
|------|------|------|
| **金额** | 正数=收入，负数=支出 | `1000` 或 `-50.5` |
| **时间** | 交易发生时间 | 选择日期和时间 |
| **类型** | 条目类型（自动创建） | "工资"、"午餐" |
| **分类** | 可选，选择已有分类 | "收入"、"餐饮" |
| **描述** | 可选的备注 | "11月工资" |

点击"提交"后，条目会保存到数据库。

#### 3. 查看数据

- **仪表盘**: 查看最近 30 天的统计数据
- **条目列表**: 查看、筛选、删除条目
- **类型管理**: 查看所有类型的使用统计

### 访问控制（高级功能）

如果需要启用访问控制（多用户管理），需要使用 CLI 工具创建访问凭证。

#### 使用 CLI 工具

```bash
# 进入 CLI 目录
cd cli

# 配置为直连模式
# 编辑 config.json，设置数据库连接

# 运行 CLI
dotnet run

# 在 CLI 中执行命令
grant root
```

这会生成一个访问密钥，然后在前端顶栏填入：
- **wl-access**: `root`
- **wl-secret**: `生成的密钥`

---

## 常见问题

### Q1: 前端一直显示"加载中..."

**原因：** 后端未启动或 CORS 未配置

**解决方案：**
1. 确认后端已启动（访问 http://localhost:5143/swagger）
2. 检查 `web/Program.cs` 是否包含 CORS 配置
3. 查看浏览器控制台（F12）的错误信息

### Q2: 日期选择器报错 `date4.isValid is not a function`

**原因：** 缺少 `dayjs` 依赖

**解决方案：**
```bash
cd frontend
pnpm install dayjs
```

### Q3: 数据库连接失败

**错误信息：** `Unable to connect to any of the specified MySQL hosts`

**解决方案：**
1. 确认 MySQL 服务已启动
2. 检查 `appsettings.json` 中的连接字符串
3. 确认用户名和密码正确
4. 检查数据库是否已创建

```bash
# 测试 MySQL 连接
mysql -u 你的用户名 -p
# 输入密码后应能成功登录
```

### Q4: 端口被占用

**错误信息：** `Address already in use`

**解决方案：**

**检查端口占用：**
```bash
# Windows
netstat -ano | findstr :5143

# Linux/macOS
lsof -i :5143
```

**修改端口：**
- 后端: 编辑 `web/Properties/launchSettings.json`
- 前端: 编辑 `frontend/vite.config.ts`

### Q5: 数据库迁移失败

**错误信息：** `The EF Core tools version 'x.x.x' is older than that of the runtime 'y.y.y'`

**解决方案：**
```bash
# 更新 EF Core 工具
dotnet tool update --global dotnet-ef

# 重新应用迁移
cd web
dotnet ef database update --project ../src
```

### Q6: 前端页面无限加载（仪表盘、类型管理）

**原因：** React Query 的 queryKey 对象引用不稳定，导致无限循环请求

**解决方案：**
这个问题已在最新版本中修复。如果仍遇到此问题，请确保：
- `Dashboard.tsx` 和 `Types.tsx` 中使用了 `useMemo` 包裹 `option` 对象
- 前端依赖已正确安装（`pnpm install`）

### Q7: 如何清空数据库重新开始

```bash
# 登录 MySQL
mysql -u root -p

# 删除数据库
DROP DATABASE WebLedger;

# 重新创建
CREATE DATABASE WebLedger CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
exit;

# 重新应用迁移
cd web
dotnet ef database update --project ../src
```

---

## 性能优化建议

### 生产环境部署

1. **启用 HTTPS**
   - 配置 SSL 证书
   - 修改 `launchSettings.json` 使用 HTTPS

2. **使用反向代理**
   - Nginx 或 Apache
   - 统一前后端域名，避免 CORS 问题

3. **数据库优化**
   - 创建适当的索引
   - 定期备份数据
   - 配置连接池

4. **前端优化**
   - 使用生产构建（`pnpm build`）
   - 启用 CDN
   - 配置缓存策略

---

## 贡献指南

如果您想为 WebLedger 项目贡献代码，请遵循以下规范：

### 前端开发规范

1. **技术栈要求**
   - 必须使用 Vite（不使用 Create React App）
   - 必须使用 TypeScript（不使用纯 JavaScript）
   - 必须使用 **pnpm** 作为包管理器（不使用 npm 或 yarn）
   - 使用 TanStack Router 和 TanStack Query
   - 使用 Ant Design 或 Tailwind CSS

2. **提交 Pull Request 要求**
   - PR 描述必须清晰说明改动内容
   - **必须包含所有页面的截图**（展示 UI 变更）
   - 代码必须包含必要的注释
   - 遵循 TypeScript 最佳实践
   - 实现响应式设计（移动端友好）
   - 包含错误处理和加载状态

3. **截图要求**
   - 使用清晰的截图展示所有 UI 变更
   - 包含桌面端和移动端视图（如适用）
   - 在 PR 描述中使用 Markdown 格式插入截图
   - 截图格式示例：
     ```markdown
     ### 功能截图
     
     **仪表盘页面：**
     ![Dashboard](./screenshots/dashboard.png)
     
     **移动端视图：**
     ![Mobile View](./screenshots/mobile.png)
     ```

4. **代码审查**
   - 所有 PR 需要经过代码审查
   - 确保通过 ESLint 检查
   - 确保构建成功（`pnpm build`）

---

## 技术支持

如果遇到问题：

1. **查看日志**
   - 后端: 控制台输出
   - 前端: 浏览器 F12 控制台

2. **检查文档**
   - `README.md` - 项目概述
   - `CLAUDE.md` - 开发指南
   - `DEPLOYMENT_GUIDE.md` - 详细部署文档

3. **验证环境**
   - 确认所有必需软件已正确安装
   - 检查版本号是否符合要求

---

**文档版本**: 2.0
**更新日期**: 2025-11-24
**适用版本**: WebLedger v1.0+

祝使用愉快！ 🎉
