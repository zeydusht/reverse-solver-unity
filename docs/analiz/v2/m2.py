import pandas as pd, numpy as np, json, itertools
F=pd.read_csv('level_features_v2.csv')
J=json.load(open('/home/claude/an/irt.json')); d=pd.Series({int(k):v for k,v in J['d'].items()})
ver={10:2,14:2}
P=F[F.set_name=='redesign-v4'].copy()
P=P[P.apply(lambda r: r.level_version==ver.get(r.slot,r.level_version),axis=1)].set_index('slot').sort_index()
P['d']=d
hb=P.bombs>0
lg=lambda p: np.log((p+0.01)/(1.01-p))
P['B_aware']=np.where(hb,-lg(P.p_survive_aware.fillna(1)),-lg(1.0))
P['B_rand']=np.where(hb,-lg(P.p_survive_rand.fillna(1)),-lg(1.0))
P['B_mix']=np.where(hb,-lg(0.5*P.p_survive_aware.fillna(1)+0.5*P.p_survive_rand.fillna(1)),-lg(1.0))
P['tight']=np.where(hb,np.exp(-P.slack_true_min.clip(lower=0)/2),0)
def fmin(x):
    try: return min(float(v) for v in str(x).split(';'))
    except: return 999
P['fmin']=P.fuse.apply(fmin)
P['early']=np.clip(np.where(hb,1-P.fmin/P.pieces,0),0,None)
P['choke']=P.choke_n
P['dvs']=P.distract_vs_slack_max.fillna(0)
P['lspm']=np.log(P.sec_per_move)
# predicted human seconds: pieces*1.34 + extra for obstacle narrow steps (from replay 3c)
P['need']=P.pieces*1.34+P.narrow_by_lock*1.23+P.narrow_by_chain*0.73+P.narrow_by_wall*0.70
P['lpress']=np.log(P.need/P.timer_s)
P['nl']=P.narrow_by_lock/P.pieces; P['nc']=P.narrow_by_chain/P.pieces; P['nw']=P.narrow_by_wall/P.pieces
P['amb']=P.dir_ambiguity
P['lp']=np.log(P.pieces)
cands=['B_aware','B_rand','B_mix','tight','early','choke','dvs','lspm','lpress','amb','nl','nc','nw','lp']
X=P
def fit(cols,lam,idx):
    Z=X.loc[idx]; M=np.column_stack([np.ones(len(Z))]+[Z[c].values for c in cols])
    R=np.eye(M.shape[1])*lam; R[0,0]=0
    return np.linalg.solve(M.T@M+R,M.T@Z.d.values)
def pr(b,cols,Z): return np.column_stack([np.ones(len(Z))]+[Z[c].values for c in cols])@b
def loo(cols,lam=0.5):
    e=[]
    for s in X.index:
        idx=[i for i in X.index if i!=s]; b=fit(cols,lam,idx); e.append(pr(b,cols,X.loc[[s]])[0]-X.loc[s,'d'])
    return np.sqrt(np.mean(np.square(e)))
res=[]
for k in range(1,5):
    for c in itertools.combinations(cands,k): res.append((loo(list(c)),c))
res.sort()
print('old model loo 1.133 ; baseline 2.18')
for r,c in res[:15]: print(round(r,3),c)
P.to_pickle('P.pkl')
