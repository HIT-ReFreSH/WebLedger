# WebLedger 部署和使用指南

## 项目概述

WebLedger 是一个基于 .NET 8 + React 的账本管理系统，包含：
- **后端**: ASP.NET Core Web API (.NET 8)
- **前端**: React + TypeScript + Vite
- **数据库**: MySQL with Entity Framework Core
- **CLI工具**: .NET 控制台应用

## 环境要求

### 必需环境
- **.NET 8.0 SDK**: [下载地址](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+**: [下载地址](https://nodejs.org/)
- **MySQL 8.0+**: [下载地址](https://dev.mysql.com/downloads/mysql/)

### 可选工具

- **Docker**: 用于容器化部署
- **MySQL Workbench**: 数据库管理工具

## 1. 数据库配置和初始化

### 1.1 MySQL 安装和配置

#### Windows 安装
```bash
# 使用 Chocolatey 安装（推荐）
choco install mysql

# 或者从官网下载安装包安装
# https://dev.mysql.com/downloads/installer/
```

#### Linux 安装
```bash
# Ubuntu/Debian
sudo apt update
sudo apt install mysql-server
sudo mysql_secure_installation

# CentOS/RHEL
sudo yum install mysql-server
sudo systemctl start mysqld
sudo systemctl enable mysqld
```

#### macOS 安装
```bash
# 使用 Homebrew
brew install mysql
brew services start mysql
```

### 1.2 数据库初始化

1. **登录 MySQL**
```bash
mysql -u root -p
```

2. **创建数据库**
```sql
CREATE DATABASE WebLedger CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

3. **创建用户并授权**（可选，用于生产环境）
```sql
CREATE USER 'webledger'@'localhost' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON WebLedger.* TO 'webledger'@'localhost';
FLUSH PRIVILEGES;
```

### 1.3 数据库连接配置

编辑 `web/appsettings.json` 文件：

```json
{
  "ConnectionStrings": {
    "mysql": "database=WebLedger; server=localhost; port=3306; user=root; password=123456; Persist Security Info=False; Connect Timeout=300"
  }
}
```

**参数说明：**
- `database`: 数据库名称
- `server`: MySQL服务器地址
- `port`: MySQL端口（默认3306）
- `user`: 数据库用户名
- `password`: 数据库密码

## 2. 后端部署 (.NET Web API)

### 2.1 安装 .NET 8 SDK

#### Windows
```bash
# 使用 Chocolatey
choco install dotnet-sdk

# 或者从官网下载安装
# https://dotnet.microsoft.com/download/dotnet/8.0
```

#### Linux
```bash
# Ubuntu/Debian
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

#### macOS
```bash
# 使用 Homebrew
brew install dotnet-sdk
```

### 2.2 还原依赖并构建（到此）

```bash
# 进入项目根目录
cd e:\学习\开源软件开发实践\OSSD25-Lab3-2023113301\WebLedger

# 还原 NuGet 包
dotnet restore

# 构建整个解决方案
dotnet build
```

### 2.3 数据库迁移

```bash
# 进入 Web API 项目目录
cd web

# 应用数据库迁移
dotnet ef database update --project ../src
```

### 2.4 启动后端服务

#### 开发模式
```bash
# 进入 Web API 项目目录
cd web

# 启动开发服务器
dotnet run

# 或者使用指定的启动配置
dotnet run --launch-profile http
```

**访问地址：**
- API文档: http://localhost:5143/swagger
- API端点: http://localhost:5143

#### 生产模式
```bash
# 发布应用
dotnet publish -c Release -o ./publish

# 运行发布版本
cd publish
dotnet WebLedger.dll
```

### 2.5 后端配置详解

**appsettings.json 配置：**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "mysql": "your_connection_string"
  },
  "AllowedHosts": "*"
}
```

**launchSettings.json 配置：**
```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "http://0.0.0.0:5143"
    }
  }
}
```

## 3. 前端部署 (React)

### 3.1 安装 Node.js 和 pnpm

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

### 3.2 安装依赖

```bash
# 进入前端项目目录
cd frontend

# 安装项目依赖
pnpm install
```

**主要依赖说明：**

| 依赖包 | 版本 | 说明 |
|--------|------|------|
| `react` | ^19.2.0 | React 框架 |
| `antd` | ^5.29.0 | Ant Design UI 组件库 |
| `dayjs` | ^1.11.19 | 日期处理库（Ant Design DatePicker 必需） |
| `@tanstack/react-query` | ^5.90.10 | 数据请求和缓存管理 |
| `@tanstack/react-router` | ^1.136.8 | 路由管理 |
| `zod` | ^4.1.12 | 数据验证 |
| `@ant-design/icons` | ^6.1.0 | Ant Design 图标库 |

**注意事项：**
- `dayjs` 是 Ant Design 5.x 的 DatePicker 和 RangePicker 组件的必需依赖
- 如果缺少 `dayjs`，日期选择器会报错：`date4.isValid is not a function`
- 推荐使用 Node.js 20.x 或 22.x 版本以获得最佳兼容性

### 3.3 开发环境启动

```bash
# 启动开发服务器
npm run dev

