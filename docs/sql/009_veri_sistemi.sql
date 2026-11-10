-- 009_veri_sistemi.sql  (PM, 31 Ekim 2026) — kendi kendini besleyen zorluk sistemi, Supabase katmani
-- Plan: docs/VERI_SISTEMI.md. Uygulandi: Supabase MCP (apply_migration "009_veri_sistemi").
--
-- Ilke: oyuncu verisi (plays, moves, player_profiles) istemciden anon ile YAZILIR.
--       Model katmani (level_features, level_targets, model_runs, level_predictions, player_bots)
--       anon ile yalniz OKUNUR; yazma PM (service role / MCP) uzerinden. Anon anahtar web build'inde acik,
--       tahminler kurcalanamasin diye.

-- 1. Oyuncu verisine eklenenler (eski istemciler etkilenmez, hepsi bos gecebilir)
alter table public.plays add column if not exists cohort text;
alter table public.plays add column if not exists platform text;
alter table public.plays add column if not exists device_id text;
alter table public.plays add column if not exists app_version text;
comment on column public.plays.cohort is 'Test grubu: pt1 = 20 kisilik ilk oyun testi, ic = ekip ici. URL ?c=pt1. Bos = etiketsiz.';
comment on column public.plays.platform is 'ios | android | desktop | mobile-web: dokunma ile fare hiz farki icin.';
comment on column public.plays.device_id is 'Cihazdaki anonim uuid (localStorage / PlayerPrefs). Ayni isimli iki kisiyi ayirir.';

alter table public.moves add column if not exists cohort text;
alter table public.moves add column if not exists device_id text;
alter table public.moves drop constraint if exists moves_kind_check;
alter table public.moves add constraint moves_kind_check check (kind = any (array[
  'exit','jam','hammer','scissors','wand','clock','touch','pause','resume',
  'start','quit','hide','show']));
comment on column public.moves.kind is 'start = bolum acildi (yarida birakmayi olcmek icin), quit = menuye donus, hide/show = sekme/uygulama arka plana gitti/geri geldi.';

create index if not exists moves_row_id_idx on public.moves (row_id);
create index if not exists plays_cohort_idx on public.plays (cohort, level_id);

-- 2. Oyuncu profili (oyun basinda tek soru; istemci yazar)
create table if not exists public.player_profiles (
  id bigint generated always as identity primary key,
  created_at timestamptz not null default now(),
  cohort text not null,
  player text not null,
  device_id text,
  platform text,
  puzzle_exp smallint check (puzzle_exp is null or puzzle_exp between 0 and 2),
  unique (cohort, player, device_id)
);
comment on column public.player_profiles.puzzle_exp is '"Bu tur bulmaca oyunlari oynar misin?" 0 = hic, 1 = ara sira, 2 = sik. Botlari gruplamak icin.';

-- 3. Bolum ozellikleri (statik + simulasyon metrikleri)
create table if not exists public.level_features (
  level_id text not null,
  level_version int not null,
  set_name text not null,
  slot int,
  w int, h int, cells int,
  pieces int, chains int,
  bombs int, fuse_min int, fuse_mean numeric, fuse_slack_min int, fuse_slack_mean numeric,
  nails int, nail_max int,
  sealed_lanes int, sealed_sides int,
  timer_s int, sol_len int, sec_per_move numeric, open_edge_pct int,
  genre text, hard_card boolean, boosters int,
  sim jsonb not null default '{}'::jsonb,
  computed_at timestamptz not null default now(),
  primary key (level_id, level_version)
);
comment on column public.level_features.fuse_slack_min is 'Kayitli cozume gore en dar fitil payi: fitil - bomba cikana kadarki cikis sayisi.';
comment on column public.level_features.sim is 'FunSim/SmartPlayer metrikleri: k0, serbestlik, darbogaz, en kisa kazma yolu, rastgele kazanma vb.';

-- 4. Hedefler (tasarim niyeti: "nasil bir zorluk bekliyoruz")
create table if not exists public.level_targets (
  set_name text not null,
  slot int not null,
  level_id text not null,
  level_version int not null,
  target_rating_lo numeric, target_rating_hi numeric,
  target_win1_lo numeric, target_win1_hi numeric,
  main_fail text check (main_fail is null or main_fail in ('bomba','sure','yok')),
  role text,
  note text,
  decided_at timestamptz not null default now(),
  primary key (set_name, slot)
);

-- 5. Model kosulari ve on-kayit (dondurulan tahmin degistirilemez)
create table if not exists public.model_runs (
  run_id text primary key,
  created_at timestamptz not null default now(),
  frozen_at timestamptz,
  model text not null,
  set_name text not null,
  git_commit text,
  description text,
  assumptions jsonb not null default '{}'::jsonb,
  backtest jsonb not null default '{}'::jsonb
);

