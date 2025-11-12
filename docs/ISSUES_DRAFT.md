# GitHub Issues Draft for WebLedger

This document contains draft issues for the WebLedger project, organized by category and difficulty level.

---

## 📚 Documentation Issues

### Issue #1: Add API Usage Examples to Documentation
**标签 / Labels:** `documentation`, `good first issue`, `help wanted`
**难度 / Difficulty:** Easy
**预计时间 / Estimated Time:** 2-4 hours

#### Description / 描述

**English:**
The current documentation lacks practical API usage examples. We need to add more code examples demonstrating common operations like:
- Creating entries with different categories
- Querying entries by date range
- Working with view templates and automations
- Error handling best practices

These examples should be added to the existing documentation in the `docs/` folder.

**中文:**
当前文档缺少实用的 API 使用示例。我们需要添加更多代码示例来演示常见操作，例如：
- 创建不同类别的账目条目
- 按日期范围查询条目
- 使用视图模板和自动化功能
- 错误处理最佳实践

这些示例应添加到 `docs/` 文件夹中的现有文档中。

#### Acceptance Criteria / 验收标准
- [ ] Add at least 5 practical code examples
- [ ] Examples should cover both REST API and CLI usage
- [ ] Include error handling examples
- [ ] All examples should be tested and working

---

### Issue #2: Create Chinese Translation for Documentation
**标签 / Labels:** `documentation`, `i18n`, `help wanted`
**难度 / Difficulty:** Easy-Medium
**预计时间 / Estimated Time:** 4-6 hours

#### Description / 描述

**English:**
We need to create Chinese translations for our documentation files:
- `docs/getting-started.md`
- `docs/frontend-integration.md`

The translations should be placed in a new `docs/zh-CN/` directory and maintain the same structure and formatting as the English versions.

**中文:**
我们需要为文档文件创建中文翻译：
- `docs/getting-started.md`
- `docs/frontend-integration.md`

翻译应放置在新的 `docs/zh-CN/` 目录中，并保持与英文版本相同的结构和格式。

#### Acceptance Criteria / 验收标准
- [ ] Create `docs/zh-CN/` directory
- [ ] Translate getting-started.md
- [ ] Translate frontend-integration.md
- [ ] Update README.md with links to Chinese documentation
- [ ] Ensure all code examples and technical terms are properly localized

---

### Issue #3: Add Docker Compose Quick Start Guide
**标签 / Labels:** `documentation`, `enhancement`, `docker`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 3-5 hours

#### Description / 描述

**English:**
Create a comprehensive Docker Compose setup guide that allows developers to start the entire stack (MySQL + WebLedger Backend) with a single command. This should include:
- A `docker-compose.yml` file
- Environment variable configuration examples
- Volume management for data persistence
- Networking setup between services
- Troubleshooting section

**中文:**
创建一个全面的 Docker Compose 设置指南，让开发者可以通过单个命令启动整个技术栈（MySQL + WebLedger 后端）。应包括：
- 一个 `docker-compose.yml` 文件
- 环境变量配置示例
- 数据持久化的卷管理
- 服务间的网络设置
- 故障排除部分

#### Acceptance Criteria / 验收标准
- [ ] Create working `docker-compose.yml` in project root
- [ ] Add `.env.example` file with all required variables
- [ ] Create `docs/docker-compose-guide.md` (bilingual)
- [ ] Test the setup on clean system
- [ ] Include healthcheck configurations

---

## 🐛 Bug Fix Issues

### Issue #4: Fix CORS Configuration for Production Deployments
**标签 / Labels:** `bug`, `security`, `cors`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 2-3 hours

#### Description / 描述

**English:**
The current implementation doesn't have proper CORS configuration, which causes issues when frontend applications try to connect from different origins. We need to:
- Add configurable CORS policy in `Program.cs`
- Allow configuration via environment variables
- Provide secure defaults for production
- Document the CORS setup in the getting-started guide

**中文:**
当前实现没有正确的 CORS 配置，这会在前端应用尝试从不同来源连接时导致问题。我们需要：
- 在 `Program.cs` 中添加可配置的 CORS 策略
- 允许通过环境变量进行配置
- 为生产环境提供安全的默认值
- 在入门指南中记录 CORS 设置

#### Acceptance Criteria / 验收标准
- [ ] Implement CORS middleware in `web/Program.cs`
- [ ] Add environment variables: `WL_CORS_ORIGINS`, `WL_CORS_METHODS`
- [ ] Update documentation with CORS configuration examples
- [ ] Test with frontend applications from different origins

---

### Issue #5: Improve Error Messages in API Responses
**标签 / Labels:** `bug`, `api`, `good first issue`
**难度 / Difficulty:** Easy-Medium
**预计时间 / Estimated Time:** 3-4 hours

#### Description / 描述

**English:**
Current API error responses are not consistent and sometimes lack helpful information. We should:
- Standardize error response format (include error code, message, details)
- Add more descriptive error messages for common failures
- Return proper HTTP status codes
- Add request ID for debugging

Example of improved error response:
```json
{
  "error": {
    "code": "TYPE_UNDEFINED",
    "message": "The specified type does not exist",
    "details": "Type 'grocery' not found. Please create it first or check spelling.",
    "requestId": "abc-123"
  }
}
```

**中文:**
当前 API 错误响应不一致，有时缺少有用的信息。我们应该：
- 标准化错误响应格式（包括错误代码、消息、详情）
- 为常见故障添加更具描述性的错误消息
- 返回正确的 HTTP 状态码
- 添加请求 ID 以便调试

改进后的错误响应示例：
```json
{
  "error": {
    "code": "TYPE_UNDEFINED",
    "message": "指定的类型不存在",
    "details": "未找到类型 'grocery'。请先创建它或检查拼写。",
    "requestId": "abc-123"
  }
}
```

