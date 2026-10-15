# Operations and troubleshooting

[Context index](../index.md)

These procedures apply after the [build/deployment prerequisites](deployment.md)
have been resolved. The repository does not establish a live host, on-call owner,
SLO, dashboard, alert destination, or production support schedule.

## Health and output checks

- **Reachability:** `GET /api/v1/Test/Hello` should return 200 with `Hi`. It does not
  exercise template access, font lookup, generation, cleanup, Drive, or a broker.
- **Functional health:** generate a tiny range, inspect the Word document and each
  number, and confirm cleanup. The app has no dedicated readiness endpoint.
- **Resource health:** observe process CPU/memory, writable disk space, output-file
  accumulation, request durations, and generation errors using host tools. No
  built-in metrics, capacity figures, or tested maximum batch size are supplied.
- **Logging health:** verify an actual sink receives startup and request messages.
  The checked-in appsettings file is not loaded into the custom logger configuration.

## Troubleshooting map

| Symptom | Likely source/check | Next step |
| --- | --- | --- |
| `MSB3202` on restore/build | Missing DocumentService/Persistence project definitions | See known issues; recover/reconstruct and track the project files. |
| Newly created project is invisible in Git | `*.csproj` / `*.sln` ignore rules | Inspect `git check-ignore -v <path>` and deliberately correct tracking. |
| API responds but generation returns 500 | Missing template, Arial, output directory, permissions, or failed read | Verify working directory and generation logs; creation can hide the original exception. |
| 200 but missing tickets or empty document | Per-ticket exceptions swallowed; reversed range; concurrent PNG collisions | Inspect logs and file contents; use small ordered sequential requests while fixing causes. |
| Word layout or image rendering is wrong | Fixed image dimensions, font placement, Open XML markup | Check the rendering guide and validate/visually inspect generated DOCX. |
| Residual DOCX/PNG files | Failure/cancellation before cleanup or swallowed deletion errors | Stop new work, identify active files, then perform controlled cleanup. |
| No useful logs / empty Compose logs | No active Serilog sink or file-only sink | Supply loaded-provider logging settings and inspect the actual log directory. |
| Swagger absent | Environment is not Development | Check environment; absence in Production is intentional. |
| HTTPS redirect/certificate failures | Listener/certificate/proxy not configured | Inspect launch profile or deployment TLS configuration; EXPOSE does not configure HTTPS. |
| Drive fails or waits for browser consent | DriveService resolved; missing client JSON/token consent | Inspect optional integration configuration in a controlled environment; ticket route does not need Drive. |
| Drive list appears incomplete | Adapter does not paginate | Implement/test pagination before relying on complete results. |
| Test command reports no tests | Runner/discovery gap | Inspect test packages and discovery; empty success is not behavior coverage. |

## Safe temporary-file cleanup

There is no scheduled cleanup process. Before manually deleting outputs, stop new
requests and wait for in-flight generation to finish, or stop the service and
record interrupted work. Verify the actual process working directory and inspect
only its `Output Files/` contents. Retain needed evidence/results according to the
operator's policy, then remove confirmed orphan outputs. Do not delete a shared
directory while requests are writing to it, and do not confuse token storage or
the template with temporary output. Avoid broad recursive cleanup commands.

After cleanup, verify writable space and permissions, generate a tiny range, check
contents and deletion, and resume sequential use. Operators must reconcile ticket
number ranges separately; the server has no issuance ledger.

## Recovery and retention

The template and source are in Git. Generated tickets are reproducible only if the
same template, font/layout behavior, and range are known; the application does not
record all of those as a job manifest. Keep the original downloaded file when print
reproducibility matters. Back up approved operational configuration through the
deployment's secret-safe process, not a repository commit.

Optional Drive tokens need protected persistence and a revocation/re-consent plan.
Server cleanup cannot remove downloaded copies or undo Drive adapter operations.
There is no tested backup/restore process, guaranteed deletion window, or disaster
recovery objective in this repository.

## Support handoff contents

Capture commit/image identifier, deployment environment, request range, timestamp,
status, exception summary with secrets removed, working-directory/permission checks,
output-file observations, and whether requests overlapped. Include actual document
validation results where relevant. Do not paste OAuth files, tokens, private logs,
or sensitive generated documents into a public issue.

Sources: [controller](../../src/TicketMapper.WebApi/Controllers/DocumentController.cs),
[generation](../../src/TicketMapper.Application/Commands/CreateDocumentCommand.cs),
[deletion](../../src/TicketMapper.Application/Commands/DeleteDocumentCommand.cs),
[logging](../../src/TicketMapper.WebApi/Configs/ConfigureLogging.cs),
[Program](../../src/TicketMapper.WebApi/Program.cs).
