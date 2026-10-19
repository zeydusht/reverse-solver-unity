-- =============================================================================
-- Reverse Solver — 006b: moves tablosuna anon SELECT politikası (2026-10-19)
--
-- ÇALIŞTIRILDI (PM, Supabase MCP ile, 2026-10-19). Kayıt için burada tutulur; tekrar çalıştırmak güvenli.
--
-- NEDEN: 006 yalnız anon INSERT politikası kurmuştu. Oyun hamleleri PostgREST'e
--   POST /rest/v1/moves?on_conflict=row_id,move_no  +  Prefer: resolution=ignore-duplicates
-- ile gönderiyor (upsert). on_conflict ile yapılan istek çakışmayı denetlemek için satırı okuyabilmeli;
-- SELECT politikası olmadığından her POST 401 aldı ve hamleler telefonda bekledi.
-- ÇÖZÜM: anon'a SELECT. UPDATE politikası YOK: istek "ignore-duplicates" ile gider, var olan satır
-- değişmez (aynı hamle ikinci kez gelirse sessizce atlanır). "merge-duplicates" KULLANILMAZ.
-- Doğrulama (PM): anon anahtarıyla upsert denendi, 201 / tekrarında çakışma yok sayıldı.
-- =============================================================================

alter table public.moves enable row level security;

do $$
begin
  if not exists (select 1 from pg_policies where tablename = 'moves' and policyname = 'moves_anon_select') then
    create policy moves_anon_select on public.moves for select to anon using (true);
  end if;
end $$;

notify pgrst, 'reload schema';

-- Doğrulama (salt okuma)
select policyname, cmd, roles from pg_policies where tablename = 'moves' order by policyname;
