-- 003: read-only analysis queries for the playtest (PRODUCT.md, "Zorluk deneyi ilkeleri":
-- attempts, time, failure rate and quit rate per level, defined before the data comes in).
--
-- TASLAK. Supabase'de henüz çalıştırılmadı; hiçbir tabloyu değiştirmez (yalnızca SELECT).
-- Nasıl çalıştırılır: Supabase → SQL Editor → bir sorguyu yapıştır → Run.
-- En üstteki playtest başlangıç zamanını (playtest_start) playtest başlayınca güncelle.
--
-- Rules every query follows (PRODUCT.md 2026-10-07 gece, karar 6):
--   * rows whose player starts with TEST_ are never analysed;
--   * Unity rows (client = 'unity-web') from before the playtest start are left out;
--   * the web game's rows (client = 'web') are the earlier study, kept apart by client;
--   * a level is identified by level_id + level_version: a changed level is a new row group.
-- Columns of plays (2026-10-08): id, created_at, player, session, lv, attempt, result
--   (win | time | bomb | quit), dur, left_s, moves, jams, stars, load, b_scissors, b_wand,
--   b_hammer, b_clock, client, level_version, level_id, row_id.

-- 0. Every query below starts with the same rs_rows filter, so each runs on its own.
--    When the playtest starts, replace 2099-01-01 in all of them with its start time.

-- 1. Per level and version: attempts, players, outcomes. --------------------------------
with rs_rows as (   -- the analysed rows; set playtest_start when the playtest begins
  select p.* from plays p
  where p.player not ilike 'TEST\_%'
    and (p.client <> 'unity-web' or p.created_at >= timestamptz '2099-01-01 00:00+03')
)
select client, level_id, level_version,
       count(*)                                            as attempts,
       count(distinct player)                              as players,
       round(avg((result = 'win')::int)  * 100, 1)         as win_pct,
       round(avg((result = 'bomb')::int) * 100, 1)         as bomb_pct,
       round(avg((result = 'time')::int) * 100, 1)         as time_pct,
       round(avg((result = 'quit')::int) * 100, 1)         as quit_pct,
       round(avg(jams), 2)                                 as mean_jams
from rs_rows
group by client, level_id, level_version
order by client, level_id, level_version;

-- 2. Time to clear a level: seconds of winning attempts (median and quartiles). ---------
with rs_rows as (   -- the analysed rows; set playtest_start when the playtest begins
  select p.* from plays p
  where p.player not ilike 'TEST\_%'
    and (p.client <> 'unity-web' or p.created_at >= timestamptz '2099-01-01 00:00+03')
)
select client, level_id, level_version,
       count(*)                                                         as wins,
       percentile_cont(0.5)  within group (order by dur)                as median_s,
       percentile_cont(0.25) within group (order by dur)                as q1_s,
       percentile_cont(0.75) within group (order by dur)                as q3_s,
       round(avg(stars), 2)                                             as mean_stars
from rs_rows
where result = 'win'
group by client, level_id, level_version
order by client, level_id, level_version;

-- 3. Attempts until the first win, per player and level (the "deneme sayısı" metric). ---
with rs_rows as (   -- the analysed rows; set playtest_start when the playtest begins
  select p.* from plays p
  where p.player not ilike 'TEST\_%'
    and (p.client <> 'unity-web' or p.created_at >= timestamptz '2099-01-01 00:00+03')
),
firsts as (
  select client, player, level_id, level_version, min(attempt) as first_win_attempt
  from rs_rows
  where result = 'win'
  group by client, player, level_id, level_version
)
select client, level_id, level_version,
       count(*)                                                    as players_who_won,
       round(avg(first_win_attempt), 2)                            as mean_attempts_to_win,
       percentile_cont(0.5) within group (order by first_win_attempt) as median_attempts_to_win
from firsts
group by client, level_id, level_version
order by client, level_id, level_version;

-- 4. Drop-off: players who tried a level but never won it, and whose last row there is a quit. --
with rs_rows as (   -- the analysed rows; set playtest_start when the playtest begins
  select p.* from plays p
  where p.player not ilike 'TEST\_%'
    and (p.client <> 'unity-web' or p.created_at >= timestamptz '2099-01-01 00:00+03')
),
per_player as (
  select client, player, level_id, level_version,
         bool_or(result = 'win')                                   as won,
         (array_agg(result order by created_at desc))[1]           as last_result
  from rs_rows
  group by client, player, level_id, level_version
)
select client, level_id, level_version,
       count(*)                                                    as players,
       count(*) filter (where not won)                             as never_won,
       count(*) filter (where not won and last_result = 'quit')    as left_on_a_quit
from per_player
group by client, level_id, level_version
order by client, level_id, level_version;

-- 5. Booster use per level (who reached for help, and which). ---------------------------
with rs_rows as (   -- the analysed rows; set playtest_start when the playtest begins
  select p.* from plays p
  where p.player not ilike 'TEST\_%'
    and (p.client <> 'unity-web' or p.created_at >= timestamptz '2099-01-01 00:00+03')
)
select client, level_id, level_version,
       round(avg((b_scissors + b_wand + b_hammer + b_clock > 0)::int) * 100, 1) as attempts_with_booster_pct,
       sum(b_scissors) as scissors, sum(b_wand) as wand, sum(b_hammer) as hammer, sum(b_clock) as clock
from rs_rows
group by client, level_id, level_version
order by client, level_id, level_version;

-- 6. Data health: duplicates and test rows that slipped through (should all be 0). -----
select
  (select count(*) from plays where row_id is not null group by row_id having count(*) > 1 limit 1) as duplicated_row_id,
  (select count(*) from plays where client = 'unity-web' and created_at >= timestamptz '2099-01-01 00:00+03' and player ilike 'test%' and player not ilike 'TEST\_%')                                         as test_like_names_left,
  (select count(*) from plays where client = 'unity-web' and level_id is null)                    as unity_rows_without_level_id;
