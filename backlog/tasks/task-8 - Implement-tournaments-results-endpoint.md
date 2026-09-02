---
id: TASK-8
title: Implement tournaments/results endpoint
status: Done
assignee:
  - '@claude'
created_date: '2026-06-28 16:31'
updated_date: '2026-09-01 04:14'
labels: []
dependencies: []
ordinal: 8000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
The results endpoint returns post-tournament results: top 3 standings plus the requesting player's own standing. It accepts multiple tournament UUIDs and also calls updateTournamentStats on the player. PHP reference: TournamentController@results in C:\Projects\Mark\boomclone\app\Http\Controllers\TournamentController.php.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [x] #1 POST /tournaments/results accepts form fields: user_uuid, tournament_uuid (array of UUIDs)
- [x] #2 For each tournament UUID, the plist root contains a dictionary keyed by tournament UUID, each holding the results dictionary (top 3 + player's own standing)
- [x] #3 After processing results, updates the player's tournament stats (wins, podiums, etc.)
- [x] #4 Response is encrypted via [EncryptResponse] filter
- [x] #5 Returns 404 if player not found
<!-- AC:END -->

## Implementation Notes

<!-- SECTION:NOTES:BEGIN -->
Implemented POST /tournaments/results in TournamentsController.Results, backed by TournamentService.Results. Added ResultsTournamentDto (request), TournamentResultsDto + ResultsResponseDto (response; ResultsResponseDto is a Dictionary<Guid, TournamentResultsDto> so the plist root can be keyed dynamically by tournament uuid). Extended PlistSerializationService.SerializeToNSDictionary to special-case IDictionary payloads for this. Player lookup uses new IPlayerService.GetPlayer (read-only, no upsert) so a missing player yields 404, matching PHP Player::findByUUID semantics (results() never calls updateInfo). Player.WcPlayed/WcWon are recomputed across all of the player's standings (not just the requested tournaments), matching PHP's Player::updateTournamentStats(). Added TournamentServiceResultsTests.cs and a PlistSerializationServiceTests case; full suite (45 tests) passes.
<!-- SECTION:NOTES:END -->

## Final Summary

<!-- SECTION:FINAL_SUMMARY:BEGIN -->
Implemented POST /tournaments/results: TournamentsController.Results looks up the player read-only (IPlayerService.GetPlayer, 404 if missing) and delegates to TournamentService.Results, which loads the requested tournaments, builds a top-3+self ResultsResponseDto keyed by tournament uuid, and recomputes the player's WcPlayed/WcWon across all their standings. PlistSerializationService now supports IDictionary payloads so the plist root can carry dynamic (uuid) keys. Verified with new TournamentServiceResultsTests + a PlistSerializationServiceTests case; full suite (dotnet test) passes at 45/45.
<!-- SECTION:FINAL_SUMMARY:END -->
