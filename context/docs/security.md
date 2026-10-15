# Information security

[Context index](../index.md)

## Scope and evidence

This is a source-based security context for the reviewed application, not a
penetration test, compliance certification, dependency audit, or assessment of a
live deployment. No live host, cloud account, OAuth grant, firewall, production
data, or private configuration was inspected. Deployment controls must be verified
with the operator rather than inferred from this repository.

## Assets and trust boundaries

| Boundary / asset | Current handling |
| --- | --- |
| HTTP caller → API | Integer query input triggers document creation. No application authentication or endpoint authorization requirement. |
| API → local filesystem | Reads a fixed template; writes temporary images/DOCX; reads full DOCX into memory; performs best-effort deletion. |
| Runtime → Google Drive | Optional OAuth client with full Drive scope; token cache on disk. Not reached by ticket-generation controller. |
| Build context → container | Dockerfile copies `src` into the build stage; no `.dockerignore` is tracked. Local files inside that context require review. |
| Compose → host network | API port 8080 and RabbitMQ ports 5672/15672 are published without a loopback-only binding. Actual reachability depends on the host/network. |
| Logs and downloaded files | Logs may expose local paths and exception messages. Downloaded DOCX copies are outside server retention control. |

The current API model does not collect purchaser names, email addresses, or payment
details. That is not a guarantee that ticket artwork, OAuth account data, logs, or
future generated content is non-sensitive. No formal classification or retention
policy is encoded.

## Controls present and limits

- `[ApiController]` provides framework binding behavior for integer parameters;
  it does not supply application-specific bounds or ordering validation.
- Client input does not directly choose the generated output path. The controller
  uses a fixed output directory and GUID filename. Internal read/delete commands
  still accept paths and should not be exposed directly without confinement.
- HTTPS redirection is configured, but certificates, TLS termination, trusted
  forwarded headers, and production listener configuration are not supplied.
- Errors returned by the controller are generic; exceptions are logged internally.
  Some handlers swallow exceptions, which weakens operational detection.
- The WebApi output directory and common logs/build artifacts are ignored by Git;
  OAuth client/token paths are not explicitly protected by the ignore rules.

## Source-observed risks and required decisions before public use

| Observation | Consequence | Recommended follow-up, not implemented here |
| --- | --- | --- |
| No authentication or authorization attributes | Any caller who can reach the API can request expensive generation. | Define intended users and enforce access at the application and/or controlled gateway. |
| No range bounds, rate limiter, or queue | Large/hostile requests consume CPU, disk and memory; `int.MaxValue` end can wrap the loop. | Validate positive ordered ranges, cap batch size safely, limit concurrency and request frequency, test cancellation. |
| Shared intermediate filenames | Overlapping requests can corrupt/interfere with another document. | Use isolated per-request temporary workspaces or in-memory image embedding. |
| Best-effort cleanup, swallowed errors | Orphan files or incomplete successful downloads; no guaranteed retention. | Reliable error propagation, cleanup in guaranteed paths, failure metrics, controlled orphan cleanup. |
| Full Drive scope and local token cache | If enabled, token compromise can affect the authorized account's Drive data. | Minimize scope and account access; secure cache permissions, backups, rotation and revocation procedures. |
| Credentials not explicitly ignored | Local OAuth files can accidentally enter commits or build context. | Add appropriate exclusions in an implementation change; use external secret provisioning and inspect staged files. |
| Development Compose defaults | Swagger, guest broker credentials, and published broker ports are unsuitable assumptions for production. | Remove unused broker or secure it; restrict ingress; use an explicit production configuration. |
| Dependency and image versions are not audited here | Current advisory status and reproducibility are unknown. | Review current direct/transitive advisories and pin approved release artifacts. |

The integer overflow and concurrency concerns are inferred from source; no exploit
or load test was run. This list is not an exhaustive vulnerability assessment.

## Data and credential handling

Use synthetic inputs and small ranges for testing. Do not commit generated documents,
OAuth client JSON, refresh/access tokens, private logs, certificates, or production
configuration. Before enabling Drive, document the authorized account, consent
scope, token-store location and access permissions, revocation owner, and recovery
procedure without recording secret values in the context package.

There is no app-defined guaranteed deletion window, encryption-at-rest mechanism,
audit trail, secret rotation, backup policy, incident contact, or regulatory
compliance statement. Set these according to the actual deployment and data.
Do not call an unauthenticated reachability route a security or readiness check.

## Incident response starting points

For suspected abuse, restrict ingress or stop the generation service, preserve
relevant logs without publishing secrets, identify active requests and leftover
files, and assess whether documents were incomplete or duplicated. For a suspected
Drive-token leak, revoke affected access through the account's approved process
and replace compromised credentials; deleting local files alone does not revoke
remote grants. Validate a repaired service with small controlled requests before
restoring access. Operator ownership and escalation contacts remain unrecorded.

Evidence: [Program](../../src/TicketMapper.WebApi/Program.cs),
[controller](../../src/TicketMapper.WebApi/Controllers/DocumentController.cs),
[renderer](../../src/TicketMapper.Application/Commands/CreateDocumentCommand.cs),
[Drive setup](../../src/TicketMapper.WebApi/Configs/ConfigureGoogleDrive.cs),
[Compose](../../docker-compose.yml), [Dockerfile](../../src/Dockerfile),
[ignore rules](../../.gitignore).
