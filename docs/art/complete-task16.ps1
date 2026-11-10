$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$out = Join-Path $repo 'Assets/Art/Paintings'
$batches = @()
$paintings = @()
foreach ($batch in 1..4) {
    $manifest = Get-Content -Raw -Encoding UTF8 (Join-Path $out "manifest-batch-$batch.json") | ConvertFrom-Json
    if ($manifest.status -notlike 'passed:*' -or $manifest.paintings.Count -ne 10) { throw "Batch $batch is not validated" }
    $batches += [ordered]@{batch=$batch;room=$manifest.room;firstLevel=($batch-1)*10+1;lastLevel=$batch*10;manifest="manifest-batch-$batch.json";preview="preview-batch-$batch.png";board="preview-board-$batch.png"}
    $paintings += $manifest.paintings
}
$paintings = @($paintings | Sort-Object level)
if ($paintings.Count -ne 40 -or @($paintings.level | Select-Object -Unique).Count -ne 40) { throw 'Expected 40 unique paintings' }
for ($i=0; $i -lt 40; $i++) {
    $p=$paintings[$i]; $name='painting-{0:D2}' -f ($i+1)
    if ($p.level -ne ($i+1) -or $p.game -ne "$name.jpg" -or $p.source -ne "Source/$name.png") { throw "Wrong level mapping: $name" }
    foreach ($asset in @(@{file=$p.source;hash=$p.sourceSha256},@{file=$p.game;hash=$p.gameSha256})) {
        if ((Get-FileHash -LiteralPath (Join-Path $out $asset.file) -Algorithm SHA256).Hash -ne $asset.hash) { throw "Changed/missing file: $($asset.file)" }
    }
    if ($p.jpegQuality -ne 85 -or (Get-Item -LiteralPath (Join-Path $out $p.game)).Length -gt 300000) { throw "Invalid JPG: $name" }
}
$catalog=[ordered]@{task='CODEX_TASK16';complete=$true;paintingCount=40;generator='built-in image_gen';styleReference='Assets/Art/Gameplay/Task15/painting-test-01.png';assetRoot='Assets/Art/Paintings/';sourceSize=@(1536,1536);gameSize=@(1024,1024);jpegQuality=85;jpegMaxBytes=300000;batches=$batches;paintings=$paintings;finale=[ordered]@{level=40;subject='The Count own grand original castle';humanFacesOrCountVisible=$false;review='Visually inspected; grandest architecture in the collection'};limitations=@('1536px sources normalized from image_gen native squares; native size recorded per asset','JPG-only mild prefilter used when needed to satisfy 300000-byte limit','Preview boards use 6x6 square cuts; no C# integration or live gameplay test')}
$catalog | ConvertTo-Json -Depth 10 | Set-Content -Encoding UTF8 (Join-Path $out 'manifest-collection.json')
$summary=[ordered]@{task='CODEX_TASK16';status='complete';paintingCount=40;sourcePngCount=40;gameJpgCount=40;assetHashesChecked=80;assetHashesChanged=0;batchPreviewCount=4;boardPreviewCount=4;minGameBytes=($paintings.gameBytes | Measure-Object -Minimum).Minimum;maxGameBytes=($paintings.gameBytes | Measure-Object -Maximum).Maximum;sourceDimensions=@(1536,1536);gameDimensions=@(1024,1024);jpegQuality=85;batchReports=@(1..4 | ForEach-Object {"docs/art/reports/task16-batch-$_-validation.json"});catalog='Assets/Art/Paintings/manifest-collection.json';finale=$catalog.finale;scope='Only Assets/Art/Paintings and docs/art; no C# edits'}
$summary | ConvertTo-Json -Depth 8 | Set-Content -Encoding UTF8 (Join-Path $PSScriptRoot 'reports/task16-collection-validation.json')
$summary | ConvertTo-Json -Depth 5
