---
id: TASK-3
title: Implement AveragePos calculation in TournamentStatsDto mapping
status: To Do
assignee: []
created_date: '2026-06-27 13:05'
labels: []
dependencies: []
ordinal: 3000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
AveragePos in the Player -> TournamentStatsDto AutoMapper mapping is currently ignored (always 0) in Boom.Business/MappingProfiles/PlayerProfile.cs. It should reflect the player's average finishing rank across all tournaments they have participated in.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [ ] #1 AveragePos is calculated as the mean rank across all of the player's standings
- [ ] #2 Returns 0 when the player has no standings
- [ ] #3 Mapping test covers the calculation
<!-- AC:END -->