create table if not exists public.level_predictions (
  run_id text not null references public.model_runs(run_id),
  level_id text not null,
  level_version int not null,
  slot int,
  n_sims int,
  p_win1 numeric, p_win1_lo numeric, p_win1_hi numeric,
  p_fail_bomb numeric, p_fail_time numeric,
  mean_attempts numeric,
  mean_dur_s numeric, mean_left_frac numeric,
  mean_jams numeric,
  pred_rating numeric, pred_rating_lo numeric, pred_rating_hi numeric,
  detail jsonb not null default '{}'::jsonb,
  primary key (run_id, level_id, level_version)
);

create or replace function public.block_frozen_predictions() returns trigger
language plpgsql set search_path = public as $$
begin
  if exists (select 1 from public.model_runs r where r.run_id = coalesce(old.run_id, new.run_id) and r.frozen_at is not null) then
    raise exception 'Bu kosu donduruldu (on-kayit). Yeni run_id ile yeni kosu ac.';
  end if;
  return coalesce(new, old);
end $$;
drop trigger if exists level_predictions_frozen on public.level_predictions;
create trigger level_predictions_frozen before insert or update or delete on public.level_predictions
  for each row execute function public.block_frozen_predictions();

-- 6. Oyuncu botlari (her oyuncuya bir parametre seti + nufus dagilimi)
create table if not exists public.player_bots (
  run_id text not null references public.model_runs(run_id),
  bot_id text not null,
  player text,
  cohort text,
  kind text not null default 'clone' check (kind in ('clone','population')),
  n_moves int, n_attempts int,
  params jsonb not null,
  loglik numeric,
  created_at timestamptz not null default now(),
  primary key (run_id, bot_id)
);
comment on column public.player_bots.params is 'think_mu, think_sigma (log-normal dusunme), jam_rate, softmax_beta (dogru hamleye yonelim), bomb_priority, learn (deneme basina iyilesme), speed_platform vb.';

-- 7. RLS
alter table public.player_profiles enable row level security;
alter table public.level_features enable row level security;
alter table public.level_targets enable row level security;
alter table public.model_runs enable row level security;
alter table public.level_predictions enable row level security;
alter table public.player_bots enable row level security;
drop policy if exists player_profiles_anon_insert on public.player_profiles;
create policy player_profiles_anon_insert on public.player_profiles for insert to anon with check (true);
drop policy if exists player_profiles_anon_select on public.player_profiles;
create policy player_profiles_anon_select on public.player_profiles for select to anon using (true);
do $$ declare t text; begin
  foreach t in array array['level_features','level_targets','model_runs','level_predictions','player_bots'] loop
    execute format('drop policy if exists %I on public.%I', t || '_anon_select', t);
    execute format('create policy %I on public.%I for select to anon using (true)', t || '_anon_select', t);
  end loop; end $$;

-- 8. Gorunumler: "veri ne diyor"
-- 8a. Temiz deneme satirlari
create or replace view public.v_attempts with (security_invoker = true) as
select p.id, p.created_at, p.cohort, p.player, coalesce(p.device_id, p.player) as player_key, p.platform,
       p.level_id, p.level_version, p.lv as slot, p.attempt, p.result,
       p.dur, p.left_s, p.moves, p.jams, p.stars,
       (coalesce(p.b_scissors,0)+coalesce(p.b_wand,0)+coalesce(p.b_hammer,0)+coalesce(p.b_clock,0)) > 0 as used_booster,
       p.zorluk_puani, p.keyif, p.row_id
from public.plays p
where p.level_id is not null
  and p.player is not null
  and p.player !~* '^(test|ddd$|show$|<$)';

-- 8b. Bolum basina gercek sonuc (kohort bazinda). Ilk deneme = attempt 1.
create or replace view public.v_level_actual with (security_invoker = true) as
with a as (select * from public.v_attempts where cohort is not null),
first as (select * from a where attempt = 1),
agg as (
  select cohort, level_id, level_version,
         max(slot) as slot,
         count(*) as n_first,
         avg((result = 'win')::int)::numeric as win1,
         avg((result = 'bomb')::int)::numeric as fail_bomb1,
         avg((result = 'time')::int)::numeric as fail_time1,
         avg((result not in ('win','bomb','time'))::int)::numeric as other1,
         avg(dur) filter (where result = 'win')::numeric as mean_dur_win1,
         avg(jams)::numeric as mean_jams1,
         avg(moves)::numeric as mean_moves1
  from first group by 1,2,3),
tries as (
  select cohort, level_id, level_version, player_key,
         min(attempt) filter (where result = 'win') as win_at,
         max(zorluk_puani) filter (where zorluk_puani is not null) as rating,
         max(keyif) filter (where keyif is not null) as keyif
  from a group by 1,2,3,4),
t2 as (
  select cohort, level_id, level_version,
         count(*) as players,
         count(win_at) as players_won,
         avg(win_at)::numeric as mean_attempts_to_win,
         avg(rating)::numeric as mean_rating, count(rating) as n_rating,
         avg((keyif = 1)::int) filter (where keyif is not null)::numeric as keyif_up, count(keyif) as n_keyif
  from tries group by 1,2,3)
