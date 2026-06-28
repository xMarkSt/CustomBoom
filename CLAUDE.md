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

---

## Reference Project

This is a rework of an existing Laravel/PHP project. The original lives at `C:\Projects\Mark\boomclone` and is the authoritative source of requirements. When implementing or clarifying behaviour, consult the original.

### API Endpoints to Implement

| Method | Path | Controller method | Description |
|---|---|---|---|
| POST | `/tournaments/schedule` | `schedule()` | Client requests next active tournament; returns plist with level/target/countdown |
| POST | `/tournaments/join` | `join()` | Submit score + ghost replay; creates/updates Standing; returns tournament standings plist |
| POST | `/tournaments/reload` | `reload()` | Re-fetch current standings plist |
| POST | `/tournaments/ghost` | `ghost()` | Download opponent's ghost binary for a tournament |
| POST | `/tournaments/update` | `update()` | Update player's score mid-tournament |
| POST | `/tournaments/results` | `results()` | Top 3 + player's own standing after tournament ends |
| GET | `/hsnews/feed/Boom/{locale}/{type?}/{timestamp?}` | `show()` | News/popup articles as plist |

### Business Logic Details

**`schedule` endpoint:**
- Returns the soonest-ending active `TournamentGroup`.
- Response plist includes: theme name, level ID, target type/amount, online flag, level URL + version, countdown to end.

**`join` endpoint:**
- Creates or updates a `Standing` for the player (keep only the player's best time per tournament).
- Creates or replaces the player's `Ghost` (gzip-compressed binary replay).
- Recalculates rank across all standings sorted ascending by time.
- If the player lands at rank #1, send a Discord webhook notification with the top 5 standings.
- Returns plist with full standings list + player's rank.

**`reload` endpoint:**
- Same standings plist as `join`, but read-only (no score submission).

**`ghost` endpoint:**
- Accepts `tournament_uuid` + `opponent_uuid` and returns the raw ghost binary for that specific player.

**`results` endpoint:**
- Returns top 3 standings plus the requesting player's own standing.

**`schedule` / `join` plist key names:**
- Use `[PlistPropertyName]` attributes to match the original PHP plist key names exactly — the iOS client depends on them.

### Ghost Binary Format

Ghost data is gzip-compressed before storage. After decompression the binary layout is:

**Header** (26 bytes):
| Offset | Size | Field |
|---|---|---|
| 0 | 1 | version |
| 1 | 1 | idk |
| 2 | 1 | hero style |
| 3 | 1 | engine style |
| 4 | 1 | wheel style |
| 5 | 21 | idk (padding) |

**Frames** (24 bytes each, repeated):
| Offset | Size | Field |
|---|---|---|
| 0 | 4 | timestamp (float) |
| 4 | 4 | x position (float) |
| 8 | 4 | y position (float) |
| 12 | 4 | rotation (float) |
| 16 | 4 | obj248 (float) |
| 20 | 4 | flags (uint32) |

Flags bitmask: `F_LEFT=1, F_RIGHT=2, F_ROCKET=4, F_OBJ200=8`, upper bits encode animation state.

### Discord Notifications

When a player achieves rank #1 after `join`, POST a notification to `DISCORD_WEBHOOK_URL` (env/config) containing the top 5 standings (nickname, time, rank).

### ELO / Tournament Group Assignment

- `TournamentGroup` has an ELO range (`elo_min`, `elo_max`).
- `Tournament` instances within a group are assigned to an ELO bucket.
- Players are matched to the tournament whose ELO range contains their rating.
- `CreateGroup()` auto-chains start times: new group begins where the previous one ended, and randomly picks a `LevelTarget`.
- When creating a group there is a 25% chance `no_super = 1` (disables "super" game mechanic).

### Standings Plist Structure

The standings list is returned as a CFArray of CFDictionaries. Each dictionary contains:
- Player fields: `nickname`, `country`, hero/engine/wheel style IDs
- Standing fields: `rank`, `time` (best lap in ms), `ghost` reference

The plist root dictionary also carries the player's own `rank` as a top-level key alongside the standings array.

### Level / LevelTarget Seeding

Initial game content (levels, themes, targets, level_targets) is loaded from SQL scripts in `/Scripts/`. The `Level` entity may reference an online `.plhs` file (binary plist); the URL and version are included in the `schedule` response so the client can download/cache the level file.

<!-- BACKLOG.MD GUIDELINES START -->
<CRITICAL_INSTRUCTION>

## Backlog.md Workflow

This project uses Backlog.md for task and project management.

**For every user request in this project, run `backlog instructions overview` before answering or taking action.**

Use the overview to decide whether to search, read, create, or update Backlog tasks.

Use the detailed guides when needed:
- `backlog instructions task-creation` for creating or splitting tasks
- `backlog instructions task-execution` for planning and implementation workflow
- `backlog instructions task-finalization` for completion and handoff

Use `backlog <command> --help` before running unfamiliar commands. Help shows options, fields, and examples.

Do not edit Backlog task, draft, document, decision, or milestone markdown files directly. Use the `backlog` CLI so metadata, relationships, and history stay consistent.

</CRITICAL_INSTRUCTION>
<!-- BACKLOG.MD GUIDELINES END -->
