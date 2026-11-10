-- Intro playback events; the client may only append.
create table if not exists public.intro_events (
  row_id text primary key,
  created_at timestamptz not null default now(),
  player text,
  device_id text,
  cohort text,
  app_version text,
  event text not null,
  part text not null,
  sec numeric not null,
  tapped boolean not null,
  replay boolean not null
);

alter table public.intro_events enable row level security;
revoke all on public.intro_events from anon;
grant insert on public.intro_events to anon;
grant select (row_id) on public.intro_events to anon;  -- needed by ?on_conflict=row_id (see 016)
drop policy if exists intro_events_anon_insert on public.intro_events;
create policy intro_events_anon_insert on public.intro_events
  for insert to anon with check (true);
