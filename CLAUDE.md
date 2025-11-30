# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

WebLedger is a .NET 8 web ledger server with CLI client for tracking financial entries, categories, and automated reporting views. The project supports both direct MySQL database access and HTTP API access patterns.

## Architecture

### Project Structure (3 Projects in Solution)

1. **LibWebLedger** (`src/`) - Core library containing:
   - Data models (EF Core entities): `LedgerEntry`, `LedgerCategory`, `LedgerEntryType`, `LedgerView`, etc.
   - Service interfaces: `ILedgerManager`, `IConfigManager`
   - Service implementations: `DirectLedgerManager`, `DirectConfigManager` (direct MySQL access)
   - Database migrations (EF Core)

2. **WebLedger** (`web/`) - ASP.NET Core web server:
   - Controllers: `LedgerController`, `ConfigController`
   - Custom middleware: `AccessMiddleware` (header-based auth using `wl-access` and `wl-secret`)
   - Razor Pages UI
   - Swagger/OpenAPI support (development only)

3. **WebLedgerTheSuit** (`cli/`) - MobileSuit CLI client:
   - HTTP-based services: `HttpLedgerManager`, `HttpConfigManager`
   - Configuration-driven (can use MySQL direct or HTTP)
   - Commands for entry management, access control, view automation

### Data Access Patterns

The project supports **two deployment modes** via dependency injection:

- **Direct Mode**: Services directly access MySQL via EF Core (`DirectLedgerManager`, `DirectConfigManager`)
- **HTTP Mode**: Services call web API via HttpClient (`HttpLedgerManager`, `HttpConfigManager`)

Both implement the same interfaces (`ILedgerManager`, `IConfigManager`), allowing the CLI to work in either mode.

### Authentication

- Uses custom HTTP headers: `wl-access` (access name) and `wl-secret` (secret key)
- Managed by `AccessMiddleware` which caches access credentials from database
- If no access records exist in DB, all requests are allowed (initial setup mode)
- Access credentials are generated via CLI using `grant <name>` command

## Development Commands

### Backend Development

**Build solution:**
```bash
dotnet build
```

**Run web server (development):**
```bash
cd web
dotnet run
# Server starts at http://localhost:5143
# Swagger UI: http://localhost:5143/swagger (dev mode only)
```

**Run CLI (direct MySQL access):**
```bash
cd cli
# Edit config.json first (see Configuration section)
dotnet run
```

**Database migrations:**
```bash
cd web
dotnet ef database update  # Apply migrations
dotnet ef migrations add <MigrationName>  # Create new migration
```

**Build Docker images:**
```bash
cd web
dotnet publish --os linux /t:PublishContainer

cd cli
dotnet publish --os linux /t:PublishContainer
```

### Frontend Development

Frontend integration uses Vite + TypeScript (React or Vue). See `docs/frontend-integration.md` for complete setup.

**Important constraints for frontend PRs:**
- MUST use Vite (not Create React App or Vue CLI)
- MUST use TypeScript (no plain JavaScript)
- MUST use yarn PnP or pnpm (not npm)
- MUST include screenshots for all UI changes

## Configuration

### Web Server Configuration

Located in `web/appsettings.json` (production) or `web/appsettings.Development.json` (development).

**Environment variables (preferred):**
- `WL_SQL_DB` - Database name (default: ledger)
- `WL_SQL_HOST` - MySQL host (default: localhost)
- `WL_SQL_PORT` - MySQL port (default: 3306)
- `WL_SQL_USER` - Database user (default: ledger-bot)
- `WL_SQL_PWD` - Database password

**Connection string format:**
```
server=<host>; port=3306; database=<dbname>; user=<user>; password=<pwd>; Persist Security Info=False; Connect Timeout=300
```

### CLI Configuration

Located in `cli/config.json`.

**Direct MySQL mode:**
```json
{
  "target": "mysql",
  "host": "server=localhost; port=3306; database=ledger; user=ledger-bot; password=pwd; Persist Security Info=False; Connect Timeout=300"
}
```

