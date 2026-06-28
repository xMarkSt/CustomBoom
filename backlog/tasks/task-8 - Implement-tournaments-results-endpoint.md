---
id: TASK-8
title: Implement tournaments/results endpoint
status: To Do
assignee: []
created_date: '2026-06-28 16:31'
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
- [ ] #1 POST /tournaments/results accepts form fields: user_uuid, tournament_uuid (array of UUIDs)
- [ ] #2 For each tournament UUID, the plist root contains a dictionary keyed by tournament UUID, each holding the results dictionary (top 3 + player's own standing)
- [ ] #3 After processing results, updates the player's tournament stats (wins, podiums, etc.)
- [ ] #4 Response is encrypted via [EncryptResponse] filter
- [ ] #5 Returns 404 if player not found
<!-- AC:END -->
