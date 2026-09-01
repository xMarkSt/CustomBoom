---
id: TASK-2
title: Implement GetOnlineTarget() and GetOnlineUrl() in TournamentGroupProfile
status: Done
assignee:
  - '@claude'
created_date: '2026-06-27 13:05'
updated_date: '2026-09-01 03:14'
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
- [x] #1 GetOnlineUrl() returns the correct public URL for online levels and null/empty for offline levels
- [x] #2 GetOnlineTarget() returns the target type string matching the PHP original
- [x] #3 schedule endpoint no longer throws NotImplementedException
- [x] #4 Existing mapping tests still pass
<!-- AC:END -->

## Implementation Plan

<!-- SECTION:PLAN:BEGIN -->
1. Change GetOnlineTarget/GetOnlineUrl to instance methods taking LevelTarget/Level params (src already in scope at call sites in TournamentGroupProfile).
<!-- SECTION:PLAN:END -->

## Implementation Notes

<!-- SECTION:NOTES:BEGIN -->
Implemented GetOnlineTarget/GetOnlineUrl per PHP original (App\Models\TournamentGroup::toCFDictionary). Added 6 mapping tests in MappingTests.cs. Full suite: 38/38 passing.
<!-- SECTION:NOTES:END -->

## Final Summary

<!-- SECTION:FINAL_SUMMARY:BEGIN -->
Implemented GetOnlineTarget()/GetOnlineUrl() in TournamentGroupProfile.cs, porting App\Models\TournamentGroup::toCFDictionary() from the PHP original. GetOnlineTarget returns '' for 'Fastest time' targets, else JSON {Type, Target} (Target key omitted when amount is null). GetOnlineUrl builds {APP_URL}/storage/{FilePath} or falls back to {APP_URL}/online-levels/{LevelId}.plhs. Added 6 tests to MappingTests.cs covering all branches; full suite passes 38/38.
<!-- SECTION:FINAL_SUMMARY:END -->
