# Known issues and verification record

[Context index](../index.md)

Baseline reviewed on **2026-10-14**: `origin/dev` commit
`5cfca982c59cd3afd3c0101a0dcc0b257a57a297` (`removing uneeded file`).
This documentation change does not repair application behavior. Entries below
distinguish executed evidence from source observations and untested implications.

## Build and deployment blockers

| ID | Finding | Evidence / confidence |
| --- | --- | --- |
| B1 | DocumentService and Persistence `.csproj` files are missing. | Tracked file inventory plus `dotnet restore src/TicketMapper.sln` failed with `MSB3202` naming both paths. Solution, WebApi and Dockerfile reference them. |
| B2 | New project/solution files may be accidentally ignored. | `.gitignore` contains `*.csproj` and `*.sln`; `git check-ignore` matches both missing project paths. This does not establish why the original files are missing. |
| B3 | Output directory bootstrap is absent. | Renderer writes to `Output Files/` without creating it; referenced placeholder is not tracked. Runtime failure not exercised. |
| B4 | Container font/output/TLS/log configuration is incomplete. | Dockerfile does not install Arial or create output directory; Compose supplies only Development environment. No container run was attempted. |

Missing paths:

```text
src/TicketMapper.Infrastructure.DocumentService/TicketMapper.Infrastructure.DocumentService.csproj
src/TicketMapper.Infrastructure.Persistence/TicketMapper.Infrastructure.Persistence.csproj
```

## Runtime and security observations

| ID | Observation | Impact / next verification |
| --- | --- | --- |
| R1 | No custom bounds or ordering validation on integer range. | Empty/reversed and huge requests not rejected; loop overflow at `int.MaxValue` is a source-inferred risk. Add bounded validation and tests. |
| R2 | Intermediate PNG paths depend only on ticket number. | Concurrent overlapping requests can interfere. Confirm with isolated tests when fixing request isolation. |
| R3 | Renderer catches per-ticket and outer errors and returns `Unit.Value`. | Partial/empty success or later misleading read failure. Verify explicit failure semantics. |
| R4 | Delete/notification handlers swallow errors; no guaranteed cleanup on earlier failure. | Orphans and unobservable deletion failure. Verify cleanup under cancellation/exception. |
| R5 | Read handler does not forward cancellation token to file read. | Cancellation contract is incomplete; create/delete also swallow some cancellation. |
| R6 | No application authentication or endpoint authorization attributes. | Network-reachable callers can trigger work. Deployment access controls unknown. |
| R7 | Custom configuration excludes JSON; example settings may appear effective but are not consumed. | Logging/settings require secrets/environment. Verify actual sink output. |
| R8 | Drive full-scope token cache and credential paths are not explicitly ignored. | Optional integration needs credential hygiene, scope review and unattended provisioning design. |
| R9 | No explicit page size/fit, reused drawing IDs, literal HTTPS picture namespace in generated markup. | Document schema/rendering correctness unverified. Add validation and visual print checks. |
| R10 | Swagger documents 404 but controller returns generic 500 for missing-file read errors. | Contract description diverges from implementation. |

## Incomplete integrations and tests

- Drive adapter source exists but ticket generation does not call it. Drive listing
  has no pagination; upload result checks and cancellation contracts are incomplete.
- Drive repository and FileIO implementations are placeholders. No database exists.
- RabbitMQ is declared in Compose but has no tracked application integration.
- Domain/WebApi test bodies are empty. Application tests contain source-visible
  inconsistencies documented in [development](development.md); they were not run.
- Test projects do not explicitly reference a Visual Studio xUnit runner. Discovery
  has not been verified.
- No generation, HTTP integration, concurrency, Drive integration, or visual DOCX
  tests are tracked. No GitHub Actions workflow or dependency lock is tracked.

## Executed checks for this documentation baseline

| Check | Result |
| --- | --- |
| Fetch `origin/dev` and create `tm/codex/project-context` | Branch created from the baseline SHA above. |
| Inspect tracked source, project files, solution, Docker/Compose, README, ignore rules | Completed; no app-specific agent instructions existed before this package. |
| `dotnet --list-sdks` | .NET 8.0.100 available; no repository SDK pin. The invoked CLI selected installed 11.0 preview SDK. |
| `dotnet build src/TicketMapper.sln --no-restore` | Failed: missing infrastructure projects (`MSB3202`) and un-restored assets (`NETSDK1004`). This was not a restored build. |
| `dotnet restore src/TicketMapper.sln` | Failed with `MSB3202` for both missing projects; confirms the independent baseline blocker. |
| `docker compose config --quiet` | Passed with a warning that the top-level `version` field is obsolete. No image/container validation implied. |
| `git check-ignore` for missing projects, output example, credential examples | Missing projects/output matched; credential examples did not. No real secrets were read. |
| Template metadata inspection | PNG, 1939 × 364 pixels. No generated document was rendered or printed. |
| Documentation link and fence check | 16 Markdown files, 134 relative links; all targets exist with exact filename case, and fenced code blocks are balanced. |

Application tests, HTTP requests, OAuth consent, dependency advisory checks, Docker
build/run, and live deployment verification were not performed. See the task's
completion report for final staged-diff/whitespace checks.

## Suggested follow-up order

1. Restore tracked project definitions and reliable SDK/test discovery; obtain a
   reproducible build and a meaningful baseline test run.
2. Establish a working small-range generation flow with the template, font, output
   directory, active logs and valid/visually checked DOCX output.
3. Fix validation, cancellation, error propagation, request isolation and cleanup.
4. Define access control and deployment security; review dependencies and credentials.
5. Decide whether Drive/RabbitMQ/FileIO should be completed or removed, then add CI
   and operational ownership/runbooks for the actual deployment.

These are recommendations, not approved implementation work or shipped fixes.

## Unverified operational facts

Production existence/location, hosting provider, DNS, TLS termination, network
restrictions, real secret provisioning, Google account/folder/consent state,
monitoring, operator identity, retention, backups, capacity/SLOs and release/rollback
history are unknown. Obtain evidence before filling these gaps in future updates.

Source entry points: [solution](../../src/TicketMapper.sln),
[WebApi project](../../src/TicketMapper.WebApi/TicketMapper.WebApi.csproj),
[renderer](../../src/TicketMapper.Application/Commands/CreateDocumentCommand.cs),
[configuration](../../src/TicketMapper.WebApi/Program.cs),
[tests](../../src/TicketMapper.Test.Application), [Dockerfile](../../src/Dockerfile),
[Compose](../../docker-compose.yml), [ignore rules](../../.gitignore).
