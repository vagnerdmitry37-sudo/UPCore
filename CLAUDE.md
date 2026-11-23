# CLAUDE.md

UPCore is a backend service built the same way as the UP API (`../UP`). Follow `../UP/CLAUDE.md` for architecture, folder, boundary, configuration and coding conventions, and its `developer` skill (`../UP/.claude/skills/developer/SKILL.md`) for how code is written. Replace `UP.` with `UPCore.` and `UP.slnx` with `UPCore.slnx` when reading them.

## Stack

ASP.NET Core on .NET 10 (SDK pinned in `global.json`), EF Core + Npgsql (PostgreSQL). Solution file is `UPCore.slnx`. Same build rules as UP: `Directory.Build.props` turns warnings into errors, package versions live in `Directory.Packages.props`.

## Commands

```bash
dotnet build UPCore.slnx
dotnet run --project src/UPCore.Api --launch-profile https   # https://localhost:7315
dotnet format UPCore.slnx --verify-no-changes

dotnet tool restore
dotnet ef migrations add <Name> --project src/UPCore.Infrastructure --startup-project src/UPCore.Api --output-dir Persistence/Migrations
dotnet ef database update --project src/UPCore.Infrastructure --startup-project src/UPCore.Api
```

## Database

UPCore owns its own database `upcore` on the same local PostgreSQL server as UP, with its own `AppDbContext` and migrations. It never reads or migrates UP's `up` database. The connection string is a user secret:

```bash
dotnet user-secrets set "Options:Database:ConnectionString" "Host=localhost;Port=5432;Database=upcore;Username=up;Password=<pw>" --project src/UPCore.Api
```

## Projects

```
UPCore.Api  →  UPCore.Infrastructure  →  UPCore.Application  →  UPCore.Domain
```

Domain and Application are empty until the first feature arrives. No tests project yet.
