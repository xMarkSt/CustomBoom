---
id: TASK-1
title: 'Implement Discord webhook notification on rank #1'
status: To Do
assignee: []
created_date: '2026-06-27 13:03'
labels: []
dependencies: []
ordinal: 1000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
After a player achieves rank #1 in TournamentService.Join(), POST a notification to the Discord webhook URL containing the top 5 standings. The TODO comment at TournamentService.cs line 113 marks the location.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [ ] #1 DISCORD_WEBHOOK_URL is read from configuration (env var / appsettings)
- [ ] #2 Notification is only sent when the submitting player lands at rank 1
- [ ] #3 Payload contains top 5 standings with nickname, time, and rank
- [ ] #4 Failure to reach Discord does not break the join response
<!-- AC:END -->
