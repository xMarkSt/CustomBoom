---
id: TASK-10
title: Fix player creation crash when NOT NULL string fields are omitted
status: Done
assignee:
  - '@claude'
created_date: '2026-07-01 20:05'
updated_date: '2026-07-01 20:09'
labels: []
dependencies: []
ordinal: 10000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
Creating a Player via schedule/join/reload throws DbUpdateException 'Column country_code cannot be null' when the client omits country_code (and other NOT NULL string fields). Root cause: Player.CountryCode/Notification/Timezone/HeroStyle/EngineStyle/WheelStyle/MaxGroupIdUnlocked are NOT NULL columns declared '= null!' with no default, and the GetScheduleDto->Player mapping (unlike Join/Reload) has no null-guards, so a missing field maps null into a NOT NULL column and the INSERT fails.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [x] #1 New Player entities default the NOT NULL string columns (CountryCode, Notification, Timezone, HeroStyle, EngineStyle, WheelStyle, MaxGroupIdUnlocked) to empty string so a create with omitted fields does not violate NOT NULL
- [x] #2 GetScheduleDto->Player mapping guards each NOT NULL string member with Condition(src => value != null), matching the Join/Reload maps, so a null DTO value does not overwrite the default/existing value
- [x] #3 Creating a player via schedule/join/reload with country_code omitted no longer throws DbUpdateException
- [x] #4 Unit test covers player creation with omitted NOT NULL string fields
<!-- AC:END -->

## Implementation Notes

<!-- SECTION:NOTES:BEGIN -->
Fix on branch fix/player-notnull-defaults (off main; independent of the ghost PR). Changes: (1) Player entity NOT NULL string columns (Notification, CountryCode, Timezone, HeroStyle, EngineStyle, WheelStyle, MaxGroupIdUnlocked) now default to string.Empty instead of null! — pure CLR default, no migration needed. (2) GetScheduleDto->Player map now guards all 7 NOT NULL string members with Condition(!= null) so an omitted field keeps the default (create) / persisted value (update). (3) ReloadTournamentDto->Player map: added HeroStyle/EngineStyle/WheelStyle guards (its style fields are nullable; other NOT NULL fields were already guarded). Join map needed no change (styles are 'required', rest already guarded). Added 2 mapping tests asserting omitted fields map to non-null. Verified: dotnet build 0 errors, 20/20 tests pass. AC #3 verified via mapping tests as proxy (the crash was a null reaching a NOT NULL column; mapped values are now non-null).
<!-- SECTION:NOTES:END -->

## Final Summary

<!-- SECTION:FINAL_SUMMARY:BEGIN -->
Fixed DbUpdateException 'Column country_code cannot be null' on player creation. Player's NOT NULL string columns now default to empty string, and the schedule/reload->Player AutoMapper maps guard those members against null so an omitted client field no longer maps null into a NOT NULL column. Added mapping tests for omitted fields; 20/20 tests pass, build clean. No DB migration required.
<!-- SECTION:FINAL_SUMMARY:END -->
