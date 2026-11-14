-- 018_pm69_features_v2.sql (PM_69, 14 Kasım 2026): zorluk özellikleri v2 ve pt1 replay tabloları.
-- Yeni tablolar; level_features ve diğer tablolara dokunmaz. Okuma anon'a açık (diğer analiz tabloları gibi).
-- Yükleme: Tools/pm69_upload.js, geçici anon insert politikasıyla (yüklemeden sonra kaldırılır, aşağıda).

create table if not exists public.level_features_v2 (
  set_name text not null, level_id text not null, level_version integer not null, slot integer,
  w integer, h integer, pieces integer, locks integer, chains integer, walls integer, bombs integer,
  timer_s integer, sec_per_move numeric, open_edge_pct integer,
  fuse jsonb, slack_true jsonb, slack_true_min integer, slack_true_mean numeric, slack_approx boolean, choke_n integer,
  p_survive_rand numeric, nat_slack_med_rand numeric, first_choke_ex_rand numeric,
  p_survive_greedy numeric, nat_slack_med_greedy numeric, first_choke_ex_greedy numeric,
  p_survive_aware numeric, nat_slack_med_aware numeric, first_choke_ex_aware numeric,
  nat_slack_per_bomb_rand jsonb,
  open_min numeric, open_mean numeric, bottleneck_n numeric, distractor_ratio numeric,
  distract_per_step jsonb, distract_max numeric, distract_vs_slack jsonb, distract_vs_slack_max numeric,
  narrow_n numeric, narrow_hidden_n numeric,
  blocked_share_lock numeric, blocked_share_chain numeric, blocked_share_wall numeric, blocked_share_piece numeric, blocked_share_tab numeric,
  narrow_by_lock numeric, narrow_by_chain numeric, narrow_by_wall numeric,
  jam_bait_start numeric, jam_bait_mid numeric, dir_ambiguity numeric, calc_s numeric,
  computed_at timestamptz not null default now(),
  primary key (set_name, level_id, level_version)
);

create table if not exists public.replay_pt1 (
  row_id text primary key, player text, level_id text, level_version integer, attempt integer,
  result text, moves integer, replay_exits integer, replay_ok boolean, reason text, inferred text,
  off_path_exits integer, on_path_exits integer,
  jams integer, jam_lock integer, jam_chain integer, jam_wall integer, jam_piece integer, jam_tab integer, jam_free integer, jam_nodir integer,
  open_mean numeric, computed_at timestamptz not null default now()
);

create table if not exists public.replay_pt1_steps (
  row_id text not null, player text, level_id text, level_version integer, attempt integer, step integer not null,
  piece integer, open_n integer, on_path_n integer, narrow boolean, hidden boolean, limiters text,
  think_ms integer, dt_ms integer,
  jams integer, jam_lock integer, jam_chain integer, jam_wall integer, jam_piece integer, jam_tab integer, jam_free integer, jam_nodir integer,
  primary key (row_id, step)
);

alter table public.level_features_v2 enable row level security;
alter table public.replay_pt1 enable row level security;
alter table public.replay_pt1_steps enable row level security;
do $$ declare t text; begin
  foreach t in array array['level_features_v2','replay_pt1','replay_pt1_steps'] loop
    execute format('drop policy if exists %I on public.%I', t || '_anon_select', t);
    execute format('create policy %I on public.%I for select to anon using (true)', t || '_anon_select', t);
  end loop; end $$;

-- Geçici (yalnız yükleme sırasında):
--   create policy <t>_tmp_insert on public.<t> for insert to anon with check (true);
-- Yüklemeden hemen sonra:
--   drop policy <t>_tmp_insert on public.<t>;
