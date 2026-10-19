-- =============================================================================
-- Reverse Solver — ZeydBot hamle kaydı: "moves" tablosu (Zeyd 2026-10-19)
--
-- TASLAK, ÇALIŞTIRILMADI. Mühendislik Supabase şemasına dokunmaz; bunu Zeyd çalıştırır.
--
-- NASIL ÇALIŞTIRILIR (Zeyd):
--   1. supabase.com → projen → sol menüde SQL Editor.
--   2. "New query" aç, bu dosyanın TAMAMINI yapıştır, "Run"a bas.
--   3. En alttaki doğrulama sorgusunun sonucunu (tablo) kopyala ve getir.
--   Tekrar çalıştırmak güvenli ("zaten varsa atla").
--
-- NE YAPAR:
--   Yalnız ?set=zeydbot ile açılan ZeydBot oturumlarında, denemenin HER eylemi bir satır olur:
--   çıkış (exit), sıkışma (jam), çekiç (hammer), makas / değnek / saat. Satır, denemenin
--   plays satırına row_id ile bağlanır (sonuç, kalan saniye, deneme no orada).
--   Hamle anındaki doğru / tuzak seçenek sayısı, seçilenin doğru mu tuzak mı olduğu ve çözücü
--   bilgisi telefonda HESAPLANMAZ: editörde hamleler yeniden oynatılarak çıkarılır.
--   Canlı 40'ın gönderimi değişmez; bu tablo yoksa oyun yalnız bu satırları bekletir.
--
-- ALANLAR (oyunun gönderdiği):
--   row_id        plays.row_id (deneme)          player   oyuncu adı (TEST_ZEYD)
--   level_id      Z01 … Z30                       level_version
--   attempt       o bölümde kaçıncı deneme (1 = analize giren ilk deneme)
--   move_no       denemede eylem sırası (1'den)   kind     exit | jam | hammer | scissors | wand | clock
--   piece         parça (çıkış, sıkışma, çekiç; makas/değnek/saatte -1)
--   dir           çıkış yönü U/D/L/R (yalnız exit)
--   t_ms          deneme başından bu eyleme ms    think_ms bir önceki eylemden (ya da başlangıçtan) bu eyleme ms
--   time_left     eylem anında kalan saniye       fuses    eylem anında bombalar "parça:fitil;…"
-- =============================================================================

create table if not exists public.moves (
  id            bigint generated always as identity primary key,
  created_at    timestamptz not null default now(),
  row_id        text not null,
  player        text,
  level_id      text not null,
  level_version int,
  attempt       int not null,
  move_no       int not null,
  kind          text not null,
  piece         int,
  dir           text,
  t_ms          int,
  think_ms      int,
  time_left     int,
  fuses         text
);

do $$
begin
  if not exists (select 1 from pg_constraint where conname = 'moves_row_move_key') then
    alter table public.moves add constraint moves_row_move_key unique (row_id, move_no);
  end if;
  if not exists (select 1 from pg_constraint where conname = 'moves_kind_check') then
    alter table public.moves add constraint moves_kind_check check (kind in ('exit','jam','hammer','scissors','wand','clock'));
  end if;
end $$;

-- the game writes with the anon key, like plays: insert only, no read for anon
alter table public.moves enable row level security;
do $$
begin
  if not exists (select 1 from pg_policies where tablename = 'moves' and policyname = 'moves_anon_insert') then
    create policy moves_anon_insert on public.moves for insert to anon with check (true);
  end if;
end $$;

notify pgrst, 'reload schema';

-- Doğrulama (salt okuma): tablo, kural ve politika yerinde mi.
select 'tablo' as ne, count(*)::text as deger from information_schema.columns where table_schema = 'public' and table_name = 'moves'
union all
select 'kural', conname from pg_constraint where conname in ('moves_row_move_key', 'moves_kind_check')
union all
select 'politika', policyname from pg_policies where tablename = 'moves';
