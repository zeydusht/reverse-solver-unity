exec(open('./load.py').read())
from scipy.optimize import minimize
D=A[A.res!='q'].copy()
D['y']=(D.res=='w').astype(int)
pids=sorted(D.pid.unique()); pi={p:i for i,p in enumerate(pids)}
slots=sorted(D.slot.unique()); si={s:i for i,s in enumerate(slots)}
P=np.array([pi[p] for p in D.pid]); S=np.array([si[s] for s in D.slot])
K=np.log(D.attempt.values)
y=D.y.values
nP,nS=len(pids),len(slots)
def unpack(x): return x[:nP],x[nP:nP+nS],x[-1]
def nll(x):
    th,d,al=unpack(x)
    z=th[P]-d[S]+al*K
    p=1/(1+np.exp(-z))
    ll=np.sum(y*np.log(p+1e-12)+(1-y)*np.log(1-p+1e-12))
    return -(ll-0.5*np.sum(th**2)-0.5*np.sum(d**2)/9)
def grad(x):
    th,d,al=unpack(x); z=th[P]-d[S]+al*K; p=1/(1+np.exp(-z)); r=y-p
    gth=np.bincount(P,r,nP)-th; gd=-np.bincount(S,r,nS)-d/9; gal=np.sum(r*K)
    return -np.concatenate([gth,gd,[gal]])
r=minimize(nll,np.zeros(nP+nS+1),jac=grad,method='L-BFGS-B')
th,d,al=unpack(r.x)
print('learning alpha per log(attempt):',round(al,2))
# reference: average player (theta=0) first attempt win
T=FF.set_index('slot')
res=[]
for s in slots:
    a1=A[(A.slot==s)&(A.attempt==1)]
    mean_th=np.mean([th[pi[p]] for p in a1.pid if p in pi])
    res.append((s,T.loc[s,'level_id'],round(1/(1+np.exp(d[si[s]])),2),round((a1.res=='w').mean(),2),round(mean_th,2),round(d[si[s]],2)))
import json
json.dump({'theta':{int(p):float(th[pi[p]]) for p in pids},'d':{int(s):float(d[si[s]]) for s in slots},'alpha':float(al)},open('./irt.json','w'))
print('slot id  avgPlayerWin1  rawWin1  meanSkillOfReachers  d')
for x in res: print(*x)
print('theta sd',round(np.std(th),2))
