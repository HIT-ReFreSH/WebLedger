# WebLedger 完整部署和启动指南

本文档提供 WebLedger 项目从零开始的完整部署和启动步骤，包括数据库、后端和前端的配置和运行。

## 目录

1. [系统要求](#系统要求)
2. [数据库部署](#数据库部署)
3. [后端部署](#后端部署)
4. [前端部署](#前端部署)
5. [完整启动流程](#完整启动流程)
6. [常见问题](#常见问题)

---

## 系统要求

### 必需软件

- **操作系统**: Windows 10+, macOS 10.15+, Linux (Ubuntu 20.04+ 或其他发行版)
- **.NET SDK**: 8.0.401 或更高版本
- **MySQL**: 8.0 或更高版本
- **Node.js**: 18.0 或更高版本
- **pnpm**: 8.0 或更高版本（推荐），或 npm/yarn

### 检查软件版本

运行以下命令检查已安装的软件版本：

```bash
# 检查 .NET SDK
dotnet --version

# 检查 MySQL
mysql --version

# 检查 Node.js
node --version

# 检查 pnpm
pnpm --version
```

---

## 数据库部署

### 1. 安装 MySQL

#### Windows

1. 下载 MySQL 安装包：https://dev.mysql.com/downloads/installer/
2. 运行安装程序，选择 "Developer Default" 安装类型
3. 按照安装向导完成安装，记住设置的 root 密码

#### macOS

```bash
# 使用 Homebrew 安装
brew install mysql

# 启动 MySQL 服务
brew services start mysql

# 设置 root 密码
mysql_secure_installation
```

#### Linux (Ubuntu/Debian)

```bash
# 更新包列表
sudo apt update

# 安装 MySQL
sudo apt install mysql-server

# 启动 MySQL 服务
sudo systemctl start mysql
sudo systemctl enable mysql

# 安全配置
sudo mysql_secure_installation
```

### 2. 创建数据库和用户

登录 MySQL：

```bash
mysql -u root -p
```

执行以下 SQL 命令：

```sql
-- 创建数据库
CREATE DATABASE ledger CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 创建用户（请修改密码）
CREATE USER 'ledger-bot'@'localhost' IDENTIFIED BY 'your_secure_password';

-- 授权
GRANT ALL PRIVILEGES ON ledger.* TO 'ledger-bot'@'localhost';

-- 刷新权限
FLUSH PRIVILEGES;

-- 验证
SHOW DATABASES;
SELECT user, host FROM mysql.user WHERE user = 'ledger-bot';

-- 退出
EXIT;
```

### 3. 测试数据库连接

```bash
mysql -u ledger-bot -p ledger
```

输入密码后，如果成功登录，说明数据库配置正确。

---

## 后端部署

### 1. 克隆或获取项目代码

```bash
cd /path/to/your/workspace
git clone <repository-url>
cd WebLedger
```

### 2. 配置数据库连接

编辑 `web/appsettings.Development.json`（开发环境）或 `web/appsettings.json`（生产环境）：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "LedgerDatabase": "server=localhost; port=3306; database=ledger; user=ledger-bot; password=your_secure_password; Persist Security Info=False; Connect Timeout=300"
  }
}
```

**或者使用环境变量**（推荐用于生产环境）：

```bash
# Linux/macOS
export WL_SQL_DB=ledger
export WL_SQL_HOST=localhost
export WL_SQL_PORT=3306
export WL_SQL_USER=ledger-bot
export WL_SQL_PWD=your_secure_password

# Windows PowerShell
$env:WL_SQL_DB="ledger"
$env:WL_SQL_HOST="localhost"
$env:WL_SQL_PORT="3306"
$env:WL_SQL_USER="ledger-bot"
$env:WL_SQL_PWD="your_secure_password"

# Windows CMD
set WL_SQL_DB=ledger
set WL_SQL_HOST=localhost
set WL_SQL_PORT=3306
set WL_SQL_USER=ledger-bot
set WL_SQL_PWD=your_secure_password
```

### 3. 构建和运行后端

```bash
# 切换到 web 目录
cd web

# 恢复依赖
dotnet restore

# 应用数据库迁移（自动创建表结构）
dotnet ef database update

# 运行开发服务器
dotnet run

# 或者构建并运行发布版本
dotnet build -c Release
dotnet run -c Release
```

后端服务将启动在 **http://localhost:5143**

### 4. 验证后端运行

打开浏览器访问：

- Swagger UI（仅开发模式）: http://localhost:5143/swagger
- API 健康检查: http://localhost:5143/Ledger/category

如果返回空数组 `[]` 或者 Swagger 页面正常显示，说明后端运行成功。

### 5. （可选）配置访问控制

如果需要启用访问控制，使用 CLI 工具生成访问凭证：

```bash
# 切换到 cli 目录
cd ../cli

# 配置 config.json 为直接数据库访问模式
cat > config.json << 'EOF'
{
  "target": "mysql",
  "host": "server=localhost; port=3306; database=ledger; user=ledger-bot; password=your_secure_password; Persist Security Info=False; Connect Timeout=300"
}
EOF

# 运行 CLI
dotnet run

# 在 CLI 中执行 grant 命令
# MobileSuit> grant root
# 系统会返回生成的 secret，请妥善保存
```

---

## 前端部署

### 1. 安装 Node.js 和 pnpm

#### 安装 Node.js

- **Windows/macOS**: 从 https://nodejs.org/ 下载安装包
- **Linux**:
  ```bash
  curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
  sudo apt-get install -y nodejs
  ```

#### 安装 pnpm

```bash
# 使用 npm 安装 pnpm
npm install -g pnpm

# 验证安装
pnpm --version
```

### 2. 安装前端依赖

```bash
# 从项目根目录
cd frontend

# 安装依赖
pnpm install
```

如果遇到网络问题，可以配置国内镜像：

```bash
pnpm config set registry https://registry.npmmirror.com
pnpm install
```

### 3. 配置环境变量

创建 `.env.local` 文件（可选，开发环境使用代理无需配置）：

```bash
# frontend/.env.local
VITE_API_BASE_URL=http://localhost:5143
```

### 4. 运行前端开发服务器

```bash
pnpm dev
```

前端将启动在 **http://localhost:5173**

### 5. 构建生产版本

```bash
# 构建
pnpm build

# 预览构建结果
pnpm preview
```

构建产物在 `frontend/dist` 目录，可以部署到任何静态文件服务器。

---

## 完整启动流程

以下是从零开始启动整个项目的完整步骤：

### 步骤 1: 启动 MySQL 数据库

```bash
# Linux/macOS
sudo systemctl start mysql

# macOS (Homebrew)
brew services start mysql

# Windows
# 在服务管理器中启动 MySQL 服务，或
net start MySQL80
```

### 步骤 2: 启动后端服务

```bash
# 打开终端 1
cd /path/to/WebLedger/web
dotnet run
```

等待看到类似以下输出：

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5143
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 步骤 3: 启动前端服务

```bash
# 打开终端 2
cd /path/to/WebLedger/frontend
pnpm dev
```

等待看到类似以下输出：

```
  VITE v7.x.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
```

### 步骤 4: 访问应用

打开浏览器访问 **http://localhost:5173**

### 步骤 5: （首次使用）初始化数据

1. 访问前端界面
2. 进入"分类管理"页面，创建一些分类（如：餐饮、交通、工资等）
3. 进入"新建条目"页面，添加几条测试数据
4. 返回"仪表盘"查看统计信息

---

## 生产环境部署

### 后端生产部署

#### 1. 使用 Docker（推荐）

```bash
cd web
dotnet publish --os linux /t:PublishContainer
docker run -d \
  -p 5143:8080 \
  -e WL_SQL_HOST=your_db_host \
  -e WL_SQL_DB=ledger \
  -e WL_SQL_USER=ledger-bot \
  -e WL_SQL_PWD=your_password \
  --name webledger-api \
  webledger:latest
```

#### 2. 使用 Nginx 反向代理

安装 Nginx 并配置：

```nginx
server {
    listen 80;
    server_name api.yourdomain.com;

    location / {
        proxy_pass http://localhost:5143;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### 前端生产部署

#### 1. 构建静态文件

```bash
cd frontend
pnpm build
```

#### 2. 部署到 Nginx

```nginx
server {
    listen 80;
    server_name yourdomain.com;
    root /path/to/WebLedger/frontend/dist;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /Ledger {
        proxy_pass http://localhost:5143;
    }

    location /Config {
        proxy_pass http://localhost:5143;
    }
}
```

#### 3. 使用 serve（简单方式）

```bash
npm install -g serve
cd frontend/dist
serve -s . -p 3000
```

---

## 常见问题

### 1. 数据库连接失败

**错误信息**: `Unable to connect to any of the specified MySQL hosts`

**解决方案**:
- 检查 MySQL 服务是否启动：`sudo systemctl status mysql`
- 验证数据库用户和密码是否正确
- 检查防火墙设置
- 尝试使用 `127.0.0.1` 替代 `localhost`

### 2. 后端迁移失败

**错误信息**: `Unable to create migration` 或 `database does not exist`

**解决方案**:
```bash
# 删除现有迁移记录
cd web
dotnet ef database drop -f

# 重新应用迁移
dotnet ef database update

# 如果仍然失败，手动创建数据库
mysql -u root -p -e "CREATE DATABASE ledger"
```

### 3. 前端无法连接后端

**错误信息**: `Network Error` 或 `Failed to fetch`

**解决方案**:
- 确认后端服务正在运行（http://localhost:5143）
- 检查浏览器控制台的网络请求
- 验证 Vite 代理配置（`vite.config.ts`）
- 检查防火墙或杀毒软件设置

### 4. 端口被占用

**错误信息**: `Address already in use`

**解决方案**:
```bash
# 查找占用端口的进程
# Linux/macOS
lsof -i :5143
lsof -i :5173

# Windows
netstat -ano | findstr :5143
netstat -ano | findstr :5173

# 终止进程或更改端口配置
```

### 5. pnpm 安装依赖失败

**错误信息**: 各种网络错误

**解决方案**:
```bash
# 清除缓存
pnpm store prune

# 使用国内镜像
pnpm config set registry https://registry.npmmirror.com

# 重新安装
rm -rf node_modules pnpm-lock.yaml
pnpm install
```

### 6. .NET SDK 版本不兼容

**错误信息**: `The current .NET SDK does not support targeting .NET 8.0`

**解决方案**:
```bash
# 下载并安装 .NET 8 SDK
# https://dotnet.microsoft.com/download/dotnet/8.0

# 验证安装
dotnet --list-sdks
```

---

## 开发工具推荐

- **IDE**: Visual Studio Code, Visual Studio, JetBrains Rider
- **数据库管理**: MySQL Workbench, DBeaver, phpMyAdmin
- **API 测试**: Postman, Insomnia, curl
- **浏览器扩展**: React Developer Tools, Vue.js devtools

---

## 安全建议

1. **生产环境**：
   - 更改默认数据库密码
   - 启用 HTTPS
   - 配置访问控制（wl-access 和 wl-secret）
   - 定期备份数据库

2. **开发环境**：
   - 不要将 `.env.local` 提交到 Git
   - 使用环境变量存储敏感信息
   - 定期更新依赖包

---

## 更多帮助

- 查看项目 README: `README.md`
- 前端文档: `frontend/README.md`
- 后端文档: `CLAUDE.md`
- 提交 Issue: GitHub Issues

---

**祝您使用愉快！**
