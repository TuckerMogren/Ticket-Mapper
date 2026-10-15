# Data, rendering, and file lifecycle

[Context index](../index.md)

## Models

| Model | Fields | Role |
| --- | --- | --- |
| `TicketDetails` | Required `int StartNumber`, `int EndNumber`, `string FileName` | In-process rendering input; required members do not implement business validation. |
| `TimeToDeleteNotification` | `Guid Id`, required `string FileName`, `DateTime TimeSent` | In-process cleanup signal. Controller supplies a GUID and UTC timestamp. No persisted event log. |
| `GoogleDriveFile` | `string Id`, `string Name`, `string MimeType` | Drive adapter metadata record; unused in ticket generation. |

There is no database schema, migration, customer entity, numbering ledger, or
durable job/result record. Requested numbers and local paths are the primary
runtime data. Artwork contents and any future customized document content need
their own sensitivity assessment.

## Rendering algorithm

1. Load `Input Files/Ticket.png` as an RGBA image.
2. Resolve system font `Arial`, size 30, regular style.
3. Create a DOCX at `TicketDetails.FileName`, add its main document and body, and
   append section properties with 180-twip margins on all four sides (0.125 inch).
4. Loop from start through end using `i <= EndNumber`. Clone the template, draw
   `i.ToString()` in black at image coordinate `(120, 290)`.
5. Save the clone as `Output Files/ticket_{i}.png`; load it into a DOCX image part.
6. Add one paragraph containing one inline image per ticket, then delete that PNG.
7. Dispose the Word document and template image when the handler scope exits.

The checked-in template is **1939 × 364 pixels**. Image extents are computed as
pixel dimensions × 9525 EMUs (a 96-DPI interpretation), approximately 20.20 × 3.79
inches before viewer layout effects. No explicit page size, fit-to-page behavior,
table grid, or per-ticket page break is configured. Verify print layout instead
of assuming a particular number of tickets fits a page.

The image construction reuses drawing IDs and contains a literal `https://` picture
namespace URI. These deserve Open XML validation when rendering is repaired;
successful opening in Word or schema validity was not tested during documentation.
Changing image dimensions, number placement, or fonts can change printed output.

## Paths and lifecycle

All runtime paths below are relative to the process working directory.

| Path | Source and retention |
| --- | --- |
| `Input Files/Ticket.png` | Committed template, marked `CopyToOutputDirectory=PreserveNewest` in WebApi project. Check published/container output separately. |
| `Output Files/<guid>.docx` | Created per request; read into memory; then best-effort deletion before returning response. |
| `Output Files/ticket_<number>.png` | Per-ticket intermediate; deleted after embedding on the success path. Names are shared between requests. |
| `Logs/Logs.txt` | Example file sink path in appsettings, but that JSON is not loaded into the separate Serilog configuration. See configuration guide. |
| `client_secrets.json`, `token.json/` | Optional Drive integration credentials/cache; not required by the current ticket route. |

The code does not create `Output Files/`. A project entry tries to copy
`Output Files/placeholder.txt`, but that file is not tracked. The operator must
provide a writable directory after resolving the build blockers.

## Cleanup and consistency

`GetDocumentQuery` reads the complete file before the controller publishes the
cleanup notification. `DeleteDocumentCommand` calls `IFileSystem.File.Delete` in
`Task.Run`, checks existence, and catches/logs failures. The notification handler
also catches errors. No `finally` guarantees deletion if generation, read, process
execution, or cancellation fails; orphan PNGs and DOCX files can remain.

Output ignore rules cover the WebApi project's output directory, not every possible
working directory. Never treat ignored files as automatically safe or ephemeral.
Downloaded copies belong to the user and are outside server cleanup.

For future fixes, use a per-request workspace, bounded inputs, consistent failure
propagation, cleanup in guaranteed paths, and tests for overlapping requests. These
are recommendations; none is implemented by this documentation change.

Sources: [TicketDetails](../../src/TicketMapper.Domain/DataModels/TicketDetails.cs),
[notification](../../src/TicketMapper.Domain/Notifications/TimeToDelete.cs),
[create](../../src/TicketMapper.Application/Commands/CreateDocumentCommand.cs),
[delete](../../src/TicketMapper.Application/Commands/DeleteDocumentCommand.cs),
[WebApi project](../../src/TicketMapper.WebApi/TicketMapper.WebApi.csproj),
[gitignore](../../.gitignore).
