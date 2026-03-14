# Fantasy Sports API

## Introduction

The Fantasy Sports APIs provide URIs used to access fantasy sports data. Currently the APIs support retrieval of Fantasy Football, Baseball, Basketball, and Hockey data including game, league, team, and player information. The APIs are based on a RESTful model — resources like game, league, team, and player form the building blocks, each identified by a resource ID, while collections are identified by their scope specified in the URI.

Yahoo! historically provided two full draft and trade style games (free and plus). With the 2010 seasons, both versions merged. Each game is comprised of many leagues (typically 8–12 teams), where players are assigned via draft at the start of the season. Undrafted players are available via free-agent or waiver wire transactions. Teams compete based on real-world statistics (touchdowns, yards, batting average, ERA, etc.), and league rules such as roster positions, scoring categories, and scoring modifiers are all configurable.

Much fantasy data is only meaningful in the context of a specific league. For example, three rushing touchdowns are irrelevant in a league that only scores defensive players. Many leagues are private — their data is only accessible to members.

---


## Resources & Collections

### Game Resource

**Description:** Represents a fantasy sports game (e.g., NFL Football, MLB Baseball). Contains metadata such as game name, code, type, URL, and season.

**HTTP Operations:** GET

**URI:** `https://fantasysports.yahooapis.com/fantasy/v2/game/<game_key>`

**Game Key Format:** `<game_id>` or `<game_code>` (e.g., `nfl`, `223`)

> **Note:** If a `game_code` is used, it is translated to the corresponding `game_id` in the response.

**Default sub-resource:** metadata

**Example URI:** `https://fantasysports.yahooapis.com/fantasy/v2/game/nfl`

**Example response fields:** `game_key`, `game_id`, `name`, `code`, `type`, `url`, `season`

---

### Games Collection

**Description:** Retrieve information from multiple games simultaneously.

**HTTP Operations:** GET

**URI patterns:**
```
/games/{sub_resource}
/games;game_keys={key1},{key2}/{sub_resource}
/games;out={sub_resource_1},{sub_resource_2}
```

**Filters:**

| Parameter | Values | Example |
|-----------|--------|---------|
| `is_available` | `1` (in-season only) | `/games;is_available=1` |
| `game_types` | `full`, `pickem-team`, `pickem-group`, `pickem-team-list` | `/games;game_types=full,pickem-team` |
| `game_codes` | Any valid game codes | `/games;game_codes=nfl,mlb` |
| `seasons` | Any valid seasons | `/games;seasons=2011,2012` |

---

### League Resource

**Description:** Leagues exist within a game and contain teams managed by users. Provides info like league name, number of teams, draft status, scoring type, and current/end week. Private league data is only accessible to members.

**HTTP Operations:** GET

**URI:** `https://fantasysports.yahooapis.com/fantasy/v2/league/<league_key>`

**Sub-resource URI:** `https://fantasysports.yahooapis.com/fantasy/v2/league/<league_key>/<sub_resource>`

**Multi sub-resource URI:** `https://fantasysports.yahooapis.com/fantasy/v2/league/<league_key>;out=<sub1>,<sub2>`

**League Key Format:** `<game_key>.l.<league_id>` (e.g., `pnfl.l.431` or `223.l.431`)

> **Note:** The separator between game key and league ID is a lowercase letter `l`, not the number `1`.

**Default sub-resource:** metadata

**Available sub-resources include:** `settings`, `standings`, `scoreboard`

**Settings sub-resource fields include:**
- `draft_type`, `scoring_type`, `uses_playoff`, `playoff_start_week`
- `uses_faab`, `trade_end_date`, `trade_ratify_type`
- `roster_positions` — list of positions with counts (e.g., QB×1, WR×3, RB×2, TE×1, K×1, DEF×1, BN×4)
- `stat_categories` — list of stats with `stat_id`, `enabled`, name, `display_name`, `sort_order`, `position_type`
- `stat_modifiers` — point values per stat (e.g., Pass Yds: 0.04, Pass TD: 4, Rush TD: 6, FG 50+: 5)
- `divisions`

**Example stat categories (NFL):**