select agg.*, t2.players, t2.players_won, t2.mean_attempts_to_win, t2.mean_rating, t2.n_rating, t2.keyif_up, t2.n_keyif,
       -- Wilson %95 araligi (20 kisiyle aralik genis: tek bolumde +-%20 normal)
       ((win1 + 1.92/n_first) - 1.96*sqrt(win1*(1-win1)/n_first + 0.9604/(n_first*n_first))) / (1 + 3.8416/n_first) as win1_lo,
       ((win1 + 1.92/n_first) + 1.96*sqrt(win1*(1-win1)/n_first + 0.9604/(n_first*n_first))) / (1 + 3.8416/n_first) as win1_hi
from agg join t2 using (cohort, level_id, level_version);

-- 8c. Huni: kac oyuncu hangi bolume ulasti (birakma egrisi)
create or replace view public.v_funnel with (security_invoker = true) as
select cohort, slot, count(distinct player_key) as players_reached,
       count(distinct player_key) filter (where result = 'win') as players_won
from public.v_attempts where cohort is not null group by 1,2;

-- 8d. Yarida birakilan denemeler (start var, sonuc satiri yok)
create or replace view public.v_abandoned with (security_invoker = true) as
select m.cohort, m.player, m.level_id, m.level_version, m.attempt, m.row_id, max(m.created_at) as last_seen,
       max(m.move_no) as last_move_no
from public.moves m
where m.cohort is not null
  and not exists (select 1 from public.plays p where p.row_id = m.row_id)
group by 1,2,3,4,5,6
having max(m.created_at) < now() - interval '30 minutes';

-- 8e. Tahmin - hedef - gercek: "ne ayarladik, ne bekledik, veri ne diyor"
create or replace view public.v_pred_vs_actual with (security_invoker = true) as
select r.run_id, r.frozen_at, a.cohort,
       p.slot, p.level_id, p.level_version,
       t.role, t.target_win1_lo, t.target_win1_hi, t.target_rating_lo, t.target_rating_hi,
       p.p_win1, p.p_win1_lo, p.p_win1_hi, p.p_fail_bomb, p.p_fail_time, p.pred_rating,
       a.n_first, a.win1, a.win1_lo, a.win1_hi, a.fail_bomb1, a.fail_time1, a.mean_rating, a.n_rating, a.keyif_up,
       a.win1 - p.p_win1 as err_win1,
       case when a.n_first is null then 'veri yok'
            when a.n_first < 8 then 'veri az'
            when p.p_win1 between a.win1_lo and a.win1_hi then 'tahmin tuttu'
            when p.p_win1 > a.win1_hi then 'beklenenden ZOR'
            else 'beklenenden KOLAY' end as tahmin_sonucu,
       case when a.n_first is null or a.n_first < 8 or t.target_win1_lo is null then null
            when a.win1_hi < t.target_win1_lo then 'hedeften ZOR'
            when a.win1_lo > t.target_win1_hi then 'hedeften KOLAY'
            else 'hedefte' end as hedef_sonucu
from public.level_predictions p
join public.model_runs r using (run_id)
left join public.level_targets t on t.set_name = r.set_name and t.slot = p.slot
left join public.v_level_actual a on a.level_id = p.level_id and a.level_version = p.level_version;

-- 8f. Model karnesi (kosu x kohort): Brier, MAE, sira korelasyonu
create or replace view public.v_model_score with (security_invoker = true) as
with f as (
  select r.run_id, x.cohort, p.level_id, p.p_win1, (x.result = 'win')::int as won
  from public.v_attempts x
  join public.level_predictions p on p.level_id = x.level_id and p.level_version = x.level_version
  join public.model_runs r on r.run_id = p.run_id
  where x.attempt = 1 and x.cohort is not null),
lvl as (
  select run_id, cohort, level_id, avg(p_win1) as p, avg(won::numeric) as y, count(*) as n
  from f group by 1,2,3),
rk as (
  select *, rank() over (partition by run_id, cohort order by p) as rp,
            rank() over (partition by run_id, cohort order by y) as ry from lvl)
select f.run_id, f.cohort,
       count(*) as n_first_attempts,
       avg(power(f.p_win1 - f.won, 2)) as brier,
       avg(power(avg_win.base - f.won, 2)) as brier_baseline_mean,
       (select avg(abs(p - y)) from lvl where lvl.run_id = f.run_id and lvl.cohort = f.cohort and n >= 8) as mae_win1_n8,
       (select corr(rp, ry) from rk where rk.run_id = f.run_id and rk.cohort = f.cohort and n >= 8) as spearman_n8
from f
cross join lateral (select avg(won::numeric) as base from f f2 where f2.cohort = f.cohort and f2.run_id = f.run_id) avg_win
group by f.run_id, f.cohort;

grant select on public.v_attempts, public.v_level_actual, public.v_funnel, public.v_abandoned,
                public.v_pred_vs_actual, public.v_model_score to anon;
