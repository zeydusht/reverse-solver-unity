$ErrorActionPreference='Stop'
$fontPath=Join-Path (Get-Location) 'Assets/Art/Gameplay/Task14/LilitaOne-Regular.ttf'
$b=[IO.File]::ReadAllBytes($fontPath)
function U16([int]$p){return ([int]$b[$p]*256+[int]$b[$p+1])}
function U32([int]$p){return ([int]$b[$p]*16777216+[int]$b[$p+1]*65536+[int]$b[$p+2]*256+[int]$b[$p+3])}
$cmap=0
for($i=0;$i -lt (U16 4);$i++){$p=12+16*$i;if([Text.Encoding]::ASCII.GetString($b,$p,4) -eq 'cmap'){$cmap=U32 ($p+8)}}
$table=0
for($i=0;$i -lt (U16 ($cmap+2));$i++){$p=$cmap+4+8*$i;$sub=$cmap+(U32 ($p+4));if((U16 $sub) -eq 4){$table=$sub}}
if($table -eq 0){throw 'Unicode cmap format 4 not found'}
$n=(U16 ($table+6))/2
$result=@(foreach($cp in @(0x011F,0x015F,0x0130,0x0131,0x00E7,0x00F6,0x00FC,0x0030,0x0039,0x003A)){
 $glyph=0
 for($i=0;$i -lt $n;$i++){
  $end=U16 ($table+14+2*$i);$start=U16 ($table+16+2*$n+2*$i)
  if($cp -ge $start -and $cp -le $end){$delta=U16 ($table+16+4*$n+2*$i);$rp=$table+16+6*$n+2*$i;$offset=U16 $rp;if($offset -eq 0){$glyph=($cp+$delta) -band 65535}else{$glyph=U16 ($rp+$offset+2*($cp-$start));if($glyph -ne 0){$glyph=($glyph+$delta) -band 65535}};break}
 }
 [pscustomobject]@{character=[string][char]$cp;unicode=('U+{0:X4}' -f $cp);present=($glyph -ne 0);glyphId=$glyph}
})
$result | ConvertTo-Json | Set-Content docs/art/reports/task14-font-coverage.json -Encoding UTF8
$result | Format-Table
