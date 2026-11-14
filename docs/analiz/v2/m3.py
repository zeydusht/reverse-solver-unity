import pandas as pd, numpy as np, itertools, json
P=pd.read_pickle('P.pkl')
n=pd.Series({o['slot']:o['n'] for o in json.load(open('/home/claude/an/lvl.json'))})
P['n']=n
sg=lambda z:1/(1+np.exp(z))
P['p']=sg(P.d)
lg=lambda p: np.log((p+0.01)/(1.01-p))
def feats(w):
    P['Bw']=np.where(P.bombs>0,-lg(w*P.p_survive_aware.fillna(1)+(1-w)*P.p_survive_rand.fillna(1)),-lg(1.0))
cands=['Bw','tight','early','choke','dvs','lspm','lpress','amb','nl','nc','nw','lp']
def fitw(cols,idx,lam=0.3):
    Z=P.loc[idx]; M=np.column_stack([np.ones(len(Z))]+[Z[c].values for c in cols])
    W=np.diag(Z.n*Z.p*(1-Z.p)+1.0)
    R=np.eye(M.shape[1])*lam; R[0,0]=0
    return np.linalg.solve(M.T@W@M+R,M.T@W@Z.d.values)
def pr(b,cols,Z): return np.column_stack([np.ones(len(Z))]+[Z[c].values for c in cols])@b
def loo(cols):
    e=[]
    for s in P.index:
        idx=[i for i in P.index if i!=s]; b=fitw(cols,idx); e.append(abs(sg(pr(b,cols,P.loc[[s]])[0])-P.loc[s,'p']))
    return np.mean(e)
best=[]
for w in [0.3,0.5,0.7,0.85]:
    feats(w)
    for k in range(1,5):
        for c in itertools.combinations(cands,k):
            best.append((loo(list(c)),w,c))
best.sort()
for r,w,c in best[:12]: print(round(r,3),w,c)
# old model in p-MAE
P['lspm0']=np.log(P.sec_per_move)
