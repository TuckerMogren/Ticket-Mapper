# Request sequence flows

[Architecture model](architecture-model.md) · [Component map](component-map.md)

All diagrams describe **Implemented source control flow**, not executed traces.
They assume successful build, DI resolution and required local assets. See
[known issues](known-issues.md) for why a live run has not been established.
Solid arrows are calls; dashed arrows are returns. Library/file operations remain
inside R01 except for the HTTP client. No sequence uses RabbitMQ or Google Drive.

## F01: successful generation and cleanup (UC01)

```mermaid
sequenceDiagram
    actor A01 as A01 Raffle operator
    participant C02 as C02 DocumentController
    participant C04 as C04 MediatR
    participant C05 as C05 Create handler
    participant FS as Local filesystem
    participant C06 as C06 Read handler
    participant C07 as C07 Notification handler
    participant C08 as C08 Delete handler
    A01->>C02: GET DownloadDocument(startNumber, endNumber)
    C02->>C02: Build TicketDetails with GUID DOCX filename
    C02->>C04: Send CreateDocumentCommand(token)
    C04->>C05: Handle request
    C05->>FS: Load D01 template, resolve D02 system font
    C05->>FS: Create D04 DOCX
    loop Each number in inclusive range
        C05->>C05: Clone template and draw number
        C05->>FS: Save D03 PNG
        C05->>FS: Read PNG into DOCX image part
        C05->>C05: Append image paragraph
        C05->>FS: Delete PNG
    end
    C05->>C05: Dispose document and image
    C05-->>C04: Unit
    C04-->>C02: Create completed
    C02->>C04: Send GetDocumentQuery(path, token)
    C04->>C06: Handle request
    C06->>FS: C09 ReadAllBytesAsync(path), no forwarded token
    FS-->>C06: Complete DOCX byte array
    C06-->>C04: Bytes
    C04-->>C02: Bytes
    C02->>C04: Publish TimeToDeleteNotification(token)
    C04->>C07: Handle notification
    C07->>C04: Send DeleteDocumentCommand(path, token)
    C04->>C08: Handle request
    C08->>FS: C09 Delete(path), then Exists(path)
    FS-->>C08: File absent on success
    C08-->>C04: Unit
    C04-->>C07: Deletion request completed
    C07-->>C04: Notification handler completed
    C04-->>C02: Publish completed
    C02-->>A01: 200 DOCX bytes, download name Tickets.docx (D05)
```

The controller awaits notification completion before constructing its file result.
Deletion is after buffering, not after transmission or client acknowledgement.
Neither `Unit` nor completion of `Publish` proves every ticket rendered or every
file was deleted. The controller does not generate a durable job or ticket ledger.

## F02: rendering/read failure alternatives (UC01)

```mermaid
sequenceDiagram
    participant C02 as C02 DocumentController
    participant C05 as C05 Create handler via C04
    participant C06 as C06 Read handler via C04
    participant Log as ILogger
    C02->>C05: Create request
    alt Per-ticket exception inside loop
        C05->>Log: Error processing ticket
        C05->>C05: Continue loop, output may be incomplete
    else Outer rendering exception
        C05->>Log: An error occurred
        Note over C05: Output may be absent, empty or partial
    end
    C05-->>C02: Unit despite handled error
    C02->>C06: Read generated DOCX path
    alt File can be read
        C06-->>C02: Bytes, possibly incomplete content
        Note over C02: Attempt cleanup (F04), then return 200
    else Read fails
        C06->>Log: Critical read error, then rethrow
        C06-->>C02: Exception
        Note over C02: Return 500, cleanup notification is not published
    end
```

An exception that escapes the initial `Send` rather than being handled by C05
(for example a dispatch/resolution failure) reaches the controller's first catch:
generic exceptions produce 500 with the generation-specific message. This is a
different path from swallowed rendering errors followed by a failed read. See
[API](api.md) for exact response text; the controller has no implemented 404 branch.

