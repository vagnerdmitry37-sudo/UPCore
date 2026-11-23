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

No tests project yet.

## UpEntities

Both resources follow UP's Products shape: `POST` and `PUT` take an array (1–100, `PUT` items carry their `Id`), `DELETE` takes `?ids=`, `GET {id}` returns one, and `GET` returns a page `{ items, totalRecords }` from `?first=&rows=` (default 50, max 200), which serves both a paginator and lazy virtual scroll. Each bulk call is all-or-nothing: missing ids return 404 listing them, and per-item validation errors return 400 keyed `[index].Field`.

`UpEntity` is the one generic model: `Id` (Guid v7), `Type` (the `Type` of an `UpEntityTemplate`) and `Data` (a JSON object, stored as `jsonb`). CRUD lives at `api/up-entities`; the list needs `?type=`. `Type` can't change after create, and `PUT` replaces `Data` as a whole. Postgres `jsonb` doesn't keep key order, so reads can return the keys of `Data` in a different order than they were sent.

`UpEntityTemplate` describes one kind of entity: a unique lower-case kebab `Type` (`car`) and its `Fields` (`Key`, `Label`, `Kind` = `Text` | `Number`, `Required`), stored as one `jsonb` column via EF `ToJson`. CRUD lives at `api/up-entity-templates`. The front end reads it to build forms. `UpEntityDataValidator` (Domain) checks `Data` against the template on every create and update: unknown keys, wrong kinds and missing required fields return 400 under `Data`. `Type` can't change after create, and a template still used by entities can't be deleted (409). Changing a template's fields doesn't re-check existing entities; they are checked again on their next update.
