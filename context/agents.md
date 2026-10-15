# Ticket Mapper: agent guide

## Start here

Ticket Mapper is a .NET 8 / C# API originally built for the Lake George Alumni
Association's fall 2023 raffle. It stamps an inclusive integer range onto a PNG
ticket template and returns a Word document. There is no frontend, ticket sales
database, payment flow, or persisted ticket-number allocation in this repository.

This file is the canonical guide for agents working anywhere in the repository.
Root `AGENTS.md` and `CLAUDE.md` are pointers; keep application instructions here.
The lowercase filename `context/agents.md` is intentional. Do not introduce a
second case-variant copy.

Read [index.md](index.md) for topic navigation, then
[known issues](docs/known-issues.md) before attempting a build or deployment.
Documentation reflects source reviewed on **2026-10-14**, based on `origin/dev`
commit `5cfca982c59cd3afd3c0101a0dcc0b257a57a297`. Recheck the code when it changes.

## Essential context

- Entry point: [Program.cs](../src/TicketMapper.WebApi/Program.cs).
- Main route: `GET /api/v1/Document/DownloadDocument?startNumber=1&endNumber=3`.
- Flow: controller → MediatR create → read complete DOCX into memory → publish
  in-process deletion notification → return `Tickets.docx`.
- Template: `Input Files/Ticket.png`; temporary output: `Output Files/`.
  These paths are relative to the process working directory.
- Rendering uses ImageSharp, Arial 30, and Open XML. Ticket numbers are inclusive
  and unpadded. Output names for intermediate PNGs are shared by ticket number.
- Google Drive has adapter code and DI registration, but the ticket endpoint
  does not use it. RabbitMQ exists only in Compose; no application integration
  is present. FileIO and the Drive repository are placeholders.
- No application authentication, endpoint authorization attributes, range limits,
  or concurrency isolation are implemented. Do not describe it as production ready.
- Two referenced infrastructure project files are absent; solution restore is
  blocked. Existing unit tests are incomplete and include suspect assertions.

## Where to work

| Change | Starting point | Read next |
| --- | --- | --- |
| HTTP contract or response | `src/TicketMapper.WebApi/Controllers/` | [API](docs/api.md) |
| Ticket image, numbering, DOCX | `src/TicketMapper.Application/Commands/CreateDocumentCommand.cs` | [Data and rendering](docs/data-and-rendering.md) |
| Read/delete or cleanup | Application `Queries/`, `Commands/`, `IntegrationEventHandlers/` | [Architecture](docs/architecture.md) |
| Configuration, logging, DI | `src/TicketMapper.WebApi/Configs/`, `Program.cs` | [Configuration](docs/configuration.md) |
| Drive integration | `src/TicketMapper.Infrastructure.DocumentService/` | [Integrations](docs/integrations.md) |
| Container or release | `src/Dockerfile`, `docker-compose.yml` | [Deployment](docs/deployment.md) |
| Tests or build | `src/TicketMapper.sln`, `src/TicketMapper.Test.*/` | [Development](docs/development.md) |

## Implementation conventions

1. Inspect current code and `git status` first. Preserve unrelated work. Follow
   the user's requested base branch; this package was created from `origin/dev`.
2. Keep domain contracts in Domain, use cases in Application, HTTP/DI wiring in
   WebApi, and external services in Infrastructure. Existing direct file and
   rendering work in Application is an implementation fact, not a claim that
   every dependency is abstracted.
3. Follow the existing .NET naming, nullable reference types, async APIs and
   cancellation-token signatures. Check both handler interfaces and concrete
   MediatR registrations when changing requests.
4. Treat template placement, inclusive numbering, MIME type, download filename,
   and cleanup timing as observable behavior. Verify document contents and
   visual output when rendering changes; HTTP 200 alone is insufficient.
5. Never put OAuth client secrets, tokens, private logs, or generated artifacts
   into commits. The current `.gitignore` does **not** protect the Drive credential
   paths; review explicitly staged files. See [security](docs/security.md).
6. Do not silently fix runtime defects as part of a documentation-only task.
   Record implemented behavior separately from recommended changes. Do not
   invent hosting, CI, live credentials, ownership, SLOs, or successful test results.
7. Keep these pointers small and put reusable documentation in `context/docs/`.
   Update the index and all inbound links when moving a document.

## Verification and handoff

From the repository root, the intended sequence is:

```sh
dotnet restore src/TicketMapper.sln
dotnet build src/TicketMapper.sln --no-restore
dotnet test src/TicketMapper.sln --no-build
docker compose config --quiet
git diff --check
```

The first command currently fails with `MSB3202` for the two missing infrastructure
projects. Do not claim the subsequent build/test sequence passed. See
[development](docs/development.md) for focused tests and the smoke procedure.
For documentation-only edits, verify local links, source references, and diff
whitespace; an application rebuild cannot validate prose.

In a handoff, state what changed, commands actually run, results, and remaining
blockers. Keep [known issues](docs/known-issues.md) and affected topic documents
consistent with any fixes.
