# 开始使用WebLedger

本指南将帮助您快速搭建环境并开始基于 WebLedger 进行开发。

## 前置条件

- .NET 8 SDK ([下载](https://dotnet.microsoft.com/download/dotnet/8.0))
- MySQL 8.0+ 服务器
- 您喜欢的 IDE（Visual Studio, VS Code, Rider 等）

## 快速开发：运行后端

### 选项 1：本地开发（推荐用于开发阶段）

#### 准备 MySQL 数据库

为 WebLedger 创建一个新的数据库和用户：

```sql
CREATE DATABASE ledger;
CREATE USER 'ledger-bot'@'localhost' IDENTIFIED BY 'your-password';
GRANT ALL PRIVILEGES ON ledger.* TO 'ledger-bot'@'localhost';
FLUSH PRIVILEGES;
```

#### 2. 配置应用程序

创建或修改 `web/appsettings.Development.json`：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "mysql": "database=ledger; server=localhost; port=3306; user=ledger-bot; password=your-password; Persist Security Info=False; Connect Timeout=300"
  },
  "AllowedHosts": "*"
}
```

**替代方案：使用环境变量**

应用程序支持通过环境变量进行配置。您可以设置：

- `WL_SQL_DB` - 数据库名称 (默认: `ledger`)
- `WL_SQL_HOST` - MySQL 服务器主机 (默认: `localhost`)
- `WL_SQL_PORT` - MySQL 端口 (默认: `3306`)
- `WL_SQL_USER` - 数据库用户 (默认: `ledger-bot`)
- `WL_SQL_PWD` - 数据库密码 (默认: `password`)

Windows （PowerShell） 示例：

```powershell
$env:WL_SQL_DB="ledger"
$env:WL_SQL_HOST="localhost"
$env:WL_SQL_USER="ledger-bot"
$env:WL_SQL_PWD="your-password"
```

Linux/macOS 示例：

```bash
export WL_SQL_DB=ledger
export WL_SQL_HOST=localhost
export WL_SQL_USER=ledger-bot
export WL_SQL_PWD=your-password
```

#### 3. 运行后端

导航到 web 目录并运行：

```bash
cd web
dotnet restore
dotnet run
```

服务器将在 http://localhost:5143（或控制台输出中显示的端口）启动。

**在开发模式下，API 文档位于：**
```
http://localhost:5143/swagger
```

### 选项 2：使用 Docker（快速测试）

如果您更喜欢使用 Docker：

```bash
docker run -tid \
  --name ledger \
  -p 5143:8080 \
  -e WL_SQL_DB='ledger' \
  -e WL_SQL_HOST='your-mysql-host' \
  -e WL_SQL_USER='ledger-bot' \
  -e WL_SQL_PWD='your-password' \
  hitrefresh/web-ledger:0.3.1
```

## 设置身份验证

WebLedger 使用自定义标头进行身份验证：`wl-access` 和 `wl-secret`。

### 生成初始访问凭据

您需要使用 CLI 生成第一个访问凭据：

#### 1.  配置 CLI 以便直接访问数据库

创建 `cli/config.json`:

```json
{
  "target": "mysql",
  "host": "server=localhost; port=3306; database=ledger; user=ledger-bot; password=your-password; Persist Security Info=False; Connect Timeout=300"
}
```

#### 2. 运行 CLI 并生成访问权限

```bash
cd cli
dotnet run
```

在 CLI 中，运行以下命令：

```bash
> ls              # List all available commands
> ls-acc          # List all access tokens (should be empty initially)
> grant root      # Create a new access named 'root'
```

CLI 将输出一个密钥 (secret key)。**请立即保存此密钥！**

示例输出：
```
Access: root
Secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w
```

### 更新 CLI 以便使用 HTTP 访问

现在更新 `cli/config.json` 以使用 HTTP：

```json
{
  "target": "http",
  "host": "http://localhost:5143",
  "access": "root",
  "secret": "5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"
}
```

## 验证设置

### 使用 Swagger UI 测试 API

1. 在浏览器打开 `http://localhost:5143/swagger`
2. 您将看到所有可用的 API 端点
3. 点击 "Authorize" 按钮（如果可用）或手动添加标头以测试请求

### 使用 cURL 测试

```bash
curl -X GET http://localhost:5143/ledger/categories \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"
```

## 可用的 API 端点

主要的控制器包括：

- `/ledger/*` - 账本操作（条目、分类、类型、视图）
- `/config/*` - 配置和访问管理

有关详细的 API 文档，请在开发模式下运行并查看 `/swagger` 处的 Swagger UI。

## 数据库迁移

应用程序使用 Entity Framework Core 进行数据库管理。

### 在开发模式下
启动应用程序时，数据库迁移会自动运行。

### 在生产模式下
启动时会自动应用迁移。

### 手动迁移（如果需要）
```bash
cd web
dotnet ef database update
```

## 故障排除

### 连接问题

1. **MySQL 连接失败**: 验证您的 MySQL 服务器是否正在运行且凭据正确。
2. **端口已被占用**: 修改 `Properties/launchSettings.json` 中的端口，或使用 `dotnet run --urls http://localhost:YOUR_PORT`。
3. **迁移错误**: 确保您的数据库用户拥有创建表的相应权限。

### 常见问题

**问题**: "Access denied for user"(用户被拒绝访问)
**解决方案**: Grant proper database privileges to your MySQL user(授予 MySQL 用户适当的数据库权限)

**问题**: "No connection could be made"（无法建立连接）
**解决方案**: Check if MySQL server is running and firewall settings(检查 MySQL 服务器是否正在运行以及防火墙设置。)

## 下一步

- [前端集成指南](./frontend-integration.md) - Learn how to build a React/Vue frontend
- 查看 `/swagger` 获取完整的 API 文档
- 探索 CLI 工具以进行快速数据管理

## 开发技巧

1. 在开发过程中使用 Swagger UI 进行快速 API 测试
2. CLI 工具非常适合快速数据操作和测试
3. 在开发中，服务器支持热重载 - 只需保存您的更改即可
4. 检查控制台中的日志以获取调试信息