---
id: TASK-5
title: Implement tournaments/reload endpoint
status: To Do
assignee: []
created_date: '2026-06-28 16:31'
labels: []
dependencies: []
ordinal: 5000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
The reload endpoint re-fetches the current standings plist for a tournament without submitting a new score. It is the read-only counterpart to join. PHP reference: TournamentController@reload in C:\Projects\Mark\boomclone\app\Http\Controllers\TournamentController.php.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [ ] #1 POST /tournaments/reload accepts form fields: tournament_uuid, user_uuid, and player info fields (same as join)
- [ ] #2 Updates player info (calls UpdatePlayer) before returning standings
- [ ] #3 Returns the same standings plist structure as the join endpoint
- [ ] #4 Response is encrypted via [EncryptResponse] filter
- [ ] #5 Returns 404 if tournament not found
<!-- AC:END -->