#### Acceptance Criteria / 验收标准
- [ ] Create `ErrorResponse` model class
- [ ] Implement global exception handler middleware
- [ ] Update all controllers to use standardized error responses
- [ ] Add unit tests for error handling
- [ ] Document error codes and responses

---

## ✨ Enhancement Issues

### Issue #6: Add Pagination Support to Entry Selection
**标签 / Labels:** `enhancement`, `api`, `performance`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 4-6 hours

#### Description / 描述

**English:**
The current `/ledger/select` endpoint returns all matching entries, which can cause performance issues with large datasets. We need to add proper pagination support:
- Implement cursor-based or offset-based pagination
- Add pagination metadata to responses (total count, page info)
- Support `page`, `pageSize` parameters
- Ensure efficient database queries

**中文:**
当前的 `/ledger/select` 端点返回所有匹配的条目，这在大数据集情况下会导致性能问题。我们需要添加适当的分页支持：
- 实现基于游标或基于偏移的分页
- 向响应添加分页元数据（总数、页面信息）
- 支持 `page`、`pageSize` 参数
- 确保高效的数据库查询

#### Acceptance Criteria / 验收标准
- [ ] Modify `SelectOption` model to include pagination parameters
- [ ] Update `ILedgerManager.Select()` to support pagination
- [ ] Add pagination metadata to response
- [ ] Update Swagger documentation
- [ ] Add performance tests with large datasets
- [ ] Update API documentation with pagination examples

---

### Issue #7: Add Data Export Functionality (CSV/Excel)
**标签 / Labels:** `enhancement`, `feature`, `api`
**难度 / Difficulty:** Medium-Hard
**预计时间 / Estimated Time:** 6-8 hours

#### Description / 描述

**English:**
Users should be able to export their ledger data in common formats:
- CSV export for entries, categories, and views
- Excel (XLSX) export with multiple sheets
- Support date range and category filters
- Include summary statistics in exports

Add new endpoints:
- `GET /ledger/export/csv`
- `GET /ledger/export/excel`

**中文:**
用户应该能够以常见格式导出账本数据：
- 导出条目、类别和视图的 CSV
- 带多个工作表的 Excel (XLSX) 导出
- 支持日期范围和类别过滤器
- 在导出中包含汇总统计信息

添加新的端点：
- `GET /ledger/export/csv`
- `GET /ledger/export/excel`

#### Acceptance Criteria / 验收标准
- [ ] Install required packages (EPPlus or similar)
- [ ] Implement CSV export endpoint
- [ ] Implement Excel export endpoint
- [ ] Support query parameters for filtering
- [ ] Add proper content-type headers and file naming
- [ ] Update API documentation
- [ ] Add integration tests

---

### Issue #8: Implement Type Autocreation Option
**标签 / Labels:** `enhancement`, `api`, `good first issue`
**难度 / Difficulty:** Easy-Medium
**预计时间 / Estimated Time:** 2-3 hours

#### Description / 描述

**English:**
Currently, when inserting an entry with an undefined type, the API throws `TypeUndefinedException`. Add an option to automatically create types when they don't exist:
- Add `autoCreateType` parameter to insert entry endpoint
- If enabled, automatically create the type with the provided category
- Return appropriate response indicating type was auto-created

**中文:**
目前，当插入具有未定义类型的条目时，API 会抛出 `TypeUndefinedException`。添加一个选项，在类型不存在时自动创建类型：
- 向插入条目端点添加 `autoCreateType` 参数
- 如果启用，则自动使用提供的类别创建类型
- 返回适当的响应，指示类型已自动创建

#### Acceptance Criteria / 验收标准
- [ ] Add `autoCreateType` parameter to Entry model
- [ ] Modify `DirectLedgerManager.Insert()` logic
- [ ] Return metadata about created type in response
- [ ] Update API documentation
- [ ] Add unit tests for autocreation logic

---

## 🧪 Testing Issues

### Issue #9: Add Unit Tests for LedgerManager
**标签 / Labels:** `testing`, `good first issue`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 6-8 hours

#### Description / 描述

**English:**
The `DirectLedgerManager` class lacks unit tests. We need comprehensive test coverage for:
- Entry CRUD operations
- Category management
- Type management
- View and ViewTemplate operations
- Error scenarios

Use xUnit and Moq for testing. Set up an in-memory database for tests.

**中文:**
`DirectLedgerManager` 类缺少单元测试。我们需要全面的测试覆盖：
- 条目的 CRUD 操作
- 类别管理
- 类型管理
- 视图和视图模板操作
- 错误场景

使用 xUnit 和 Moq 进行测试。为测试设置内存数据库。

#### Acceptance Criteria / 验收标准
- [ ] Create test project: `tests/LibWebLedger.Tests`
- [ ] Set up in-memory database for testing
- [ ] Write tests for all public methods in `DirectLedgerManager`
- [ ] Achieve >80% code coverage
- [ ] Add tests to CI/CD pipeline

---

### Issue #10: Add Integration Tests for API Endpoints
**标签 / Labels:** `testing`, `api`
**难度 / Difficulty:** Medium-Hard
**预计时间 / Estimated Time:** 8-10 hours

#### Description / 描述

**English:**
Create integration tests for all API endpoints using WebApplicationFactory:
- Test all endpoints in `LedgerController`
- Test all endpoints in `ConfigController`
- Test authentication/authorization flows
- Test error responses
- Test with different data scenarios

**中文:**
使用 WebApplicationFactory 为所有 API 端点创建集成测试：
- 测试 `LedgerController` 中的所有端点
- 测试 `ConfigController` 中的所有端点
- 测试身份验证/授权流程
- 测试错误响应
- 使用不同的数据场景进行测试

