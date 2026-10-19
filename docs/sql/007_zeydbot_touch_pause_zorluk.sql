-- =============================================================================
-- Reverse Solver — 007: ZeydBot ek verileri (Zeyd 2026-10-19, "her veri kıymetli")
--
-- TASLAK, ÇALIŞTIRILMADI. PM Supabase'de çalıştırır (Zeyd onayıyla). Tekrar çalıştırmak güvenli.
--
-- NE YAPAR:
--   moves.kind'e üç yeni tür:
--     touch   parçaya dokunup bıraktı ya da sürükleyip iptal etti (parça çıkmadı); hold_ms = basılı tutma süresi
--     pause   uygulama arka plana gitti (deneme sürerken)
--     resume  geri döndü
--   moves.hold_ms   yalnız touch satırlarında dolu (ms)
--   plays.zorluk_puani  bölüm sonundaki isteğe bağlı 1–10 zorluk cevabı (yalnız ?set=zeydbot); boş = cevaplanmadı
--
-- Oyun BU SQL'den önce yeni türleri gönderirse moves_kind_check onları reddeder (400) ve oyun o satırları
-- kenara ayırır; bu yüzden Build'den ÖNCE çalıştırılmalı. plays.zorluk_puani yoksa plays satırı 400 alır:
-- yine Build'den önce.
-- =============================================================================

alter table public.moves add column if not exists hold_ms int;

do $$
begin
  if exists (select 1 from pg_constraint where conname = 'moves_kind_check') then
    alter table public.moves drop constraint moves_kind_check;
  end if;
  alter table public.moves add constraint moves_kind_check
    check (kind in ('exit','jam','hammer','scissors','wand','clock','touch','pause','resume'));
end $$;

alter table public.plays add column if not exists zorluk_puani smallint;

do $$
begin
  if not exists (select 1 from pg_constraint where conname = 'plays_zorluk_puani_check') then
    alter table public.plays add constraint plays_zorluk_puani_check check (zorluk_puani is null or zorluk_puani between 1 and 10);
  end if;
end $$;

comment on column public.plays.zorluk_puani is 'ZeydBot: optional 1-10 difficulty answer after the level (?set=zeydbot), null = not answered.';
comment on column public.moves.hold_ms is 'ZeydBot: touch rows only, how long the piece was held (ms).';

notify pgrst, 'reload schema';

-- Doğrulama (salt okuma)
select 'moves.hold_ms' as ne, data_type as deger from information_schema.columns where table_name = 'moves' and column_name = 'hold_ms'
union all
select 'plays.zorluk_puani', data_type from information_schema.columns where table_name = 'plays' and column_name = 'zorluk_puani'
union all
select conname, pg_get_constraintdef(oid) from pg_constraint where conname in ('moves_kind_check', 'plays_zorluk_puani_check');
