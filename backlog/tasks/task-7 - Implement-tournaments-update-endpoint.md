---
id: TASK-7
title: Implement tournaments/update endpoint
status: To Do
assignee: []
created_date: '2026-06-28 16:31'
labels: []
dependencies: []
ordinal: 7000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
The update endpoint allows a player to update their score and ghost data mid-tournament. Unlike join it does not enforce best-time logic — it overwrites unconditionally. Also triggers a Discord notification if the player now holds rank #1. PHP reference: TournamentController@update in C:\Projects\Mark\boomclone\app\Http\Controllers\TournamentController.php.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [ ] #1 POST /tournaments/update accepts form fields: tournament_uuid, user_uuid, time, hero_style, engine_style, wheel_style, ghostData, plus player info fields
- [ ] #2 Finds the player's existing standing in the tournament (does not create a new one)
- [ ] #3 Overwrites time, styles, and ghost data unconditionally
- [ ] #4 Updates player info before saving
- [ ] #5 Sends Discord webhook notification (top 5) if updated standing is rank #1
- [ ] #6 Returns the same standings plist as join/reload
- [ ] #7 Response is encrypted via [EncryptResponse] filter
- [ ] #8 Returns 404 if tournament or standing not found
<!-- AC:END -->
