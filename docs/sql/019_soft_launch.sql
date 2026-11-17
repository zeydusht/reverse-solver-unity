-- PM_81 §5: the soft launch (iOS, cohort sl1). NOT APPLIED: the PM applies it with Zeyd's approval.
-- The game (SlTelemetry, Core/Telemetry/SlEventRow) posts to sl_events?on_conflict=row_id; every row carries every
-- column (null when not used). Events:
--   first_open, session_start, session_end (seconds), att_status (value: authorized / denied / not_determined /
--   restricted / unavailable), ad_shown (placement, network, revenue USD from MAX OnAdRevenuePaidEvent), ad_reward,
--   ad_failed (value: the error), ad_skipped_not_ready, crash (value: message + 5 stack lines)
-- Placements: r1_sure (time-up +30 s), r2_altin (2x gold), r3_booster (+1 booster), r4_hediye (menu gift),
--   i1_gecis (interstitial).

create table if not exists public.sl_events (
  row_id text primary key,
  created_at timestamptz not null default now(),
  event text not null check (event in ('first_open','session_start','session_end','att_status','ad_shown','ad_reward',
                                       'ad_failed','ad_skipped_not_ready','crash')),
  player text,
  session text,
  cohort text,
  platform text,
  device_id text,
  app_version text,
  content_version text,
  placement text,
  network text,
  revenue numeric,
  value text,
  seconds integer,
  lv integer,
  level_id text
);

alter table public.sl_events enable row level security;
revoke all on public.sl_events from anon;
grant insert on public.sl_events to anon;
drop policy if exists sl_events_anon_insert on public.sl_events;
create policy sl_events_anon_insert on public.sl_events for insert to anon with check (true);

create index if not exists sl_events_created_idx on public.sl_events (created_at);
create index if not exists sl_events_device_idx on public.sl_events (device_id, created_at);
create index if not exists sl_events_event_idx on public.sl_events (event, created_at);

-- R1 paid with an ad or with gold (sl1 rows only send it)
alter table public.continue_events add column if not exists continue_type text check (continue_type in ('gold','ad'));

