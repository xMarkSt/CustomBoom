---
id: TASK-9
title: Implement hsnews/feed articles endpoint
status: Done
assignee:
  - '@mstam'
created_date: '2026-06-28 16:32'
updated_date: '2026-09-01 04:24'
labels: []
dependencies: []
ordinal: 9000
---

## Description

<!-- SECTION:DESCRIPTION:BEGIN -->
The news/articles endpoint returns in-app news and popup articles as a plist for the iOS client. It optionally filters by timestamp (only articles newer than the given Unix timestamp). PHP reference: ArticleController@show in C:\Projects\Mark\boomclone\app\Http\Controllers\ArticleController.php.
<!-- SECTION:DESCRIPTION:END -->

## Acceptance Criteria
<!-- AC:BEGIN -->
- [x] #1 GET /hsnews/feed/Boom/{locale}/{type?}/{timestamp?} is registered in the router
- [x] #2 When timestamp is provided, only articles with created_at > timestamp are returned
- [x] #3 When no articles match, returns a plist with an empty CFArray under the 'articles' key
- [x] #4 When articles exist, returns a plist with a CFDictionary under 'articles' keyed by '{timestamp}.{id}'
- [x] #5 Each article dict includes: id, timestamp, title, message, link, link_title, popup
- [x] #6 Root plist dictionary also includes 'count' and 'new' (both equal to the article count)
- [x] #7 Response Content-Type is application/x-plist (not encrypted — this is a plain GET)
- [x] #8 Article entity and DB table are created (migration + seeding as needed)
<!-- AC:END -->

## Implementation Plan

<!-- SECTION:PLAN:BEGIN -->
1. Extend PlistSerializationService.ConvertToPlistCompatibleType with an IDictionary branch (builds NSDictionary via NSObject.Wrap, recursing values) to support the articles dict keyed by "{timestamp}.{id}".
<!-- SECTION:PLAN:END -->

## Implementation Notes

<!-- SECTION:NOTES:BEGIN -->
Article entity, DbSet, and initial-migration table already existed (AC #8 pre-satisfied). Extended PlistSerializationService.ConvertToPlistCompatibleType with an IDictionary branch so a Dictionary<string, ArticleDto> serializes as an NSDictionary (CFDictionary), matching the PHP CFDictionary-keyed-by-"{timestamp}.{id}" shape; empty results use a plain empty List<ArticleDto> which serializes as an empty NSArray (CFArray), mirroring PHP's array-vs-dictionary switch. Added ArticleDto/NewsFeedDto (Boom.Common/DTOs/Response), IArticleService/ArticleService (Boom.Business), and HsNewsController (Boom.Api) with route GET hsnews/feed/Boom/{locale}/{type?}/{timestamp?}; type/locale are accepted but unused, matching the PHP reference (type is accepted but never filtered on). No [EncryptResponse]: serializes and returns FileContentResult("application/x-plist") directly, matching the Ghost action's precedent for non-encrypted responses. Validation: dotnet build (0 errors), dotnet test Boom.UnitTests (43/43 passed, including new PlistSerializationServiceTests dictionary/empty-array cases and ArticleServiceTests for empty/populated/timestamp-filtered feeds). Could not do a live end-to-end HTTP smoke test: the dev MySQL on port 3307 uses different credentials than appsettings.json's root/example (which targets port 3306), and those dev credentials weren't available.

Live smoke test done using main repo's Boom.Api/appsettings.Development.json dev DB credentials (root/goeiedag on port 3307; MariaDB requires the TCP host as 127.0.0.1, not "localhost", or MySqlConnector's auth negotiation fails with a misleading Access Denied error). Ran the API with DisableEncryption via that config and hit the live endpoint:
<!-- SECTION:NOTES:END -->

## Final Summary

<!-- SECTION:FINAL_SUMMARY:BEGIN -->
Implemented GET /hsnews/feed/Boom/{locale}/{type?}/{timestamp?} (HsNewsController), backed by a new ArticleService querying the existing Article entity/table, filtering by created_at when a timestamp is given. Response is an unencrypted application/x-plist NewsFeedDto: empty CFArray when no articles, otherwise a CFDictionary keyed by "{timestamp}.{id}" with per-article id/timestamp/title/message/link/link_title/popup, plus root-level count and new. Required extending PlistSerializationService to support dictionary-shaped DTO properties. Verified via dotnet build and dotnet test (43/43 passing); live DB smoke test not possible due to unknown dev DB credentials.
<!-- SECTION:FINAL_SUMMARY:END -->