## F03: cancellation alternatives (UC01)

```mermaid
sequenceDiagram
    actor Client as HTTP caller
    participant C02 as C02 DocumentController
    participant C05 as C05 Create handler via C04
    participant C06 as C06 Read handler via C04
    Client->>C02: Request, request token later becomes cancelled
    C02->>C05: Create with token
    alt Cancellation observed by PNG SaveAsync
        C05->>C05: Per-ticket catch(Exception) logs it
        Note over C05: Loop has no explicit cancellation check, may continue
    else OperationCanceledException reaches outer create catch
        C05->>C05: Log cancellation and return normally
    end
    C05-->>C02: Unit if cancellation was swallowed
    C02->>C06: Read with cancelled token argument
    Note over C06: Handler does not forward token to ReadAllBytesAsync
    alt Read succeeds
        C06-->>C02: Bytes
        Note over C02: Cleanup may swallow cancellation (F04), 200 remains possible
    else Read throws ordinary error
        C06-->>C02: Error rethrown
        Note over C02: Generic 500
    end
    Note over Client,C02: Any OperationCanceledException escaping Send or Publish reaches controller catch: 400
    Note over Client,C02: A disconnected client may receive no response
```

This diagram shows alternative paths, not guaranteed timing. An async method's
cancellation-token parameter alone does not guarantee prompt stopping. C08 passes
the token to `Task.Run`; once scheduled, the synchronous delete operation does not
observe that token. C07/C08 can swallow cancellation before it reaches C02.

## F04: cleanup failure or cancellation after a successful read (UC01)

```mermaid
sequenceDiagram
    participant C02 as C02 DocumentController
    participant C04 as C04 MediatR
    participant C07 as C07 Notification handler
    participant C08 as C08 Delete handler
    participant FS as C09 Local filesystem
    Note over C02: DOCX bytes already buffered
    C02->>C04: Publish notification
    C04->>C07: Handle
    C07->>C04: Send DeleteDocumentCommand
    C04->>C08: Handle
    alt Task.Run cancelled before delete or filesystem error
        C08->>C08: Log and swallow caught exception
    else Delete returns but Exists is true
        C08->>FS: Delete then Exists
        FS-->>C08: Still present
        C08->>C08: Throw InvalidOperationException, catch and log
    else Delete succeeds
        C08->>FS: Delete then Exists
        FS-->>C08: Absent
    end
    C08-->>C04: Unit
    C04-->>C07: Send completes
    C07-->>C04: Handler completes
    C04-->>C02: Publish completes
    Note over C02: Return 200 bytes even if DOCX remains
```

C08 validates a blank path before its try/catch. An error escaping C08 or dispatch
is caught/logged by C07 in the normal notification path. A failure escaping
`Publish` itself can still trigger C02's second catch (400 for cancellation,
otherwise 500). PNGs left by C05 errors are not handled by the DOCX deletion command.
There is no retry queue, transaction or guaranteed `finally` cleanup.

## Sources and validation boundary

- [DocumentController](../../src/TicketMapper.WebApi/Controllers/DocumentController.cs): order, awaiting and response catches.
- [Create handler](../../src/TicketMapper.Application/Commands/CreateDocumentCommand.cs): rendering, per-ticket/outer catches and token use.
- [Read handler](../../src/TicketMapper.Application/Queries/GetDocumentQuery.cs): full buffering, rethrows and absent token forwarding.
- [Notification handler](../../src/TicketMapper.Application/IntegrationEventHandlers/TimeToDeleteNotificationHandler.cs): nested mediator call and catches.
- [Delete handler](../../src/TicketMapper.Application/Commands/DeleteDocumentCommand.cs): pre-try validation, Task.Run and swallowed failure.

The existing tests cover only portions of these handlers and have the limitations
in [development](development.md). Full flow, cancellation and failure tests remain
unverified; do not treat diagrams as test evidence.
