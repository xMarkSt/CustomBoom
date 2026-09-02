---
id: TASK-4
title: Write unit tests for TournamentService.Join()
status: Done
assignee:
  - '@claude'
created_date: '2026-06-27 13:06'
updated_date: '2026-09-01 03:59'
labels: []
dependencies: []
ordinal: 4000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
TournamentService.Join() has no unit test coverage. The method handles best-time guarding, new vs update standing paths, ghost replacement, rank calculation, IsSelf flagging, and the rank-0 duplicate. All paths should be covered.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [x] #1 Returns null when tournament group is not found
- [x] #2 Returns null when tournament group has ended
- [x] #3 Returns existing standings without saving when submitted time is not a personal best
- [x] #4 Creates a new standing and ghost when player has no existing standing
- [x] #5 Updates standing and replaces ghost when submitted time is a new personal best
- [x] #6 Standings are sorted ascending by time with correct 1-based ranks
- [x] #7 The submitting player's standing has IsSelf = true
- [x] #8 Rank-0 duplicate of the #1 standing is prepended to the standings list
<!-- AC:END -->

## Implementation Notes

<!-- SECTION:NOTES:BEGIN -->
Added Boom.UnitTests/Services/TournamentServiceJoinTests.cs covering all 8 acceptance criteria (group not found/ended, PB-guard no-save, new standing+ghost creation, PB update+ghost replace, ranking, IsSelf, rank-0 duplicate). Full suite: 46/46 passing.
<!-- SECTION:NOTES:END -->

## Final Summary

<!-- SECTION:FINAL_SUMMARY:BEGIN -->
Added Boom.UnitTests/Services/TournamentServiceJoinTests.cs (8 tests, following the pattern established by TournamentServiceUpdateTests) covering all acceptance criteria for TournamentService.Join(): group-not-found/ended -> null, non-PB submission returns standings without saving, new-standing+ghost creation, PB update with old-ghost removal, ascending rank ordering, IsSelf flagging, and the rank-0 duplicate of the #1 standing. Verified with dotnet test: 46/46 passing.
<!-- SECTION:FINAL_SUMMARY:END -->
