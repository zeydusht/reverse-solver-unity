const fs=require('fs'),path=require('path'),http=require('http'),os=require('os'),{spawn}=require('child_process');
const out=path.resolve(__dirname,'..'), root=path.resolve('Builds/publish/br');
const sleep=ms=>new Promise(r=>setTimeout(r,ms));
let chrome,ws,server,control,profile,cdp,ev,tap,drag,shot,record,base,logs=[];
async function main(){
server=http.createServer((req,res)=>{let f=path.resolve(root,'.'+decodeURIComponent(new URL(req.url,'http://x').pathname));if(!f.startsWith(root+path.sep)&&f!==root){res.writeHead(403);return res.end();}if(f===root)f=path.join(root,'index.html');const types={'.html':'text/html','.js':'application/javascript','.css':'text/css','.json':'application/json','.wasm':'application/wasm','.mp4':'video/mp4','.png':'image/png'};fs.stat(f,(err,s)=>{if(err){res.writeHead(404);return res.end();}res.writeHead(200,{'Content-Type':types[path.extname(f)]||'application/octet-stream','Content-Length':s.size});fs.createReadStream(f).pipe(res);});});
await new Promise(r=>server.listen(0,'127.0.0.1',r));base='http://127.0.0.1:'+server.address().port;
const net=require('net'),portServer=net.createServer();await new Promise(r=>portServer.listen(0,'127.0.0.1',r));const port=portServer.address().port;await new Promise(r=>portServer.close(r));
console.log('NODE '+process.pid);console.log('PORT '+port);profile=fs.mkdtempSync(path.join(os.tmpdir(),'pm62-chrome-'));
chrome=spawn('C:/Program Files/Google/Chrome/Application/chrome.exe',['--headless=new','--disable-gpu-sandbox','--no-sandbox','--remote-debugging-port='+port,'--use-angle=d3d11','--enable-gpu','--ignore-gpu-blocklist','--user-data-dir='+profile,'--no-first-run','--no-default-browser-check','about:blank'],{stdio:['ignore',fs.openSync(path.join(__dirname,'chrome.log'),'a'),fs.openSync(path.join(__dirname,'chrome.log'),'a')],windowsHide:true});
console.log('CHROME '+chrome.pid);chrome.on('error',console.error);chrome.on('exit',(c,s)=>console.log('CHROME EXIT',c,s));let t;for(let i=0;i<80;i++){try{t=await(await fetch('http://127.0.0.1:'+port+'/json/new?about:blank',{method:'PUT'})).json();break;}catch{await sleep(250);}}
console.log('TARGET '+JSON.stringify(t));ws=new WebSocket(t.webSocketDebuggerUrl);await new Promise((r,j)=>{ws.addEventListener('open',r);ws.addEventListener('error',j);});let id=0;const pending=new Map();ws.addEventListener('message',e=>{const m=JSON.parse(e.data);if(m.id){pending.get(m.id)?.(m);pending.delete(m.id);}if(m.method==='Runtime.consoleAPICalled')logs.push(m.params.args.map(a=>a.value||a.description).join(' '));});
cdp=(method,params={})=>new Promise((resolve,reject)=>{const i=++id;pending.set(i,m=>m.error?reject(Error(JSON.stringify(m.error))):resolve(m.result));ws.send(JSON.stringify({id:i,method,params}));});
ev=async expression=>(await cdp('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true}));
const touch=(type,x,y)=>cdp('Input.dispatchTouchEvent',{type,touchPoints:type==='touchEnd'?[]:[{x,y,id:1}]});
tap=async(x,y)=>{await touch('touchStart',x,y);await sleep(60);await touch('touchEnd');await sleep(150);};
drag=async(x,y,dx,dy,ms=250)=>{await touch('touchStart',x,y);await sleep(40);let n=Math.max(3,Math.round(ms/16));for(let i=1;i<=n;i++){await touch('touchMove',x+dx*i/n,y+dy*i/n);await sleep(16);}await touch('touchEnd');};
shot=async(name)=>{const b=Buffer.from((await cdp('Page.captureScreenshot',{format:'png',captureBeyondViewport:false})).data,'base64');fs.writeFileSync(path.join(__dirname,name+'.png'),b);return name;};
record=async(name,ms,action)=>{const d=path.join(__dirname,name);fs.mkdirSync(d,{recursive:true});const times=[],start=Date.now();let actionResult;if(action)actionResult=action();while(Date.now()-start<ms){const at=Date.now()-start;const b=Buffer.from((await cdp('Page.captureScreenshot',{format:'png',captureBeyondViewport:false})).data,'base64');fs.writeFileSync(path.join(d,String(times.length).padStart(4,'0')+'.png'),b);times.push(at);await sleep(Math.max(0,start+times.length*100-Date.now()));}await actionResult;fs.writeFileSync(path.join(d,'times.json'),JSON.stringify(times));return {frames:times.length,ms:Date.now()-start};};
await cdp('Page.enable');await cdp('Runtime.enable');await cdp('Network.enable');await cdp('Network.setBlockedURLs',{urls:['*supabase.co*']});await cdp('Emulation.setDeviceMetricsOverride',{width:390,height:844,deviceScaleFactor:1,mobile:true});await cdp('Emulation.setTouchEmulationEnabled',{enabled:true,maxTouchPoints:5});
await cdp('Page.navigate',{url:base+'/?c=p21&test=1&lang=tr'});
control=http.createServer(async(req,res)=>{let b='';for await(const c of req)b+=c;try{const v=await eval('(async()=>{'+b+'})()');res.end(JSON.stringify(v??null));}catch(e){res.statusCode=500;res.end(e.stack);}});await new Promise(r=>control.listen(0,'127.0.0.1',r));fs.writeFileSync(path.join(__dirname,'session.json'),JSON.stringify({control:control.address().port,base,chromePid:chrome.pid,port,profile}));console.log('READY '+control.address().port);
}
async function cleanup(){try{await cdp('Browser.close');}catch{}server?.close();control?.close();await sleep(1500);try{fs.rmSync(profile,{recursive:true,force:true});}catch{}process.exit();}
main().catch(e=>{console.error(e);cleanup();});




