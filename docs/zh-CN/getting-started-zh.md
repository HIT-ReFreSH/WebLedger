# WebLedger 入门指南

本指南将帮助您快速搭建并开始使用 WebLedger 进行开发。

## 环境要求

- .NET 8 SDK ([下载](https://dotnet.microsoft.com/download/dotnet/8.0))
- MySQL 8.0+ 数据库服务器
- 您喜欢的 IDE（Visual Studio、VS Code、Rider 等）

## 快速开始：运行后端服务

### 选项 1：本地开发（推荐用于开发环境）

#### 1. 准备 MySQL 数据库

为 WebLedger 创建新的数据库和用户：

```sql
CREATE DATABASE ledger;
CREATE USER 'ledger-bot'@'localhost' IDENTIFIED BY 'your-password';
GRANT ALL PRIVILEGES ON ledger.* TO 'ledger-bot'@'localhost';
FLUSH PRIVILEGES;
```

#### 2. 配置应用程序

在 `web/` 目录下创建开发环境配置文件：

```bash
# 从基础配置复制
cp appsettings.json appsettings.Development.json

# 编辑配置文件（使用你喜欢的编辑器）
nano appsettings.Development.json
```

添加以下数据库连接和 JWT 配置：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ledger;User=ledger-bot;Password=your-password;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long",
    "Issuer": "WebLedger",
    "Audience": "WebLedgerUsers"
  }
}
```

#### 3. 还原依赖并运行

```bash
# 进入后端项目目录
cd web

# 还原 NuGet 包
dotnet restore

# 运行应用程序
dotnet run
```

应用程序将在 `https://localhost:5001` 和 `http://localhost:5000` 启动。

### 选项 2：使用 Docker 运行

#### 1. 启动所有服务

```bash
# 在项目根目录运行
docker-compose up -d
```

这将启动：
- MySQL 数据库（端口 3306）
- WebLedger 后端 API（端口 5000）
- 管理界面（如果配置了）

#### 2. 检查服务状态

```bash
docker-compose ps
```

#### 3. 查看日志

```bash
docker-compose logs -f web
```

## 验证安装

### 1. 检查健康状态

```bash
curl http://localhost:5000/health
```

应该返回：`{"status":"Healthy"}`

### 2. 获取 API 文档

访问 `http://localhost:5000/swagger` 查看 Swagger UI 界面。

## 前端开发

前端项目位于 `frontend/` 目录。请参考 [前端集成指南](frontend-integration-zh.md) 了解如何设置和运行前端。

## 故障排除

### 常见问题

#### 数据库连接失败
- 确保 MySQL 服务正在运行
- 检查连接字符串中的密码是否正确
- 验证用户权限

#### 端口已被占用
- 检查 5000 和 5001 端口是否被其他程序占用
- 修改 `appsettings.json` 中的 URL 设置

#### .NET SDK 版本不匹配
```bash
# 检查已安装的 .NET 版本
dotnet --list-sdks

# 如果未安装 .NET 8，请从官网下载
```

## 下一步

- 阅读 [API 文档](../api/) 了解可用端点
- 查看 [前端集成指南](frontend-integration-zh.md) 学习如何连接前端
- 探索 [贡献指南](../CONTRIBUTING.md) 参与项目开发
- 查看 [示例项目](../examples/) 获取更多使用示例

## 获取帮助

- 查看 [GitHub Issues](https://github.com/HIT-ReFreSH/WebLedger/issues)
- 查阅项目 Wiki
- 联系维护团队

---