#### Acceptance Criteria / 验收标准
- [ ] Create test project: `tests/WebLedger.IntegrationTests`
- [ ] Use `WebApplicationFactory` for testing
- [ ] Write tests for all API endpoints
- [ ] Mock external dependencies
- [ ] Add authentication tests
- [ ] Integrate with CI/CD pipeline

---

## 🚀 Feature Issues

### Issue #11: Add Budget Tracking Feature
**标签 / Labels:** `feature`, `enhancement`
**难度 / Difficulty:** Hard
**预计时间 / Estimated Time:** 12-16 hours

#### Description / 描述

**English:**
Implement a budget tracking system that allows users to:
- Set monthly/yearly budgets for categories
- Track spending against budgets
- Get alerts when approaching budget limits
- View budget vs actual reports

New models needed:
- `Budget` (category, amount, period, startDate, endDate)
- `BudgetAlert` (threshold percentage)

New endpoints:
- `POST /ledger/budget` - Create budget
- `GET /ledger/budgets` - List budgets
- `GET /ledger/budget/{id}/status` - Get budget status
- `DELETE /ledger/budget/{id}` - Delete budget

**中文:**
实现预算跟踪系统，允许用户：
- 为类别设置月度/年度预算
- 跟踪预算支出
- 在接近预算限制时获取提醒
- 查看预算与实际报告

需要的新模型：
- `Budget`（类别、金额、期间、开始日期、结束日期）
- `BudgetAlert`（阈值百分比）

新端点：
- `POST /ledger/budget` - 创建预算
- `GET /ledger/budgets` - 列出预算
- `GET /ledger/budget/{id}/status` - 获取预算状态
- `DELETE /ledger/budget/{id}` - 删除预算

#### Acceptance Criteria / 验收标准
- [ ] Design database schema for budgets
- [ ] Create EF Core migrations
- [ ] Implement `IBudgetManager` interface
- [ ] Create `BudgetController` with all endpoints
- [ ] Add budget calculation logic
- [ ] Update Swagger documentation
- [ ] Write unit and integration tests
- [ ] Add documentation with examples

---

### Issue #12: Implement Multi-Currency Support
**标签 / Labels:** `feature`, `enhancement`, `i18n`
**难度 / Difficulty:** Hard
**预计时间 / Estimated Time:** 10-14 hours

#### Description / 描述

**English:**
Add support for multiple currencies:
- Store currency code with each entry (ISO 4217 codes)
- Add currency conversion rates table
- Allow viewing reports in different currencies
- Support currency conversion in views

Changes needed:
- Add `currency` field to `Entry` model
- Create `CurrencyRate` model for exchange rates
- Update views to support currency conversion
- Add endpoints to manage currency rates

**中文:**
添加多货币支持：
- 在每个条目中存储货币代码（ISO 4217 代码）
- 添加货币转换汇率表
- 允许以不同货币查看报告
- 在视图中支持货币转换

需要的更改：
- 向 `Entry` 模型添加 `currency` 字段
- 创建 `CurrencyRate` 模型用于汇率
- 更新视图以支持货币转换
- 添加管理货币汇率的端点

#### Acceptance Criteria / 验收标准
- [ ] Design database schema for currencies
- [ ] Add migration to add currency field to entries
- [ ] Implement currency conversion logic
- [ ] Create currency management endpoints
- [ ] Update all views to support currency conversion
- [ ] Add unit tests for currency conversion
- [ ] Update documentation

---

## 🔧 DevOps / Infrastructure Issues

### Issue #13: Set Up GitHub Actions CI/CD Pipeline
**标签 / Labels:** `ci/cd`, `devops`, `github-actions`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 4-6 hours

#### Description / 描述

**English:**
Set up automated CI/CD pipeline using GitHub Actions:
- Build and test on every push/PR
- Run unit and integration tests
- Build Docker images
- Push Docker images to registry on main branch
- Add code coverage reporting

Create workflows for:
- `.github/workflows/build-test.yml` - Build and test
- `.github/workflows/docker-publish.yml` - Build and publish Docker images

**中文:**
使用 GitHub Actions 设置自动化 CI/CD 流程：
- 在每次推送/PR 时构建和测试
- 运行单元测试和集成测试
- 构建 Docker 镜像
- 在主分支上将 Docker 镜像推送到仓库
- 添加代码覆盖率报告

创建工作流：
- `.github/workflows/build-test.yml` - 构建和测试
- `.github/workflows/docker-publish.yml` - 构建和发布 Docker 镜像

#### Acceptance Criteria / 验收标准
- [ ] Create GitHub Actions workflow files
- [ ] Configure MySQL service for tests
- [ ] Set up Docker build and push
- [ ] Add code coverage reporting (Codecov or similar)
- [ ] Add status badges to README
- [ ] Test workflows with test PR

---

### Issue #14: Add Health Check Endpoint
**标签 / Labels:** `enhancement`, `devops`, `monitoring`
**难度 / Difficulty:** Easy-Medium
**预计时间 / Estimated Time:** 2-3 hours

#### Description / 描述

**English:**
Implement health check endpoints for monitoring:
- Basic health check: `GET /health`
- Detailed health check: `GET /health/detailed`
- Check database connectivity
- Check service availability
- Return proper HTTP status codes

Use ASP.NET Core Health Checks middleware.

**中文:**
实现用于监控的健康检查端点：
- 基本健康检查：`GET /health`
- 详细健康检查：`GET /health/detailed`
- 检查数据库连接
- 检查服务可用性
- 返回适当的 HTTP 状态码

使用 ASP.NET Core Health Checks 中间件。

