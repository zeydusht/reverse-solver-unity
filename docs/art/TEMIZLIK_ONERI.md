# Task22 — Temizlik önerisi (hiçbir işlem uygulanmadı)

Tarama kapsamı yalnız **Assets/Art/** ve **docs/art/**. Kullanım araması yalnız **Assets/** metinleri, .meta GUID'leri ve Resources/StreamingAssets içerik eşitliğidir. Library/, Logs/, Builds/ taranmadı. Git, Unity ve build çalıştırılmadı. Silme/taşıma/yeniden adlandırma yapılmadı.

## Kanıt ve güven sınırı

- GUID kullanımı kendi .meta satırı çıkarılarak Assets içindeki tüm okunabilir kod/prefab/sahne/manifest/metinlerde arandı. Kodla ad yükleme ve t20-/m13- gibi runtime takma adları da tarandı.
- Resources içindeki bir kopya kaynakla SHA-256 eşitse kaynak **kalmalı**. GUID ile kullanılmıyor diye kopyanın üretim kaynağı silinebilir sayılmadı.
- Yalnız manifest/belge bağlantısı canlı oyunda kullanım kanıtı değildir; ayrı authoringReferences alanıdır. Dinamik ad birleştirme ve dış build araçları tümüyle kanıtlanamaz; şüpheli dosya kalmalı grubuna alındı.
- Task19/19B/19C ve iptal adayı taslaklar otomatik silinebilir sayılmadı: onay/yeniden üretilebilirlik kesin değilse arşiv önerildi. Referanslar, eğitim eli, onaylı aktif setler korunur.
- .task15-python ve .task16-python altlarında erişim reddi var. Bu ortamlar **kalmalı / ölçüm eksik**; bilinmeyen baytlar toplam kazanca dahil değil. Önbelleklerin silinmesi gelecekte art scriptleri için cache/bağımlılık kurulumunu gerektirebilir.
- MB = 1.000.000 bayt. Dosya toplamı çakışmaz. Klasör CSV'si recursive olduğundan satırları toplanmaz. Listeler mevcut tarama anının fotoğrafıdır; Claude eşzamanlı çalıştığı için uygulama öncesi yeniden kontrol gerekir.

## Her dosya ve her klasör

Tam döküm: [temizlik-final-dosyalar.csv](task22/temizlik-final-dosyalar.csv) — her dosya için yol, boyut, grup, oyun kullanımı, GUID, GUID/ad başvuruları, Resources hash eşleri ve gerekçe. [temizlik-final-klasorler.csv](task22/temizlik-final-klasorler.csv) — her erişilebilir klasörün boyutu ve grupları. [Erişim hataları](task22/temizlik-final-erisim-hatalari.json) erişilemeyen dizinleri ayrıca gösterir. Bu CSV'ler raporun ayrılmaz ekidir; burada aşağıda okunur karar özeti bulunur.

## Güvenle silinebilir

| Yol | Boyut (MB) | Neden |
|---|---:|---|
| `docs/art/.task15-pip-cache` (12 dosya; bu gruba düşen kısmı) | 20.910 | Yeniden üretilebilir tarayıcı/pip önbelleği; Assets runtime ad/GUID/hash kullanım kanıtı yok. Art araçları yeniden çalıştırılırsa cache/bağımlılık yeniden kurulur. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/.chrome-task6` (102 dosya; bu gruba düşen kısmı) | 4.685 | Yeniden üretilebilir tarayıcı/pip önbelleği; Assets runtime ad/GUID/hash kullanım kanıtı yok. Art araçları yeniden çalıştırılırsa cache/bağımlılık yeniden kurulur. Oyun kullanım kanıtlı dosya: 0. |

Grup toplamı: **25.595 MB**, 114 dosya. Ayrı dosyaların gerçek grup ve gerekçesi CSV’de.

## Arşive taşınabilir

| Yol | Boyut (MB) | Neden |
|---|---:|---|
| `docs/art/drafts` (1973 dosya; bu gruba düşen kısmı) | 975.718 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Menu` (276 dosya; bu gruba düşen kısmı) | 185.404 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/backups` (264 dosya; bu gruba düşen kısmı) | 164.098 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/corridor-victory` (247 dosya; bu gruba düşen kısmı) | 72.932 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task19d` (35 dosya; bu gruba düşen kısmı) | 42.137 | Eski taslak/onay geçmişi; Assets içinde ad/GUID/runtime hash kullanım kanıtı bulunmadı. Kaynak/onay geçmişi nedeniyle silme önerilmiyor. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task19c` (25 dosya; bu gruba düşen kısmı) | 28.749 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Review` (22 dosya; bu gruba düşen kısmı) | 20.323 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Gameplay` (194 dosya; bu gruba düşen kısmı) | 14.762 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task20` (10 dosya; bu gruba düşen kısmı) | 14.489 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Pieces` (492 dosya; bu gruba düşen kısmı) | 11.656 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/AnimationDemo` (18 dosya; bu gruba düşen kısmı) | 10.671 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Ornekler` (8 dosya; bu gruba düşen kısmı) | 7.813 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/ObstacleIntros` (10 dosya; bu gruba düşen kısmı) | 5.272 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/UI` (104 dosya; bu gruba düşen kısmı) | 5.139 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Characters` (4 dosya; bu gruba düşen kısmı) | 3.824 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Icon` (8 dosya; bu gruba düşen kısmı) | 2.877 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Defeat` (8 dosya; bu gruba düşen kısmı) | 1.977 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/PieceSkin` (24 dosya; bu gruba düşen kısmı) | 1.462 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/HardWarning` (4 dosya; bu gruba düşen kısmı) | 1.171 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Victory` (4 dosya; bu gruba düşen kısmı) | 0.841 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/reports` (6 dosya; bu gruba düşen kısmı) | 0.827 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task21` (1 dosya; bu gruba düşen kısmı) | 0.609 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/templates` (16 dosya; bu gruba düşen kısmı) | 0.143 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Inventory` (56 dosya; bu gruba düşen kısmı) | 0.054 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Audio` (4 dosya; bu gruba düşen kısmı) | 0.037 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/MissingScreens` (14 dosya; bu gruba düşen kısmı) | 0.036 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/ProductionReview.html` (1 dosya; bu gruba düşen kısmı) | 0.004 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Task4` (2 dosya; bu gruba düşen kısmı) | 0.003 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/ProductionReview.html.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Eş varlıkla birlikte değerlendirilir: Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Geçmiş inceleme değeri var; arşivlenirse belge yolları ayrıca güncellenmeli. Oyun kullanım kanıtlı dosya: 0. |

Grup toplamı: **1573.027 MB**, 3831 dosya. Ayrı dosyaların gerçek grup ve gerekçesi CSV’de.

## Kalmalı

| Yol | Boyut (MB) | Neden |
|---|---:|---|
| `Assets/Art/References` (61 dosya; bu gruba düşen kısmı) | 418.262 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Paintings` (199 dosya; bu gruba düşen kısmı) | 307.439 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Redesign_T22` (384 dosya; bu gruba düşen kısmı) | 220.092 | Task22 teslimi; henüz onaysız yeni aday, bu görevin silme önerisine girmez. Oyun kullanım kanıtlı dosya: 59. |
| `docs/art/drafts` (1368 dosya; bu gruba düşen kısmı) | 196.064 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 1318. |
| `docs/art/task22` (145 dosya; bu gruba düşen kısmı) | 122.827 | Task22 teslimi; henüz onaysız yeni aday, bu görevin silme önerisine girmez. Oyun kullanım kanıtlı dosya: 7. |
| `Assets/Art/Menu` (561 dosya; bu gruba düşen kısmı) | 118.898 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 265. |
| `Assets/Art/Gameplay` (514 dosya; bu gruba düşen kısmı) | 115.496 | Kullanım kanıtı bulunmadı fakat dinamik yükleme/onay durumu belirsiz; yalnız yoklukla silme güveni verilmez. Oyun kullanım kanıtlı dosya: 173. |
| `Assets/Art/Ornekler` (68 dosya; bu gruba düşen kısmı) | 61.408 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 33. |
| `docs/art/backups` (79 dosya; bu gruba düşen kısmı) | 37.047 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Üretim/referans kaynağı korunmalı. Oyun kullanım kanıtlı dosya: 59. |
| `Assets/Art/ObstacleIntros` (30 dosya; bu gruba düşen kısmı) | 18.339 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 12. |
| `docs/art/task20` (26 dosya; bu gruba düşen kısmı) | 17.457 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Üretim/referans kaynağı korunmalı. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Defeat` (26 dosya; bu gruba düşen kısmı) | 13.315 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 3. |
| `Assets/Art/Obstacles` (30 dosya; bu gruba düşen kısmı) | 10.849 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Üretim/referans kaynağı korunmalı. Oyun kullanım kanıtlı dosya: 9. |
| `Assets/Art/Pieces` (12 dosya; bu gruba düşen kısmı) | 10.252 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 7. |
| `docs/art/task21` (14 dosya; bu gruba düşen kısmı) | 10.193 | Kullanım kanıtı bulunmadı fakat dinamik yükleme/onay durumu belirsiz; yalnız yoklukla silme güveni verilmez. Oyun kullanım kanıtlı dosya: 5. |
| `docs/art/task19d` (19 dosya; bu gruba düşen kısmı) | 9.179 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Üretim/referans kaynağı korunmalı. Oyun kullanım kanıtlı dosya: 7. |
| `Assets/Art/Boosters` (21 dosya; bu gruba düşen kısmı) | 8.755 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Üretim/referans kaynağı korunmalı. Oyun kullanım kanıtlı dosya: 8. |
| `Assets/Art/Review` (16 dosya; bu gruba düşen kısmı) | 6.513 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 7. |
| `Assets/Art/UI` (170 dosya; bu gruba düşen kısmı) | 5.900 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 107. |
| `Assets/Art/HardWarning` (6 dosya; bu gruba düşen kısmı) | 5.315 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Reference` (10 dosya; bu gruba düşen kısmı) | 3.473 | Onaylı aktif set, kaynak referans, tablo veya eğitim varlığı; runtime başvurusu bulunmasa da koruma kararı. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Victory` (4 dosya; bu gruba düşen kısmı) | 2.516 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Icon` (7 dosya; bu gruba düşen kısmı) | 2.423 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 2. |
| `docs/art/references-menu-contact-sheet.png` (1 dosya; bu gruba düşen kısmı) | 2.314 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/TutorialHand` (20 dosya; bu gruba düşen kısmı) | 2.150 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 2. |
| `docs/art/.chrome-task6` (89 dosya; bu gruba düşen kısmı) | 2.049 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 89. |
| `Assets/Art/PieceSkin` (13 dosya; bu gruba düşen kısmı) | 1.192 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 7. |
| `docs/art/Task8-review.png` (1 dosya; bu gruba düşen kısmı) | 0.663 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Audio` (20 dosya; bu gruba düşen kısmı) | 0.471 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 9. |
| `docs/art/reports` (30 dosya; bu gruba düşen kısmı) | 0.271 | Kullanım kanıtı bulunmadı fakat dinamik yükleme/onay durumu belirsiz; yalnız yoklukla silme güveni verilmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Tutorial` (6 dosya; bu gruba düşen kısmı) | 0.181 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 2. |
| `docs/art/HANDOFF.md` (1 dosya; bu gruba düşen kısmı) | 0.097 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 1. |
| `docs/art/TEMIZLIK_ONERI.md` (1 dosya; bu gruba düşen kısmı) | 0.047 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/DECISIONS.md` (1 dosya; bu gruba düşen kısmı) | 0.032 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 1. |
| `docs/art/corridor-victory` (9 dosya; bu gruba düşen kısmı) | 0.031 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 5. |
| `docs/art/task19c` (7 dosya; bu gruba düşen kısmı) | 0.031 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Üretim/referans kaynağı korunmalı. Oyun kullanım kanıtlı dosya: 2. |
| `docs/art/DENETIM_2026-11-03.md` (1 dosya; bu gruba düşen kısmı) | 0.029 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/ASSET_IHTIYAC.md` (1 dosya; bu gruba düşen kısmı) | 0.027 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/t22_build_ai.py` (1 dosya; bu gruba düşen kısmı) | 0.025 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task15.py` (1 dosya; bu gruba düşen kısmı) | 0.024 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/export-task14.ps1` (1 dosya; bu gruba düşen kısmı) | 0.020 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task19.py` (1 dosya; bu gruba düşen kısmı) | 0.019 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/export-menu-task20.py` (1 dosya; bu gruba düşen kısmı) | 0.019 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task5.cjs` (1 dosya; bu gruba düşen kısmı) | 0.018 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task19d.py` (1 dosya; bu gruba düşen kısmı) | 0.017 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/TASK19_PARCALAR.md` (1 dosya; bu gruba düşen kısmı) | 0.015 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/menu-ui-task12.ps1` (1 dosya; bu gruba düşen kısmı) | 0.015 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task7.ps1` (1 dosya; bu gruba düşen kısmı) | 0.014 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task16-batch-4-prompts.json` (1 dosya; bu gruba düşen kısmı) | 0.014 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/t22_audit_ai.py` (1 dosya; bu gruba düşen kısmı) | 0.014 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/finalize-task20.py` (1 dosya; bu gruba düşen kısmı) | 0.013 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/DENETIM_T22_AI_EK.md` (1 dosya; bu gruba düşen kısmı) | 0.013 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task11-build.ps1` (1 dosya; bu gruba düşen kısmı) | 0.012 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task21.py` (1 dosya; bu gruba düşen kısmı) | 0.012 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task19c.py` (1 dosya; bu gruba düşen kısmı) | 0.012 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task16-batch-3-prompts.json` (1 dosya; bu gruba düşen kısmı) | 0.011 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task19b.py` (1 dosya; bu gruba düşen kısmı) | 0.011 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/gameplay-task12-palette.ps1` (1 dosya; bu gruba düşen kısmı) | 0.011 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/render-task20-preview.py` (1 dosya; bu gruba düşen kısmı) | 0.010 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/finalize-task21.py` (1 dosya; bu gruba düşen kısmı) | 0.010 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task9-toy-assets.ps1` (1 dosya; bu gruba düşen kısmı) | 0.010 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-corridor-continuity.py` (1 dosya; bu gruba düşen kısmı) | 0.010 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/preview-gameplay-task14-v2.ps1` (1 dosya; bu gruba düşen kısmı) | 0.009 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task17.py` (1 dosya; bu gruba düşen kısmı) | 0.009 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/t22_cleanup.py` (1 dosya; bu gruba düşen kısmı) | 0.008 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK19.md` (1 dosya; bu gruba düşen kısmı) | 0.008 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task16-batch-2-prompts.json` (1 dosya; bu gruba düşen kısmı) | 0.008 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/render-menu-task13.ps1` (1 dosya; bu gruba düşen kısmı) | 0.008 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/check-corridor-continuity.py` (1 dosya; bu gruba düşen kısmı) | 0.008 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/AnimationDemo` (6 dosya; bu gruba düşen kısmı) | 0.008 | Çalışma kaynağı/manifest veya belge başvurusu var; runtime kanıtı yok. Üretim/referans kaynağı korunmalı. Oyun kullanım kanıtlı dosya: 4. |
| `docs/art/build-task17-colors.py` (1 dosya; bu gruba düşen kısmı) | 0.007 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task16-batch-1-prompts.json` (1 dosya; bu gruba düşen kısmı) | 0.007 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/preview-gameplay-task14.ps1` (1 dosya; bu gruba düşen kısmı) | 0.007 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/STYLE_CARD.md` (1 dosya; bu gruba düşen kısmı) | 0.007 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/sync-menu-task13.ps1` (1 dosya; bu gruba düşen kısmı) | 0.007 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/t22_validate_ai.py` (1 dosya; bu gruba düşen kısmı) | 0.007 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/TASK19B_PROMPTS.json` (1 dosya; bu gruba düşen kısmı) | 0.007 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task10-build.ps1` (1 dosya; bu gruba düşen kısmı) | 0.006 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/TASK19C_NOTLAR.md` (1 dosya; bu gruba düşen kısmı) | 0.006 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/STIL_REHBERI_RM.md` (1 dosya; bu gruba düşen kısmı) | 0.006 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/prepare-production-exports.ps1` (1 dosya; bu gruba düşen kısmı) | 0.006 | Kullanım kanıtı bulunmadı fakat dinamik yükleme/onay durumu belirsiz; yalnız yoklukla silme güveni verilmez. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK22.md` (1 dosya; bu gruba düşen kısmı) | 0.006 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/SECIM.html` (1 dosya; bu gruba düşen kısmı) | 0.006 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task16.ps1` (1 dosya; bu gruba düşen kısmı) | 0.006 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-task6.cjs` (1 dosya; bu gruba düşen kısmı) | 0.006 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/TASK14_EXPORT.md` (1 dosya; bu gruba düşen kısmı) | 0.006 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/REFERANS_DURUMU.md` (1 dosya; bu gruba düşen kısmı) | 0.005 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/finalize-task16.ps1` (1 dosya; bu gruba düşen kısmı) | 0.005 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/GECE_LOG.md` (1 dosya; bu gruba düşen kısmı) | 0.005 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/validate-task14-export.ps1` (1 dosya; bu gruba düşen kısmı) | 0.005 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/TASK15_EXPORT.md` (1 dosya; bu gruba düşen kısmı) | 0.005 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK19C.md` (1 dosya; bu gruba düşen kısmı) | 0.005 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/TASK19B_PILOT.md` (1 dosya; bu gruba düşen kısmı) | 0.005 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/prepare-task8.ps1` (1 dosya; bu gruba düşen kısmı) | 0.005 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/menu-task13-spec.json` (1 dosya; bu gruba düşen kısmı) | 0.005 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/PALETTE.md` (1 dosya; bu gruba düşen kısmı) | 0.004 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 1. |
| `docs/art/CODEX_TASK21.md` (1 dosya; bu gruba düşen kısmı) | 0.004 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/corridor-victory-prompts.json` (1 dosya; bu gruba düşen kısmı) | 0.004 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK19B.md` (1 dosya; bu gruba düşen kısmı) | 0.004 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/create-play-button-v2.ps1` (1 dosya; bu gruba düşen kısmı) | 0.004 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-menu-task13.ps1` (1 dosya; bu gruba düşen kısmı) | 0.004 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/LEVEL_GECISI.md` (1 dosya; bu gruba düşen kısmı) | 0.004 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK19D.md` (1 dosya; bu gruba düşen kısmı) | 0.004 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/t22_prepare.py` (1 dosya; bu gruba düşen kısmı) | 0.003 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/write-task14-handoff.ps1` (1 dosya; bu gruba düşen kısmı) | 0.003 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/INCELEME.html` (1 dosya; bu gruba düşen kısmı) | 0.003 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK15.md` (1 dosya; bu gruba düşen kısmı) | 0.003 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/complete-task16.ps1` (1 dosya; bu gruba düşen kısmı) | 0.003 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/IKON_SECIMI.md` (1 dosya; bu gruba düşen kısmı) | 0.003 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/corridor-victory-v2-prompts.json` (1 dosya; bu gruba düşen kısmı) | 0.003 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK20.md` (1 dosya; bu gruba düşen kısmı) | 0.003 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/TASK14_FONT.md` (1 dosya; bu gruba düşen kısmı) | 0.002 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task14-preview-board.json` (1 dosya; bu gruba düşen kısmı) | 0.002 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK16.md` (1 dosya; bu gruba düşen kısmı) | 0.002 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/verify-task5.ps1` (1 dosya; bu gruba düşen kısmı) | 0.002 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/EKSIKLER.md` (1 dosya; bu gruba düşen kısmı) | 0.002 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task8-composite-preview.ps1` (1 dosya; bu gruba düşen kısmı) | 0.002 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK17.md` (1 dosya; bu gruba düşen kısmı) | 0.002 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/CODEX_TASK18.md` (1 dosya; bu gruba düşen kısmı) | 0.002 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task16-batch-1-inputs.json` (1 dosya; bu gruba düşen kısmı) | 0.001 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task16-batch-2-inputs.json` (1 dosya; bu gruba düşen kısmı) | 0.001 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task16-batch-3-inputs.json` (1 dosya; bu gruba düşen kısmı) | 0.001 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/task16-batch-4-inputs.json` (1 dosya; bu gruba düşen kısmı) | 0.001 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/corridor-continuity-prompts.json` (1 dosya; bu gruba düşen kısmı) | 0.001 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/check-task14-font.ps1` (1 dosya; bu gruba düşen kısmı) | 0.001 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/verify-task6.ps1` (1 dosya; bu gruba düşen kısmı) | 0.001 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/encode-task16.py` (1 dosya; bu gruba düşen kısmı) | 0.001 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/STIL_ADAYLARI.md` (1 dosya; bu gruba düşen kısmı) | 0.001 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/corridor-victory-input.json` (1 dosya; bu gruba düşen kısmı) | 0.000 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-corridor-victory-v2.py` (1 dosya; bu gruba düşen kısmı) | 0.000 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/build-corridor-victory.py` (1 dosya; bu gruba düşen kısmı) | 0.000 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/check-corridor-victory-v2.py` (1 dosya; bu gruba düşen kısmı) | 0.000 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/AnimationDemo.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Audio.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Boosters.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Characters.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Kullanım kanıtı bulunmadı fakat dinamik yükleme/onay durumu belirsiz; yalnız yoklukla silme güveni verilmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Defeat.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Gameplay.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/HardWarning.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Icon.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Inventory.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Menu.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/MissingScreens.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Assets kod/serialized/ad/GUID başvurusu veya Resources içeriğiyle kullanım/bağımlılık kanıtı var. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/ObstacleIntros.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Obstacles.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Ornekler.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Paintings.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Pieces.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/PieceSkin.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Redesign_T22.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Reference.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/References.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Review.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/Task4.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Kullanım kanıtı bulunmadı fakat dinamik yükleme/onay durumu belirsiz; yalnız yoklukla silme güveni verilmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Tutorial.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/TutorialHand.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/UI.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 0. |
| `Assets/Art/Victory.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Klasör .meta dosyası; içinde kalması gereken kaynaklar var, tek başına arşivlenmez. Oyun kullanım kanıtlı dosya: 1. |
| `Assets/Art/prepare-production-exports.ps1.meta` (1 dosya; bu gruba düşen kısmı) | 0.000 | Eş varlıkla birlikte değerlendirilir: Kullanım kanıtı bulunmadı fakat dinamik yükleme/onay durumu belirsiz; yalnız yoklukla silme güveni verilmez. Oyun kullanım kanıtlı dosya: 0. |
| `docs/art/.gitignore` (1 dosya; bu gruba düşen kısmı) | 0.000 | Kök görev/stil/palet/karar/handoff belgesi veya üretim scripti; güncel üretim bağlamı korunmalı. Oyun kullanım kanıtlı dosya: 0. |

Grup toplamı: **1732.248 MB**, 4098 dosya. Ayrı dosyaların gerçek grup ve gerekçesi CSV’de.

## Öncelikli eski klasörler — ayrıntılı karar özeti

Bu tablo yukarıdaki grupların alt kırılımıdır; toplam kazanca bir daha eklenmez. Aynı klasörde aktif kaynak varsa yalnız arşiv grubuna giren dosyalar arşiv adayıdır; tam dosya CSV'si belirleyicidir.

| Yol | Okunabilen MB | Güvenli cache MB | Arşiv adayı MB | Kalmalı MB | Neden |
|---|---:|---:|---:|---:|---|
| `Assets/Art/Menu/Task19` | 48.272 | 0.000 | 46.471 | 1.801 | Geçmiş taslak/onay referansı; aktif bağımlılığı olan alt dosya korunur. |
| `Assets/Art/Menu/Task19B` | 28.643 | 0.000 | 28.630 | 0.012 | Geçmiş taslak/onay referansı; aktif bağımlılığı olan alt dosya korunur. |
| `Assets/Art/Menu/Task19C` | 12.753 | 0.000 | 12.753 | 0.000 | Geçmiş taslak/onay referansı; aktif bağımlılığı olan alt dosya korunur. |
| `Assets/Art/Menu/Task19D` | 30.439 | 0.000 | 30.439 | 0.000 | Geçmiş taslak/onay referansı; aktif bağımlılığı olan alt dosya korunur. |
| `docs/art/drafts` | 1171.782 | 0.000 | 975.718 | 196.064 | Geçmiş taslak/onay referansı; aktif bağımlılığı olan alt dosya korunur. |
| `docs/art/backups` | 201.145 | 0.000 | 164.098 | 37.047 | Geçmiş taslak/onay referansı; aktif bağımlılığı olan alt dosya korunur. |
| `docs/art/.task15-python` | 0.000 | 0.000 | 0.000 | 0.000 | Erişim reddi veren alt dizinler var; bilinen boyut alt sınırdır, kaldırma önerilmedi. |
| `docs/art/.task16-python` | 0.000 | 0.000 | 0.000 | 0.000 | Erişim reddi veren alt dizinler var; bilinen boyut alt sınırdır, kaldırma önerilmedi. |
| `docs/art/.task15-pip-cache` | 20.910 | 20.910 | 0.000 | 0.000 | Yeniden üretilebilir araç önbelleği; oyunda kullanım kanıtı yok. |
| `docs/art/.chrome-task6` | 6.734 | 4.685 | 0.000 | 2.049 | Yeniden üretilebilir araç önbelleği; oyunda kullanım kanıtı yok. |

## Kazanç hesabı

- Güvenle silinebilir, yeniden kurulabilir önbellek: **25.595 MB**.
- Arşive taşınabilir tarihçe: **1573.027 MB**. Aynı diskte başka klasöre taşımak alan kazandırmaz; ancak dış arşivleme sonrası ayrı onaylı kaldırma ile bu miktar kazanılır.
- **Toplam olası alan kazanımı: 1598.622 MB** (güvenli cache + dış arşivleme senaryosu; erişilemeyen Python içeriği hariç).
- **Bu görevde gerçekte kazanılan: 0 MB.** Hiçbir dosya silinmedi veya taşınmadı.

PM dosya listesini kontrol edip Zeyd onayladığında ayrı bir görev uygulanabilir. Bu rapor silme yetkisi veya silme komutu içermez.

Son kanıt kesimi: `task22/temizlik-final-ozet.json`. Sonradan üretilen teslim metadata/kopya tabloları bu tarama anının dışında kalır; hepsi kalmalı teslim dosyalarıdır, kazanç hesabına dahil değildir.
