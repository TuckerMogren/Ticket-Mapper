# Integrations and dependencies

[Context index](../index.md)

## Google Drive: adapter exists, not used by the ticket endpoint

[IGoogleDrive](../../src/TicketMapper.Infrastructure.DocumentService/Interfaces/IGoogleDrive.cs)
and [GoogleDrive](../../src/TicketMapper.Infrastructure.DocumentService/GoogleDrive.cs)
provide these methods:

| Method | Behavior |
| --- | --- |
| `UploadFileAsync` | Upload caller's stream with name/MIME type; optional parent folder; request response field `id`. Can return null if no response body despite its non-null return annotation. |
| `DownloadFileAsync` | Download by file ID into a MemoryStream, rewind, and return it. Caller owns disposal of the returned stream. |
| `DeleteFileAsync` | Delete the file ID through the Drive API. |
| `GetFileMetadataAsync` | Fetch `id`, `name`, and `mimeType` into `GoogleDriveFile`. |
| `ListFilesInFolderAsync` | Query non-folder entries by parent and return metadata. Does not request/iterate `nextPageToken`; do not assume a complete large-folder listing. |

The list method accepts a folder ID; it does not implement the interface comment's
implied null-to-root behavior. It interpolates folder ID into the Drive query and
does not include a `trashed = false` filter. No adapter method accepts a cancellation
token. No application-defined retry policy or upload-result status validation is
present; behavior internal to the Google client library is not assessed here.

The [Drive factory](../../src/TicketMapper.WebApi/Configs/ConfigureGoogleDrive.cs)
loads `client_secrets.json`, uses full `DriveService.Scope.Drive`, fixed user `user`,
and persists tokens with `FileDataStore("token.json", true)`. It is registered as
a singleton factory and may trigger browser authorization when first resolved.
There is no service-account implementation or managed workload identity flow.

[GoogleDriveRepository](../../src/TicketMapper.Infrastructure.Persistence/Repositories/GoogleDriveRepository.cs)
only receives `IGoogleDrive`; its interface and implementation expose no repository
operations. No controller uses either abstraction. Successful ticket generation
would therefore not prove Drive access works.

## RabbitMQ and FileIO

[Compose](../../docker-compose.yml) starts `rabbitmq:3-management`, exposes ports
5672/15672, uses guest credentials, and makes WebApi depend on the service. There
is no RabbitMQ client package, connection setting, producer, consumer, or queue
in the tracked application code. MediatR notifications do not use RabbitMQ.

`TicketMapper.Infrastructure.FileIO` contains an empty `Class1` and is not in the
solution. Current read/delete handlers instead use the `System.IO.Abstractions`
package. Do not document FileIO as a working storage service.

## Declared library inventory

Versions below are declarations in tracked project files, not a resolved dependency
lock or a security clearance. Missing infrastructure project files limit inventory.

| Component | Declared versions | Role |
| --- | --- | --- |
| .NET target | `net8.0` | API and library runtime target. |
| MediatR / DI extensions | 11.1.0 | In-process requests/notifications and registration. |
| ImageSharp / Drawing / Fonts | 3.0.1 / 1.0.0 / 1.0.0 | Template image manipulation and text drawing. |
| DocumentFormat.OpenXml | 3.0.0 | DOCX package creation. |
| System.IO.Abstractions | 20.0.4 | File read/delete abstraction and mocks. |
| Google.Apis.Drive.v3 | 1.65.0.3256 | Drive API client, declared by WebApi. |
| Serilog / hosting / configuration / file sink | 3.1.1 / 8.0.0 / 8.0.0 / 5.0.0 | Logging setup. |
| Swashbuckle / annotations | 6.5.0 | Development Swagger UI and metadata. |
| Microsoft.NET.Test.Sdk / xUnit | 17.8.0 / 2.6.4 | Test infrastructure. |
| Moq / Shouldly | 4.20.70 / 4.2.1 | Mocks and assertions. |
| coverlet.collector | 3.2.0 in Application/WebApi tests; 6.0.0 in Domain tests | Coverage collection. |

Other declared packages include Microsoft configuration/logging/DI helpers,
`Microsoft.AspNetCore.OpenApi`, and `Shouldly` in WebApi. Use the `.csproj` files
for the complete package list. No `packages.lock.json`, central package file,
`global.json`, or dependency-update workflow is tracked.

Before a release, restore a repaired solution, review direct/transitive dependency
advisories using current tooling, and verify package/font license obligations.
No vulnerability-feed query or dependency audit was run for this documentation.
The repository's [LICENSE](../../LICENSE) contains GPL version 3 text; third-party
dependencies and fonts have their own terms.

Sources: [Application project](../../src/TicketMapper.Application/TicketMapper.Application.csproj),
[WebApi project](../../src/TicketMapper.WebApi/TicketMapper.WebApi.csproj),
[Application test project](../../src/TicketMapper.Test.Application/TicketMapper.Test.Application.csproj).