-- ---- views (Istanbul days; Zeyd's devices and test names out, as in 013: excluded_players / excluded_devices) ----

create or replace view public.v_sl_events with (security_invoker = true) as
select e.*, (e.created_at at time zone 'Europe/Istanbul')::date as gun
from public.sl_events e
where e.cohort = 'sl1'
  and coalesce(e.player, '') !~* '^(test|ic_)' and coalesce(e.player, '') !~* 'zeyd'
  and not exists (select 1 from public.excluded_players x where lower(x.player) = lower(btrim(coalesce(e.player, ''))))
  and not exists (select 1 from public.excluded_devices d where d.device_id = e.device_id);

-- the plays of the soft launch (v_attempts already drops the excluded players and devices)
create or replace view public.v_sl_attempts with (security_invoker = true) as
select a.*, (a.created_at at time zone 'Europe/Istanbul')::date as gun
from public.v_attempts a
where a.cohort = 'sl1';

-- installs: the first first_open of a device (its install day)
create or replace view public.v_sl_kurulum with (security_invoker = true) as
select device_id, min(gun) as kurulum_gunu, min(created_at) as kurulum
from public.v_sl_events
where event = 'first_open' and device_id is not null
group by device_id;

-- active days of a device: any event or play on that day
create or replace view public.v_sl_aktif with (security_invoker = true) as
select distinct device_id, gun from public.v_sl_events where device_id is not null
union
select distinct player_key as device_id, gun from public.v_sl_attempts;

-- D1 / D3 / D7 retention by install day (a day counts once it has passed)
create or replace view public.v_sl_tutma with (security_invoker = true) as
select k.kurulum_gunu,
  count(*) as kurulum,
  count(*) filter (where exists (select 1 from public.v_sl_aktif a where a.device_id = k.device_id and a.gun = k.kurulum_gunu + 1)) as d1,
  count(*) filter (where exists (select 1 from public.v_sl_aktif a where a.device_id = k.device_id and a.gun = k.kurulum_gunu + 3)) as d3,
  count(*) filter (where exists (select 1 from public.v_sl_aktif a where a.device_id = k.device_id and a.gun = k.kurulum_gunu + 7)) as d7,
  round(100.0 * count(*) filter (where exists (select 1 from public.v_sl_aktif a where a.device_id = k.device_id and a.gun = k.kurulum_gunu + 1)) / count(*), 1) as d1_yuzde,
  round(100.0 * count(*) filter (where exists (select 1 from public.v_sl_aktif a where a.device_id = k.device_id and a.gun = k.kurulum_gunu + 3)) / count(*), 1) as d3_yuzde,
  round(100.0 * count(*) filter (where exists (select 1 from public.v_sl_aktif a where a.device_id = k.device_id and a.gun = k.kurulum_gunu + 7)) / count(*), 1) as d7_yuzde
from public.v_sl_kurulum k
group by k.kurulum_gunu
order by k.kurulum_gunu;

-- the funnel by level: players who reached it (an attempt), won it, and whose last level it is (left there)
create or replace view public.v_sl_huni with (security_invoker = true) as
with per as (
  select player_key, slot, bool_or(result in ('win','won_after_continue')) as won
  from public.v_sl_attempts group by player_key, slot
), last as (
  select player_key, max(slot) as son from public.v_sl_attempts group by player_key
)
select p.slot as bolum,
  count(*) as ulasan,
  count(*) filter (where p.won) as kazanan,
  count(*) filter (where l.son = p.slot and not p.won) as birakan,
  round(100.0 * count(*) filter (where p.won) / count(*), 1) as kazanma_yuzde
from per p join last l using (player_key)
group by p.slot
order by p.slot;

-- ads by day: impressions, per DAU, rewarded watch rate (rewarded viewers / DAU), revenue
create or replace view public.v_sl_reklam with (security_invoker = true) as
with dau as (select gun, count(distinct device_id) as dau from public.v_sl_aktif group by gun)
select d.gun,
  d.dau,
  count(e.*) filter (where e.event = 'ad_shown') as gosterim,
  count(e.*) filter (where e.event = 'ad_shown' and e.placement = 'i1_gecis') as gecis_gosterim,
  count(e.*) filter (where e.event = 'ad_reward') as odul,
  round(1.0 * count(e.*) filter (where e.event = 'ad_shown') / nullif(d.dau, 0), 2) as dau_basina_gosterim,
  round(100.0 * count(distinct e.device_id) filter (where e.event = 'ad_reward') / nullif(d.dau, 0), 1) as odullu_izleme_yuzde,
  count(e.*) filter (where e.event = 'ad_skipped_not_ready') as hazir_degil,
  count(e.*) filter (where e.event = 'ad_failed') as hata,
  round(coalesce(sum(e.revenue) filter (where e.event = 'ad_shown'), 0), 4) as gelir_usd
from dau d left join public.v_sl_events e on e.gun = d.gun
group by d.gun, d.dau
order by d.gun;

-- the day: installs, DAU, sessions, minutes per DAU, crash-free sessions
create or replace view public.v_sl_gunluk with (security_invoker = true) as
with dau as (select gun, count(distinct device_id) as dau from public.v_sl_aktif group by gun),
     ev as (
       select gun,
         count(*) filter (where event = 'first_open') as kurulum,
         count(*) filter (where event = 'session_start') as oturum,
         coalesce(sum(seconds) filter (where event = 'session_end'), 0) as saniye,
         count(distinct session) filter (where event = 'crash') as cokme_oturum
       from public.v_sl_events group by gun)
select d.gun, coalesce(ev.kurulum, 0) as kurulum, d.dau, coalesce(ev.oturum, 0) as oturum,
  round(coalesce(ev.saniye, 0) / 60.0 / nullif(d.dau, 0), 1) as dk_dau,
  round(100.0 - 100.0 * coalesce(ev.cokme_oturum, 0) / nullif(ev.oturum, 0), 2) as cokmesiz_oturum_yuzde
from dau d left join ev using (gun)
order by d.gun;
