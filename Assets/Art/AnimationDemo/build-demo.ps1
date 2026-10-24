Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName PresentationCore,WindowsBase
Add-Type -TypeDefinition 'using System; using System.Runtime.InteropServices; public static class GdiLifetime { [DllImport("gdi32.dll")] public static extern bool DeleteObject(IntPtr handle); }'
$root=Split-Path -Parent $MyInvocation.MyCommand.Path
$bg=[System.Drawing.Bitmap]::FromFile((Join-Path $root '..\Menu\corridor-v04-source.png'))
$jiggles=@([System.Drawing.Bitmap]::FromFile((Join-Path $root 'sprites\jiggle-run-01.png')),[System.Drawing.Bitmap]::FromFile((Join-Path $root 'sprites\jiggle-run-02.png')))
$barons=@([System.Drawing.Bitmap]::FromFile((Join-Path $root 'sprites\baron-run-01.png')),[System.Drawing.Bitmap]::FromFile((Join-Path $root 'sprites\baron-run-02.png')))
$frames=[System.Collections.Generic.List[System.Drawing.Bitmap]]::new();$w=480;$h=720;$count=28
for($i=0;$i -lt $count;$i++){
  $t=$i/$count;$ease=$t*$t*(3-2*$t);$pose=[int]([math]::Floor($t*8)%2)
  $frame=[System.Drawing.Bitmap]::new($w,$h,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $g=[System.Drawing.Graphics]::FromImage($frame);$g.Clear([System.Drawing.Color]::FromArgb(16,42,49));$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::HighQuality
  $scale=1.06-.10*$ease;$dw=$w*$scale;$dh=$h*$scale;$g.DrawImage($bg,[System.Drawing.RectangleF]::new([single](($w-$dw)/2+40*$ease),[single](($h-$dh)/2-5*$ease),[single]$dw,[single]$dh))
  $bob=[math]::Sin($t*24)*3;$bs=.47+.03*[math]::Sin($t*18);$hs=.56+.12*$ease
  $g.DrawImage($barons[$pose],[System.Drawing.RectangleF]::new([single](303-22*$ease-240*$bs),[single](508+$bob-320*$bs),[single](480*$bs),[single](640*$bs)))
  $g.DrawImage($jiggles[$pose],[System.Drawing.RectangleF]::new([single](215+8*$ease-250*$hs),[single](600+$bob-330*$hs),[single](500*$hs),[single](660*$hs)))
  $font=[System.Drawing.Font]::new('Arial',15,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel);$brush=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(245,244,232,207));$label=if($t -lt .72){'LEVEL 12 RESCUED'}else{'LEVEL 13'};$g.DrawString($label,$font,$brush,16,18);$font.Dispose();$brush.Dispose();$g.Dispose();$frames.Add($frame)
}
$posterPath=Join-Path $root 'poster-v01.png';$frames[0].Save($posterPath,[System.Drawing.Imaging.ImageFormat]::Png)
$frames|ForEach-Object{$_.Dispose()};$bg.Dispose();$jiggles|ForEach-Object{$_.Dispose()};$barons|ForEach-Object{$_.Dispose()}
Get-Item $posterPath|Select-Object Name,Length
