from t23_common import *
SOURCE='docs/qa/2026-11-03-tam/tr_375x667_won12_8_bolumler.png'
def make(v):
 t=Trial('01-bolumler',v,'Bölümler · basamağa oturan düğümler',SOURCE,'LevelsMap','Task20/node-* + LevelsMap numara/rozet/temas yerleşimi')
 cur=t.use('node-current','Assets/Art/Menu/Task20/node-current.png',(66,66));done=t.use('node-complete','Assets/Art/Menu/Task20/node-complete.png',(54,54));locked=t.use('node-locked','Assets/Art/Menu/Task20/node-locked-base.png',(54,54));hard=t.use('node-hard-locked','Assets/Art/Menu/Task20/node-hard-base.png',(54,54));bubble=t.use('play-bubble','Assets/Art/Redesign_T22/110-ai-harita/play-bubble-@3x.png',(84 if v=='B' else 88,44 if v=='B' else 48),hit=[84 if v=='B' else 88,44 if v=='B' else 48]);check=t.use('complete-check','Assets/Art/Menu/Task21/check-badge.png',(18,18));lock=t.use('lock-badge','Assets/Art/Menu/Task20/icon-lock.png',(20 if v=='B' else 16,22 if v=='B' else 18));avatar=t.use('current-avatar','Assets/Art/Menu/Task20/avatar-blue.png',(34,34));t.use('hard-marker','Assets/Art/Menu/Task20/baron-head.png',(25,29))
 shadow=Image.new('RGBA',(150,39));d=ImageDraw.Draw(shadow);d.ellipse((9,9,141,30),fill=(11,42,54,110 if v=='B' else 75));shadow=shadow.filter(ImageFilter.GaussianBlur(5));t.save('node-contact-shadow',shadow,'Basamağa değen dar temas gölgesi; ayrı katman',source='Task23 procedural soft UI shadow')
 before=art(SOURCE).resize((1125,2001),R);bg=art('Assets/Art/Menu/Task20/scene-levels.png').resize(before.size,R);tile=art('Assets/Art/Menu/Task20/levels-path-repeat.png').resize((256,512),R);small=before.resize((375,667),R)
 samples=[(x,y) for x in [177,190,203] for y in range(125,540,17)];best=(float('inf'),0)
 for offset in range(512):
  score=count=0
  for x,y in samples:
   a=tile.getpixel((round(x-59.5),(y+offset)%512));b=small.getpixel((x,y))
   if a[3]>240:score+=sum((a[k]-b[k])**2 for k in range(3));count+=1
  if count and score/count<best[0]:best=(score/count,offset)
 tile3=tile.resize((768,1536),R)
 for y in range(-512,1180,512):bg.alpha_composite(tile3,(round(59.5*3),round((y-best[1])*3)))
 full=before.copy();box=(0,282,1125,1752);full.paste(bg.crop(box),box[:2])
 anchors=[]
 for i in range(11,18):
  x=123 if i%2 else 251;y=416+(13-i)*76+(2 if v=='B' else 0);iscurrent=i==13;unlocked=i<13;a=cur if iscurrent else done if unlocked else locked;sz=66 if iscurrent else 54
  paste(full,shadow,(x-25,y+sz/2-4,50,13),True);paste(full,a,(x-sz/2,y-sz/2,sz,sz),True)
  t.text.append(txt(full,str(i),x,y+1,24 if iscurrent else 20 if v=='B' else 18,INK if iscurrent else CREAM,not iscurrent,sz-10))
  if i>13:paste(full,lock,(x+sz*.18,y+sz*.18,lock.width/3,lock.height/3))
  elif i<13:paste(full,check,(x+sz*.18,y+sz*.18,18,18))
  if iscurrent:
   bw,bh=(84,44) if v=='B' else(88,48);by=299 if v=='B' else 297;paste(full,bubble,(x-bw/2,by,bw,bh),True);t.text.append(txt(full,'Oyna',x,by+bh*.43,17,CREAM,True,bw-13,True));paste(full,avatar,(x-17,349,34,34))
  anchors.append({'level':i,'centerPt':[x,y],'sizePt':sz,'numberInside':True,'badge':'lock-bottom-right' if i>13 else 'check-bottom-right' if i<13 else None,'contactY':y+sz/2})
 # Keep the original stationary title and nav (including selected icon overhang).
 full.paste(before.crop((0,1698,1125,2001)),(0,1698));patch=full.crop((45,282,1080,1698));t.notes+=['PM36 bölüm4 esas: numaralar içeride, kilit sağ altta, bitende tik. Altın güncel ve turkuaz biten kaynaklar korunur.','Temas gölgesi2pt kalın; kalın dış ışımayla havada gösterilmez. Nihai merkezler ve basamak fazı entegrasyon notundadır.','Zor kilitli durum aynı koyu yüzey + onaylı kırmızı çerçeve; aktif zor durumda altın yüzey ve ayrı Baron işareti kullanılmalı.']
 record=t.finish(patch,(15,94,345,472),{'anchors':anchors,'pathRepeatPhasePt':best[1],'variantDifference':'A:18pt/16pt kilit, B:20pt/20pt kilit, daha tok temas ve84×44 Oyna'})
 full.save(t.path/'harita-375x667.png')
 # Standalone state plate includes the hard node and non-color status indicators.
 plate=Image.new('RGBA',(1005,330));
 for i,(a,label) in enumerate([(cur,'Şimdi'),(done,'Bitti'),(locked,'Kilitli'),(hard,'Zor')]):
  paste(plate,a,(18+i*82,8,60,60),True);txt(plate,str(13+i),48+i*82,39,21,INK if i==0 else CREAM,i!=0,48)
  if i==1:paste(plate,check,(63+i*82,51,18,18))
  if i>=2:paste(plate,lock,(63+i*82,49,20,22))
  txt(plate,label,48+i*82,91,14,maxw=75)
 plate.save(t.path/'durumlar.png');return record
if __name__=='__main__':
 rows=[make(v) for v in ['A','B']];(WORK/'merdiven-denemeleri.json').write_text(json.dumps(rows,ensure_ascii=False,indent=2),encoding='utf-8');contact([OUT/'DENEMELER'/v/'01-bolumler'/'onizleme.png' for v in ['A','B']],WORK/'merdiven-A-B.jpg',cols=2,cw=520,ch=800);print('Map A/B ready; not selected before visual review')
