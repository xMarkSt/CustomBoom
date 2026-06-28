---
id: TASK-6
title: Implement tournaments/ghost endpoint
status: To Do
assignee: []
created_date: '2026-06-28 16:31'
labels: []
dependencies: []
ordinal: 6000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
The ghost endpoint returns the raw binary ghost data for a specific opponent in a tournament. The client uses this to download a replay to race against. PHP reference: TournamentController@ghost in C:\Projects\Mark\boomclone\app\Http\Controllers\TournamentController.php.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [ ] #1 POST /tournaments/ghost accepts form fields: tournament_uuid, opponent_uuid
- [ ] #2 Looks up the standing for the given opponent_uuid within the tournament
- [ ] #3 Returns the raw decompressed ghost binary with Content-Type: application/octet-stream
- [ ] #4 Returns 404 if tournament or opponent standing not found
- [ ] #5 Does NOT go through EncryptResponse — response is raw binary
<!-- AC:END -->