#### Acceptance Criteria / 验收标准
- [ ] Install `Microsoft.Extensions.Diagnostics.HealthChecks` package
- [ ] Configure health checks in `Program.cs`
- [ ] Add database health check
- [ ] Implement `/health` endpoint
- [ ] Implement `/health/detailed` endpoint
- [ ] Update documentation
- [ ] Test with failing database connection

---

### Issue #15: Add Logging and Monitoring Support
**标签 / Labels:** `enhancement`, `monitoring`, `logging`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 4-6 hours

#### Description / 描述

**English:**
Improve logging and add structured logging support:
- Implement structured logging with Serilog
- Add request/response logging middleware
- Log important business events
- Support different log outputs (Console, File, Seq, etc.)
- Add correlation IDs for request tracking

**中文:**
改进日志记录并添加结构化日志支持：
- 使用 Serilog 实现结构化日志
- 添加请求/响应日志中间件
- 记录重要的业务事件
- 支持不同的日志输出（控制台、文件、Seq 等）
- 添加请求跟踪的关联 ID

#### Acceptance Criteria / 验收标准
- [ ] Install Serilog packages
- [ ] Configure Serilog in `Program.cs`
- [ ] Add request logging middleware
- [ ] Add logging to all managers and controllers
- [ ] Support configuration via environment variables
- [ ] Add documentation for logging configuration
- [ ] Test different log outputs

---

## 📱 CLI Enhancement Issues

### Issue #16: Add Interactive Mode to CLI
**标签 / Labels:** `enhancement`, `cli`, `ux`
**难度 / Difficulty:** Medium-Hard
**预计时间 / Estimated Time:** 6-8 hours

#### Description / 描述

**English:**
Enhance the CLI with an interactive mode using a library like `Spectre.Console`:
- Add colored output
- Add progress bars for long operations
- Add interactive prompts for user input
- Add table formatting for data display
- Add command auto-completion

**中文:**
使用 `Spectre.Console` 等库增强 CLI 的交互模式：
- 添加彩色输出
- 为长时间操作添加进度条
- 为用户输入添加交互式提示
- 为数据显示添加表格格式
- 添加命令自动完成

#### Acceptance Criteria / 验收标准
- [ ] Install `Spectre.Console` package
- [ ] Add colored output for different message types
- [ ] Implement table display for list commands
- [ ] Add interactive prompts for configuration
- [ ] Add progress indicators
- [ ] Update CLI documentation
- [ ] Maintain backward compatibility

---

## 🔒 Security Issues

### Issue #17: Implement Rate Limiting
**标签 / Labels:** `security`, `enhancement`, `api`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 4-5 hours

#### Description / 描述

**English:**
Add rate limiting to prevent API abuse:
- Implement request rate limiting per access token
- Use sliding window algorithm
- Add configurable limits (requests per minute/hour)
- Return proper 429 status codes
- Add rate limit headers to responses

**中文:**
添加速率限制以防止 API 滥用：
- 为每个访问令牌实现请求速率限制
- 使用滑动窗口算法
- 添加可配置的限制（每分钟/小时的请求数）
- 返回正确的 429 状态码
- 向响应添加速率限制头

#### Acceptance Criteria / 验收标准
- [ ] Install rate limiting middleware package
- [ ] Configure rate limiting in `Program.cs`
- [ ] Add rate limit configuration via environment variables
- [ ] Implement per-access-token limiting
- [ ] Add rate limit headers (`X-RateLimit-*`)
- [ ] Add documentation
- [ ] Add tests for rate limiting

---

## 🎨 Frontend Issues

> **⚠️ Important Technical Requirements / 重要技术要求:**
> - Use **Vite** for project setup (NOT Create React App or Vue CLI)
> - Use **TypeScript** (NOT JavaScript)
> - Use **yarn with PnP** or **pnpm** for package management
> - All PRs MUST include screenshots / 所有 PR 必须附带截图
> - Comment on the issue before starting work / 开始工作前请在 issue 下评论

### Issue #18: Create React Admin Dashboard Example
**标签 / Labels:** `frontend`, `react`, `typescript`, `good first issue`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 8-12 hours

#### Description / 描述

**English:**
Create a modern React admin dashboard as a reference implementation for WebLedger. This will serve as an example for developers building frontend applications.

**Features to implement:**
- Dashboard overview with summary statistics
- Entry list with filtering and sorting
- Entry creation form with validation
- Category management page
- Type management page
- Responsive design (mobile-friendly)

**Tech Stack:**
- React 18+ with TypeScript
- Vite for build tooling
- Yarn PnP or pnpm
- TanStack Router (React Router v6+) for routing
- TanStack Query for data fetching
- Tailwind CSS or Ant Design for UI
- Zod for validation

**中文:**
创建一个现代化的 React 管理仪表板作为 WebLedger 的参考实现。这将作为开发者构建前端应用程序的示例。

**要实现的功能:**
- 带有汇总统计的仪表板概览
- 带过滤和排序的条目列表
- 带验证的条目创建表单
- 类别管理页面
- 类型管理页面
- 响应式设计（移动端友好）

**技术栈:**
- React 18+ with TypeScript
- Vite 构建工具
- Yarn PnP 或 pnpm
- TanStack Router (React Router v6+) 路由
- TanStack Query 数据获取
- Tailwind CSS 或 Ant Design UI
- Zod 验证

#### Acceptance Criteria / 验收标准
- [ ] Project initialized with Vite + React + TypeScript
- [ ] Use yarn PnP or pnpm (NOT npm)
- [ ] All components written in TypeScript (NO .jsx files)
- [ ] Implement all required pages
- [ ] Fully responsive design
- [ ] Error handling and loading states
- [ ] Code is well-commented and follows best practices
- [ ] **PR MUST include screenshots of all pages**
- [ ] README with setup instructions

#### Before Starting / 开始前
**Please comment on this issue to let us know you're working on it!**
**请在此 issue 下评论，让我们知道你正在处理它！**