# 或者使用 yarn
yarn dev
```

**访问地址：** http://localhost:5173

### 3.4 前端配置详解

**vite.config.ts 配置：**
```typescript
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

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
```

**代理配置说明：**
- `/Ledger` 和 `/Config` 路径的请求会被代理到后端 API (localhost:5143)
- 这样前端可以直接使用相对路径访问后端 API

### 3.5 生产环境构建

```bash
# 构建生产版本
npm run build

# 预览构建结果
npm run preview
```

构建后的文件会生成在 `frontend/dist` 目录中，可以部署到任何静态文件服务器。

## 4. CLI 工具部署

### 4.1 构建 CLI 应用

```bash
# 进入 CLI 项目目录
cd cli

# 构建应用
dotnet build

# 发布应用
dotnet publish -c Release -o ./publish
```

### 4.2 使用 CLI 工具

```bash
# 运行 CLI 应用
dotnet run

# 或者运行发布版本
cd publish
./WebLedgerTheSuit
```

**CLI 配置文件：**
- `config.json`: 主配置文件
- `config.direct.json`: 直连模式配置
- `config.http.json`: HTTP 模式配置

## 5. Docker 部署（可选）

### 5.1 安装 Docker

#### Windows
```bash
# 下载 Docker Desktop
# https://www.docker.com/products/docker-desktop
```

#### Linux
```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install docker.io docker-compose-v2
sudo systemctl start docker
sudo systemctl enable docker
```

### 5.2 构建 Docker 镜像

```bash
# 构建后端镜像
cd web
docker build -t webledger-api .

# 构建前端镜像
cd ../frontend
docker build -t webledger-frontend .
```

### 5.3 使用 Docker Compose

创建 `docker-compose.yml` 文件：

```yaml
version: '3.8'
services:
  mysql:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: root_password
      MYSQL_DATABASE: WebLedger
    ports:
      - "3306:3306"
    volumes:
      - mysql_data:/var/lib/mysql

  webledger-api:
    image: webledger-api
    ports:
      - "5143:5143"
    depends_on:
      - mysql
    environment:
      ConnectionStrings__mysql: "database=WebLedger; server=mysql; port=3306; user=root; password=root_password;"

  webledger-frontend:
    image: webledger-frontend
    ports:
      - "5173:5173"
    depends_on:
      - webledger-api

volumes:
  mysql_data:
```

启动服务：
```bash
docker-compose up -d
```

## 6. 系统使用指南

### 6.1 启动顺序

1. **启动 MySQL 服务**
```bash
# Windows
net start mysql

# Linux
sudo systemctl start mysql

# macOS
brew services start mysql
```

2. **启动后端 API**
```bash
cd web
dotnet run
```

3. **启动前端应用**
```bash
cd frontend
npm run dev
```

### 6.2 访问系统

- **前端界面**: http://localhost:5173
- **API 文档**: http://localhost:5143/swagger
- **健康检查**: http://localhost:5143/health

### 6.3 默认配置

**数据库默认用户：**
- 用户名: admin
- 密码: admin123

**API 端口：**
- 后端: 5143
- 前端: 5173

### 6.4 常用命令汇总

```bash
# 数据库操作
mysql -u root -p
CREATE DATABASE WebLedger CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# 后端操作
cd web
dotnet restore
dotnet build
dotnet ef database update --project ../src
dotnet run

# 前端操作
cd frontend
npm install
npm run dev
npm run build

# CLI工具操作
cd cli
dotnet run
```

## 7. 故障排除

### 7.1 常见问题

**数据库连接失败**
- 检查 MySQL 服务是否启动
- 验证连接字符串配置
- 确保数据库已创建

**端口被占用**
- 检查端口占用情况: `netstat -ano | findstr :5143`
- 修改配置文件中的端口

**依赖包缺失**
- 运行 `dotnet restore` 或 `npm install`
- 检查网络连接

### 7.2 日志查看

**后端日志**
- 控制台输出
- Windows 事件查看器
- Linux: `/var/log/` 目录

**前端日志**
- 浏览器开发者工具 (F12)
- 控制台 (Console) 标签页

### 7.3 性能优化

**数据库优化**
- 添加适当的索引
- 定期清理无用数据
- 使用连接池

**API 优化**
- 启用响应压缩
- 使用缓存
- 异步处理

## 8. 安全建议

### 8.1 生产环境配置

- 使用强密码
- 启用 HTTPS
- 配置防火墙
- 定期备份数据库
- 更新依赖包

### 8.2 数据库安全

- 限制数据库用户权限
- 使用专用数据库用户
- 启用 SSL 连接
- 定期更新密码

---

**文档版本**: 1.0
**更新日期**: 2024-11-17
**维护团队**: WebLedger 开发团队