# Configuration and secrets

[Context index](../index.md)

## Two configuration paths

[Program.cs](../../src/TicketMapper.WebApi/Program.cs) first calls
`WebApplication.CreateBuilder(args)`, then builds a **separate** configuration for
application settings and Serilog:

```text
AddUserSecrets<Program>() → AddEnvironmentVariables() → SetBasePath(current directory)
```

Environment variables override matching user-secret keys. The `AddJsonFile` call
is commented out. `SetBasePath` does not load a file. Consequently the checked-in
`appsettings.json` is not the source consumed by `AppSettingsConfiguration` or
`ConfigureLogging`, even though the ASP.NET web builder has its own default
configuration providers for host/server behavior. Command-line values passed to
the builder are not automatically copied to this separate configuration either.

## Settings inventory

| Setting/source | Consumer | Current meaning |
| --- | --- | --- |
| `GoogleDriveSettings:ApplicationName` | DriveService factory | Application name supplied to the Google client. |
| `GoogleDriveSettings:ClientId`, `ProjectId`, `AuthUrl`, `TokenUri`, `AuthProviderCertUrl`, `ClientSecret` | Settings binding | Properties exist but are not used by the OAuth loader; filling them does not replace `client_secrets.json`. |
| `Serilog:*` | `ReadFrom.Configuration(configuration)` | Sink, level, enrichment configuration from secrets/environment. |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET host | Development enables Swagger. Compose explicitly uses Development. |
| `ASPNETCORE_URLS` | ASP.NET host | Can set listen URLs when no launch profile is used. |
| `client_secrets.json` | `GoogleClientSecrets.Load` | OAuth client JSON read from current directory when DriveService is resolved. |
| `token.json` | `FileDataStore(credPath, true)` | OAuth token-cache **directory**, despite the name resembling a JSON file. |

There are no implemented settings for template path, output path, maximum tickets,
font, placement, Drive folder ID, RabbitMQ connection, or download filename.

## Local application configuration

Use WebApi's user-secrets store, not Application's separate `UserSecretsId`; the
loader uses `Program` from the WebApi assembly. Example from the repository root:

```sh
dotnet user-secrets set 'GoogleDriveSettings:ApplicationName' 'TicketMapper-local' \
  --project src/TicketMapper.WebApi/TicketMapper.WebApi.csproj
```

No Drive credential is used by the current document endpoint. Only provision OAuth
credentials if explicitly testing the adapter. Its loader requests full Drive scope
for the fixed user key `user`, and may require interactive consent. Do not assume
the existing flow can bootstrap unattended in a container.

Environment variables use double underscores for nested settings, for example
`GoogleDriveSettings__ApplicationName=TicketMapper-local`. Do not paste real
credential values into docs, shell examples, issue comments, or logs.

## Logging example

The JSON file shows an Information-level file sink with Microsoft overridden to
Warning and System to Error, `Logs/Logs.txt`, minute rolling, and enrichment names.
Those settings are inactive for the custom logger unless supplied through a loaded
provider. No console sink is explicitly configured. Startup and endpoint logs may
therefore be invisible; `docker compose logs` alone is not proof of success.

For a local shell session, a minimal environment-based file sink configuration is:

```sh
export Serilog__Using__0=Serilog.Sinks.File
export Serilog__MinimumLevel__Default=Information
export Serilog__WriteTo__0__Name=File
export Serilog__WriteTo__0__Args__path=Logs/Logs.txt
export Serilog__WriteTo__0__Args__rollingInterval=Day
```

This is a suggested local configuration, not a change to the checked-in default.
Create a writable `Logs/` directory in the run directory and verify actual log
output. Do not assume machine/thread enrichers from the JSON are installed; confirm
the needed packages if enabling them. This repository does not define an application
log retention, central collection, alerting, or redaction policy.

## Secret storage caveats

No real values are present in the checked-in `appsettings.json`; it contains
`{Secret}` placeholders. The repository's ignore rules do not explicitly exclude
`client_secrets.json` or `token.json/`. Before local Drive testing, protect these
paths with local exclusions or mount credentials outside the checkout through an
explicitly reviewed configuration change. Never stage credentials. There is no
implemented production secret manager or deployment-specific credentials setup.

Sources: [settings binding](../../src/TicketMapper.WebApi/Configs/ConfigureApplicationSettings.cs),
[Drive configuration](../../src/TicketMapper.WebApi/Configs/ConfigureGoogleDrive.cs),
[logging](../../src/TicketMapper.WebApi/Configs/ConfigureLogging.cs),
[appsettings](../../src/TicketMapper.WebApi/appsettings.json),
[WebApi project](../../src/TicketMapper.WebApi/TicketMapper.WebApi.csproj).
