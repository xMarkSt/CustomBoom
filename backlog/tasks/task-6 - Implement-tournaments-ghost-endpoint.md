---
id: TASK-6
title: Implement tournaments/ghost endpoint
status: Done
assignee:
  - '@claude'
created_date: '2026-06-28 16:31'
updated_date: '2026-06-29 20:18'
labels: []
dependencies: []
ordinal: 6000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
The ghost endpoint returns the raw binary ghost data for a specific opponent in a tournament. The client uses this to download a replay to race against. PHP reference: TournamentController@ghost in C:\Projects\Mark\boomclone\app\Http\Controllers\TournamentController.php.
<!-- SECTION:DESCRIPTION:END -->




## Implementation Plan

<!-- SECTION:PLAN:BEGIN -->
1. Add GhostTournamentDto request DTO (tournament_uuid, opponent_uuid form fields).
<!-- SECTION:PLAN:END -->

## Implementation Notes

<!-- SECTION:NOTES:BEGIN -->
Added GhostTournamentDto (tournament_uuid, opponent_uuid). Added ITournamentService.GetGhost + TournamentService.GetGhost: single Standing query filtering on Tournament.Uuid + Player.Uuid, includes Ghost, returns Ghost.Data or null. Added Ghost POST action to TournamentsController WITHOUT [EncryptResponse], returns File(bytes, application/octet-stream) or NotFound(). Ghost bytes returned as-is (gzip-compressed as stored) to match authoritative PHP TournamentController@ghost; AC #3 'decompressed' wording deviates — confirmed with user to match PHP for iOS client compatibility. Verified: dotnet build (0 errors) and full test suite (21 passed, incl. 3 new GetGhost tests).
<!-- SECTION:NOTES:END -->

## Final Summary

<!-- SECTION:FINAL_SUMMARY:BEGIN -->
Implemented POST /tournaments/ghost. New GhostTournamentDto request DTO, ITournamentService.GetGhost/TournamentService.GetGhost (looks up the opponent's Standing within the tournament and returns the stored Ghost bytes, or null), and a controller action returning the raw binary with Content-Type application/octet-stream (no EncryptResponse) or 404 when not found. Bytes are served verbatim to match the authoritative PHP reference (user confirmed over AC #3's 'decompressed' wording). Verified with dotnet build (0 errors) and 21/21 unit tests passing (3 new).
<!-- SECTION:FINAL_SUMMARY:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [x] #1 POST /tournaments/ghost accepts form fields: tournament_uuid, opponent_uuid
- [x] #2 Looks up the standing for the given opponent_uuid within the tournament
- [x] #3 Returns the stored ghost binary verbatim (as-is, still gzip-compressed; the client decompresses) with Content-Type: application/octet-stream, matching the authoritative PHP TournamentController@ghost
- [x] #4 Returns 404 if tournament or opponent standing not found
- [x] #5 Does NOT go through EncryptResponse — response is raw binary
<!-- AC:END -->
