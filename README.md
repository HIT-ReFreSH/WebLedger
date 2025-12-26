<div  align=center>
    <img src="https://raw.githubusercontent.com/HIT-ReFreSH/WebLedger/main/images/Full_2048.png?raw=true" width = 30% height = 30%  />
</div>

# WebLedger

![DockerHub-v](https://img.shields.io/docker/v/hitrefresh/web-ledger?style=flat-square)
![DockerHub-DL](https://img.shields.io/docker/pulls/hitrefresh/web-ledger?style=flat-square)
![DockerHub-size](https://img.shields.io/docker/image-size/hitrefresh/web-ledger?style=flat-square)
![GitHub](https://img.shields.io/github/license/HIT-ReFreSH/WebLedger?style=flat-square)
![GitHub last commit](https://img.shields.io/github/last-commit/HIT-ReFreSH/WebLedger?style=flat-square)
![GitHub repo size](https://img.shields.io/github/repo-size/HIT-ReFreSH/WebLedger?style=flat-square)
![GitHub code size](https://img.shields.io/github/languages/code-size/HIT-ReFreSH/WebLedger?style=flat-square)

This project is a convenience and simple .NET Web Ledger Server & Cli.

## Environment Requirement

.NET 8 SDK is required for development or compiling the source, and the binary could be native-ready

## Usage

### Prepare Database

You need a MySQL server `$host` deployed, and an account `$user` with `$pwd` with access on schema `$dbname`.

Variables here is just for easy description.

### Deploy Server

```bash
docker pull hitrefresh/web-ledger:0.3.1 
# if you are in China, login and use aliyun mirror: 
# docker pull registry.cn-hangzhou.aliyuncs.com/hitrefresh/web-ledger:0.3.1
# and tag
# docker tag registry.cn-hangzhou.aliyuncs.com/hitrefresh/web-ledger:0.3.1 hitrefresh/web-ledger:0.3.1
docker run -tid \
  --name ledger \
  --restart always \
  -p <port>:8080 \
  -e WL_SQL_DB='<dbname>' \
  -e WL_SQL_HOST='<host>' \
  -e WL_SQL_HOST='<user>' \
  -e WL_SQL_PWD='<pwd>' \
  hitrefresh/web-ledger:0.3.1
Start CLI (Direct)
bash
docker pull hitrefresh/web-ledger-cli:0.3.1
# if you are in China, login and use aliyun mirror: 
# docker pull registry.cn-hangzhou.aliyuncs.com/hitrefresh/web-ledger-cli:0.3.1
# and tag
# docker tag registry.cn-hangzhou.aliyuncs.com/hitrefresh/web-ledger-cli:0.3.1 hitrefresh/web-ledger-cli:0.3.1
docker run --rm -i\
  --name ledger-cli \
  -e WL_METHOD='mysql' \
  -e WL_SQL_DB='<dbname>' \
  -e WL_SQL_HOST='<host>' \
  -e WL_SQL_HOST='<user>' \
  -e WL_SQL_PWD='<pwd>' \
  hitrefresh/web-ledger-cli:0.3.1
And then you can configure the access.

Start CLI (Remote)
bash
docker pull hitrefresh/web-ledger-cli:0.3.1 
# if you are in China, login and use aliyun mirror: 
# docker pull registry.cn-hangzhou.aliyuncs.com/hitrefresh/web-ledger-cli:0.3.1
# and tag
# docker tag registry.cn-hangzhou.aliyuncs.com/hitrefresh/web-ledger-cli:0.3.1 hitrefresh/web-ledger-cli:0.3.1
docker run --rm -i \
  --name ledger-cli \
  -e WL_METHOD='http' \
  -e WL_HOST='<http-or-https-url>' \
  -e WL_ACCESS='<access>' \
  -e WL_SECRET='<secret>' \
  hitrefresh/web-ledger-cli:0.3.1
And then you can configure the access.

🩺 Health Check Endpoints
WebLedger provides comprehensive health check endpoints for monitoring and container orchestration.
### Health Checks

The application provides health check endpoints for monitoring system status, database connectivity, and external service availability.

* **Basic Check**: `GET /health`
  * Returns `Healthy` or `Unhealthy` string.
  * Useful for load balancers or simple uptime monitoring.

* **Detailed Check**: `GET /health/detailed`
  * Returns JSON with detailed status of individual components (Database, External Services).
  * Includes password masking for database connection strings in the output.
  * Example response:
    ```json
    {
      "status": "Healthy",
      "totalDuration": "00:00:00.045",
      "checks": [
        {
          "name": "database",
          "status": "Healthy",
          "description": "Database reachable",
          "details": {
             "connectionString.masked": "server=...;password=REDACTED",
             "host": "localhost",
             "port": "3306"
          }
        },
        {
          "name": "external_service",
          "status": "Healthy",
          "description": "No service URL configured"
        }
      ]
    }
    ```
## For Developers

Available Endpoints
GET /health - Basic Health Check

Returns overall application status
- **[Getting Started Guide](./docs/getting-started.md)** - Quick setup and backend development
- **[Frontend Integration Guide](./docs/frontend-integration.md)** - Build React/Vue apps with TypeScript
- **[API Usage Examples](./docs/API_USAGE_EXAMPLES.md)** - Practical code examples for REST API and CLI

Checks critical dependencies only

Suitable for load balancers and monitoring

GET /health/detailed - Detailed Health Report

Returns comprehensive health information

Includes all health checks with details

Add ?pretty=true for formatted JSON output

GET /health/live - Liveness Probe

Checks if application is running

Used by Kubernetes and Docker for liveness probes

Does not check external dependencies

Health Checks Performed
MySQL Database Connection

Verifies database connectivity

Executes test query (SELECT 1)

Reports connection details (server, database name)

Memory Usage

Monitors application memory consumption

Provides warnings at 80% usage

Reports critical status at 90% usage

Application Self-Check

Verifies application is responsive

Includes version information

Swagger UI Accessibility (Development only)

Checks if API documentation is accessible

Response Examples
Basic Health Check (GET /health):

json
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "duration": "45.2ms",
  "environment": "Production",
  "version": "0.3.1",
  "checks": 3,
  "healthy": 3,
  "degraded": 0,
  "unhealthy": 0
}
Detailed Health Check (GET /health/detailed):

json
{
  "status": "Degraded",
  "timestamp": "2024-01-15T10:30:00Z",
  "totalDuration": "102.5ms",
  "environment": "Development",
  "version": "0.3.1",
  "checks": [
    {
      "name": "mysql-database",
      "status": "Healthy",
      "description": "Database connection is healthy",
      "duration": "25.1ms",
      "tags": ["database", "ready"],
      "data": {
        "provider": "MySQL",
        "test_query_result": 1,
        "connection_info": {
          "server": "localhost",
          "database": "webledger",
          "user": "admin",
          "port": "3306"
        }
      }
    },
    {
      "name": "memory-usage",
      "status": "Degraded",
      "description": "Memory usage high: 85.5%",
      "duration": "0.5ms",
      "tags": ["system", "ready"],
      "data": {
        "allocated_mb": 850.3,
        "total_available_mb": 1024.0,
        "memory_usage_percent": 85.5,
        "gen0_collections": 5,
        "gen1_collections": 2,
        "gen2_collections": 1
      }
    }
  ],
  "summary": {
    "healthy": 1,
    "degraded": 1,
    "unhealthy": 0,
    "total": 2
  }
}
Docker Health Check Configuration
yaml
# docker-compose.yml
version: '3.8'
services:
  webledger:
    image: hitrefresh/web-ledger:latest
    ports:
      - "8080:8080"
    environment:
      - ConnectionStrings__mysql=${MYSQL_CONNECTION_STRING}
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
Kubernetes Liveness and Readiness Probes
yaml
# kubernetes-deployment.yaml
apiVersion: apps/v1
kind: Deployment
spec:
  template:
    spec:
      containers:
      - name: webledger
        image: hitrefresh/web-ledger:latest
        ports:
        - containerPort: 8080
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
Monitoring Integration
These endpoints can be integrated with:

Prometheus + Grafana - via HTTP endpoint scraping

Azure Monitor - via Application Insights

AWS CloudWatch - via custom metrics

Datadog - via HTTP check integration

New Relic - via synthetic monitoring

Troubleshooting
If health checks fail:

Database Connection Issues:

Verify ConnectionStrings:mysql configuration

Check MySQL server is running

Confirm network connectivity

High Memory Usage:

Review recent changes to the application

Consider increasing container memory limits

Check for memory leaks in long-running operations

Endpoint Unreachable:

Verify application is running on correct port

Check firewall and network policies

Review CORS configuration if accessing from different origin

For Developers
Quick Start Guides
Getting Started Guide - Quick setup and backend development

Frontend Integration Guide - Build React/Vue apps with TypeScript

Build Image
bash
cd web
dotnet publish --os linux /t:PublishContainer
cd cli
dotnet publish --os linux /t:PublishContainer
Deployment/Initialize
Assuming you already have the binaries.

Prepare Database
You need a MySQL server $host deployed, and an account $user with $pwd with access on schema $dbname.

Variables here is just for easy description.

Configuration
Server
The server use appsettings.json at runtime and appsettings.development.json at develop time.

json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "mysql": "server=$host; port=3306; database=$dbname; user=$user; password=$pwd; Persist Security Info=False; Connect Timeout=300"
  },
  "AllowedHosts": "*"
}
Normally, the only thing you need to customize is the connection string.

CLI Client
CLI use config.json as configuration file. There are two ways for the client to connect with WebLegder Server, through HTTP, or directly through mysql connection.

For Mysql Connection, just make the connection string as the server's.:

json
{
  "target": "mysql",
  "host": "server=$host; port=3306; database=$dbname; user=$user; password=$pwd; Persist Security Info=False; Connect Timeout=300"
}
For Http Connection, access and secret are part of the authorization, which will be explained later:

json
{
  "target": "http",
  "host": "http://localhost:5143",
  "access": "root",
  "secret": "5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"
}
Authorization
Generate initial access and secret
Connect to the database using CLI.

Use ls command to view all available CLI commands.

Usels-acc command to view all accesses.

Use grant <access-name> to create new access, and store the secret; replace <access-name> with whatever you like.

How to use?
To use access and secret for CLI client, just edit the configuration as said previously

To use them for your own Http client, configure your http client as following; to develop them, you can take cli/services/*Manager.cs as reference:

csharp
http.BaseAddress = new(builder.Configuration["host"]);
http.DefaultRequestHeaders.Add("wl-access", builder.Configuration["access"]);
http.DefaultRequestHeaders.Add("wl-secret", builder.Configuration["secret"]);
Concepts
Category
The category of a ledger entry, such as "daily necessities", "daily chemicals", "food", "beverages", can have an optional parent category.

Type
A specific type of ledger entry, such as "cola," "shampoo," or "bread," represents a specific type rather than a type or category of income/consumption, and is associated with a category when it first appears (default category thereafter).

Entry
A ledger entry, has its Category, Type, Amount, GivenTime, etc.

View
A summarized report for certain categories in a certain time range, it can only be created from View Templates.

View Template
Describe a class of reports (View) for certain categories, which store the categories information.

View Automation
Automation to create View from View Template every year/day/week/month/quarter(3 months).

Usage
Functions can be accessed through CLI or other clients, for development server, you can view /swagger to get OpenApi Support.

Notice: Use ls to view CLI commands.