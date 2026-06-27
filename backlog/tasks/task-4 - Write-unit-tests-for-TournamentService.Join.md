---
id: TASK-4
title: Write unit tests for TournamentService.Join()
status: To Do
assignee: []
created_date: '2026-06-27 13:06'
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
- [ ] #1 Returns null when tournament group is not found
- [ ] #2 Returns null when tournament group has ended
- [ ] #3 Returns existing standings without saving when submitted time is not a personal best
- [ ] #4 Creates a new standing and ghost when player has no existing standing
- [ ] #5 Updates standing and replaces ghost when submitted time is a new personal best
- [ ] #6 Standings are sorted ascending by time with correct 1-based ranks
- [ ] #7 The submitting player's standing has IsSelf = true
- [ ] #8 Rank-0 duplicate of the #1 standing is prepended to the standings list
<!-- AC:END -->
