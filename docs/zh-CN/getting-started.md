```markdown
# 开始使用 WebLedger

## 简介

WebLedger 是一个基于 HTTP 的轻量级账本系统，旨在帮助用户轻松管理个人财务。

### 主要功能

- 📝 **记录收入和支出** - 轻松添加财务记录
- 🏷️ **分类管理** - 自定义分类标签
- 📊 **自动报表** - 生成财务分析报告
- 🔐 **安全验证** - 基于 Token 的认证机制

## 快速开始

### 1. 安装

使用以下命令克隆仓库：

```bash
git clone https://github.com/HIT-ReFreeSH/WebLedger.git
cd WebLedger
```

### 2. 安装依赖

确保已安装 Go 1.18 或更高版本：

```bash
# 使用 Go 模块下载依赖
go mod download

# 或者使用 go get（旧版本）
go get -d ./...
```

### 3. 配置环境

复制环境变量示例文件并根据需要修改：

```bash
cp .env.example .env
# 编辑 .env 文件设置你的配置
```

### 4. 启动服务器

启动 WebLedger 服务器：

```bash
# 开发模式（带热重载）
go run main.go

# 生产模式
go build -o webledger
./webledger
```

服务器默认运行在 `http://localhost:8080`。

### 5. 验证安装

打开浏览器或使用 curl 验证服务器是否正常运行：

```bash
curl http://localhost:8080/health
```

你应该看到类似 `{"status":"ok"}` 的响应。

## 基本概念

### 记录（Record）

记录是 WebLedger 中的基本数据单元，包含以下字段：

| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 唯一标识符 |
| title | string | 记录标题 |
| amount | number | 金额（正数为收入，负数为支出） |
| type | enum | 类型：`income`（收入）或 `expense`（支出） |
| category | string | 分类标签 |
| date | string | 日期（ISO 8601 格式） |
| description | string | 详细描述（可选） |

### 分类（Category）

分类用于组织记录，支持嵌套结构：

- 基本分类：餐饮、交通、娱乐、工资等
- 自定义分类：用户可创建自己的分类
- 标签系统：一个记录可以有多个标签

### 报表（Report）

WebLedger 自动生成多种报表：
- 每日/每周/每月汇总
- 分类支出分析
- 收入 vs 支出对比
- 趋势图表

## 认证和授权

### 获取访问令牌

```bash
# 使用 curl 登录获取令牌
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"your_username","password":"your_password"}'
```

响应示例：
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires_at": "2024-12-31T23:59:59Z",
  "user": {
    "id": "123",
    "username": "your_username",
    "email": "user@example.com"
  }
}
```

### 使用令牌访问 API

在所有 API 请求的 Header 中包含令牌：

```http
Authorization: Bearer your_token_here
```

## API 使用示例

### 创建新记录

```bash
curl -X POST http://localhost:8080/api/records \
  -H "Authorization: Bearer your_token" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "工资收入",
    "amount": 10000,
    "type": "income",
    "category": "工资",
    "date": "2024-11-01",
    "description": "十一月份工资"
  }'
```

### 查询记录

```bash
# 获取所有记录
curl -H "Authorization: Bearer your_token" \
  http://localhost:8080/api/records

# 带过滤条件查询
curl -H "Authorization: Bearer your_token" \
  "http://localhost:8080/api/records?type=income&category=工资"
```

### 生成报表

```bash
# 获取月度报表
curl -H "Authorization: Bearer your_token" \
  http://localhost:8080/api/reports/monthly

# 获取分类统计
curl -H "Authorization: Bearer your_token" \
  http://localhost:8080/api/reports/by-category
```

## 命令行客户端

WebLedger 提供了命令行客户端，便于快速操作：

### 安装 CLI

```bash
# 从源码构建
go build -o wledger cmd/cli/main.go

# 或直接使用 go run
go run cmd/cli/main.go --help
```

### 常用命令

```bash
# 登录
wledger login --username admin --password password123

# 添加记录
wledger add --title "午餐" --amount -50 --category "餐饮"

# 查看记录
wledger list --limit 10

# 生成报表
wledger report --period month --output html
```

## 配置选项

### 服务器配置

通过环境变量或配置文件设置：

```yaml
# config.yaml 示例
server:
  port: 8080
  host: "0.0.0.0"
  debug: true

database:
  type: "sqlite"
  path: "./data/webledger.db"

auth:
  secret_key: "your-secret-key"
  token_expiry: 24h
```

### 环境变量

```bash
# .env 文件示例
PORT=8080
DATABASE_URL=sqlite://./data/webledger.db
JWT_SECRET=your-secret-key-here
LOG_LEVEL=info
```

## 故障排除

### 常见问题

1. **服务器无法启动**
   - 检查端口是否被占用：`netstat -ano | findstr :8080`
   - 确认 Go 版本：`go version`
   - 检查依赖：`go mod tidy`

2. **认证失败**
   - 确认令牌未过期
   - 检查 Header 格式：`Authorization: Bearer <token>`
   - 验证用户名密码是否正确

3. **数据库问题**
   - 确保数据库文件可写
   - 检查数据库迁移：`go run cmd/migrate/main.go`
   - 查看日志获取详细错误信息

### 获取日志

```bash
# 查看服务器日志
tail -f logs/webledger.log

# 启用调试模式
LOG_LEVEL=debug go run main.go
```

## 下一步

- 查看 [API 参考文档](../api-reference.md) 获取完整的 API 说明
- 阅读 [前端集成指南](./frontend-integration.md) 了解如何构建 Web 界面
- 探索 [示例项目](../examples/) 获取更多使用灵感
- 查看 [贡献指南](../CONTRIBUTING.md) 参与项目开发

## 获取帮助

如果你遇到问题：

1. **查看文档**：仔细阅读相关文档
2. **搜索 Issues**：在 [GitHub Issues](https://github.com/HIT-ReFreeSH/WebLedger/issues) 中查找类似问题
3. **提问**：如果找不到解决方案，创建新的 Issue，包含：
   - 详细的问题描述
   - 复现步骤
   - 错误日志
   - 环境信息（操作系统、Go 版本等）

## 贡献

WebLedger 是开源项目，欢迎贡献！

- 报告 Bug
- 请求新功能
- 改进文档
- 提交代码

请阅读 [贡献指南](../CONTRIBUTING.md) 了解如何开始。
```
