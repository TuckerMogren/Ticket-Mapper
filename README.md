# Ticket Mapper

Ticket Mapper is a .NET 8 API created for the Lake George Alumni Association's
fall 2023 raffle. It places sequential numbers onto a ticket image and returns a
downloadable Word document. Swagger is the development interface; this repository
does not contain a separate frontend.

## Documentation

The application context package starts at **[context/index.md](context/index.md)**.
AI agents should read **[context/agents.md](context/agents.md)** first; root
`AGENTS.md` and `CLAUDE.md` both point there.

- [Purpose and usage](context/docs/overview-and-usage.md)
- [Architecture and request flow](context/docs/architecture.md)
- [Actor-to-component architecture](context/docs/architecture-model.md)
- [Local setup and testing](context/docs/development.md)
- [API contract](context/docs/api.md)
- [Configuration](context/docs/configuration.md)
- [Information security](context/docs/security.md)
- [Deployment](context/docs/deployment.md)
- [Operations and troubleshooting](context/docs/operations.md)

## Current build status

The reviewed `dev` baseline references two infrastructure `.csproj` files that are
not committed. A clean solution restore therefore fails with `MSB3202`. See
[known issues](context/docs/known-issues.md) before following the setup guide.
Docker Compose is a development scaffold, not a verified production deployment.

## Contributing

Create a branch from the intended base, follow the [agent guide](context/agents.md),
make a focused change, and run the relevant checks in the
[development guide](context/docs/development.md). Update affected documentation
alongside behavior changes and report baseline blockers separately from regressions.

## License

The repository includes the GNU General Public License, version 3 text in
[LICENSE](LICENSE).
