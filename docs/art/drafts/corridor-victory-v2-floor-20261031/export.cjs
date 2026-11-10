// Draft-only browser rendering/export. All writes stay in this folder.
const fs=require('fs'),path=require('path'),http=require('http'),crypto=require('crypto'),vm=require('vm'),{spawn}=require('child_process');
const dir=__dirname,port=9487,debug=9488,W=780,H=1688;
// Reuse the existing exporter's standalone MP4 muxer, without invoking its main or modifying it.
const previous=fs.readFileSync(path.resolve(dir,'../../corridor-victory/export-preview-mp4.cjs'),'utf8');
const defs=previous.slice(previous.indexOf('const u16='),previous.indexOf('async function main()'));
const mux=vm.runInNewContext(defs.replaceAll('1920','780').replaceAll('1080','1688')+';mux;',{Buffer});
const wait=ms=>new Promise(r=>setTimeout(r,ms));
const server=http.createServer((req,res)=>{const file=path.resolve(dir,'.'+decodeURIComponent(req.url.split('?')[0]==='/'?'/index.html':req.url.split('?')[0]));if(!file.startsWith(dir+path.sep)){res.writeHead(403);res.end();return;}try{res.setHeader('Content-Type',file.endsWith('.html')?'text/html; charset=utf-8':file.endsWith('.js')?'text/javascript':file.endsWith('.png')?'image/png':'application/octet-stream');res.end(fs.readFileSync(file));}catch{res.writeHead(404);res.end();}});
async function main(){
// Recover only this exporter's own dedicated debug browser after an interrupted capture.
try{const info=await(await fetch(`http://127.0.0.1:${debug}/json/version`,{signal:AbortSignal.timeout(1500)})).json();const old=new WebSocket(info.webSocketDebuggerUrl);await new Promise(r=>old.addEventListener('open',r));old.send(JSON.stringify({id:1,method:'Browser.close'}));await wait(700);old.close();}catch{}
await new Promise(r=>server.listen(port,'127.0.0.1',r));
const profile=path.join(dir,'.chrome-profile');
const chrome=spawn('C:/Program Files/Google/Chrome/Application/chrome.exe',['--headless=new',`--remote-debugging-port=${debug}`,`--user-data-dir=${profile}`,'--no-first-run','about:blank'],{stdio:'ignore',windowsHide:true});
let ws;
try{for(let i=0;i<100;i++){try{await fetch(`http://127.0.0.1:${debug}/json/version`);break;}catch{}await wait(100);}
const target=await(await fetch(`http://127.0.0.1:${debug}/json/new?about:blank`,{method:'PUT'})).json();
ws=new WebSocket(target.webSocketDebuggerUrl);await new Promise(r=>ws.addEventListener('open',r));let seq=0;const pending=new Map();ws.addEventListener('message',e=>{const m=JSON.parse(e.data);if(pending.has(m.id)){pending.get(m.id)(m);pending.delete(m.id);}});
const cdp=(method,params={})=>new Promise(r=>{const id=++seq;pending.set(id,r);ws.send(JSON.stringify({id,method,params}));});
const ev=async expression=>{const r=await cdp('Runtime.evaluate',{expression,awaitPromise:true,returnByValue:true});if(r.error||r.result?.exceptionDetails)throw Error(JSON.stringify(r));return r.result?.result?.value;};
await cdp('Page.enable');await cdp('Page.navigate',{url:`http://127.0.0.1:${port}/`});
for(let i=0;i<100;i++){const error=await ev('window.bayError');if(error)throw Error(error);if(await ev('!!window.bayDemo?.ready'))break;await wait(100);}
if(!await ev('!!window.bayDemo?.ready'))throw Error('Draft did not load');
fs.mkdirSync(path.join(dir,'qa'),{recursive:true});
const save=(name,png)=>fs.writeFileSync(path.join(dir,name),Buffer.from(png.split(',')[1],'base64'));
for(const [name,p] of [['start',0],['middle',.5],['end',1]]){await ev(`bayDemo.setProgress(${p})`);save('frame-'+name+'.png',await ev('bayDemo.png()'));}
const metrics=await ev('bayDemo.metrics()');
const oldPreview=fs.readFileSync(path.resolve(dir,'../corridor-victory-v2-panorama-matched-20261031/preview.js'));
if(!oldPreview.equals(fs.readFileSync(path.join(dir,'preview.js'))))throw Error('Camera/render implementation changed');
if(metrics.endpointMaxChannelError!==0)throw Error('Endpoint pixels do not match');
const sourceHash=crypto.createHash('sha256').update(fs.readFileSync(path.join(dir,'original-panorama.png'))).digest('hex');
const authoritativeHash=crypto.createHash('sha256').update(fs.readFileSync(path.resolve(dir,'../corridor-victory-v2-panorama-20261030/panorama-clean.png'))).digest('hex');
if(sourceHash!==authoritativeHash)throw Error('Source panorama differs');
console.log('Original preserved; endpoint pixels identical. Capturing one bay at 60 FPS.');
await ev(`(async()=>{
 const canvas=document.querySelector('canvas'),samples=[];let description,error;
 const b64=bytes=>{let s='';for(let i=0;i<bytes.length;i+=32768)s+=String.fromCharCode(...bytes.subarray(i,i+32768));return btoa(s);};
 const config={codec:'avc1.42002a',width:780,height:1688,bitrate:14000000,framerate:60,latencyMode:'realtime',hardwareAcceleration:'prefer-software',avc:{format:'avc'}};
 if(!(await VideoEncoder.isConfigSupported(config)).supported)throw Error('H264 encoding unsupported');
 const encoder=new VideoEncoder({output(chunk,meta){const bytes=new Uint8Array(chunk.byteLength);chunk.copyTo(bytes);samples.push({timestamp:chunk.timestamp,key:chunk.type==='key',data:b64(bytes)});if(meta.decoderConfig?.description)description=b64(new Uint8Array(meta.decoderConfig.description));},error(e){error=e.message;}});encoder.configure(config);
 const small=document.createElement('canvas');small.width=390;small.height=844;const qc=small.getContext('2d');
 window.captureDraft={async frame(i){bayDemo.setProgress(i/180);const frame=new VideoFrame(canvas,{timestamp:Math.round(i*1e6/60),duration:Math.round(1e6/60)});encoder.encode(frame,{keyFrame:i%60===0});frame.close();if(encoder.encodeQueueSize>3)await encoder.flush();if(error)throw Error(error);qc.drawImage(canvas,0,0,390,844);return small.toDataURL('image/png');},async finish(){await encoder.flush();encoder.close();return{samples,description,config};}};
 return true;})()`);
for(let i=0;i<=180;i++){save('qa/frame-'+String(i).padStart(3,'0')+'.png',await ev(`captureDraft.frame(${i})`));if(i%30===0)console.log('Captured frame '+i+'/180');}
const encoded=await ev('captureDraft.finish()');
if(encoded.samples.length!==181||!encoded.description)throw Error('Incomplete video');
const bytes=mux(encoded);fs.writeFileSync(path.join(dir,'floor-revision-step.mp4'),bytes);
const reviewSamples=[];
for(let cycle=0;cycle<3;cycle++)for(let i=0;i<180;i++)reviewSamples.push({...encoded.samples[i],timestamp:Math.round((cycle*180+i)*1e6/60)});
reviewSamples.push({...encoded.samples[180],timestamp:9000000});
const review=mux({...encoded,samples:reviewSamples});fs.writeFileSync(path.join(dir,'floor-review-three-cycles.mp4'),review);
const decoded=await ev(`(async()=>{const s=atob(${JSON.stringify(bytes.toString('base64'))}),b=new Uint8Array(s.length);for(let i=0;i<s.length;i++)b[i]=s.charCodeAt(i);const v=document.createElement('video');v.muted=true;v.src=URL.createObjectURL(new Blob([b],{type:'video/mp4'}));await new Promise((r,j)=>{v.onloadedmetadata=r;v.onerror=j;});const c=document.createElement('canvas');c.width=780;c.height=1688;const ctx=c.getContext('2d'),frames={};for(const [n,t]of [[0,.00001],[90,1.50001],[179,2.98334],[180,3.00001]]){v.currentTime=t;await new Promise(r=>v.onseeked=r);ctx.drawImage(v,0,0);frames[n]=c.toDataURL('image/png');}return{width:v.videoWidth,height:v.videoHeight,duration:v.duration,frames};})()`);
for(const [n,png]of Object.entries(decoded.frames))save('qa/decoded-'+n+'.png',png);
if(decoded.width!==W||decoded.height!==H||Math.abs(decoded.duration-181/60)>.002)throw Error('MP4 validation failed');
const protectedFiles=JSON.parse(fs.readFileSync(path.join(dir,'protected-before.json'),'utf8').replace(/^\uFEFF/,''));
const changed=protectedFiles.filter(f=>crypto.createHash('sha256').update(fs.readFileSync(f.path)).digest('hex').toUpperCase()!==f.sha256.toUpperCase());
if(changed.length)throw Error('Protected files changed: '+JSON.stringify(changed));
const report={...metrics,sourceSha256:sourceHash,sourceCopyByteIdentical:true,protectedFilesUnchanged:true,fps:60,frames:181,videoDuration:decoded.duration,videoWidth:W,videoHeight:H,bytes:bytes.length,sha256:crypto.createHash('sha256').update(bytes).digest('hex'),browserDecodePassed:true};
report.source='Previous matched revision upper scene, with one wholly rebuilt periodic floor from original carpet/gold materials';
report.cameraCodeByteIdenticalToPrevious=true;
const reviewDecoded=await ev(`(async()=>{const s=atob(${JSON.stringify(review.toString('base64'))}),b=new Uint8Array(s.length);for(let i=0;i<s.length;i++)b[i]=s.charCodeAt(i);const v=document.createElement('video');v.muted=true;v.src=URL.createObjectURL(new Blob([b],{type:'video/mp4'}));await new Promise((r,j)=>{v.onloadedmetadata=r;v.onerror=j;});const c=document.createElement('canvas');c.width=780;c.height=1688;const x=c.getContext('2d'),frames={};for(const t of [2.98334,3.00001,3.01668,5.98334,6.00001,6.01668,8.98334,9.00001]){v.currentTime=t;await new Promise(r=>v.onseeked=r);x.drawImage(v,0,0);frames[t]=c.toDataURL('image/png');}return{duration:v.duration,width:v.videoWidth,height:v.videoHeight,frames};})()`);
if(Math.abs(reviewDecoded.duration-541/60)>.002)throw Error('Three-cycle video timing failed');
for(const [t,png]of Object.entries(reviewDecoded.frames))save('qa/review-decoded-'+t.replace('.','-')+'.png',png);
report.review={cycles:3,frames:541,duration:reviewDecoded.duration,bytes:review.length,decodePassed:true,identicalEncodedCycleSamplesReused:true,idrAtEachCycleStart:true};
fs.writeFileSync(path.join(dir,'export-report.json'),JSON.stringify(report,null,2));console.log(JSON.stringify(report));
}finally{if(ws)ws.close();chrome.kill();server.close();}}
main().catch(e=>{console.error(e.stack);server.close();process.exitCode=1;});
