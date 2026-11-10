-- PM_59: apply manually, never from the client. Independent queues need no foreign key.
create table if not exists public.tutorial_events (
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
  event text not null check (event = 'tut_step'),
  step integer not null check (step between 1 and 4),
  result text not null check (result in ('jam', 'exit')),
  seconds double precision not null check (seconds >= 0),
  wrong_tries integer not null check (wrong_tries >= 0)
);
alter table public.tutorial_events enable row level security;
revoke all on public.tutorial_events from anon;
grant insert on public.tutorial_events to anon;
grant select (row_id) on public.tutorial_events to anon;
drop policy if exists tutorial_events_anon_insert on public.tutorial_events;
create policy tutorial_events_anon_insert on public.tutorial_events for insert to anon with check (true);
create index if not exists tutorial_events_attempt_idx on public.tutorial_events (attempt_row_id);
create index if not exists tutorial_events_level_idx on public.tutorial_events (cohort, level_id, level_version);
create index if not exists tutorial_events_created_idx on public.tutorial_events (created_at);
