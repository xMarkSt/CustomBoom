---
id: TASK-2
title: Implement GetOnlineTarget() and GetOnlineUrl() in TournamentGroupProfile
status: To Do
assignee: []
created_date: '2026-06-27 13:05'
labels: []
dependencies: []
ordinal: 2000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
Both methods currently throw NotImplementedException in Boom.Business/MappingProfiles/TournamentGroupProfile.cs. They are called during schedule endpoint mapping. GetOnlineUrl() should return the public HTTP URL to the level's .plhs file; GetOnlineTarget() should return the target type string. Consult the PHP original at C:\Projects\Mark\boomclone for exact values.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [ ] #1 GetOnlineUrl() returns the correct public URL for online levels and null/empty for offline levels
- [ ] #2 GetOnlineTarget() returns the target type string matching the PHP original
- [ ] #3 schedule endpoint no longer throws NotImplementedException
- [ ] #4 Existing mapping tests still pass
<!-- AC:END -->