**HTTP mode:**
```json
{
  "target": "http",
  "host": "http://localhost:5143",
  "access": "root",
  "secret": "your-secret-here"
}
```

## Core Concepts

### Ledger Data Model

- **Category**: Hierarchical categorization (e.g., "Food" → "Beverages")
- **Type**: Specific entry types (e.g., "Cola", "Bread") with default category
- **Entry**: Individual ledger record with Type, Category, Amount, GivenTime, Description
  - Amount sign determines income/expense (positive = income)
  - Type auto-created on first use if category provided
- **View**: Summarized report for categories over time range
- **View Template**: Reusable template defining which categories to include in views
- **View Automation**: Automated view generation (daily/weekly/monthly/quarterly/yearly)

### Key Business Logic

When inserting an entry (see `DirectLedgerManager.Insert`):
1. If Type doesn't exist AND Category is provided → create Type with that default Category
2. If Type doesn't exist AND Category is missing → throw `TypeUndefinedException`
3. If Type exists → use Type's default Category (entry's Category is optional)
4. Amount sign determines `IsIncome` flag

## API Endpoints

All endpoints require `wl-access` and `wl-secret` headers (except during initial setup).

**Main routes:**
- `/ledger/*` - Entry, category, type, view operations (`LedgerController`)
- `/config/*` - Access management (`ConfigController`)

**Common operations:**
- `POST /ledger/entry` - Create entry
- `DELETE /ledger/entry?id={guid}` - Delete entry
- `POST /ledger/select` - Query entries with filters (SelectOption)
- `PUT /ledger/category` - Add/update category
- `GET /ledger/categories` - List all categories
- `PUT /ledger/type` - Add/update type
- `GET /ledger/types` - List all types

See Swagger UI in development mode for complete API documentation.

## Initial Setup Workflow

1. Create MySQL database and user with privileges
2. Configure `web/appsettings.Development.json` with connection string
3. Run web server (migrations apply automatically in development)
4. Configure CLI for direct MySQL access (`config.json`)
5. Run CLI and execute: `grant root` to generate first access credentials
6. Update CLI config.json to use HTTP mode with generated credentials

## Migration Strategy

- Migrations in `src/Migrations/` directory
- Auto-applied in development when running `dotnet run`
- Auto-applied in production on app startup (see `web/Program.cs`)
- Migrations assembly: "WebLedger" (web project)

## Dependencies

- .NET 8 SDK (version 8.0.401 specified in global.json)
- MySQL 8.0+ server
- Entity Framework Core with MySQL provider
- Steeltoe Configuration (for placeholder resolution)
- MobileSuit framework (CLI)

## Coding Standards

### C# Backend

- Use primary constructors for dependency injection where appropriate
- Follow XML documentation conventions for public APIs
- Use async/await patterns for database operations
- Implement services via interfaces (`ILedgerManager`, `IConfigManager`)
- EF Core queries should use `Include()` for eager loading when needed

### TypeScript Frontend

- Functional components with typed props interfaces
- Use Composition API for Vue, hooks for React
- API client should use axios interceptors for auth headers
- Store credentials in environment variables (`.env.local`)
- Never commit `.env.local` or actual credentials

## Common Tasks

**Adding a new ledger endpoint:**
1. Add method to `ILedgerManager` interface
2. Implement in `DirectLedgerManager` (direct DB access)
3. Implement in `HttpLedgerManager` (HTTP client calls)
4. Add controller action in `LedgerController`

**Adding a new migration:**
1. Modify entity classes in `src/Data/`
2. Run `dotnet ef migrations add <Name>` from `web/` directory
3. Review generated migration in `src/Migrations/`
4. Test migration with `dotnet ef database update`

**Testing API with cURL:**
```bash
curl -X GET http://localhost:5143/ledger/categories \
  -H "wl-access: root" \
  -H "wl-secret: your-secret-key"
```
