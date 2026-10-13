-- =============================================================================
-- Reverse Solver — plays tablosuna "keyif" sütunu (Faz 4 G8, 👍/👎 sorusu)
--
-- TASLAK, ÇALIŞTIRILMADI. Mühendislik Supabase şemasına dokunmaz; bunu Zeyd çalıştırır.
--
-- NASIL ÇALIŞTIRILIR (Zeyd):
--   1. supabase.com → projen (vwneqdhzfevgbuwoadod) → sol menüde SQL Editor.
--   2. "New query" aç, bu dosyanın TAMAMINI yapıştır, "Run"a bas.
--   3. En alttaki doğrulama sorgusunun sonucunu (tablo) kopyala ve getir.
--   Tekrar çalıştırmak güvenli: "zaten varsa atla" şeklinde yazıldı.
--
-- NE YAPAR:
--   keyif  Kazanılan bir bölümün sonunda oyuncunun cevabı: 1 = 👍 (Beğendim),
--          -1 = 👎 (Beğenmedim), boş = sorulmadı ya da cevaplanmadı.
--          Boş bırakılabilir; mevcut satırlar ve web sürümü etkilenmez.
--   Oyun bu alanı yalnızca SetFlags.AskFun açıkken ve oyuncu cevap verince
--   gönderir. Bayrak, bu SQL çalıştırılıp doğrulandıktan sonra ve yalnızca
--   Zeyd'in onayıyla açılır (PRODUCT.md "Faz 4", G8).
-- =============================================================================

alter table public.plays add column if not exists keyif smallint;

do $$
begin
  if not exists (select 1 from pg_constraint where conname = 'plays_keyif_check') then
    alter table public.plays add constraint plays_keyif_check check (keyif is null or keyif in (-1, 1));
  end if;
end $$;

comment on column public.plays.keyif is
  'Faz 4: answer after a won level, 1 = thumbs up, -1 = thumbs down, null = not asked or not answered.';

-- Let PostgREST see the new column immediately.
notify pgrst, 'reload schema';

-- Doğrulama (salt okuma): sütun var mı, boş mu, kural yerinde mi.
select column_name, data_type, is_nullable
from information_schema.columns
where table_schema = 'public' and table_name = 'plays' and column_name = 'keyif'
union all
select 'kural: ' || conname, pg_get_constraintdef(oid), ''
from pg_constraint where conname = 'plays_keyif_check';