---

### Issue #19: Create Vue 3 Admin Dashboard Example
**标签 / Labels:** `frontend`, `vue`, `typescript`, `good first issue`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 8-12 hours

#### Description / 描述

**English:**
Create a modern Vue 3 admin dashboard as a reference implementation for WebLedger using Composition API and `<script setup>`.

**Features to implement:**
- Dashboard overview with summary statistics
- Entry list with filtering and sorting
- Entry creation form with validation
- Category management page
- Type management page
- Responsive design (mobile-friendly)

**Tech Stack:**
- Vue 3 with TypeScript and Composition API
- Vite for build tooling
- Yarn PnP or pnpm
- Vue Router for routing
- Pinia for state management
- VueUse for utilities
- Element Plus or Naive UI for components
- Zod or Yup for validation

**中文:**
使用 Composition API 和 `<script setup>` 创建一个现代化的 Vue 3 管理仪表板作为 WebLedger 的参考实现。

**要实现的功能:**
- 带有汇总统计的仪表板概览
- 带过滤和排序的条目列表
- 带验证的条目创建表单
- 类别管理页面
- 类型管理页面
- 响应式设计（移动端友好）

**技术栈:**
- Vue 3 with TypeScript 和 Composition API
- Vite 构建工具
- Yarn PnP 或 pnpm
- Vue Router 路由
- Pinia 状态管理
- VueUse 工具库
- Element Plus 或 Naive UI 组件
- Zod 或 Yup 验证

#### Acceptance Criteria / 验收标准
- [ ] Project initialized with Vite + Vue 3 + TypeScript
- [ ] Use yarn PnP or pnpm (NOT npm)
- [ ] Use `<script setup>` syntax with TypeScript
- [ ] Implement all required pages
- [ ] Fully responsive design
- [ ] Error handling and loading states
- [ ] Code is well-commented and follows best practices
- [ ] **PR MUST include screenshots of all pages**
- [ ] README with setup instructions

#### Before Starting / 开始前
**Please comment on this issue to let us know you're working on it!**
**请在此 issue 下评论，让我们知道你正在处理它！**

---

### Issue #20: Add Data Visualization Dashboard
**标签 / Labels:** `frontend`, `visualization`, `enhancement`
**难度 / Difficulty:** Medium-Hard
**预计时间 / Estimated Time:** 10-14 hours

#### Description / 描述

**English:**
Create an interactive data visualization dashboard for WebLedger analytics. Can be built as a standalone project or integrated into Issue #18 or #19.

**Features to implement:**
- Spending trends over time (line/area charts)
- Category breakdown (pie/donut charts)
- Monthly comparison (bar charts)
- Top spending categories
- Budget vs actual visualization (if budget feature exists)
- Date range selector
- Export charts as images

**Tech Stack:**
- React or Vue 3 with TypeScript
- Vite + (Yarn PnP or pnpm)
- Recharts / Chart.js / Apache ECharts for React
- ECharts or Chart.js for Vue
- TanStack Query for data fetching

**中文:**
为 WebLedger 分析创建交互式数据可视化仪表板。可以作为独立项目构建，或集成到 Issue #18 或 #19 中。

**要实现的功能:**
- 时间趋势支出（折线图/面积图）
- 类别细分（饼图/环形图）
- 月度对比（柱状图）
- 支出最多的类别
- 预算与实际可视化（如果预算功能存在）
- 日期范围选择器
- 导出图表为图片

**技术栈:**
- React 或 Vue 3 with TypeScript
- Vite + (Yarn PnP 或 pnpm)
- Recharts / Chart.js / Apache ECharts (React)
- ECharts 或 Chart.js (Vue)
- TanStack Query 数据获取

#### Acceptance Criteria / 验收标准
- [ ] Project uses Vite + TypeScript
- [ ] Use yarn PnP or pnpm
- [ ] Implement all required charts
- [ ] Charts are interactive and responsive
- [ ] Date range filtering works correctly
- [ ] Export functionality works
- [ ] Proper error handling
- [ ] **PR MUST include screenshots of different chart views**
- [ ] README with setup instructions

#### Before Starting / 开始前
**Please comment on this issue to let us know you're working on it!**
**请在此 issue 下评论，让我们知道你正在处理它！**

---

### Issue #21: Build Reusable Component Library
**标签 / Labels:** `frontend`, `component-library`, `typescript`
**难度 / Difficulty:** Hard
**预计时间 / Estimated Time:** 16-20 hours

#### Description / 描述

**English:**
Create a reusable component library for WebLedger that can be used across different frontend implementations. This will help maintain consistency and reduce code duplication.

**Components to create:**
- `EntryCard` - Display entry information
- `EntryForm` - Create/edit entry form
- `CategorySelector` - Category dropdown/select
- `TypeSelector` - Type dropdown/select
- `DateRangePicker` - Date range selection
- `AmountInput` - Currency input with formatting
- `EntryList` - List with pagination and filtering
- `StatCard` - Statistics display card

**Tech Stack:**
- React or Vue 3 with TypeScript
- Vite for development and build
- Yarn PnP or pnpm
- Storybook for component documentation
- Vitest for unit testing
- CSS Modules or Tailwind for styling

**中文:**
为 WebLedger 创建一个可重用的组件库，可在不同的前端实现中使用。这将有助于保持一致性并减少代码重复。

**要创建的组件:**
- `EntryCard` - 显示条目信息
- `EntryForm` - 创建/编辑条目表单
- `CategorySelector` - 类别下拉选择
- `TypeSelector` - 类型下拉选择
- `DateRangePicker` - 日期范围选择
- `AmountInput` - 带格式化的货币输入
- `EntryList` - 带分页和过滤的列表
- `StatCard` - 统计显示卡片

