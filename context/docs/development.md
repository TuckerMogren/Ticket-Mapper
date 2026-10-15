# Local development and testing

[Context index](../index.md)

## Prerequisites and current blocker

The tracked projects target `net8.0`; several explicitly select C# 12. Use a .NET
8 SDK and an editor/IDE with C# support. No `global.json` pins SDK selection, so
`dotnet --version` may select a newer installed SDK. Docker with Compose is optional
for container validation. Git and NuGet access are required for a normal checkout
and restore.

**The baseline does not build from a clean checkout.** The solution, WebApi project,
and Dockerfile reference these absent files:

```text
src/TicketMapper.Infrastructure.DocumentService/TicketMapper.Infrastructure.DocumentService.csproj
src/TicketMapper.Infrastructure.Persistence/TicketMapper.Infrastructure.Persistence.csproj
```

Recover or reconstruct those project definitions in a separate implementation
change, validate their dependencies, and ensure they are committed. `.gitignore`
matches `*.csproj` and `*.sln`, which can hide newly created project/solution files;
already tracked files remain tracked. Do not infer the missing files' intended
package graph from their directory names alone. See [known issues](known-issues.md).

## Intended build and run sequence

After resolving that prerequisite, from the repository root:

```sh
dotnet --version
dotnet restore src/TicketMapper.sln
dotnet build src/TicketMapper.sln --no-restore
dotnet test src/TicketMapper.sln --no-build
```

Run from the WebApi project directory to make hardcoded relative paths explicit:

```sh
cd src/TicketMapper.WebApi
mkdir -p 'Output Files' Logs
dotnet dev-certs https --trust
dotnet run --launch-profile https
```

The profile sets Development and listens at `https://localhost:8078` and
`http://localhost:8079`, opening Swagger. Certificate trust is a local machine
setup step; follow platform prompts when needed. The `iisSettings` block describes
IIS Express settings separately and is not used by `dotnet run --launch-profile https`.

Ensure the runtime can resolve Arial and read `Input Files/Ticket.png`. A generic
Linux image does not necessarily include Arial. Font provisioning and licensing
must be resolved by the operator; changing fonts requires visual regression review.
See [configuration](configuration.md) to enable observable logging.

## Test inventory and limitations

| Suite | Existing intent | Evidence boundary |
| --- | --- | --- |
| Application / GetDocumentQuery | Read bytes and reject invalid paths | Whitespace tests await a throwing handler without asserting the exception. |
| Application / DeleteDocumentCommand | Delete, invalid path, cancellation, logging | One test creates a real temp file but invokes a mocked filesystem, then checks real deletion. The mock is not configured to delete it. |
| Application / TimeToDeleteNotificationHandler | Send deletion request and log failures | One assertion checks a separately constructed command, not the dispatched command's filename. |
| Domain / UnitTest1 | Placeholder | Empty test body. |
| WebApi / UnitTest1 | Placeholder | Empty test body; no controller integration coverage. |

These are source observations, not executed test failures. Test projects reference
xUnit and `Microsoft.NET.Test.Sdk` but do not declare `xunit.runner.visualstudio`;
verify test discovery and add the appropriate runner as part of test repair. A
zero-test run is not evidence that behavior passed.

Once builds are repaired, focused test commands include:

```sh
dotnet test src/TicketMapper.Test.Application/TicketMapper.Test.Application.csproj
dotnet test src/TicketMapper.sln --collect:'XPlat Code Coverage'
```

## Functional smoke procedure

1. Start with a clean, writable output directory and the expected template/font.
2. Call `GET /api/v1/Test/Hello`; verify 200 and `Hi`.
3. Generate `1..3`; verify status, DOCX MIME type, and `Tickets.docx` download name.
4. Open the DOCX and verify all three ticket numbers, image content, page layout,
   and a sample print. Archive validation evidence outside the repository if needed.
5. Confirm no request DOCX or intermediate PNG remains after success. Inspect logs
   for errors even when HTTP returned 200.
6. For behavior fixes, test invalid/reversed/oversized ranges, cancellation, missing
   template/font/output directory, permission failures, cleanup failures, and
   simultaneous overlapping requests in an isolated test environment. Do not
   stress a shared service to demonstrate known resource-exhaustion risks.

No generation test, schema/visual DOCX validation, Drive integration test, HTTP
integration test, concurrency test, or CI workflow is tracked in the baseline.

## Documentation verification

For documentation changes, check links resolve with exact filename case, verify
claims against source, run `git diff --check`, and inspect the staged diff for
secrets/generated output. [Known issues](known-issues.md) records the checks actually
performed for this package. Do not mix a missing-project baseline failure with a
claim that documentation introduced a runtime regression.

Sources: [solution](../../src/TicketMapper.sln),
[launch settings](../../src/TicketMapper.WebApi/Properties/launchSettings.json),
[Application tests](../../src/TicketMapper.Test.Application),
[Domain tests](../../src/TicketMapper.Test.Domain),
[WebApi tests](../../src/TicketMapper.Test.WebApi), [.gitignore](../../.gitignore).
