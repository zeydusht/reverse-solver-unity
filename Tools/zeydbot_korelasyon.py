import csv,sys
sys.path.insert(0,'Tools'); import zeydbot_model as zm
D='docs/analiz/zeydbot/'
F={r['level_id']:r for r in csv.DictReader(open(D+'bolum_ozellik.csv',encoding='utf-8'))}
A=[r for r in csv.DictReader(open(D+'denemeler.csv',encoding='utf-8'))]
first={r['level_id']:r for r in A if r['attempt']=='1'}
sc={r['level_id']:int(r['zorluk_puani']) for r in A if r['zorluk_puani']}
L=[l for l in sorted(first) if l in sc]
def f(l,k,d=10.0):
    v=F[l][k]; return float(v) if v else d
feats={'rastgele_kazanma':lambda l:f(l,'rastgele_kazanma'),'baslangic_en_dar_pay':lambda l:f(l,'baslangic_en_dar_pay'),
'yol_tek_dogru':lambda l:f(l,'yol_tek_dogru'),'yol_tuzakli_hamle':lambda l:f(l,'yol_tuzakli_hamle'),'yol_ort_secenek':lambda l:f(l,'yol_ort_secenek'),
'm2_en_cok_serbest':lambda l:f(l,'m2_en_cok_serbest'),'en_derin_kazi':lambda l:f(l,'en_derin_kazi',0),'bomba':lambda l:f(l,'bomba'),'parca':lambda l:f(l,'parca'),
'tek_dogru_orani':lambda l:f(l,'yol_tek_dogru')/f(l,'en_kisa_cozum'),'m4':lambda l:f(l,'m4'),'civi':lambda l:f(l,'civi')}
print('n=',len(L))
for k,fn in sorted(feats.items(),key=lambda kv:-abs(zm.spearman([kv[1](l) for l in L],[sc[l] for l in L]))):
    x=[fn(l) for l in L]
    loss=[0 if first[l]['result']=='win' else 1 for l in sorted(first)]
    print(f"{k:22s} puan rho={zm.spearman(x,[sc[l] for l in L]):+.2f}  ilk-kayıp rho={zm.spearman([fn(l) for l in sorted(first)],loss):+.2f}  süre-payı rho={zm.spearman([fn(l) for l in sorted(first)],[float(first[l]['dur'])/(float(first[l]['dur'])+float(first[l]['left_s'])) for l in sorted(first)]):+.2f}")
for l in ['Z12','Z13','Z18','Z07','Z08','Z16','Z17']:
    r=first[l]; print(l,F[l]['tur'],r['result'],'puan',sc.get(l),'dur',r['dur'],'think ort/med/max',r['think_ort_ms'],r['think_medyan_ms'],r['think_max_ms'],'touch',r['touch'],'| tek_dogru',F[l]['yol_tek_dogru'],'tuzakli',F[l]['yol_tuzakli_hamle'],'ort secenek',F[l]['yol_ort_secenek'],'pay',F[l]['baslangic_en_dar_pay'],'kazi',F[l]['en_derin_kazi'],'m2',F[l]['m2_en_cok_serbest'],'civi',F[l]['civi'],'sure',F[l]['sure_limiti'])
