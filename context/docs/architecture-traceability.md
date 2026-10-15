# Architecture traceability and evidence gaps

[Architecture model](architecture-model.md) · [Actors](actors-and-use-cases.md) ·
[Components](component-map.md) · [Resources](runtime-view.md)

IDs below refer to the canonical catalogs in those documents. **Implemented** means
source support only. Manual actions and actual deployment/account assignments are
**Unverified**. This matrix connects business interactions to concrete code without
implying a verified production architecture.

## Actor-to-source matrix

| Use case / status | Actors | Endpoint or entry | Component path | Data / resource | Source and verification |
| --- | --- | --- | --- | --- | --- |
| UC01 Generate tickets / Implemented | A01 | `GET /api/v1/Document/DownloadDocument` with range | C02 → C04 → {C05, C06, C07}; C07 → C04 → C08; C06/C08 → C09 | TicketDetails; D01/D02 → D03/D04 → D05; TimeToDeleteNotification | [Controller](../../src/TicketMapper.WebApi/Controllers/DocumentController.cs), [create](../../src/TicketMapper.Application/Commands/CreateDocumentCommand.cs), [read](../../src/TicketMapper.Application/Queries/GetDocumentQuery.cs), [notification](../../src/TicketMapper.Application/IntegrationEventHandlers/TimeToDeleteNotificationHandler.cs), [delete](../../src/TicketMapper.Application/Commands/DeleteDocumentCommand.cs). F01–F04 source-derived; no live/visual verification. |
| UC02 Reachability / Implemented | A01, A03 | `GET /api/v1/Test/Hello` | C03; host C13 logging | `Hi`; D09 if configured | [TestController](../../src/TicketMapper.WebApi/Controllers/TestController.cs). No live request or meaningful HTTP test established. |
| UC03 Review/print / Unverified manual | A01 | External viewer/printing E02; no route | No app component after C02 download | D05; manually recorded issued range | [Usage](overview-and-usage.md), [download result](../../src/TicketMapper.WebApi/Controllers/DocumentController.cs). Viewer, printer and business process unverified. |
| UC04 Maintain artwork/code / Unverified manual | A02 | Repository edits/build tools; no route | C05 rendering code; C01 if wiring changes | D01; D02/layout constants as needed | [Renderer](../../src/TicketMapper.Application/Commands/CreateDocumentCommand.cs), [WebApi project](../../src/TicketMapper.WebApi/TicketMapper.WebApi.csproj). Committed template exists; no visual output validation. |
| UC05 Build/deploy/configure / Unverified operational | A02, A03 | CLI/host/Compose; no deployment API | C01 → C12/C13; R01 and optional R02 definitions | D06/D09; template/font/output prerequisites | [Solution](../../src/TicketMapper.sln), [Program](../../src/TicketMapper.WebApi/Program.cs), [Dockerfile](../../src/Dockerfile), [Compose](../../docker-compose.yml). Restore blocked by missing projects; Compose syntax previously passed, no live deployment verified. |
| UC06 Investigate/clean orphans / Unverified manual | A03 | Host filesystem and logs; no route | No scheduled component; inspect C05/C06/C07/C08 failures | D03/D04/D09; never treat D07/D08 as ticket output | [Operations](operations.md), [delete handler](../../src/TicketMapper.Application/Commands/DeleteDocumentCommand.cs). Automatic path only deletes requested DOCX; manual procedure not exercised. |
| UC07 Optional Drive operations / Implemented adapter, unused by API | A02, A04 | `IGoogleDrive` methods; no HTTP route | C10 → C11 → E01; C14 only holds dependency | D06/D07/D08; external file IDs, streams and GoogleDriveFile | [Adapter](../../src/TicketMapper.Infrastructure.DocumentService/GoogleDrive.cs), [credential loader](../../src/TicketMapper.WebApi/Configs/ConfigureGoogleDrive.cs). No real account/grant/adapter call verified. |

UC01's braces group the controller's separately dispatched stages. See sequence
F01 for exact order. C15 has no
use-case path because it is an empty, unreferenced FileIO placeholder.

## Relationship coverage checks

| Relationship | Status / evidence boundary |
| --- | --- |
| Human actor → API | Implemented endpoints, unverified actor assignment; no API identity or role enforcement. |
| S01 → R01 → C01–C14 | Source organization and registrations support the model; successful runtime construction unverified. |
| C02 → C04 → handlers | Explicit bindings in [ConfigureDependencies](../../src/TicketMapper.WebApi/Configs/ConfigureDependencies.cs); no full mediator/HTTP integration test established. |
| C05 → filesystem vs C06/C08 → C09 | Direct rendering I/O versus injected read/delete abstraction; do not merge these implementation paths. |
| C07 → C08 | In-process mediator request; not an external event or deferred job. |
| R01 → R02 | Compose `depends_on` only; no business/message edge. |
| C10 → E01 | Optional implemented adapter; no ticket-route participation or verified credentials. |
| C14 → C10 | Constructor dependency only; repository has no business methods. |
| D04 → D05 | Bytes are copied into the response after a local deletion attempt; downloaded data is outside server lifecycle. |

## Production completeness checklist

This is an evidence checklist for future documentation, not an added deployment
approval requirement. Each row is currently **Unverified**; retain that status until
the listed evidence is obtained. The source-based diagrams can be used now.

| Unknown | Evidence needed | Model update when verified |
| --- | --- | --- |
| Named owners and users | Actual operating/support responsibilities and permitted audience | A01–A04 assignments, escalation and access boundaries. |
| Buildable/runtime baseline | Restored project definitions, successful build and meaningful test discovery/results | S01/R01 verification metadata and current blockers. |
| Complete valid ticket output | Small range request, document/schema and print-layout review, cleanup observations | UC01/UC03 validation and F01 evidence. |
| Hosting topology | Actual host/provider, process/container inventory, image/artifact IDs and volumes | H03 and R01 placements; distinguish actual environments. |
| Ingress/TLS/access control | Actual listeners, DNS, certificate termination, firewall/gateway/application policy | Network/trust edges and P01 if implemented. |
| Filesystem and font provisioning | Working directory, runtime identity/permissions, template/font deployment, temporary storage | D01–D04 physical placement and lifecycle. |
| Logging and support | Loaded sink settings, actual logs, retention, monitoring and responsible operator | C13/D09 and UC06 operating procedure. |
| Drive use | Decision to enable/remove; explicit caller, account grant/scope and token storage without secret values | C10/C11/E01 optional status and UC07 evidence. |
| RabbitMQ use | Decision to remove scaffold or evidence of new producer/consumer/queue code | R02 edges; do not add a business edge without implementation. |
| Release and recovery | Real CI/release process, rollback artifact and exercised recovery results | UC05 and P04 if implemented; deployment/operations docs. |
| Architecture interchange requirements | Target schema/template, supported diagram format, import or review result | Tool-specific export/mapping; do not claim compatibility beforehand. |

## Updating this model

When a source behavior changes, update its use case, component/DI entry, resource
relationship, affected sequence, and matrix row together. Keep stable IDs and link
new evidence. Mark removed components as removed in the change description rather
than silently reusing IDs for unrelated elements. Record proposed capabilities in
the future-state list until code and wiring support moving them into current views.

Documentation link/diagram checks establish structural consistency only. They do
not establish application execution, external connectivity, or operational controls.
