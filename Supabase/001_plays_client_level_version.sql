-- Reverse Solver: separate Unity rows from the original web build, and tag
-- every row with the version of the level it was played on.
--
-- Defaults make this safe to run while the web build is still live:
--   * existing rows and anything the web build keeps posting get client = 'web'
--     and level_version = 1 (the web levels were never versioned, so they are v1)
--   * the Unity build sends client = 'unity-web' and the level's own version
--
-- Web rows have known bugs (attempt is always 1, 'quit' is never written), so
-- filter on client when comparing attempts or quit rates.

alter table public.plays
  add column if not exists client        text    not null default 'web',
  add column if not exists level_version integer not null default 1;

comment on column public.plays.client is
  'Which build wrote the row: web (original HTML game) or unity-web.';
comment on column public.plays.level_version is
  'Version of the level definition played; bump whenever a level''s layout, obstacles or timer change.';

-- Let PostgREST see the new columns immediately.
notify pgrst, 'reload schema';
