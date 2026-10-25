$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$manifest=Get-Content -Raw -Encoding utf8 (Join-Path $root 'Assets/Art/Gameplay/Task6/manifest.json') | ConvertFrom-Json
$files=@()
foreach($a in $manifest.assets){
  $file=Join-Path $root $a.path
  $im=[Drawing.Bitmap]::FromFile($file)
  if($im.Width -ne $a.width -or $im.Height -ne $a.height){throw ('Dimension: '+$a.name)}
  $size=(Get-Item -LiteralPath $file).Length
  $min=255;$max=0
  for($y=0;$y -lt $im.Height;$y++){for($x=0;$x -lt $im.Width;$x++){$v=$im.GetPixel($x,$y).A;$min=[Math]::Min($min,$v);$max=[Math]::Max($max,$v)}}
  if($a.name -eq 'loader-bg'){
    if($size -gt 250000 -or $min -ne 255){throw 'Loader budget/alpha'}
  }else{
    if($min -ne 0 -or $max -ne 255){throw ('Alpha: '+$a.name)}
    if($a.name -like 'joint-target*' -and $im.GetPixel(33,32).A -ne 255){throw 'Target center'}
  }
  if($null -ne $a.slice){throw 'Unexpected slice'}
  $files+= [ordered]@{name=$a.name;width=$im.Width;height=$im.Height;bytes=$size;alphaMin=$min;alphaMax=$max}
  $im.Dispose()
}
$files | ConvertTo-Json -Depth 5 | Set-Content -Encoding utf8 (Join-Path $root 'docs/art/reports/task6-validation.json')
$files | ConvertTo-Json -Depth 5
