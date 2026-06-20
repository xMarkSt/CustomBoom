# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CustomBoom is an ASP.NET Core 8.0 REST API backend for a mobile tournament game. It manages players, tournament brackets, leaderboards, and ghost (replay) data. Client-server communication is encrypted with AES-256, and responses are serialized to Apple Plist (XML) format for iOS/macOS clients.

## Commands

```bash
# Build
dotnet build Boom.sln

# Run API (Swagger at http://localhost:5237/swagger)
cd Boom.Api && dotnet run

# Run all tests
dotnet test Boom.UnitTests/Boom.UnitTests.csproj

# Run a single test class
dotnet test --filter EncryptionServiceTests

# EF Core migrations
dotnet ef database update --project Boom.Infrastructure --startup-project Boom.Api
dotnet ef migrations add <MigrationName> --project Boom.Infrastructure --startup-project Boom.Api
```

## Architecture

Four-project solution following a clean layered architecture:

- **Boom.Api** — ASP.NET Core entry point: controllers, middleware, action filters
- **Boom.Business** — Services with domain logic; AutoMapper profiles
- **Boom.Infrastructure** — EF Core DbContext, generic `Repository<T>`, entities, migrations, seeding
- **Boom.Common** — Shared DTOs (Request/Response), serialization attributes, extensions

### Request/Response Pipeline

All client traffic goes through two custom components:

1. **`RequestDecryptionMiddleware`** — Extracts `_p` (payload), `_s` (IV), `_u` (UUID), `_ct` fields from the form body, looks up the player's `SecretKey` from the DB, AES-256 decrypts the payload, and replaces the request body with plain JSON before the controller sees it.
2. **`EncryptResponseFilter`** — After the controller returns, serializes the DTO to Plist format, encrypts it with the player's `SecretKey`, and sends the ciphertext back.

In Development, set `"DisableEncryption": true` in `appsettings.Development.json` to bypass this pipeline.

### Key Domain Concepts

| Entity | Description |
|---|---|
| `Player` | User profile with UUID, stats, hero/wheel/engine styles, and per-player AES `SecretKey` |
| `TournamentGroup` | A bracket: duration, target level, ELO range, start/end times |
| `Tournament` | An instance within a group, assigned an ELO group |
| `Standing` | Leaderboard entry linking a Player + Tournament with their best time |
| `Ghost` | Binary replay blob attached to a Standing |
| `Level` / `LevelTarget` / `Theme` | Game content describing playable levels and their variants |

### Service Layer Highlights

- **`TournamentService.Join()`** — Core business logic: creates/updates a `Ghost`, creates/updates a `Standing` (keeping only the player's best time), and returns the tournament.
- **`TournamentService.CreateGroup()`** — Auto-chains tournament times (new group starts where the last one ended) and randomly picks level targets.
- **`EncryptionService`** — AES-256 CBC. Key derivation uses MD5 of a custom string + SecretKey. SecretKey generation appends a random 8-digit suffix to a hex string.
- **`PlistSerializationService`** — Reflection-based serializer; respects `[PlistPropertyName]` attribute on DTO properties for custom key naming.

### Configuration

| Setting | Dev value | Prod value |
|---|---|---|
| MySQL port | 3307 | 3306 |
| DB name | `boomclone` | `boomclone` |
| Encryption | disabled | enabled |

Connection string key: `"BoomConnection"`.

Initial data is loaded from SQL scripts in `/Scripts/` (levels, themes, targets, level_targets).

## Testing

Tests use NUnit + Moq + FluentAssertions. Moq.EntityFrameworkCore and MockQueryable.Moq are used to mock `DbSet<T>` / `IQueryable<T>` without hitting a real database. Test fixtures live in `Boom.UnitTests/Responses/` (e.g., `schedule.plist`).
