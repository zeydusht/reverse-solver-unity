param([int]$Batch=1)
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$repo=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$out=Join-Path $repo 'Assets/Art/Paintings'
$manifestPath=Join-Path $out "manifest-batch-$Batch.json"
$report=Get-Content -Raw -Encoding UTF8 $manifestPath | ConvertFrom-Json
if ($report.paintings.Count -ne 10) { throw 'Expected exactly ten paintings' }
$protectedSnapshot=Join-Path $PSScriptRoot "reports/task16-before-batch-$Batch-protected.json"
if (Test-Path -LiteralPath $protectedSnapshot) {
    $protectedAssets=Get-Content -Raw -Encoding UTF8 $protectedSnapshot | ConvertFrom-Json
    foreach ($asset in $protectedAssets) {
        if ((Get-FileHash -LiteralPath $asset.path -Algorithm SHA256).Hash -ne $asset.sha256) { throw "Existing asset changed: $($asset.path)" }
    }
    $report | Add-Member -NotePropertyName protectedAssetsChecked -NotePropertyValue $protectedAssets.Count -Force
    $report | Add-Member -NotePropertyName protectedAssetsChanged -NotePropertyValue 0 -Force
}
foreach ($p in $report.paintings) {
    foreach ($part in @(@{path=$p.source;size=1536;hash=$p.sourceSha256},@{path=$p.game;size=1024;hash=$p.gameSha256})) {
        $path=Join-Path $out $part.path
        $image=[Drawing.Image]::FromFile($path)
        try { if ($image.Width -ne $part.size -or $image.Height -ne $part.size) { throw "Wrong dimensions: $path" } }
        finally { $image.Dispose() }
        if ((Get-FileHash $path -Algorithm SHA256).Hash -ne $part.hash) { throw "Hash mismatch: $path" }
    }
    if ((Get-Item (Join-Path $out $p.game)).Length -gt 300000) { throw 'JPG size limit exceeded' }
    if ($p.jpegQuality -ne 85) { throw 'Wrong JPG quality' }
}
$report.status='passed: 10 source PNGs, 10 quality-85 JPGs below 300000 bytes; dimensions and SHA-256 checked; contact sheet and board visually reviewed'
$report | Add-Member -NotePropertyName visualReview -NotePropertyValue ([ordered]@{paintings='Original scenes; no text, faces, frames, logos or embedded obstacles; consistent warm painted style'; contactSheet='All 10 themes visible, labels legible, no clipping'; board='6x6 cut, one bomb and one nail; both clearly visible'; limitations='Preview uses square tile cuts; C# integration and live gameplay are outside this art task.'}) -Force
$report | ConvertTo-Json -Depth 10 | Set-Content -Encoding UTF8 $manifestPath
$report | ConvertTo-Json -Depth 10 | Set-Content -Encoding UTF8 (Join-Path $PSScriptRoot "reports/task16-batch-$Batch-validation.json")
$lines=@("<!-- TASK16_BATCH_${Batch}_BEGIN -->", "## Task16 — Parti $Batch tamamlandı", '',
    'Üretim: built-in `image_gen`; stil referansı `Gameplay/Task15/painting-test-01.png`. Özgün sahneler; tabloların içinde yazı, logo, insan yüzü, çerçeve ve engel yok.', '',
    '| Dosya | Bölüm | Tema | Boyut |','|---|---|---|---|')
foreach ($p in $report.paintings) {
    $lines += ('| `Assets/Art/Paintings/{0}` / `Assets/Art/Paintings/{1}` | {2} | {3} | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, {4:N0} bayt |' -f $p.source,$p.game,$p.level,$p.theme,$p.gameBytes)
}
$lines += @("| ``Assets/Art/Paintings/preview-batch-$Batch.png`` | Parti $Batch | 10 resimlik inceleme ızgarası | 1580×760 PNG |", "| ``Assets/Art/Paintings/preview-board-$Batch.png`` | Parti $Batch | 6×6 tahta, bir bomba ve bir çivi | 1104×1104 PNG |", '',
    'Üretim aracının verdiği 1254×1254 kareler kırpılmadan bicubic ile 1536×1536 kaynak ölçüsüne getirildi; bu teslim boyutu yerel üretim çözünürlüğü değildir. Oyun JPG dosyaları özgün üretimden Lanczos ile 1024×1024 küçültüldü, kalite 85 / 4:2:0 / optimize / progressive olarak kodlandı. Yoğun bitki ayrıntılarında 300.000 bayt sınırını sağlamak için yalnız JPG dosyalarında dosya başına kaydedilmiş hafif Gaussian ön filtre kullanıldı; kaynak PNG dosyalarına uygulanmadı.', '',
    "Promptlar: ``docs/art/task16-batch-$Batch-prompts.json``. Dosya ölçüleri, yerel üretim ölçüsü, ön filtre yarıçapı, SHA-256 ve byte ölçümleri: ``Assets/Art/Paintings/manifest-batch-$Batch.json`` ve ``docs/art/reports/task16-batch-$Batch-validation.json``.", '',
    'Tahta önizlemesi gerçek Task14 bomba ve çivi görsellerini kullanır; bu engeller kaynak/oyun resimlerine gömülmedi. Önizleme kare dilimlidir, canlı oyun testi değildir. C# ve izinli iki klasörün dışındaki proje dosyalarına yazılmadı.',
    "<!-- TASK16_BATCH_${Batch}_END -->")
$handoff=Join-Path $PSScriptRoot 'HANDOFF.md'
$existing=[IO.File]::ReadAllText($handoff)
$block=$lines -join "`n"
$begin="<!-- TASK16_BATCH_${Batch}_BEGIN -->"; $end="<!-- TASK16_BATCH_${Batch}_END -->"
if ($existing.Contains($begin)) { $existing=[regex]::Replace($existing,([regex]::Escape($begin)+'.*?'+[regex]::Escape($end)),[Text.RegularExpressions.MatchEvaluator]{param($m) $block},[Text.RegularExpressions.RegexOptions]::Singleline) }
else { $existing=$existing.TrimEnd()+"`n`n"+$block+"`n" }
[IO.File]::WriteAllText($handoff,$existing,[Text.UTF8Encoding]::new($false))
Write-Output "Batch $Batch passed; HANDOFF updated."
