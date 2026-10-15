# API contract

[Context index](../index.md)

These contracts come from controller source, not a running OpenAPI capture.
Base URLs for the local `https` launch profile are `https://localhost:8078` and
`http://localhost:8079`. HTTPS redirection is enabled. Compose maps HTTP port 8080;
it does not configure a working HTTPS listener or certificate.

## Routes

| Method and path | Input | Intended success |
| --- | --- | --- |
| `GET /api/v1/Test/Hello` | None | 200 with string `Hi`. |
| `GET /api/v1/Document/DownloadDocument` | Query integers `startNumber`, `endNumber` | 200 binary DOCX download. |

Swagger UI is at `/swagger` and the default v1 specification at
`/swagger/v1/swagger.json` in Development. The configured title is `Document API`,
version `v1`. There is no implemented authentication scheme or `[Authorize]`
attribute on these controllers. `UseAuthorization()` alone does not restrict them.

## Download request and success

```http
GET /api/v1/Document/DownloadDocument?startNumber=1&endNumber=3
```

Both query parameters bind to C# `int` values. Swagger marks them required, but
there is no `[BindRequired]`, custom validator, minimum, maximum, or range-order
check. Do not rely on Swagger annotations as server validation. Missing-value
binding behavior has not been HTTP-tested; callers should always send both values.
Malformed or out-of-`int` values are subject to ASP.NET Core `[ApiController]`
model-binding validation, whose exact response has not been captured here.

The controller creates `TicketDetails` with a GUID-based internal filename. The
client cannot choose a path, template, output format, font, or download filename.
The intended count is `endNumber - startNumber + 1` for an ordered, bounded range.

Successful response:

- Status: `200`.
- Content type: `application/vnd.openxmlformats-officedocument.wordprocessingml.document`.
- Download filename: `Tickets.docx` via MVC's `FileDownloadName`.
- Body: the entire generated file, already buffered in a `byte[]`.

## Explicit controller error responses

| Where caught | Exception | Status and body |
| --- | --- | --- |
| Create stage | `OperationCanceledException` | 400, `Operation was canceled by the client` |
| Create stage | Other exception | 500, `Internal server error: Unable to generate document.` |
| Read/notification stage | `OperationCanceledException` | 400, `Operation was canceled by the client` |
| Read/notification stage | Other exception | 500, `Internal server error` |

Swagger advertises 404 for a missing file, but the controller has no 404 return
branch. A missing generated file is rethrown by the read handler and reaches the
generic 500 catch. Exact serialized content types for error objects/strings depend
on MVC output negotiation; no consistent custom error envelope is defined.

## Important behavioral limits

- This GET creates and deletes server files. Clients, proxies, or automation must
  not assume it is an inexpensive read; there is no application cache policy or
  idempotency key.
- Reversed ranges can yield an empty document rather than a validation error.
  Negative numbers are not rejected. Huge ranges are unbounded; an `int.MaxValue`
  end can wrap the loop counter in the default unchecked context.
- Creation catches per-ticket exceptions and also catches outer failures. It can
  return success to MediatR even when the document is incomplete or absent.
- Cancellation is not uniformly propagated: creation can swallow cancellation;
  file reading does not forward its received token to `ReadAllBytesAsync`;
  deletion/notification can swallow cancellation too.
- There is no upload, Drive, list-results, job-status, health/readiness, or delete
  HTTP endpoint. The `Hello` route checks only basic reachability.

Sources: [DocumentController](../../src/TicketMapper.WebApi/Controllers/DocumentController.cs),
[TestController](../../src/TicketMapper.WebApi/Controllers/TestController.cs),
[Program](../../src/TicketMapper.WebApi/Program.cs),
[CreateDocumentCommand](../../src/TicketMapper.Application/Commands/CreateDocumentCommand.cs),
[GetDocumentQuery](../../src/TicketMapper.Application/Queries/GetDocumentQuery.cs).
