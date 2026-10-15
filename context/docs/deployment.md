# Deployment and release

[Context index](../index.md)

## What is actually supplied

The repository supplies a [Dockerfile](../../src/Dockerfile) and
[Compose file](../../docker-compose.yml). There is no tracked CI/CD workflow,
infrastructure-as-code deployment, Kubernetes manifest, reverse-proxy config,
registry destination, production hostname, environment inventory, release
promotion rule, or deployment history. No live environment was inspected.

**Deployment is currently blocked:** Dockerfile COPY/restore references the same
two absent infrastructure `.csproj` files as the solution. Compose syntax validation
passes, but does not build an image or prove the application runs.

## Dockerfile stages

The meaningful build chain uses `mcr.microsoft.com/dotnet/aspnet:8.0` as `base`
and `mcr.microsoft.com/dotnet/sdk:8.0` as `build`, copies project definitions,
restores WebApi, copies source, builds Release, publishes, and copies publish output
into the final runtime image. The final work directory is `/app` and the entry
point is `dotnet TicketMapper.WebApi.dll`. Ports 8080 and 8081 are declared with
`EXPOSE`; that declaration does not publish them or configure TLS.

Earlier Alpine and Ubuntu 16.04 stages install curl but are disconnected from the
final image. They do not make curl available in the runtime image; modern builders
may skip those unused stages. No explicit final `USER`, font installation, output
directory creation, health check, or credential mount is defined. Do not assume
Arial, non-root execution, or writable temporary storage is configured.

## Compose topology

| Service | Configuration |
| --- | --- |
| `webapi` | Build context `./src`; Dockerfile `Dockerfile`; publish `8080:8080`; `ASPNETCORE_ENVIRONMENT=Development`; depends on rabbitmq. |
| `rabbitmq` | `rabbitmq:3-management`; hostname `rabbitmq`; publish 5672 and 15672; guest username/password. |

There are no volumes, resource limits, restart policies, readiness checks, secret
mounts, TLS settings, or explicit production logging. `depends_on` has no health
condition and does not establish application-level broker readiness. The API has
no broker integration; do not diagnose ticket generation as requiring RabbitMQ.

## Release prerequisites

Before treating any deployment as usable:

1. Restore the missing project definitions; build and execute meaningful tests.
2. Ensure published output includes `Input Files/Ticket.png`, a usable Arial font
   or deliberately approved replacement, and a writable `Output Files/` directory.
   Verify the actual process working directory and runtime user permissions.
3. Resolve access control, request bounds, concurrency isolation, error propagation,
   and cleanup issues in [security](security.md) and [known issues](known-issues.md).
4. Supply a verified logging configuration. Choose TLS termination and ingress
   restrictions; verify redirects and proxy headers for the actual host.
5. If Drive is needed, implement/prove noninteractive credential provisioning,
   least-privilege access and persistent protected token storage. Otherwise do not
   add credentials merely because Drive registration exists.
6. Review dependencies and build context; exclude credentials and generated files.
   There is no `.dockerignore`, so local contents of `src` need explicit review.
7. Define image tagging/digests, environment owner, monitoring, rollback artifact,
   and retention. These operational choices are not established by the repository.

## Candidate local container procedure

Only after the prerequisites are addressed, from the repository root:

```sh
docker compose config --quiet
docker compose build webapi
docker compose up -d webapi
docker compose ps
docker compose logs --tail=100 webapi
```

Compose also starts the declared dependency unless explicitly disabled. It publishes
the broker ports; inspect the target host's access rules before use. Use the mapped
`http://localhost:8080/api/v1/Test/Hello` to inspect reachability and any redirect,
then perform the small generation/visual/cleanup smoke procedure from
[development](development.md). Do not claim HTTPS works from the `EXPOSE 8081` line.

No image was built or container started during documentation. These commands are
a candidate procedure, not an existing automated release pipeline.

## Direct publish alternative

After build repair, the standard candidate command is:

```sh
dotnet publish src/TicketMapper.WebApi/TicketMapper.WebApi.csproj \
  -c Release -o ./artifacts/publish
```

Verify template/font/output directory and settings in the publish directory before
running `dotnet TicketMapper.WebApi.dll` from there. Configure a process supervisor,
runtime, permissions, HTTPS/ingress and logs for the actual target; none is provided.
Keep publish artifacts out of commits.

## Rollback and recovery

For a future managed deployment, retain the previous tested image or publish
artifact and its configuration version. Stop accepting new generation requests,
let active requests finish or explicitly account for their interruption, redeploy
the prior artifact, and run reachability plus small-document checks. Reconcile
orphan files and operator-issued number ranges before resuming printing.

There are no application database migrations to roll back. That does not remove
the need to account for generated/downloaded documents or optional Drive changes.
No last-known-good production artifact or tested rollback is recorded here.
