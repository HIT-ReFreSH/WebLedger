# WebLedger 前端项目开发说明
# 项目结构
- `/src`：项目业务实现源码
- `/client`：前端项目，使用vue3+element-plus开发
- `/web`：后端web项目，使用.NET开发
- `/cli`:命令行项目，使用.NET开发
## 前端项目结构
遵循vue3的通用目录结构
- `/src`：源码目录
    - `/views`：页面目录
    - `/components`：组件目录
    - `/assets`：静态文件目录
    - `/router`：路由目录
    - `/store`：状态管理目录
    - `/api`：api请求目录
- `/public`：静态文件目录
# 前端运行说明

## 安装依赖
```bash
cd client
npm install
```
## 运行
```bash
cd client
npm run dev
```
## 修改请求后端的api
- 修改`/vite.config.ts`中的`proxy`配置
# 后端运行说明
首先，确保你有.NET相关环境
## 环境配置
- 配置`/web/appsettings.development.json`，修改数据库连接字符串
- 在数据库内运行`/src/WebLedger.sql`和`/src/ViewSQL.sql`初始化数据库
## 运行
```bash
cd web
dotnet run
```
## 其他
- 使用`localhost:5143/swagger`可以查看API文档
- 后端接口位于`/web/Controllers`，api的路径为`/localhost:5143/Controller前的文件名/定义的路径`，在遇到问题时，可以查看代码的具体实现
> 例如：`/web/Controllers/LedgerController.cs`中的`HttpGet["view-automation"]`方法，api路径为`/Ledger/view-automation`
- 业务的具体实现位于`/src/Services`。IXX为接口，DirectXX为实现
> 注意，该`src`位于根目录下，不是`/client/src`
