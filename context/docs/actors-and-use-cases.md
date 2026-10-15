# Actors and use cases

[Architecture model](architecture-model.md) · [Context index](../index.md)

## Actor catalog

These are human responsibilities inferred from the app's workflow and operational
needs. All assignments are **Unverified**: the repository identifies no named
operator, administrator, deployment owner or permission hierarchy. One person may
perform several roles. The API does not authenticate these personas or enforce
role-based access.

| ID | Actor | Goal and interaction | Boundary / evidence |
| --- | --- | --- | --- |
| A01 | Raffle operator | Choose a number range, download tickets, review and print them; coordinate issued numbers externally. | HTTP generation exists; range coordination and printing are manual. [Controller](../../src/TicketMapper.WebApi/Controllers/DocumentController.cs), [usage](overview-and-usage.md). |
| A02 | Developer / maintainer | Edit template/code, build/test, update configuration or integrations and documentation. | Repository/tooling access, not an API permission. [Development](development.md), [solution](../../src/TicketMapper.sln). |
| A03 | Deployment / support operator | Provision runtime, configure secrets/logs, check service/output health, deploy/roll back, remove confirmed orphan files. | Host/container authority is external to the app. [Deployment](deployment.md), [operations](operations.md). |
| A04 | Google Drive account owner | Consent to optional Drive access and manage credential revocation. | Only relevant if Drive client is resolved; no association with an API user's identity. [OAuth loader](../../src/TicketMapper.WebApi/Configs/ConfigureGoogleDrive.cs). |

No purchaser, seller, winner, finance user or system administrator role is implemented
in the API. Do not create interactions for those roles from the word “raffle” alone.

## Use-case catalog

Use-case status applies to application support. Manual activities are **Unverified**
operational procedures, even when a guide recommends them.

| ID / status | Actor and trigger | Preconditions | Main outcome and limits |
| --- | --- | --- | --- |
| UC01 / Implemented | A01 requests numbered tickets through Swagger or HTTP. | Repaired build; running API; readable template, available Arial, writable output directory; caller supplies range. | DOCX bytes returned as `Tickets.docx`; cleanup attempted. No guaranteed completeness or number uniqueness. |
| UC02 / Implemented | A01 or A03 checks HTTP reachability. | Running API. | `Hi` with 200. Does not validate generation, disk, fonts, Drive or broker. |
| UC03 / Unverified | A01 opens, verifies and prints the downloaded document. | Successful download and compatible external tooling. | Manually reviewed print batch; no application acknowledgement or print integration. |
| UC04 / Unverified | A02 changes event artwork or rendering code. | Repository access; current template/layout understood. | Reviewed source/template change; not an upload or customization endpoint. |
| UC05 / Unverified | A02 builds/tests; A03 deploys/configures/releases. | Project-file blocker repaired; runtime prerequisites supplied. | Candidate runtime; no established successful release or production host. |
| UC06 / Unverified | A03 investigates failure and cleans confirmed orphan outputs. | Active requests accounted for; actual working directory known. | Controlled manual recovery; no scheduled cleanup endpoint or worker. |
| UC07 / Implemented, optional adapter only | A02 exercises Drive operations; A04 supplies consent if needed. | Missing infrastructure projects repaired; explicit adapter caller; authorized credential/account. | Adapter can upload/download/delete/list/inspect metadata. No HTTP route, production caller or live access verified. |

## UC01 contract and alternative outcomes

The request contains `startNumber` and `endNumber`; the server chooses a GUID DOCX
path. The controller creates, reads, then publishes a cleanup notification before
returning bytes. Number allocation is external; retries can duplicate a printed range.

Success intent: every integer in the inclusive range appears in a valid, printable
document and temporary files are removed. This is **not a guaranteed postcondition**
of current code: per-ticket failures can be swallowed; deletion is best effort.
The user must inspect the output. Reversed/negative/oversized ranges have no business
validation. Missing/failed output reads become 500; some propagated cancellation
becomes 400, while swallowed cancellation may still lead to 200 or 500.

See [API](api.md) for exact responses and [sequence flows](sequence-flows.md) for
failure and cancellation paths. A disconnected caller may receive no response at
all, regardless of the controller's chosen status.

## UC07 boundary

OAuth consent is authorization to access a Google account, not login to Ticket
Mapper. The fixed credential-cache user key is `user`; no mapping from API callers
to Drive identities exists. Ticket generation does not require Drive, does not
upload its result there, and does not delete Drive files during local cleanup.

## Responsibilities outside the application

Range assignment, event approval, template rights, printing, physical ticket
distribution and result retention remain manual/business responsibilities. Host
access, TLS, credential custody and operational escalation belong to the deployment
environment. These are boundary statements, not assertions that a particular person
or control has been assigned. [Traceability](architecture-traceability.md) lists
the evidence needed to resolve those unknowns.
