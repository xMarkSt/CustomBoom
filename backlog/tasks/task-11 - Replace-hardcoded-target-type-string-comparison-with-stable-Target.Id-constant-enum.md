---
id: TASK-11
title: >-
  Replace hardcoded target type string comparison with stable Target.Id
  constant/enum
status: Done
assignee:
  - '@claude'
created_date: '2026-09-01 03:33'
updated_date: '2026-09-01 03:39'
labels: []
dependencies: []
references:
  - Boom.Business/MappingProfiles/TournamentGroupProfile.cs
  - Boom.Infrastructure/Data/Entities/Target.cs
ordinal: 11000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
GetOnlineTarget() in Boom.Business/MappingProfiles/TournamentGroupProfile.cs special-cases the 'Fastest time' target by comparing the human-readable Target.Type display string. Since Target.Type is DB-editable content (Scripts seeds it, and it exists to be shown/localized), any rename, typo fix, or translation of that string silently changes which target type is treated as the special 'no goal JSON' case. The targets table is seeded with fixed, stable ids (id 1 = 'Fastest time') - the comparison should key off Target.Id (or a small enum/const mapping to it) instead of the display text.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [x] #1 GetOnlineTarget() no longer compares against the literal string "Fastest time"
- [x] #2 A well-known identifier (enum or constant) for the Fastest Time target id is defined in a shared location and referenced by GetOnlineTarget()
- [x] #3 Existing MappingTests.cs coverage for GetOnlineTarget() still passes and continues to reference the well-known identifier rather than a raw string where applicable
- [x] #4 Renaming Target.Type in the database no longer changes which target is treated as the fastest-time/no-goal case
<!-- AC:END -->

## Implementation Plan

<!-- SECTION:PLAN:BEGIN -->
1. Add Boom.Infrastructure/Data/Entities/TargetType.cs - enum mirroring the stable ids seeded in Scripts/targets.sql (FastestTime=1 .. TouchBowlingPin=17). 2. In TournamentGroupProfile.GetOnlineTarget, compare levelTarget.TargetId == (long)TargetType.FastestTime instead of levelTarget.Target.Type == "Fastest time". 3. Update MappingTests.cs OnlineLevelTarget helper to take a TargetType + a separate display-text string, and rework the fastest-time test to use a non-matching display string to prove the logic no longer depends on it.
<!-- SECTION:PLAN:END -->

## Implementation Notes

<!-- SECTION:NOTES:BEGIN -->
Added TargetType enum (Boom.Infrastructure/Data/Entities/TargetType.cs) mirroring Scripts/targets.sql ids. GetOnlineTarget() now compares levelTarget.TargetId against TargetType.FastestTime. Updated MappingTests.cs to pass TargetType + independent display text, with the fastest-time test using a deliberately different type string to prove decoupling. Full suite: 38/38 passing.
<!-- SECTION:NOTES:END -->

## Final Summary

<!-- SECTION:FINAL_SUMMARY:BEGIN -->
Replaced the 'Fastest time' string comparison in TournamentGroupProfile.GetOnlineTarget() with a TargetType enum keyed on the stable Target.Id (TargetType.FastestTime = 1), matching the ids already seeded in Scripts/targets.sql. Renaming Target.Type in the DB can no longer change which target is treated as the no-goal case. MappingTests.cs updated accordingly, including a test that renames the display text while keeping TargetId=1 to prove the decoupling. Full suite passes 38/38.
<!-- SECTION:FINAL_SUMMARY:END -->
