param([int]$Batch = 1)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$out = Join-Path $repo 'Assets/Art/Paintings'
$src = Join-Path $out 'Source'
[IO.Directory]::CreateDirectory($src) | Out-Null
$spec = Get-Content -Raw -Encoding UTF8 (Join-Path $PSScriptRoot "task16-batch-$Batch-prompts.json") | ConvertFrom-Json
$inputs = Get-Content -Raw -Encoding UTF8 (Join-Path $PSScriptRoot "task16-batch-$Batch-inputs.json") | ConvertFrom-Json
function New-Canvas([int]$width,[int]$height) {
    return [Drawing.Bitmap]::new($width,$height,[Drawing.Imaging.PixelFormat]::Format24bppRgb)
}
function New-Graphics($image) {
    $g = [Drawing.Graphics]::FromImage($image)
    $g.CompositingQuality = [Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.PixelOffsetMode = [Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.SmoothingMode = [Drawing.Drawing2D.SmoothingMode]::AntiAlias
    return $g
}
function Save-Png($image,$path) { $image.Save($path,[Drawing.Imaging.ImageFormat]::Png) }
$records = @()
foreach ($painting in $spec.paintings) {
    $name = 'painting-{0:D2}' -f [int]$painting.level
    $inputPath = $inputs.PSObject.Properties[$name].Value
    if (-not $inputPath) { throw "Missing input: $name" }
    $raw = [Drawing.Image]::FromFile($inputPath)
    if ($raw.Width -ne $raw.Height) { throw "Non-square input: $name" }
    $source = New-Canvas 1536 1536
    $g = New-Graphics $source
    $g.DrawImage($raw,[Drawing.Rectangle]::new(0,0,1536,1536)); $g.Dispose()
    $sourcePath = Join-Path $src "$name.png"
    Save-Png $source $sourcePath
    $gamePath = Join-Path $out "$name.jpg"
    $jpegReport = & 'C:/Users/zeydt/AppData/Local/Python/pythoncore-3.14-64/python.exe' (Join-Path $PSScriptRoot 'encode-task16.py') $inputPath $gamePath
    if ($LASTEXITCODE -ne 0) { throw "JPEG encoder failed: $name" }
    $jpegInfo = $jpegReport | ConvertFrom-Json
    $bytes = (Get-Item -LiteralPath $gamePath).Length
    if ($bytes -gt 300000) { throw "$name JPG exceeds 300 KB: $bytes bytes" }
    $records += [ordered]@{level=[int]$painting.level; theme=$painting.theme; source="Source/$name.png"; sourceSize=@(1536,1536); sourceBytes=(Get-Item $sourcePath).Length; generatedSize=@($raw.Width,$raw.Height); normalization='HighQualityBicubic, no crop; generated original preserved in image_gen output'; game="$name.jpg"; gameSize=@(1024,1024); jpegQuality=85; gameBytes=$bytes; sourceSha256=(Get-FileHash $sourcePath -Algorithm SHA256).Hash; gameSha256=(Get-FileHash $gamePath -Algorithm SHA256).Hash}
    $raw.Dispose(); $source.Dispose()
    $records[-1]['jpegPrefilterRadius'] = $jpegInfo.prefilterRadius
}
# Five columns, two rows; names sit outside each unmodified painting.
$sheet = New-Canvas 1580 760
$g = New-Graphics $sheet; $g.Clear([Drawing.ColorTranslator]::FromHtml('#113F49'))
$font = [Drawing.Font]::new('Segoe UI',13,[Drawing.FontStyle]::Bold)
for ($i=0; $i -lt $records.Count; $i++) {
    $x=20+($i%5)*312; $y=20+[math]::Floor($i/5)*368
    $im=[Drawing.Image]::FromFile((Join-Path $out $records[$i].game))
    $g.DrawImage($im,[Drawing.Rectangle]::new($x,$y,292,292)); $im.Dispose()
    $g.DrawString(('{0:D2}  {1}' -f $records[$i].level,$records[$i].theme),$font,[Drawing.Brushes]::AntiqueWhite,[single]$x,[single]($y+301))
}
$g.Dispose(); $font.Dispose(); Save-Png $sheet (Join-Path $out "preview-batch-$Batch.png"); $sheet.Dispose()
# A real 6x6 cut with narrow gaps; only this review image contains obstacles.
$board = New-Canvas 1104 1104
$g = New-Graphics $board; $g.Clear([Drawing.ColorTranslator]::FromHtml('#153F49'))
$painting=[Drawing.Image]::FromFile((Join-Path $src $records[0].source.Split('/')[-1]))
for ($row=0; $row -lt 6; $row++) {
    for ($col=0; $col -lt 6; $col++) {
        $g.DrawImage($painting,[Drawing.Rectangle]::new((24+$col*176),(24+$row*176),172,172),($col*256),($row*256),256,256,[Drawing.GraphicsUnit]::Pixel)
    }
}
$painting.Dispose()
foreach ($item in @(@{file='bomb-384.png';col=2;row=3;size=172},@{file='nail-head-384.png';col=4;row=1;size=156})) {
    $obstacle=[Drawing.Image]::FromFile((Join-Path $repo ('Assets/Art/Gameplay/Task14/'+$item.file)))
    $x=24+$item.col*176+(172-$item.size)/2; $y=24+$item.row*176+(172-$item.size)/2
    $g.DrawImage($obstacle,[Drawing.Rectangle]::new($x,$y,$item.size,$item.size)); $obstacle.Dispose()
}
$g.Dispose(); Save-Png $board (Join-Path $out "preview-board-$Batch.png"); $board.Dispose()
$room = if ($spec.room) { $spec.room } elseif ($Batch -eq 1) { 'Akdeniz salonu' } else { 'Orman ve kır salonu' }
$report=[ordered]@{batch=$Batch; room=$room; generator='built-in image_gen'; styleReference=$spec.reference; paintings=$records; previewBatch="preview-batch-$Batch.png"; previewBoard="preview-board-$Batch.png"; boardGrid=@(6,6); boardPainting=$records[0].source; obstacles=@('Assets/Art/Gameplay/Task14/bomb-384.png','Assets/Art/Gameplay/Task14/nail-head-384.png'); sourceNormalization='image_gen returned native squares; exported at requested 1536x1536 using bicubic resampling; game JPG directly from native original'; jpegMaxBytes=300000; status='exported; visual QA required'}
$report | ConvertTo-Json -Depth 9 | Set-Content -Encoding UTF8 (Join-Path $out "manifest-batch-$Batch.json")
$report | ConvertTo-Json -Depth 9 | Set-Content -Encoding UTF8 (Join-Path $PSScriptRoot "reports/task16-batch-$Batch-validation.json")
$records | ForEach-Object { [pscustomobject]$_ } | Select-Object level,theme,generatedSize,gameBytes | Format-Table -AutoSize