| stat_id | Name | Display | Pos Type |
|---------|------|---------|----------|
| 4 | Passing Yards | Pass Yds | O |
| 5 | Passing Touchdowns | Pass TD | O |
| 6 | Interceptions | Int | O |
| 9 | Rushing Yards | Rush Yds | O |
| 10 | Rushing Touchdowns | Rush TD | O |
| 11 | Receptions | Rec | O |
| 12 | Reception Yards | Rec Yds | O |
| 13 | Reception Touchdowns | Rec TD | O |
| 15 | Return Touchdowns | Ret TD | O |
| 16 | 2-Point Conversions | 2-PT | O |
| 18 | Fumbles Lost | Fum Lost | O |
| 57 | Offensive Fumble Return TD | Fum Ret TD | O |
| 19–23 | Field Goals (by range) | FG 0-19 … FG 50+ | K |
| 24–25 | FG Missed (0-19, 20-29) | FGM 0-19, FGM 20-29 | K |
| 29 | PAT Made | PAT Made | K |
| 30 | PAT Missed | PAT Miss | K |
| 31 | Points Allowed | Pts Allow | DT (display only) |
| 32–37 | Sack, Int, Fum Rec, TD, Safety, Blk Kick | — | DT |
| 50–56 | Points Allowed (range buckets: 0, 1-6, 7-13, 14-20, 21-27, 28-34, 35+) | — | DT |

**Standings sub-resource** returns team standings including `team_key`, `team_id`, name, `division_id`, `faab_balance`, `clinched_playoffs`, and manager details.

**Scoreboard sub-resource** returns matchup data per week including `week`, `status`, `is_tied`, `winner_team_key`, and per-team `team_points` and `team_projected_points`.

---

### Leagues Collection

**Description:** Retrieve information from multiple leagues simultaneously.

**HTTP Operations:** GET

**URI patterns:**
```
/leagues/{sub_resource}
/leagues;league_keys={key1},{key2}/{sub_resource}
/leagues;out={sub_resource_1},{sub_resource_2}
/leagues;league_keys={key1},{key2};out={sub_resource_1},{sub_resource_2}
```

---

### Team Resource

**Description:** The team is the basic unit for tracking rosters. Teams are managed by one or (optionally) two managers. Provides info like team name, managers, logos, stats, points, and rosters. Teams only exist in a league context. Private league data is only accessible to members.

**HTTP Operations:** GET

**URI:** `https://fantasysports.yahooapis.com/fantasy/v2/team/<team_key>`

**Sub-resource URI:** `https://fantasysports.yahooapis.com/fantasy/v2/team/<team_key>/<sub_resource>`

**Team Key Format:** `<game_key>.l.<league_id>.t.<team_id>` (e.g., `pnfl.l.431.t.1` or `223.l.431.t.1`)

**Default sub-resource:** metadata

**Example metadata fields:** `team_key`, `team_id`, name, `url`, `team_logos`, `division_id`, `faab_balance`, managers (with `manager_id`, `nickname`, `guid`)

**Matchups sub-resource** (`/matchups;weeks=1,5`) returns per-week matchup details including `week`, `status`, `is_tied`, `winner_team_key`, and per-team points and projected points.

**Roster sub-resource** returns the team's player list for a given date with each player's `player_key`, `player_id`, full name, team, position, `display_position`, `image_url`, `is_undroppable`, `eligible_positions`, and `selected_position`.

#### Roster PUT

Modify player positions on the roster for a specific day or week.

**URL:** `https://fantasysports.yahooapis.com/fantasy/v2/team/<team_key>/roster`

Any players not listed retain their current positions. Invalid moves result in an error with no changes applied.

**NFL (week-based):**
```xml
<fantasy_content>
  <roster>
    <coverage_type>week</coverage_type>
    <week>13</week>
    <players>
      <player><player_key>242.p.8332</player_key><position>WR</position></player>
      <player><player_key>242.p.1423</player_key><position>BN</position></player>
    </players>
  </roster>
</fantasy_content>
```

**MLB/NBA/NHL (date-based):**
```xml
<fantasy_content>
  <roster>
    <coverage_type>date</coverage_type>
    <date>2011-05-01</date>
    <players>
      <player><player_key>253.p.8332</player_key><position>1B</position></player>
      <player><player_key>253.p.1423</player_key><position>BN</position></player>
    </players>
  </roster>
</fantasy_content>
```

---

### Teams Collection

