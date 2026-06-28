---
id: TASK-9
title: Implement hsnews/feed articles endpoint
status: To Do
assignee: []
created_date: '2026-06-28 16:32'
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
- [ ] #1 GET /hsnews/feed/Boom/{locale}/{type?}/{timestamp?} is registered in the router
- [ ] #2 When timestamp is provided, only articles with created_at > timestamp are returned
- [ ] #3 When no articles match, returns a plist with an empty CFArray under the 'articles' key
- [ ] #4 When articles exist, returns a plist with a CFDictionary under 'articles' keyed by '{timestamp}.{id}'
- [ ] #5 Each article dict includes: id, timestamp, title, message, link, link_title, popup
- [ ] #6 Root plist dictionary also includes 'count' and 'new' (both equal to the article count)
- [ ] #7 Response Content-Type is application/x-plist (not encrypted — this is a plain GET)
- [ ] #8 Article entity and DB table are created (migration + seeding as needed)
<!-- AC:END -->
