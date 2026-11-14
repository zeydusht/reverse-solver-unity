exec(open('./load.py').read())
R=pd.read_csv('./reg_out.csv').set_index('slot')
B=pd.DataFrame([r.split(',') for r in open('./bex.txt').read().split(';')],columns='level_id,piece,fuse0,n,q10,q50,q90'.split(','))
for c in B.columns[1:]: B[c]=pd.to_numeric(B[c])
B['hs50']=B.fuse0-B.q50; B['hs10']=B.fuse0-B.q10
g=B.groupby('level_id').agg(hs50=('hs50','min'),hs10=('hs10','min')).reset_index()
X=R.reset_index().merge(FF[['slot','slack_min','bombs','fuse_min','pieces']],on='slot').merge(g,on='level_id',how='left')
X=X[(X.bombs>0)&(~X.level_id.isin(['Z21','Z24']))]
print(X[['slot','level_id','slack_min','hs10','hs50','p_irt','res_']].round(2).to_string(index=False))
print('corr d~listed slack',round(np.corrcoef(X.slack_min,X.d)[0,1],2),' d~hs10',round(np.corrcoef(X.hs10,X.d)[0,1],2),' d~hs50',round(np.corrcoef(X.hs50,X.d)[0,1],2))
print('corr residual~hs50',round(np.corrcoef(X.hs50,X.res_)[0,1],2))