**技术栈:**
- React 或 Vue 3 with TypeScript
- Vite 开发和构建
- Yarn PnP 或 pnpm
- Storybook 组件文档
- Vitest 单元测试
- CSS Modules 或 Tailwind 样式

#### Acceptance Criteria / 验收标准
- [ ] All components written in TypeScript
- [ ] Use Vite + (yarn PnP or pnpm)
- [ ] Each component has Storybook stories
- [ ] Each component has unit tests (>80% coverage)
- [ ] Components are fully typed with proper interfaces
- [ ] Comprehensive documentation
- [ ] **PR MUST include screenshots from Storybook**
- [ ] Published as npm package (optional)

#### Before Starting / 开始前
**Please comment on this issue to let us know you're working on it!**
**请在此 issue 下评论，让我们知道你正在处理它！**

---

### Issue #22: Add Frontend Unit Tests
**标签 / Labels:** `frontend`, `testing`, `typescript`, `good first issue`
**难度 / Difficulty:** Easy-Medium
**预计时间 / Estimated Time:** 6-8 hours

#### Description / 描述

**English:**
Add comprehensive unit tests for frontend components and utilities. This can be applied to any existing frontend implementation (Issue #18, #19, or others).

**What to test:**
- Component rendering
- User interactions (clicks, form inputs)
- API integration layer
- Form validation logic
- State management
- Utility functions

**Tech Stack:**
- Vitest (preferred) or Jest
- Testing Library (React Testing Library or Vue Testing Library)
- MSW (Mock Service Worker) for API mocking

**中文:**
为前端组件和工具添加全面的单元测试。这可以应用于任何现有的前端实现（Issue #18、#19 或其他）。

**要测试的内容:**
- 组件渲染
- 用户交互（点击、表单输入）
- API 集成层
- 表单验证逻辑
- 状态管理
- 工具函数

**技术栈:**
- Vitest（首选）或 Jest
- Testing Library (React Testing Library 或 Vue Testing Library)
- MSW (Mock Service Worker) API 模拟

#### Acceptance Criteria / 验收标准
- [ ] Set up testing framework (Vitest recommended)
- [ ] Add tests for all major components
- [ ] Achieve >80% code coverage
- [ ] All tests pass in CI/CD
- [ ] Include integration tests for API calls
- [ ] Document testing approach in README
- [ ] **PR MUST include screenshot of coverage report**

#### Before Starting / 开始前
**Please comment on this issue to let us know you're working on it!**
**请在此 issue 下评论，让我们知道你正在处理它！**

---

### Issue #23: Implement Progressive Web App (PWA)
**标签 / Labels:** `frontend`, `pwa`, `enhancement`
**难度 / Difficulty:** Medium
**预计时间 / Estimated Time:** 6-8 hours

#### Description / 描述

**English:**
Convert an existing frontend implementation (from Issue #18 or #19) into a Progressive Web App with offline support.

**Features to implement:**
- Service worker for offline caching
- App manifest for installability
- Offline data persistence (IndexedDB)
- Sync data when back online
- Push notifications (optional)
- App-like experience on mobile

**Tech Stack:**
- vite-plugin-pwa
- Workbox for service worker
- IndexedDB or localForage for storage

**中文:**
将现有的前端实现（来自 Issue #18 或 #19）转换为具有离线支持的渐进式 Web 应用程序。

**要实现的功能:**
- Service Worker 离线缓存
- App manifest 可安装性
- 离线数据持久化（IndexedDB）
- 恢复在线时同步数据
- 推送通知（可选）
- 移动端类似应用的体验

**技术栈:**
- vite-plugin-pwa
- Workbox for service worker
- IndexedDB 或 localForage 存储

#### Acceptance Criteria / 验收标准
- [ ] Install and configure vite-plugin-pwa
- [ ] Create app manifest
- [ ] Implement service worker with caching strategies
- [ ] Add offline data persistence
- [ ] Implement sync when online
- [ ] App is installable on mobile devices
- [ ] Lighthouse PWA score >90
- [ ] **PR MUST include screenshots of PWA install prompt and offline mode**

#### Before Starting / 开始前
**Please comment on this issue to let us know you're working on it!**
**请在此 issue 下评论，让我们知道你正在处理它！**

---

### Issue #24: Create Mobile-First Responsive Design
**标签 / Labels:** `frontend`, `ui/ux`, `mobile`, `good first issue`
**难度 / Difficulty:** Easy-Medium
**预计时间 / Estimated Time:** 6-8 hours

#### Description / 描述

**English:**
Improve and optimize the existing frontend for mobile devices. This can be applied to any frontend implementation.

**Improvements needed:**
- Mobile-first CSS approach
- Touch-friendly UI elements (larger buttons, proper spacing)
- Bottom navigation for mobile
- Swipe gestures for common actions
- Optimized forms for mobile input
- Responsive tables/lists
- Dark mode support

**Tech Stack:**
- Tailwind CSS (recommended) or plain CSS
- CSS Grid and Flexbox
- Mobile-first media queries
- Touch event handling

**中文:**
改进和优化现有前端以适配移动设备。这可以应用于任何前端实现。

**需要的改进:**
- 移动优先的 CSS 方法
- 触摸友好的 UI 元素（更大的按钮、适当的间距）
- 移动端底部导航
- 常见操作的滑动手势
- 针对移动输入优化的表单
- 响应式表格/列表
- 深色模式支持

**技术栈:**
- Tailwind CSS（推荐）或纯 CSS
- CSS Grid 和 Flexbox
- 移动优先的媒体查询
- 触摸事件处理

#### Acceptance Criteria / 验收标准
- [ ] All pages are fully responsive
- [ ] Touch targets are at least 44x44px
- [ ] Navigation works well on mobile
- [ ] Forms are easy to use on mobile
- [ ] Tables/lists have mobile-optimized layout
- [ ] Dark mode implemented
- [ ] Tested on multiple devices/screen sizes
- [ ] **PR MUST include screenshots from mobile devices (phone and tablet)**

#### Before Starting / 开始前
**Please comment on this issue to let us know you're working on it!**
**请在此 issue 下评论，让我们知道你正在处理它！**

---

### Issue #25: Add End-to-End Tests with Playwright
**标签 / Labels:** `frontend`, `testing`, `e2e`
**难度 / Difficulty:** Medium-Hard
**预计时间 / Estimated Time:** 8-10 hours

#### Description / 描述

**English:**
Add end-to-end tests for critical user flows using Playwright. This ensures the entire application works correctly from the user's perspective.

**User flows to test:**
- User creates a new entry
- User edits an existing entry
- User filters entries by date range
- User manages categories
- User manages types
- User views dashboard statistics
- Error handling scenarios

**Tech Stack:**
- Playwright for E2E testing
- TypeScript for test code
- GitHub Actions for CI integration

**中文:**
使用 Playwright 为关键用户流程添加端到端测试。这确保整个应用程序从用户角度正常工作。

**要测试的用户流程:**
- 用户创建新条目
- 用户编辑现有条目
- 用户按日期范围过滤条目
- 用户管理类别
- 用户管理类型
- 用户查看仪表板统计
- 错误处理场景

**技术栈:**
- Playwright E2E 测试
- TypeScript 测试代码
- GitHub Actions CI 集成

#### Acceptance Criteria / 验收标准
- [ ] Install and configure Playwright
- [ ] Write E2E tests in TypeScript
- [ ] Cover all critical user flows
- [ ] Tests run in multiple browsers (Chrome, Firefox, Safari)
- [ ] Tests run in CI/CD pipeline
- [ ] Include visual regression tests (optional)
- [ ] **PR MUST include screenshots/videos of test runs**
- [ ] README with instructions to run tests locally

#### Before Starting / 开始前
**Please comment on this issue to let us know you're working on it!**
**请在此 issue 下评论，让我们知道你正在处理它！**

---

## Summary / 总结

### Issue Distribution / Issue 分布
- **Documentation (文档)**: 3 issues (#1-3)
- **Bug Fix (错误修复)**: 2 issues (#4-5)
- **Enhancement (增强)**: 3 issues (#6-8)
- **Feature (新功能)**: 2 issues (#11-12)
- **Testing (测试)**: 2 issues (#9-10)
- **DevOps (运维)**: 3 issues (#13-15)
- **CLI (命令行)**: 1 issue (#16)
- **Security (安全)**: 1 issue (#17)
- **Frontend (前端)**: 8 issues (#18-25)

**Total: 25 issues / 总共：25 个 issue**

### Difficulty Levels / 难度分布
- **Easy / 简单** (good first issue): 4 issues
- **Easy-Medium / 简单-中等**: 6 issues
- **Medium / 中等**: 8 issues
- **Medium-Hard / 中等-困难**: 4 issues
- **Hard / 困难**: 3 issues

### Labels to Create / 需要创建的标签
- `documentation` - 文档相关
- `bug` - 错误修复
- `enhancement` - 功能增强
- `feature` - 新功能
- `testing` - 测试相关
- `ci/cd` - CI/CD 相关
- `devops` - DevOps 相关
- `security` - 安全相关
- `api` - API 相关
- `cli` - CLI 相关
- `frontend` - 前端相关
- `react` - React 框架
- `vue` - Vue 框架
- `typescript` - TypeScript
- `component-library` - 组件库
- `visualization` - 数据可视化
- `pwa` - Progressive Web App
- `ui/ux` - 用户界面/体验
- `mobile` - 移动端
- `e2e` - 端到端测试
- `good first issue` - 适合新手
- `help wanted` - 需要帮助
- `i18n` - 国际化
- `performance` - 性能相关
- `docker` - Docker 相关
- `monitoring` - 监控相关

---

## 📋 Contribution Guidelines / 贡献指南

### How to Claim an Issue / 如何领取 Issue

**English:**
1. **Comment on the issue** before starting work to let maintainers and other contributors know you're working on it
2. Wait for a maintainer to assign the issue to you (optional but recommended)
3. Fork the repository and create a new branch for your work
4. Follow the technical requirements specified in the issue
5. Submit a PR when ready

**中文:**
1. **在开始工作之前在 issue 下评论**，让维护者和其他贡献者知道你正在处理它
2. 等待维护者将 issue 分配给你（可选但推荐）
3. Fork 仓库并为你的工作创建新分支
4. 遵循 issue 中指定的技术要求
5. 准备好后提交 PR

### Pull Request Requirements / PR 要求

#### For ALL PRs / 所有 PR 的要求

**English:**
- [ ] Code follows the project's coding standards
- [ ] All tests pass locally
- [ ] No merge conflicts
- [ ] Clear PR description explaining what was done and why
- [ ] Reference the related issue number (e.g., "Closes #18")

**中文:**
- [ ] 代码遵循项目的编码标准
- [ ] 所有测试在本地通过
- [ ] 没有合并冲突
- [ ] 清晰的 PR 描述，解释做了什么以及为什么
- [ ] 引用相关的 issue 编号（例如 "Closes #18"）

#### For Frontend PRs / 前端 PR 的要求

**⚠️ MANDATORY / 强制要求:**

**English:**
- [ ] **MUST include screenshots** showing the implemented functionality
  - For UI changes: Show before/after (if applicable) and different states
  - For responsive design: Show mobile, tablet, and desktop views
  - For dark mode: Show both light and dark themes
  - For charts/visualizations: Show different data scenarios
- [ ] Use **Vite** (NOT Create React App, Vue CLI, or other deprecated tools)
- [ ] Use **TypeScript** (NO plain JavaScript files)
- [ ] Use **yarn with PnP** or **pnpm** (NOT npm)
- [ ] Include a README with:
  - Setup instructions
  - Development commands
  - Build commands
  - Environment variables (if any)

**中文:**
- [ ] **必须包含截图**，展示实现的功能
  - 对于 UI 更改：显示前后对比（如适用）和不同状态
  - 对于响应式设计：显示移动端、平板和桌面视图
  - 对于深色模式：显示浅色和深色主题
  - 对于图表/可视化：显示不同的数据场景
- [ ] 使用 **Vite**（不使用 Create React App、Vue CLI 或其他已过时的工具）
- [ ] 使用 **TypeScript**（不使用纯 JavaScript 文件）
- [ ] 使用 **yarn with PnP** 或 **pnpm**（不使用 npm）
- [ ] 包含 README，内含：
  - 安装说明
  - 开发命令
  - 构建命令
  - 环境变量（如果有）

#### Screenshot Examples / 截图示例

**Good PR Screenshots / 好的 PR 截图:**
```
## Screenshots

### Desktop View
![Desktop view](link-to-image)

### Mobile View
![Mobile view](link-to-image)

### Dark Mode
![Dark mode](link-to-image)

### Loading State
![Loading state](link-to-image)

### Error State
![Error state](link-to-image)
```

### PR Template / PR 模板

```markdown
## Description / 描述
<!-- Brief description of what this PR does -->
<!-- 简要描述此 PR 的作用 -->

## Related Issue / 相关 Issue
Closes #[issue number]

## Type of Change / 更改类型
- [ ] Bug fix / 错误修复
- [ ] New feature / 新功能
- [ ] Enhancement / 功能增强
- [ ] Documentation / 文档
- [ ] Testing / 测试

## Technical Stack (for frontend PRs) / 技术栈（前端 PR）
- Framework: React/Vue 3
- Build tool: Vite
- Package manager: yarn PnP / pnpm
- Language: TypeScript
- UI Library: [name]
- Other: [list other major dependencies]

## Screenshots / 截图
<!-- REQUIRED for frontend PRs / 前端 PR 必需 -->

### Desktop / 桌面端
[Add screenshots here]

### Mobile / 移动端
[Add screenshots here]

### Other Views / 其他视图
[Add any other relevant screenshots]

## Checklist / 检查清单
- [ ] My code follows the project's coding standards
- [ ] I have commented my code where necessary
- [ ] I have updated the documentation accordingly
- [ ] My changes generate no new warnings
- [ ] I have added tests that prove my fix/feature works
- [ ] All tests pass locally
- [ ] For frontend PRs: I have included screenshots
- [ ] For frontend PRs: I used Vite + TypeScript + (yarn PnP or pnpm)

## Additional Notes / 附加说明
<!-- Any additional information -->
<!-- 任何其他信息 -->
```

### Important Reminders / 重要提醒

**English:**
1. **PRs without screenshots will not be reviewed** (for frontend issues)
2. **Always comment on issues before starting work** to avoid duplicate efforts
3. Follow the technical requirements strictly - PRs using deprecated tools (CRA, Vue CLI, npm) may be rejected
4. Be patient - maintainers may take time to review PRs
5. Be open to feedback and willing to make changes

**中文:**
1. **没有截图的 PR 将不会被审查**（针对前端 issue）
2. **在开始工作前始终在 issue 上评论**，以避免重复工作
3. 严格遵循技术要求 - 使用已过时工具（CRA、Vue CLI、npm）的 PR 可能会被拒绝
4. 保持耐心 - 维护者可能需要时间审查 PR
5. 对反馈持开放态度，愿意进行修改

---

## 🚀 Getting Started for Contributors / 贡献者入门

### For Backend Contributors / 后端贡献者

1. Read [Getting Started Guide](./getting-started.md)
2. Set up your local development environment
3. Pick an issue and comment on it
4. Fork, code, test, and submit PR

### For Frontend Contributors / 前端贡献者

1. Read [Frontend Integration Guide](./frontend-integration.md)
2. Ensure you have Node.js 18+ installed
3. Pick an issue and comment on it
4. Create your frontend project with:
   ```bash
   # For React
   npm create vite@latest my-app -- --template react-ts
   cd my-app
   pnpm install  # or: yarn install (with PnP)

   # For Vue
   npm create vite@latest my-app -- --template vue-ts
   cd my-app
   pnpm install  # or: yarn install (with PnP)
   ```
5. Implement the feature following the issue requirements
6. Take screenshots of your work
7. Submit PR with screenshots

### Need Help? / 需要帮助？

- Check existing documentation in `docs/`
- Ask questions in the issue comments
- Look at the Swagger UI at `/swagger` for API documentation
- Review existing code for examples

---

## 📊 Priority Recommendations / 优先级推荐

### High Priority / 高优先级
These issues address immediate needs and will have the most impact:

- **#4** - Fix CORS Configuration (blocks frontend development)
- **#13** - Set Up CI/CD Pipeline (improves development workflow)
- **#18** or **#19** - Create Admin Dashboard (provides reference implementation)
- **#6** - Add Pagination Support (performance issue)

### Good First Issues / 适合新手的 Issue
Start here if you're new to the project:

- **#1** - Add API Usage Examples
- **#5** - Improve Error Messages
- **#8** - Implement Type Autocreation
- **#22** - Add Frontend Unit Tests
- **#24** - Mobile-First Responsive Design

### Advanced / 进阶
For experienced contributors:

- **#11** - Budget Tracking Feature
- **#12** - Multi-Currency Support
- **#21** - Build Component Library
- **#25** - E2E Tests with Playwright
