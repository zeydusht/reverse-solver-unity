# PM_97 denetimi: model v2 LOO, repodaki docs/analiz/v2 tablolarından (cwd docs/analiz/v2). python -I <bu dosya>
import csv, math, numpy as np
F={}
for r in csv.DictReader(open('level_features_v2.csv',encoding='utf8')):
    if r['set_name']=='redesign-v4': F[r['level_id']]=r
D={r['level_id']:r for r in csv.DictReader(open('irt_level_d.csv'))}
ids=[i for i in D if i in F]
sg=lambda z:1/(1+np.exp(z)); lg=lambda p: np.log((p+0.01)/(1.01-p))
def f(x,d=1.0):
    try: return float(x)
    except: return d
rows=[]
for i in ids:
    r=F[i]; hb=f(r['bombs'],0)>0
    pa=f(r['p_survive_aware']); pr=f(r['p_survive_rand'])
    Bw=-lg(.85*pa+.15*pr) if hb else -lg(1.0)
    nc=f(r.get('narrow_by_chain',''),0)/f(r['pieces'])
    rows.append((i,int(D[i]['pt1_slot']),int(D[i]['n']),float(D[i]['d_obs']),Bw,math.log(f(r['sec_per_move'])),nc,hb,int(f(r['locks'],0)>0),int(f(r['chains'],0)>0),int(f(r['walls'],0)>0)))
X=np.array([[1,r[4],r[5],r[6]] for r in rows]); d=np.array([r[3] for r in rows]); n=np.array([r[2] for r in rows]); p=sg(d)
def fit(idx):
    M=X[idx]; W=np.diag(n[idx]*p[idx]*(1-p[idx])+1); R=np.eye(4)*.3; R[0,0]=0
    return np.linalg.solve(M.T@W@M+R,M.T@W@d[idx])
b=fit(np.arange(len(rows))); print('coef',np.round(b,2))
loo=np.array([sg(X[k]@fit(np.array([j for j in range(len(rows)) if j!=k]))) for k in range(len(rows))])
err=loo-p
print(f'n={len(rows)} LOO MAE {np.abs(err).mean()*100:.1f} puan, RMSE {np.sqrt((err**2).mean())*100:.1f}, max {np.abs(err).max()*100:.0f}; in-sample MAE {np.abs(sg(X@b)-p).mean()*100:.1f}')
print(f'taban (hep ortalama) MAE {np.abs(p-p.mean()).mean()*100:.1f}')
# by type
def grp(name,mask):
    if mask.sum(): print(f'  {name:22s} n={mask.sum():2d} MAE {np.abs(err[mask]).mean()*100:5.1f}  ort. sapma {err[mask].mean()*100:+5.1f}')
hb=np.array([r[7] for r in rows]); slot=np.array([r[1] for r in rows])
grp('bombasız',~hb); grp('bombalı',hb); grp('gerçek ilk deneme <%50',p<.5); grp('gerçek >=%80',p>=.8)
grp('slot 1-10',slot<=10); grp('slot 11-20',(slot>10)&(slot<=20)); grp('slot 21-40',slot>20)
print('en büyük 6:'); 
for k in np.argsort(-np.abs(err))[:6]: print(f'  {rows[k][0]} gerçek {p[k]*100:.0f} tahmin {loo[k]*100:.0f}')
# calibration of interval: share within +-10.7
print('±10,7 içinde:', f'{(np.abs(err)<=.107).mean()*100:.0f}%')