**Description:** Retrieve information from multiple teams simultaneously. Qualified by a particular league (to get league teams) or by a user and game (to get a user's teams).

**HTTP Operations:** GET

**URI patterns:**
```
/teams/{sub_resource}
/teams;team_keys={key1},{key2}/{sub_resource}
/teams;out={sub_resource_1},{sub_resource_2}
/teams;team_keys={key1},{key2};out={sub_resource_1},{sub_resource_2}
```

---

### Player Resource

**Description:** Represents a professional athlete in the context of a particular game. Provides name, professional team, eligible positions, and stats.

**HTTP Operations:** GET

**URI:** `https://fantasysports.yahooapis.com/fantasy/v2/player/<player_key>`

**Sub-resource URI:** `https://fantasysports.yahooapis.com/fantasy/v2/player/<player_key>/<sub_resource>`

**Player Key Format:** `<game_key>.p.<player_id>` (e.g., `pnfl.p.5479` or `223.p.5479`)

**Default sub-resource:** metadata

**Example metadata fields:** `player_key`, `player_id`, full/first/last name, `status`, `editorial_team_key`, `editorial_team_full_name`, `editorial_team_abbr`, `bye_weeks`, `uniform_number`, `display_position`, `image_url`, `is_undroppable`, `position_type`, `eligible_positions`

**Stats sub-resource** (`/stats`) returns `player_stats` with `coverage_type`, `season`, and a list of `stat_id`/`value` pairs, plus `player_points` with a season total.

---

### Players Collection

**Description:** Retrieve information from multiple players simultaneously. Can be qualified by game, league, or team. League and team context enables additional sub-resources and filters.

**HTTP Operations:** GET

**URI patterns:**
```
/players/{sub_resource}
/players;player_keys={key1},{key2}/{sub_resource}
/players;out={sub_resource_1},{sub_resource_2}
/players;player_keys={key1},{key2};out={sub_resource_1},{sub_resource_2}
```

**Filters** (league context only):

| Parameter | Values | Example |
|-----------|--------|---------|
| `position` | Valid player positions | `/players;position=QB` |
| `status` | `A` (all available), `FA` (free agents), `W` (waivers), `T` (taken), `K` (keepers) | `/players;status=A` |
| `search` | Player name | `/players;search=smith` |
| `sort` | `{stat_id}`, `NAME`, `OR`, `AR`, `PTS` | `/players;sort=60` |
| `sort_type` | `season`, `date` (non-NFL), `week` (NFL), `lastweek` (non-NFL), `lastmonth` | `/players;sort_type=season` |
| `sort_season` | Year | `/players;sort_type=season;sort_season=2010` |
| `sort_date` | `YYYY-MM-DD` (non-NFL) | `/players;sort_type=date;sort_date=2010-02-01` |
| `sort_week` | Week number (NFL) | `/players;sort_type=week;sort_week=10` |
| `start` | Integer ≥ 0 | `/players;start=25` |
| `count` | Integer > 0 | `/players;count=5` |

---

### Transaction Resource

**Description:** Represents a transaction (add, drop, trade, or league settings change) within a league. Can be requested directly by transaction key, or discovered via the Transactions collection. Supports GET, PUT (edit waivers/trades), and DELETE (cancel pending transactions).

> **Note:** Pending waiver claims and pending trades are only visible to the relevant teams. They will not appear in a general league transaction list — you must filter by type (`waiver` or `pending_trade`) and by `team_key`.

**HTTP Operations:** GET, PUT, DELETE

**URI:** `https://fantasysports.yahooapis.com/fantasy/v2/transaction/<transaction_key>`

**Transaction Key Formats:**
- Completed: `<game_key>.l.<league_id>.tr.<transaction_id>` (e.g., `223.l.431.tr.26`)
- Waiver claim: `<game_key>.l.<league_id>.w.c.<claim_id>` (e.g., `257.l.193.w.c.2_6390`)
- Pending trade: `<game_key>.l.<league_id>.pt.<pending_trade_id>` (e.g., `257.l.193.pt.1`)

**Default sub-resources:** metadata, players

**Example response fields (completed add/drop):** `transaction_key`, `transaction_id`, `type` (e.g., `add/drop`), `status` (e.g., `successful`), `timestamp`, players with `transaction_data` (type, source_type, destination_type, source/destination team keys)

**Example response fields (waiver claim):** `type: waiver`, `status: pending`, `waiver_player_key`, `waiver_team_key`, `waiver_date`, `waiver_priority`

**Example response fields (pending trade):** `type: pending_trade`, `status: proposed`, `trader_team_key`, `tradee_team_key`, `trade_proposed_time`, `trade_note`, players with source/destination team keys

#### Transaction PUT

**URL:** `https://fantasysports.yahooapis.com/fantasy/v2/transaction/<transaction_key>`

Only `waiver` and `pending_trade` transactions can be PUTted.

**Edit waiver priority / FAAB bid:**
```xml
<fantasy_content>
  <transaction>
    <transaction_key>248.l.55438.w.c.2_6093</transaction_key>
    <type>waiver</type>
    <waiver_priority>1</waiver_priority>
    <faab_bid>20</faab_bid>
  </transaction>
</fantasy_content>
```

**Accept trade:** `<action>accept</action>` (with optional `<trade_note>`)

**Reject trade:** `<action>reject</action>` (with optional `<trade_note>`)

**Allow trade** (commissioner): `<action>allow</action>`

**Disallow trade** (commissioner): `<action>disallow</action>`

**Vote against trade** (manager): `<action>vote_against</action>` + `<voter_team_key>...</voter_team_key>`

#### Transaction DELETE

**URL:** `https://fantasysports.yahooapis.com/fantasy/v2/transaction/<transaction_key>`

Cancels a pending waiver claim or a proposed trade (if not yet accepted). Only `waiver` and unaccepted `pending_trade` transactions can be deleted.

---

### Transactions Collection

**Description:** Retrieve multiple transactions simultaneously within a league context. Also supports POST for add/drop operations and trade proposals.

**HTTP Operations:** GET, POST

**URI patterns:**
```
/transactions/{sub_resource}
/transactions;transaction_keys={key1},{key2}/{sub_resource}
/transactions;out={sub_resource_1},{sub_resource_2}
/transactions;transaction_keys={key1},{key2};out={sub_resource_1},{sub_resource_2}
```

**Filters:**

| Parameter | Values | Example |
|-----------|--------|---------|
| `type` | `add`, `drop`, `commish`, `trade` | `/transactions;type=add` |
| `types` | Any valid types | `/transactions;types=add,trade` |
| `team_key` | A team_key within the league | `/transactions;team_key=257.l.193.t.1` |
| `type` with `team_key` | `waiver`, `pending_trade` | `/transactions;team_key=257.l.193.t.1;type=waiver` |
| `count` | Integer > 0 | `/transactions;count=5` |

#### Transactions POST

**URL:** `https://fantasysports.yahooapis.com/fantasy/v2/league/<league_key>/transactions`

**Add a player:**
```xml
<fantasy_content>
  <transaction>
    <type>add</type>
    <player>
      <player_key>{player_key}</player_key>
      <transaction_data>
        <type>add</type>
        <destination_team_key>{team_key}</destination_team_key>
      </transaction_data>
    </player>
  </transaction>
</fantasy_content>
```

**Drop a player:**
```xml
<fantasy_content>
  <transaction>
    <type>drop</type>
    <player>
      <player_key>{player_key}</player_key>
      <transaction_data>
        <type>drop</type>
        <source_team_key>{team_key}</source_team_key>
      </transaction_data>
    </player>
  </transaction>
</fantasy_content>
```

**Add/Drop (replace one player with another):**
```xml
<fantasy_content>
  <transaction>
    <type>add/drop</type>
    <players>
      <player>
        <player_key>{player_key_to_add}</player_key>
        <transaction_data><type>add</type><destination_team_key>{team_key}</destination_team_key></transaction_data>
      </player>
      <player>
        <player_key>{player_key_to_drop}</player_key>
        <transaction_data><type>drop</type><source_team_key>{team_key}</source_team_key></transaction_data>
      </player>
    </players>
  </transaction>
</fantasy_content>
```

**Waiver claim (FAAB leagues):** Add `<faab_bid>25</faab_bid>` as a sibling of `<type>` in the above add/drop structure. Players on waivers will not be immediately added — the claim is processed according to league rules. Multiple teams may compete for the same player. Once a waiver claim exists, you can edit the waiver priority, FAAB bid, or cancel it entirely.

**Propose a trade:**
```xml
<fantasy_content>
  <transaction>
    <type>pending_trade</type>
    <trader_team_key>248.l.55438.t.11</trader_team_key>
    <tradee_team_key>248.l.55438.t.4</tradee_team_key>
    <trade_note>Optional note</trade_note>
    <players>
      <player>
        <player_key>{player_key}</player_key>
        <transaction_data>
          <type>pending_trade</type>
          <source_team_key>248.l.55438.t.11</source_team_key>
          <destination_team_key>248.l.55438.t.4</destination_team_key>
        </transaction_data>
      </player>
      <player>
        <player_key>{player_key}</player_key>
        <transaction_data>
          <type>pending_trade</type>
          <source_team_key>248.l.55438.t.4</source_team_key>
          <destination_team_key>248.l.55438.t.11</destination_team_key>
        </transaction_data>
      </player>
    </players>
  </transaction>
</fantasy_content>
```

Once a pending trade exists, it can be accepted, rejected, allowed/disallowed (commissioner), or voted against (depending on league role). It can also be cancelled.

---

### User Resource

**Description:** Retrieve fantasy information for a particular Yahoo! user, including which games they're playing, which leagues they belong to, and which teams they own. Currently only supports retrieving info for the logged-in user.

**HTTP Operations:** GET

> **Recommendation:** Use the Users collection with the `use_login` flag instead of requesting a User resource directly.

**Default sub-resource:** N/A

---

### Users Collection

**Description:** Retrieve information from a collection of users simultaneously. Each element is a User Resource.

**HTTP Operations:** GET

**URI patterns:**
```
/users;use_login=1/{sub_resource}
/users;use_login=1;out={sub_resource_1},{sub_resource_2}
/users;field={field_name1},{field_name2}
```
