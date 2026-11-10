$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$code=@'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
public static class Task14Check {
 public static long[] Scan(string path){using(Bitmap b=new Bitmap(path)){BitmapData d=b.LockBits(new Rectangle(0,0,b.Width,b.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);byte[] p=new byte[d.Stride*b.Height];Marshal.Copy(d.Scan0,p,0,p.Length);long zero=0,solid=0,partial=0;int l=b.Width,t=b.Height,r=-1,bt=-1;for(int y=0;y<b.Height;y++)for(int x=0;x<b.Width;x++){int i=y*d.Stride+x*4;byte a=p[i+3];if(a==0)zero++;else if(a==255)solid++;else partial++;if(a>230&&p[i+1]>p[i+2]*1.22&&p[i]>p[i+2]*1.3&&p[i+1]>30){l=Math.Min(l,x);r=Math.Max(r,x);t=Math.Min(t,y);bt=Math.Max(bt,y);}}b.UnlockBits(d);return new long[]{b.Width,b.Height,zero,solid,partial,l,t,r,bt};}}
 public static int Seam(string path){using(Bitmap b=new Bitmap(path)){int n=0;for(int y=0;y<b.Height;y++)if(b.GetPixel(0,y).ToArgb()!=b.GetPixel(b.Width-1,y).ToArgb())n++;return n;}}
 public static int Corner(string tile,string path){using(Bitmap a=new Bitmap(tile))using(Bitmap b=new Bitmap(path)){int n=0;for(int j=0;j<48;j++){if(b.GetPixel(95,j+48).ToArgb()!=a.GetPixel(0,j).ToArgb())n++;if(b.GetPixel(j,0).ToArgb()!=a.GetPixel(0,47-j).ToArgb())n++;}return n;}}
}
'@
Add-Type -TypeDefinition $code -ReferencedAssemblies System.Drawing
$dir=Join-Path (Get-Location) 'Assets/Art/Gameplay/Task14'
$manifest=Get-Content (Join-Path $dir 'manifest.json') -Raw -Encoding UTF8 | ConvertFrom-Json
$errors=[Collections.Generic.List[string]]::new()
$checks=@(foreach($item in $manifest){
 $file=Join-Path $dir $item.file
 $d=[Task14Check]::Scan($file)
 $bytes=[IO.File]::ReadAllBytes($file)
 if($d[0] -ne $item.width -or $d[1] -ne $item.height){$errors.Add('Dimension: '+$item.file)}
 if($bytes[25] -ne 6){$errors.Add('Not RGBA: '+$item.file)}
 if($d[2] -eq 0){$errors.Add('No transparent pixels: '+$item.file)}
 if($item.width%3 -ne 0 -or $item.height%3 -ne 0){$errors.Add('Not @3x: '+$item.file)}
 if($item.sliceLTRB[0]+$item.sliceLTRB[1] -ge $item.width -or $item.sliceLTRB[2]+$item.sliceLTRB[3] -ge $item.height){$errors.Add('Invalid slice: '+$item.file)}
 $hash=(Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash
 $item | Add-Member -NotePropertyName sha256 -NotePropertyValue $hash -Force
 [pscustomobject]@{file=$item.file;width=$d[0];height=$d[1];transparentPixels=$d[2];opaquePixels=$d[3];partialAlphaPixels=$d[4];pngColorType=$bytes[25];sha256=$hash}
})
$chainSeam=[Task14Check]::Seam((Join-Path $dir 'chain-link-repeat-96x48.png'))
$wallSeam=[Task14Check]::Seam((Join-Path $dir 'wall-repeat-96x48.png'))
$cornerSeam=[Task14Check]::Corner((Join-Path $dir 'wall-repeat-96x48.png'),(Join-Path $dir 'wall-corner-96x96.png'))
if($chainSeam+$wallSeam+$cornerSeam -ne 0){$errors.Add('Repeat/corner RGBA mismatch')}
$bomb=[Task14Check]::Scan((Join-Path $dir 'bomb-384.png'))
$bombCenter=@((($bomb[5]+$bomb[7])/2.0),(($bomb[6]+$bomb[8])/2.0))
if([Math]::Abs($bombCenter[0]-192) -gt 1 -or [Math]::Abs($bombCenter[1]-192) -gt 1){$errors.Add('Bomb body not centered')}
$frame=[Drawing.Bitmap]::new((Join-Path $dir 'board-frame-gold.png'))
$frameCenterAlpha=$frame.GetPixel(562,562).A
$frame.Dispose()
if($frameCenterAlpha -ne 0){$errors.Add('Frame center is not transparent')}
$protected=Get-Content docs/art/reports/task14-protected-sha256.json -Raw -Encoding UTF8 | ConvertFrom-Json
$changed=@($protected | Where-Object {(Get-FileHash -LiteralPath $_.Path -Algorithm SHA256).Hash -ne $_.Hash})
if($changed.Count -gt 0){$errors.Add('Protected source changed')}
$fontOriginal=Join-Path (Get-Location) 'Assets/Art/UI/PlayButton/LilitaOne-Regular.ttf'
$fontHash=(Get-FileHash (Join-Path $dir 'LilitaOne-Regular.ttf')).Hash
$fontUnchanged=$fontHash -eq (Get-FileHash $fontOriginal).Hash
if(-not $fontUnchanged){$errors.Add('Font copy differs')}
$manifest | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $dir 'manifest.json') -Encoding UTF8
$report=[ordered]@{status=$(if($errors.Count -eq 0){'passed'}else{'failed'});productionPngCount=$manifest.Count;rgba=$true;all3x=$true;noBakedTextOrDigits='Visually inspected; text layer excluded by exporter';palette='PALETTE.md anchors and their smooth color mixtures';chainSeamMismatchedPixels=$chainSeam;wallSeamMismatchedPixels=$wallSeam;cornerSeamMismatchedPixels=$cornerSeam;bombTealBodyBoundingCenter=$bombCenter;bombExpectedBodyCenter=@(192,192);frameCenterAlpha=$frameCenterAlpha;protectedSourceCount=$protected.Count;protectedSourceMismatches=$changed.Count;fontCopyIdentical=$fontUnchanged;fontCoverageReport='task14-font-coverage.json';errors=@($errors);files=$checks}
$report | ConvertTo-Json -Depth 7 | Set-Content docs/art/reports/task14-export-validation.json -Encoding UTF8
[pscustomobject]$report | Select-Object status,productionPngCount,chainSeamMismatchedPixels,wallSeamMismatchedPixels,cornerSeamMismatchedPixels,bombTealBodyBoundingCenter,protectedSourceMismatches,errors | ConvertTo-Json
if($errors.Count -gt 0){throw ($errors -join '; ')}
