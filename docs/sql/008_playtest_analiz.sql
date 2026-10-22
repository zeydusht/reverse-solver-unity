-- 008: Build 5 playtest analysis (SET40 v3), per level. DO NOT RUN — PM runs it.
-- Excludes test players (ddd, TEST_*). Set the playtest start date below.
-- Columns: lv = slot no, dur = seconds played, keyif = 👍(1)/👎(-1), zorluk_puani = 1–10.

with pt as (
  select p.* from plays p
  where p.player not ilike 'TEST\_%'
    and p.player <> 'ddd'
    and p.client = 'unity-web'
    and p.created_at >= timestamptz '2099-01-01 00:00+03'          -- playtest start
),
first_try as (
  select * from pt where attempt = 1 and result <> 'quit'
),
last_level as (                                                   -- drop-off: each player's last played slot
  select distinct on (player) player, lv
  from pt order by player, created_at desc
)
select f.lv, f.level_id,
       count(*)                                                                    as first_tries,
       round(avg((f.result = 'win')::int) * 100, 1)                                as first_win_pct,
       percentile_cont(0.5) within group (order by f.dur)                          as median_dur_s,
       (select percentile_cont(0.5) within group (order by q.zorluk_puani)
          from pt q where q.lv = f.lv and q.zorluk_puani is not null)              as median_score,
       (select round(avg((q.keyif = 1)::int) * 100, 1)
          from pt q where q.lv = f.lv and q.keyif is not null)                     as thumbs_up_pct,
       (select round(avg((q.b_scissors + q.b_wand + q.b_hammer + q.b_clock > 0)::int) * 100, 1)
          from pt q where q.lv = f.lv)                                             as booster_use_pct,
       (select count(*) from last_level l where l.lv = f.lv)                       as players_stopped_here
from first_try f
group by f.lv, f.level_id
order by f.lv;
