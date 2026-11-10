$ErrorActionPreference='Stop'
[Reflection.Assembly]::LoadFrom('C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Drawing.dll')|Out-Null
$root=(Get-Location).Path
$dir='Assets/Art/Menu/Task13'
$specs=Get-Content 'docs/art/menu-task13-spec.json' -Encoding utf8 -Raw|ConvertFrom-Json
$specs+=@(
 [pscustomobject]@{name='profile-button';width=216;height=216;use='Profil avatar düğmesi';slice=@(0,0,0,0)},
 [pscustomobject]@{name='profile-button-pressed';width=216;height=216;use='Profil avatar düğmesi basılı';slice=@(0,0,0,0)},
 [pscustomobject]@{name='menu-play-button';width=684;height=336;use='Onaylı Level tuşu';slice=@(92,92,92,92)},
 [pscustomobject]@{name='menu-play-button-pressed';width=684;height=336;use='Onaylı Level tuşu basılı';slice=@(92,92,92,92)}
)
$labels=@{
 'profile-frame'='Avatarın içi şeffaf altın-turkuaz çerçevesi';'profile-avatar'='Onaylı Jiggle kimliğinde avatar portresi';'counter-pill'='Ortak kaynak sayacı zemini';'settings-button'='Ana menü ayarlar dişlisi';'profile-box'='Profil adı ve kurtarılan sayısı zemini';
 'token-coin'='Para göstergesi simgesi';'token-heart'='Can göstergesi simgesi';'token-star'='Yıldız göstergesi simgesi';'token-jiggle'='Kurtarılan Jiggles göstergesi simgesi';
 'nav-home'='Alt gezinmede şato/ana ekran';'nav-collection'='Alt gezinmede tablo koleksiyonu';'nav-jiggles'='Alt gezinmede kurtarılan Jiggles';'nav-trophy'='Alt gezinmede başarılar';
 'progress-track'='Kurtarılan hedefi ilerleme çubuğu boş izi';'progress-fill'='İlerleme dolgusu; oyun tarafından kırpılır';'bottom-nav'='Alt gezinme şeridi; ikon ve etiketler ayrı';'nav-active'='Seçili alt sekme zemini';
 'plus-button'='Küçük ekleme/artı kontrolü';'notification-badge'='Bildirim sayısı rozeti; sayı ayrı';'levels-button'='Bölüm listesi kısayolu';'profile-box-pressed'='Profil kutusu basılı';'settings-button-pressed'='Ayarlar dişlisi basılı';'plus-button-pressed'='Artı kontrolü basılı';'profile-frame-pressed'='Avatar çerçevesi basılı'
}
$assets=@();$missing=@()
foreach($s in $specs){
 $path=$dir+'/'+$s.name+'.png'
 if(-not (Test-Path -LiteralPath $path)){$missing+=$s.name;continue}
 $image=[System.Drawing.Bitmap]::new((Join-Path $root $path))
 if($image.Width -ne $s.width -or $image.Height -ne $s.height){throw ('Wrong dimensions: '+$path)}
 $alpha=$image.GetPixel(0,0).A;$image.Dispose()
 $use=if($labels.ContainsKey($s.name)){$labels[$s.name]}else{$s.use}
 $source=if(Test-Path ($dir+'/Source/'+$s.name+'.png')){$dir+'/Source/'+$s.name+'.png'}else{$null}
 $assets+=[ordered]@{name=$s.name;path=$path;width=$s.width;height=$s.height;sliceLTRB=@($s.slice);use=$use;transparent=$alpha -eq 0;textBaked=$false;source=$source;approval=if($s.name -like 'menu-play-button*'){'Zeyd onaylı; birebir kopya'}else{'Zeyd incelemesi'}}
}
$anchor=@()
$pairs=@([pscustomobject]@{canonical='Assets/Art/UI/PlayButton/menu-play-button-v2.png';copy=($dir+'/menu-play-button.png')},[pscustomobject]@{canonical='Assets/Art/UI/PlayButton/menu-play-button-v2-pressed.png';copy=($dir+'/menu-play-button-pressed.png')})
foreach($p in $pairs){$a=(Get-FileHash -LiteralPath $p.canonical -Algorithm SHA256).Hash;$b=(Get-FileHash -LiteralPath $p.copy -Algorithm SHA256).Hash;if($a -ne $b){throw 'Approved anchor changed'};$anchor+=[ordered]@{canonical=$p.canonical;copy=$p.copy;sha256=$a}}
$previews=@();foreach($name in @('preview-menu@3x.png','preview-menu.png','preview-header.png','preview-nav.png','preview-controls.png','preview.png','preview-review-page.png')){$path=$dir+'/'+$name;if(Test-Path $path){$i=[System.Drawing.Bitmap]::new((Join-Path $root $path));$previews+=[ordered]@{path=$path;width=$i.Width;height=$i.Height};$i.Dispose()}}
$manifest=[ordered]@{version='menu-task13-v01';styleAnchor='Assets/Art/UI/PlayButton/menu-play-button-v2.png';status='Zeyd incelemesi';assets=$assets;missing=$missing;approvedAnchorCopies=$anchor;previews=$previews;sourcePolicy='Source klasörü üretim kaynağıdır; runtime atlasına yalnız assets listesi alınır.'}
[System.IO.File]::WriteAllText((Join-Path $root ($dir+'/manifest.json')),($manifest|ConvertTo-Json -Depth 10),[System.Text.UTF8Encoding]::new($false))
$lines=@('<!-- MENU_TASK13_START -->','## Task13 — onaylı Level tuşuyla aynı dilde ana menü','','Onaylı Level normal/basılı dosyaları SHA-256 ile doğrulanan birebir kopyadır. Yeni çizimler Zeyd incelemesindedir. Tüm sprite PNG dosyalari ayrı @3x şeffaf PNG; dinamik yazı ve sayılar ayrı kalır. `manifest.json` ölçü, kaynak ve 9-slice için esas kayıttır. Source klasörü runtime atlasına alınmaz.','','| Dosya | Oyunda nereye | Boyut / 9-slice |','|---|---|---|')
foreach($a in $assets){$slice=if(($a.sliceLTRB|Measure-Object -Sum).Sum -gt 0){'; L/R/T/B '+($a.sliceLTRB -join '/')}else{''};$lines+=('| `'+$a.path+'` | '+$a.use+' | '+$a.width+'×'+$a.height+$slice+' |')}
foreach($p in $previews){$lines+=('| `'+$p.path+'` | Sanat inceleme önizlemesi; oyun sprite dosyası değildir | '+$p.width+'×'+$p.height+' |')}
if(Test-Path ($dir+'/index.html')){$lines+=('| `'+$dir+'/index.html` | 390×844 telefon menüsü ve tüm sprite dosyalarının tek sayfalık incelemesi | HTML; yerel açılabilir |')}
$lines+=@('','İlerleme dolgusu maske/kırpma ile gösterilir; iki PNG üst üste yerleşir. Gösterge ikonları sayaç zemininin sol kenarına bindirilir. Profil portresi/çerçeve/kutu ayrı parçalar ve birleşik avatar düğmesi olarak teslim edilir. Önizlemedeki kullanıcı adı, sayaçlar ve alt gezinme etiketleri örnektir.','<!-- MENU_TASK13_END -->')
$hp=Join-Path $root 'docs/art/HANDOFF.md';$h=[System.IO.File]::ReadAllText($hp);$block=$lines -join [Environment]::NewLine;$h=[regex]::Replace($h,'(?s)<!-- MENU_TASK13_START -->.*?<!-- MENU_TASK13_END -->',[System.Text.RegularExpressions.MatchEvaluator]{param($m)$block});[System.IO.File]::WriteAllText($hp,$h,[System.Text.UTF8Encoding]::new($false))
$np=Join-Path $root 'docs/art/ASSET_IHTIYAC.md';$n=[System.IO.File]::ReadAllText($np)
$table=@('<!-- MENU_TASK13_FILES_BEGIN -->','### Task13 dosya durumu','','| Dosya | Ölçü @3x | Durum |','|---|---|---|');foreach($s in $specs){$status=if(Test-Path ($dir+'/'+$s.name+'.png')){'🎨 Hazır; Zeyd incelemesi'}else{'⏳ Üretiliyor'};if($s.name -like 'menu-play-button*'){$status='✅ Onaylı kaynak korunuyor'};$table+=('| `'+$s.name+'.png` | '+$s.width+'×'+$s.height+' | '+$status+' |')};$table+='<!-- MENU_TASK13_FILES_END -->';$fileBlock=$table -join [Environment]::NewLine
if($n.Contains('<!-- MENU_TASK13_FILES_BEGIN -->')){$n=[regex]::Replace($n,'(?s)<!-- MENU_TASK13_FILES_BEGIN -->.*?<!-- MENU_TASK13_FILES_END -->',[System.Text.RegularExpressions.MatchEvaluator]{param($m)$fileBlock})}else{$n+=[Environment]::NewLine+[Environment]::NewLine+$fileBlock};[System.IO.File]::WriteAllText($np,$n,[System.Text.UTF8Encoding]::new($false))
Write-Output ('Task13: '+$assets.Count+' sprites, '+$missing.Count+' remaining, '+$previews.Count+' previews; approved anchor hashes match.')
