-- PM_MESAJ_57: continue_events inserts return 401 (edge log 2026-11-10 12:00 UTC).
-- The client posts ?on_conflict=row_id with Prefer: resolution=ignore-duplicates (retry-safe, same as plays).
-- INSERT ... ON CONFLICT needs SELECT on the conflict column; anon only has INSERT here.
-- Column-level grant: anon can name row_id, RLS still returns no rows (no SELECT policy), nothing becomes readable.
grant select (row_id) on public.continue_events to anon;
-- Same pattern for intro_events once 011 is applied:
-- grant select (row_id) on public.intro_events to anon;
