# Overview and usage

[Context index](../index.md)

## Purpose and scope

Ticket Mapper turns a static raffle-ticket image into a Word document containing
numbered copies. The repository describes its original use as the Lake George
Alumni Association (LGAA) fall 2023 raffle. The implemented input is an integer
start/end range; the output is a downloadable `Tickets.docx`.

The intended operator is someone preparing tickets for printing. There is no
implemented user-account system, purchase/checkout process, customer record,
ticket inventory, draw/winner selection, or unique-number reservation. Repeating
a range generates the same numbers again. Operators must coordinate ranges and
print quantities outside this application.

## Prerequisite gate

The reviewed checkout cannot restore the full solution because two infrastructure
project files are missing. Follow [development](development.md) and resolve the
[known issues](known-issues.md) before trying the workflow below. These are usage
instructions derived from the code, not a report of a successful live run.

## Operator workflow

1. Confirm the ticket artwork is appropriate for the event. It is bundled at
   `src/TicketMapper.WebApi/Input Files/Ticket.png`; the API has no upload or
   template-selection endpoint. Changing the event artwork is a file change.
2. Start the API using the development guide. Open
   `https://localhost:8078/swagger` when using its `https` launch profile.
   Swagger is enabled only in the Development environment.
3. Check `GET /api/v1/Test/Hello`; an HTTP 200 with `Hi` confirms HTTP reachability.
   It does not validate document generation, filesystem permissions, or Google Drive.
4. Open `GET /api/v1/Document/DownloadDocument`, use **Try it out**, and enter a
   small range such as `startNumber=1`, `endNumber=3`.
5. Execute and save the response as `Tickets.docx`. Both endpoints of the range
   are included, so that example requests three tickets.
6. Open the document in Word or a compatible viewer. Verify every requested number,
   the artwork, page breaks, number placement, and print scaling. Test-print a
   small sample before producing a full batch. Layout is not validated by the API.
7. Record the issued range externally. Keep requests sequential: simultaneous
   overlapping ranges currently share intermediate PNG filenames.

## Command-line example

After local HTTPS is configured and the server is running:

```sh
curl --fail-with-body \
  'https://localhost:8078/api/v1/Document/DownloadDocument?startNumber=1&endNumber=3' \
  --output Tickets.docx
```

Only open the file as a document if curl succeeded. Error responses can also be
written to the output path. Use a trusted development certificate rather than
disabling TLS validation; see [development](development.md).

## Usage constraints

Numbers are plain decimal strings, without leading zeros or a configurable
prefix. No business-range validation or maximum batch size exists. Use small
positive ranges for manual testing; no safe production batch limit has been
measured. Large ranges increase disk, CPU and memory use. There is no job status,
progress, cancellation UI, download history, retry deduplication, or saved result
endpoint. A 200 response can contain an incomplete document because rendering
errors may be swallowed; always inspect the contents.

The requested DOCX is read into memory, then the server attempts deletion before
returning the bytes. This is temporary file handling, not durable storage or a
guaranteed retention policy. See [data and rendering](data-and-rendering.md).

## Source map

- [DocumentController](../../src/TicketMapper.WebApi/Controllers/DocumentController.cs): generation/download workflow.
- [TestController](../../src/TicketMapper.WebApi/Controllers/TestController.cs): reachability endpoint.
- [CreateDocumentCommand](../../src/TicketMapper.Application/Commands/CreateDocumentCommand.cs): numbering and document layout.
- [Launch settings](../../src/TicketMapper.WebApi/Properties/launchSettings.json): local URLs.
