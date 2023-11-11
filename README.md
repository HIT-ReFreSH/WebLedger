<div  align=center>
    <img src="./images/Full_2048.png?raw=true" width = 30% height = 30%  />
</div>

# WebLedger

![GitHub](https://img.shields.io/github/license/HIT-ReFreSH/WebLedger?style=flat-square)
![GitHub last commit](https://img.shields.io/github/last-commit/HIT-ReFreSH/WebLedger?style=flat-square)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/HIT-ReFreSH/WebLedger/publish_to_nuget?style=flat-square)
![GitHub repo size](https://img.shields.io/github/repo-size/HIT-ReFreSH/WebLedger?style=flat-square)
![GitHub code size](https://img.shields.io/github/languages/code-size/HIT-ReFreSH/WebLedger?style=flat-square)

This project is a convenience and simple .NET Web Ledger Server & Cli.
**Notice: DesignTimeDbContextFactory is gitignored for pravicy.**

## Environment Requirement

.NET 7 SDK is required for development or compiling the source, and the binary could be native-ready

## Deployment/Initialize

Assuming you already have the binaries.

### Prepare Database

You need a MySQL server `$host` deployed, and a account `$user` with `$pwd` with access on schema `$dbname`.

Variables here is just for easy description.

### Configuration

#### Server

The server use `appsettings.json` at runtime and `appsettings.development.json` at develop time.

```json
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

```

Normally, the only thing you need to customize is the connection string.

#### CLI Client

CLI use `config.json` as configuration file. There are two ways for the client to connect with WebLegder Server, through HTTP, or directly through mysql connection. 

- For Mysql Connection, just make the connection string as the server's.:

```json
{
  "target": "mysql",
  "host": "server=$host; port=3306; database=$dbname; user=$user; password=$pwd; Persist Security Info=False; Connect Timeout=300"
}

```

- For Http Connection, `access` and `secret` are part of the authorization, which will be explained later:

```json
{
  "target": "http",
  "host": "http://localhost:5143",
  "access": "root",
  "secret": "5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"
}
```

### Authorization

#### Generate initial `access` and `secret`

1. Connect to the database using CLI.
2. Use `ls` command to view all available CLI commands.
3. Use`ls-acc` command to view all accesses.
4. Use `grant <access-name>` to create new access, and store the secret; replace `<access-name>` with whatever you like.

#### How to use?

- To use `access` and `secret` for CLI client, just edit the configuration as said previously
- To use them for your own Http client, configure your http client as following; to develop them, you can take `cli/services/*Manager.cs` as reference:

```csharp
http.BaseAddress = new(builder.Configuration["host"]);
http.DefaultRequestHeaders.Add("wl-access", builder.Configuration["access"]);
http.DefaultRequestHeaders.Add("wl-secret", builder.Configuration["secret"]);
```

## Usage

These functions can be accessed through CLI or other clients.

### Create Categories

//TODO

