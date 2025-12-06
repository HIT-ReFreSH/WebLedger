# Getting Started with WebLedger

This guide will help you quickly set up and start developing with WebLedger.

## Prerequisites

- .NET 8 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- MySQL 8.0+ server
- Your favorite IDE (Visual Studio, VS Code, Rider, etc.)

## Quick Start: Running the Backend

### Option 1: Local Development (Recommended for Development)

#### 1. Prepare MySQL Database

Create a new database and user for WebLedger:

```sql
CREATE DATABASE ledger;
CREATE USER 'ledger-bot'@'localhost' IDENTIFIED BY 'your-password';
GRANT ALL PRIVILEGES ON ledger.* TO 'ledger-bot'@'localhost';
FLUSH PRIVILEGES;
```

#### 2. Configure the Application

Create or modify `web/appsettings.Development.json`:

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

**Alternative: Using Environment Variables**

The application supports environment variables for configuration. You can set:

- `WL_SQL_DB` - Database name (default: `ledger`)
- `WL_SQL_HOST` - MySQL server host (default: `localhost`)
- `WL_SQL_PORT` - MySQL port (default: `3306`)
- `WL_SQL_USER` - Database user (default: `ledger-bot`)
- `WL_SQL_PWD` - Database password (default: `password`)

Example on Windows (PowerShell):
```powershell
$env:WL_SQL_DB="ledger"
$env:WL_SQL_HOST="localhost"
$env:WL_SQL_USER="ledger-bot"
$env:WL_SQL_PWD="your-password"
```

Example on Linux/macOS:
```bash
export WL_SQL_DB=ledger
export WL_SQL_HOST=localhost
export WL_SQL_USER=ledger-bot
export WL_SQL_PWD=your-password
```

#### 3. Run the Backend

Navigate to the web directory and run:

```bash
cd web
dotnet restore
dotnet run
```

The server will start on `http://localhost:5143` (or the port shown in console output).

**In development mode, the API documentation is available at:**
```
http://localhost:5143/swagger
```

### Option 2: Using Docker (Quick Testing)

If you prefer using Docker:

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

## Setting Up Authentication

WebLedger uses custom headers for authentication: `wl-access` and `wl-secret`.

### Generate Initial Access Credentials

You need to use the CLI to generate the first access credentials:

#### 1. Configure CLI for Direct Database Access

Create `cli/config.json`:

```json
{
  "target": "mysql",
  "host": "server=localhost; port=3306; database=ledger; user=ledger-bot; password=your-password; Persist Security Info=False; Connect Timeout=300"
}
```

#### 2. Run CLI and Generate Access

```bash
cd cli
dotnet run
```

In the CLI, run the following commands:

```bash
> ls              # List all available commands
> ls-acc          # List all access tokens (should be empty initially)
> grant root      # Create a new access named 'root'
```

The CLI will output a secret key. **Save this secret key immediately!**

Example output:
```
Access: root
Secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w
```

### Update CLI for HTTP Access

Now update `cli/config.json` to use HTTP:

```json
{
  "target": "http",
  "host": "http://localhost:5143",
  "access": "root",
  "secret": "5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"
}
```

## Verifying the Setup

### Test the API with Swagger UI

1. Open browser to `http://localhost:5143/swagger`
2. You'll see all available API endpoints
3. Click "Authorize" button (if available) or manually add headers to test requests

### Test with cURL

```bash
curl -X GET http://localhost:5143/ledger/categories \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"
```

## Available API Endpoints

The main controllers are:

- `/ledger/*` - Ledger operations (entries, categories, types, views)
- `/config/*` - Configuration and access management

For detailed API documentation, check the Swagger UI at `/swagger` when running in development mode.

## Database Migrations

The application uses Entity Framework Core for database management.

### In Development Mode
Database migrations run automatically when you start the application.

### In Production Mode
Migrations are applied automatically on startup.

### Manual Migration (if needed)
```bash
cd web
dotnet ef database update
```

## Troubleshooting

### Connection Issues

1. **MySQL Connection Failed**: Verify your MySQL server is running and credentials are correct
2. **Port Already in Use**: Change the port in `Properties/launchSettings.json` or use `dotnet run --urls http://localhost:YOUR_PORT`
3. **Migration Errors**: Ensure your database user has proper permissions to create tables

### Common Issues

**Issue**: "Access denied for user"
**Solution**: Grant proper database privileges to your MySQL user

**Issue**: "No connection could be made"
**Solution**: Check if MySQL server is running and firewall settings

## Next Steps

- [Frontend Integration Guide](./frontend-integration.md) - Learn how to build a React/Vue frontend
- Check `/swagger` for complete API documentation
- Explore the CLI tool for quick data management

## Development Tips

1. Use Swagger UI for quick API testing during development
2. The CLI tool is great for quick data manipulation and testing
3. In development, the server runs with hot reload - just save your changes
4. Check logs in the console for debugging information
