from t23_common import *
def select(folder,variant,reason,qa_notes=None):
 a=OUT/'DENEMELER/A'/folder;b=OUT/'DENEMELER/B'/folder
 assert (a/'deneme.json').exists() and (b/'deneme.json').exists(),'At least two meaningful trials required'
 chosen=OUT/'DENEMELER'/variant/folder;target=OUT/'NIHAI'/folder;target.mkdir(exist_ok=True)
 info=json.loads((chosen/'deneme.json').read_text(encoding='utf-8'))
 for p in chosen.iterdir():
  if p.is_file():shutil.copyfile(p,target/p.name)
 decision={'family':folder,'chosen':variant,'reason':reason,'reviewedAt':datetime.now().astimezone().isoformat(timespec='seconds'),'trials':['DENEMELER/A/'+folder,'DENEMELER/B/'+folder],'qaNotes':qa_notes or [],'items':info['assets']}
 (target/'secim.json').write_text(json.dumps(decision,ensure_ascii=False,indent=2),encoding='utf-8')
 (target/'NOT.md').write_text(f'# {folder} — {info["title"]}\n\nTek nihai aday: **{variant}**. Oyuna bağlanmadı.\n\n## İki deneme ve seçim\n\n{reason}\n\nA ve B denemelerinin bitmapleri, onizleme/once-sonra ve deneme.json dosyaları `../../DENEMELER/` altında korunur. Ana inceleme yalnız bu seçimi gösterir.\n\n## Değişim / entegrasyon\n\n- Sınıf: `{info["ui"]}`.\n- Yerine önerilen kaynak/anahtar: `{info["target"]}`.\n- Önce kaynağı: `{info["source"]}`.\n- Her runtime adayının@3x boyutu, mantıksal boyutu, L,T,R,B9-slice kenarı ve SHA-256 değeri `deneme.json` içinde. Unity aktarımında L,B,R,T sırasına çevrilir. Daireler ve karakter sahneleri9-slice yapılmaz.\n- Yazılar yalnız önizlemede; ayrı Nunito Black metin katmanıdır. PNG içine oyun metni gömülmez.\n\n## Kalite / sınırlar\n\n'+ '\n'.join('- '+x for x in info.get('notes',[])+(qa_notes or []))+'\n\nKod ve sahne/prefab değiştirilmedi. Gerçek telefon/Unity testi yapılmadı; grafik ve mantıksal ölçü doğrulaması yapıldı.\n',encoding='utf-8')
 return decision
if __name__=='__main__':
 import sys
 if sys.argv[1]=='map':
  d=select('01-bolumler','B','A18pt numara ve16pt kilitle denendi. B20pt numara,20×22pt kilit ve84×44pt Oyna ile küçük ekranda daha rahat okunuyor; basamak teması daha tok. İkisinde de güncel düğüm altın, biten turkuaz, kapalı koyu; merkezde yalnız numara var. B seçildi.',['Kilidin sağ alt rozet oluşu PM36 bölüm4 ile sabitlenmiştir; eski büyük merkez kilit kaynakları bu finale alınmadı.','Her düğümün temas y koordinatı ve örnek merdiven tekrarı deneme.json anchors alanında; gerçek scroll katmanına aynen kopyalama yerine ilgili basamak fazıyla eşleştirilmelidir.','54pt gövde dokunma hedefi en az54×54, Oyna en az84×44; rozetler bağımsız düğme değildir.']);print('Map final ready')
