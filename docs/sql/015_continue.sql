-- Append-only decisions, separate from plays. Apply manually; never add a plays column.
create table if not exists public.continue_events (
  row_id text primary key,
  created_at timestamptz not null default now(),
  attempt_row_id text not null,
  player text,
  cohort text,
  device_id text,
  app_version text,
  level_id text not null,
  level_version integer not null,
  attempt integer not null,
  continue_no integer not null,
  kind text not null check (kind in ('time','bomb','final')),
  price integer not null,
  seconds integer not null,
  coins_before integer not null,
  result text not null check (result in ('accepted','declined','withdrawn','won_after_continue','bomb','time','quit'))
);

alter table public.continue_events enable row level security;
revoke all on public.continue_events from anon;
grant insert on public.continue_events to anon;
drop policy if exists continue_events_anon_insert on public.continue_events;
create policy continue_events_anon_insert on public.continue_events
  for insert to anon with check (true);

-- No foreign key: the independent queues may deliver decisions before the plays row.
create index if not exists continue_events_attempt_idx on public.continue_events (attempt_row_id);
create index if not exists continue_events_level_idx on public.continue_events (cohort, level_id, level_version);
create index if not exists continue_events_created_idx on public.continue_events (created_at);

/*
View audit: docs/sql/009_veri_sistemi.sql, v_attempts, v_level_actual,
v_pred_vs_actual and v_model_score.
plays.result is free text: continued wins send 'won_after_continue' in that
existing column. v_attempts passes result through. v_level_actual.win1 and
v_model_score.won test result = 'win'; v_pred_vs_actual uses v_level_actual.
Consequently these wins are automatically excluded from first-attempt win
numerators. No view replacement is required. A continued win stays in the
first-attempt denominator as an unassisted failure, as intended for the model.
fail_bomb1/fail_time1 use explicit bomb/time equality, so a continued win is
neither bomb nor time; other1 captures it. No fail-rate bucket calls it a loss.

One old standalone report needs a fix if reused: 005_playtest.sql query 2
uses result <> 'win' for answers_after_losses/up_after_losses_pct, which
would misclassify continued wins (and quits). Use explicit loss outcomes.
The following is a commented-out replacement report only; nothing is applied.
*/
-- with pt as (
--   select p.* from public.plays p
--   where p.player not ilike 'TEST\_%'
--     and p.client = 'unity-web' and p.level_id like 'R%'
--     and p.created_at >= timestamptz '2099-01-01 00:00+03'
-- )
-- select level_id, level_version,
--   count(*) filter (where keyif is not null) as answers,
--   round(avg((keyif = 1)::int) filter (where keyif is not null) * 100, 1) as thumbs_up_pct,
--   count(*) filter (where keyif is not null and result = 'win') as answers_after_win,
--   round(avg((keyif = 1)::int) filter (where keyif is not null and result = 'win') * 100, 1) as up_after_win_pct,
--   count(*) filter (where keyif is not null and result in ('bomb','time')) as answers_after_losses,
--   round(avg((keyif = 1)::int) filter (where keyif is not null and result in ('bomb','time')) * 100, 1) as up_after_losses_pct
-- from pt group by level_id, level_version order by level_id, level_version;
