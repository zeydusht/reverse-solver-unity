-- 005: playtest analysis (Faz 3 with the redesigned set), frozen before the data comes in
-- (PRODUCT.md, P1 karar 1–6, 2026-10-14 gece (3)).
--
-- TASLAK. Supabase'de çalıştırılmadı; hiçbir tabloyu değiştirmez (yalnızca SELECT).
-- Nasıl çalıştırılır: Supabase → SQL Editor → bir sorguyu yapıştır → Run.
-- Önce playtest başlangıç zamanını (2099-01-01 yerine) yaz; 003'teki F4 ile aynı.
--
-- Rules (003's shared filter F1–F5 applies; here only the redesigned set's rows):
--   * level_id starts with 'R' (R01 … R40), client = 'unity-web', TEST_ names out (F1)
--   * a "completed game" is a player's attempt = 1 row on a level (P1 karar 3); quits apart
--   * primary analysis: levels with n >= 20 completed games; 10–19 secondary; < 10 out
--   * difficulty is compared on booster-free first attempts only (b_* = 0) (P1 karar 6, M-K4)
--   * fun: the 👍/👎 answer (keyif) rides on the attempt that was asked: the won attempt or the
--     third lost one (P1 karar 4); result tells which

-- 1. Per level: completed games, first-try win (booster-free and all), quits. -------------
with pt as (
  select p.* from plays p
  where p.player not ilike 'TEST\_%'
    and p.client = 'unity-web'
    and p.level_id like 'R%'
    and p.created_at >= timestamptz '2099-01-01 00:00+03'          -- playtest start
),
first_try as (
  select * from pt where attempt = 1 and result <> 'quit'
)
select level_id, level_version,
       count(*)                                                                    as completed,
       case when count(*) >= 20 then 'birincil' when count(*) >= 10 then 'ikincil' else 'dışarıda' end as analiz,
       round(avg((result = 'win')::int) * 100, 1)                                  as first_win_pct,
       count(*) filter (where b_scissors + b_wand + b_hammer + b_clock = 0)       as completed_no_booster,
       round(avg((result = 'win')::int) filter (where b_scissors + b_wand + b_hammer + b_clock = 0) * 100, 1)
                                                                                   as first_win_no_booster_pct,
       (select count(*) from pt q where q.level_id = f.level_id and q.result = 'quit') as quits
from first_try f
group by level_id, level_version
order by level_id, level_version;

-- 2. Fun: 👍 share per level, split by when it was asked (after a win / after the 3rd loss). --
with pt as (
  select p.* from plays p
  where p.player not ilike 'TEST\_%'
    and p.client = 'unity-web'
    and p.level_id like 'R%'
    and p.created_at >= timestamptz '2099-01-01 00:00+03'
)
select level_id, level_version,
       count(*) filter (where keyif is not null)                                  as answers,
       round(avg((keyif = 1)::int) filter (where keyif is not null) * 100, 1)       as thumbs_up_pct,
       count(*) filter (where keyif is not null and result = 'win')               as answers_after_win,
       round(avg((keyif = 1)::int) filter (where keyif is not null and result = 'win') * 100, 1)  as up_after_win_pct,
       count(*) filter (where keyif is not null and result <> 'win')              as answers_after_losses,
       round(avg((keyif = 1)::int) filter (where keyif is not null and result <> 'win') * 100, 1) as up_after_losses_pct
from pt
group by level_id, level_version
order by level_id, level_version;

-- 3. Players: how far each got (for the "n >= 20 per level" count and the one-week rule). ---
with pt as (
  select p.* from plays p
  where p.player not ilike 'TEST\_%'
    and p.client = 'unity-web'
    and p.level_id like 'R%'
    and p.created_at >= timestamptz '2099-01-01 00:00+03'
)
select count(distinct player)                                                       as players,
       count(distinct level_id) filter (where attempt = 1)                          as levels_reached,
       (select count(*) from (select level_id from pt where attempt = 1 and result <> 'quit'
                              group by level_id having count(*) >= 20) x)          as levels_with_n20
from pt;

-- The comparison with the factory's estimates (Spearman ρ, mean difference d, per genre) is done
-- outside SQL, joining these rows with redesign.json's "fabrika" scores (ilkDeneme, keyif) by id.
-- Success (frozen): difficulty ρ >= 0.70 on booster-free first tries; secondary |d| <= 10 points.
-- Fun has no threshold: reported only (Spearman, mean difference, per-genre calibration).
