-- =============================================================================
-- Reverse Solver — plays tablosuna level_id ve row_id ekler
--
-- NASIL ÇALIŞTIRILIR (Zeyd):
--   1. supabase.com → projen (vwneqdhzfevgbuwoadod) → sol menüde SQL Editor.
--   2. "New query" aç, bu dosyanın TAMAMINI yapıştır, "Run"a bas.
--   3. En alttaki doğrulama sorgusunun sonucunu (tablo) kopyala ve getir.
--   Tekrar çalıştırmak güvenli: her adım "zaten varsa atla" şeklinde yazıldı.
--   Web sürümü bu sırada çalışmaya devam eder; yeni sütunları göndermez.
--
-- NE YAPAR:
--   level_id  Seviyenin kalıcı kimliği (L01, L02, ...). level_version ile
--             birlikte bir satırın hangi seviyeye ait olduğunu söyler; lv
--             yalnızca o build'deki sıradır ve yeniden tasarımda değişir.
--   row_id    Unity'nin her kayda verdiği benzersiz kimlik. Bağlantı kopup
--             aynı kayıt yeniden gönderilirse ikinci satır yazılmaz
--             (deneme sayısı modelin ana metriği; çift satır onu bozar).
--   Eski web satırları: level_id, lv'den doldurulur (web'in 40 seviyesi
--   web sırasıyla L01..L40'tır, sürüm 1).
-- =============================================================================

alter table public.plays add column if not exists level_id text;
alter table public.plays add column if not exists row_id text;

comment on column public.plays.level_id is
  'Permanent level id (L01, L02, ...); pairs with level_version. lv is only the position in a build.';
comment on column public.plays.row_id is
  'Client-generated unique id of the attempt row; retries of the same row are ignored.';

-- One row per row_id. NULLs (web rows) do not collide with each other.
do $$
begin
  if not exists (select 1 from pg_constraint where conname = 'plays_row_id_key') then
    alter table public.plays add constraint plays_row_id_key unique (row_id);
  end if;
end $$;

update public.plays
   set level_id = 'L' || lpad(lv::text, 2, '0')
 where level_id is null
   and client = 'web'
   and lv between 1 and 40;

create index if not exists plays_level_id_version_idx on public.plays (level_id, level_version);

-- Let PostgREST see the new columns immediately.
notify pgrst, 'reload schema';

-- ---- doğrulama: sonucu getir ------------------------------------------------
select
  (select count(*) from information_schema.columns
    where table_schema = 'public' and table_name = 'plays'
      and column_name in ('client', 'level_version', 'level_id', 'row_id'))      as yeni_sutunlar_4_olmali,
  (select count(*) from pg_constraint where conname = 'plays_row_id_key')        as tekil_kisit_1_olmali,
  (select count(*) from public.plays where client = 'web')                       as web_satirlari,
  (select count(*) from public.plays where client = 'web' and level_id is null) as level_id_bos_web_satirlari;
