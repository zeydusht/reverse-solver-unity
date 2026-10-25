-- 008: Build 5 playtest analysis (SET40 v3). DRAFT; PM runs it, not the agent.
-- Read-only. Replace the future start date before use; it intentionally returns no rows.
-- Requires plays fields from migrations 001/002/004/007 and the existing booster fields.
-- Only SET40 v3 levels (ids S..); classic/debug sets must not enter the Build 5 cohort.
-- attempt = 1 is the persisted start counter, NOT the first row after the date filter.
-- Quits are excluded from first-try outcomes. Retries are not renumbered as first tries.
-- Group by slot AND permanent id/version so changed/reordered levels never mix.
-- median_dur_s: completed first tries without clock (003/F3); all other boosters allowed.
-- median_score: optional 1-10 difficulty answer, NOT stars; unanswered = NULL.
-- thumbs_up_pct: answered rows only, across all attempts (005).
-- booster_use_pct: all recorded attempts including quits; counters are emitted as integers.
-- Last played means latest received created_at (offline uploads can reorder play time).
-- id breaks timestamp ties consistently; it does not establish actual play chronology.
-- players_stopped_here/last_players describe the last observed row, not proven abandonment.

with pt as (
  select p.* from public.plays p
  where p.player not ilike 'TEST\_%'
    and p.player <> 'ddd'
    and p.client = 'unity-web'
    and p.level_id like 'S%'                                     -- SET40 v3 ids are S.. (classic L.., ZeydBot Z..)
    and p.created_at >= timestamptz '2099-01-01 00:00+03' -- playtest start
),
last_level as (
  select distinct on (player) player, lv, level_id, level_version
  from pt
  order by player, created_at desc, id desc
),
last_counts as (
  select lv, level_id, level_version,
         count(*) as players_stopped_here,
         jsonb_agg(player order by player) as last_players
  from last_level
  group by lv, level_id, level_version
),
metrics as (
  select lv, level_id, level_version,
         count(*) filter (where attempt = 1 and result <> 'quit') as first_tries,
         round(avg((result = 'win')::int)
           filter (where attempt = 1 and result <> 'quit') * 100, 1) as first_win_pct,
         count(*) filter (where attempt = 1 and result <> 'quit'
           and b_scissors + b_wand + b_hammer + b_clock = 0) as first_tries_no_booster,
         round(avg((result = 'win')::int) filter (where attempt = 1 and result <> 'quit'
           and b_scissors + b_wand + b_hammer + b_clock = 0) * 100, 1) as first_win_no_booster_pct,
         percentile_cont(0.5) within group (order by dur)
           filter (where attempt = 1 and result <> 'quit' and b_clock = 0) as median_dur_s,
         percentile_cont(0.5) within group (order by zorluk_puani) as median_score,
         count(keyif) as fun_answers,
         round(avg((keyif = 1)::int) * 100, 1) as thumbs_up_pct,
         round(avg((b_scissors + b_wand + b_hammer + b_clock > 0)::int) * 100, 1) as booster_use_pct
  from pt
  group by lv, level_id, level_version
)
select m.*, coalesce(l.players_stopped_here, 0) as players_stopped_here,
       coalesce(l.last_players, '[]'::jsonb) as last_players
from metrics m
left join last_counts l using (lv, level_id, level_version)
order by m.lv, m.level_id, m.level_version;
