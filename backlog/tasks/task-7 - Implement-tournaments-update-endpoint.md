---
id: TASK-7
title: Implement tournaments/update endpoint
status: Done
assignee:
  - '@claude'
created_date: '2026-06-28 16:31'
updated_date: '2026-07-12 13:51'
labels: []
dependencies: []
ordinal: 7000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
The update endpoint allows a player to resubmit their run (time + hero/engine/wheel styles + ghost replay) mid-tournament. It finds the player's existing standing (never creates one) and overwrites the whole run atomically only when the submitted time is strictly better than the stored time; equal or slower times leave the standing unchanged. Time, styles, and ghost are treated as one unit because the styles are encoded in the ghost header, so the ghost never diverges from its recorded time. Triggers a Discord notification if the player now holds rank #1. NOTE: this intentionally diverges from the PHP original (TournamentController@update in C:\Projects\Mark\boomclone\app\Http\Controllers\TournamentController.php), which overwrote unconditionally.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [x] #1 POST /tournaments/update accepts form fields: tournament_uuid, user_uuid, time, hero_style, engine_style, wheel_style, ghostData, plus player info fields
- [x] #2 Finds the player's existing standing in the tournament (does not create a new one)
- [x] #3 Updates player info before saving
- [x] #4 Sends Discord webhook notification (top 5) if updated standing is rank #1
- [x] #5 Returns the same standings plist as join/reload
- [x] #6 Response is encrypted via [EncryptResponse] filter
- [x] #7 Returns 404 if tournament or standing not found
- [x] #8 Overwrites time/styles/ghost together (one run) only when the submitted time is strictly better than the existing standing's time; equal or slower times leave the standing unchanged
<!-- AC:END -->

## Implementation Plan

<!-- SECTION:PLAN:BEGIN -->
1. New request DTO UpdateTournamentDto (keyed on tournament_uuid, required time/styles/ghostData, IPlayerInfo metadata).
<!-- SECTION:PLAN:END -->

## Implementation Notes

<!-- SECTION:NOTES:BEGIN -->
Implemented POST /tournaments/update across DTO, service, controller, and mapping layers:

E2E verified against live MySQL (port 3307, DisableEncryption=true). Correct client flow is schedule -> join -> update (join/update maps intentionally ignore Uuid; players register via schedule first).

Added unit tests: Boom.UnitTests/Services/TournamentServiceUpdateTests.cs (6 tests, Moq + MockQueryable + FluentAssertions, mirroring PlayerServiceTests/UnitTest1 patterns):

Behavior change (per user request 2026-07-12): Update no longer overwrites unconditionally. It now overwrites only when dto.Time <= standing.Time (better or equal); a worse time leaves the standing untouched and returns current standings. This intentionally diverges from the PHP original (which overwrote unconditionally). Guard added in TournamentService.Update before ghost replacement. Tests updated: replaced the 'overwrites even when worse' case with Update_DoesNotOverwrite_WhenNewTimeIsWorse, Update_OverwritesTimeAndStyles_WhenNewTimeIsBetter, and Update_Overwrites_WhenNewTimeIsEqual. Full suite 26/26 passing.

Refinement (per user request 2026-07-12): tightened Update to overwrite only on a strictly-better time (dto.Time < standing.Time). Rationale: a ghost — and the hero/engine/wheel styles encoded in its header — belong to a single run, so time+styles+ghost are now overwritten atomically or not at all. Equal times no longer overwrite (reverses the earlier equal-overwrite allowance) to prevent the ghost/styles from diverging from the recorded time. Guard changed from 'dto.Time > standing.Time' to 'dto.Time >= standing.Time'. Tests updated accordingly (equal-time -> no overwrite; ghost/rank tests use strictly-better times). Full suite 32/32 passing.
<!-- SECTION:NOTES:END -->
