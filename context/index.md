# Ticket Mapper context index

Start with [agents.md](agents.md) for a compact application briefing and working
conventions. This package documents the repository, not a verified live service.
Baseline: `origin/dev` at `5cfca982c59cd3afd3c0101a0dcc0b257a57a297`, reviewed
2026-10-14. Source links are relative to the current checkout.

## Topic map

| Document | Questions it answers |
| --- | --- |
| [Overview and usage](docs/overview-and-usage.md) | What is it for? How do I generate, review, and print tickets? |
| [Architecture](docs/architecture.md) | What are the layers, dependencies, and request lifecycle? |
| [API contract](docs/api.md) | Which routes, parameters, responses, and errors exist? |
| [Data and rendering](docs/data-and-rendering.md) | How do numbering, images, DOCX layout, files, and cleanup work? |
| [Configuration](docs/configuration.md) | Which settings are actually loaded? Where are credentials and logs? |
| [Local development and tests](docs/development.md) | How do I build, run, debug, and verify changes? |
| [Integrations and dependencies](docs/integrations.md) | What uses Google Drive, RabbitMQ, and each library? |
| [Information security](docs/security.md) | What are the trust boundaries, controls, risks, and data-handling expectations? |
| [Deployment](docs/deployment.md) | What does Docker do? What must be resolved before releasing? |
| [Operations and troubleshooting](docs/operations.md) | How do I check health, diagnose errors, recover, and clean up? |
| [Known issues and evidence](docs/known-issues.md) | What is incomplete, verified, inferred, or still unknown? |

## Reading routes

- **First contribution:** agents → architecture → development → known issues.
- **Use the app:** overview and usage → API → operations.
- **Deploy or review security:** configuration → security → deployment → operations.
- **Change ticket output:** data and rendering → API → development.

## Evidence and maintenance

The code is authoritative for implemented behavior. Documents distinguish source
observations, executed checks, and proposed future work. In particular, setup
commands following a prerequisite gate are instructions to validate later, not
evidence that the current checkout starts successfully.

When changing an endpoint, setting, dependency, path, rendering rule, security
control, or deployment behavior, update the matching document in the same change.
Record newly verified outcomes in the known-issues document; do not leave resolved
items presented as current. Do not add secrets or host-specific machine state.
