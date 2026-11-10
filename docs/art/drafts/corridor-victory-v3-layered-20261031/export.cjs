// Task18: all generated files and browser profile are strictly inside docs/art.
const fs=require('fs'),path=require('path'),http=require('http'),vm=require('vm'),crypto=require('crypto'),{spawn}=require('child_process');
const dir=__dirname,port=9497,debug=9498;
const old=fs.readFileSync(path.resolve(dir,'../../corridor-victory/export-preview-mp4.cjs'),'utf8');
const defs=old.slice(old.indexOf('const u16='),old.indexOf('async function main()'));
const makeMux=(W,H)=>vm.runInNewContext(defs.replaceAll('1920',String(W)).replaceAll('1080',String(H))+';mux;',{Buffer});
const sleep=ms=>new Promise(r=>setTimeout(r,ms));
const save=(name,png)=>fs.writeFileSync(path.join(dir,name),Buffer.from(png.split(',')[1],'base64'));
const server=http.createServer((req,res)=>{const filename=path.resolve(dir,'.'+decodeURIComponent(req.url==='/'?'/index.html':req.url.split('?')[0]));if(!filename.startsWith(dir+path.sep)){res.writeHead(403);res.end();return;}try{res.setHeader('Content-Type',filename.endsWith('.html')?'text/html; charset=utf-8':filename.endsWith('.js')?'text/javascript':filename.endsWith('.png')?'image/png':'application/octet-stream');res.end(fs.readFileSync(filename));}catch{res.writeHead(404);res.end();}});
async function main(){
await new Promise(r=>server.listen(port,'127.0.0.1',r));
const chrome=spawn('C:/Program Files/Google/Chrome/Application/chrome.exe',['--headless=new',`--remote-debugging-port=${debug}`,`--user-data-dir=${path.join(dir,'.chrome-profile')}`,'--no-first-run','about:blank'],{stdio:'ignore',windowsHide:true});let ws;
try{
for(let i=0;i<100;i++){try{await fetch(`http://127.0.0.1:${debug}/json/version`,{signal:AbortSignal.timeout(1000)});break;}catch{}await sleep(100);}
const target=await(await fetch(`http://127.0.0.1:${debug}/json/new?about:blank`,{method:'PUT'})).json();
ws=new WebSocket(target.webSocketDebuggerUrl);await new Promise(r=>ws.addEventListener('open',r));let seq=0;const pending=new Map();
ws.addEventListener('message',e=>{const m=JSON.parse(e.data);if(pending.has(m.id)){pending.get(m.id)(m);pending.delete(m.id);}});
const cdp=(method,params={})=>new Promise(r=>{const id=++seq;pending.set(id,r);ws.send(JSON.stringify({id,method,params}));});
const ev=async expression=>{const r=await cdp('Runtime.evaluate',{expression,awaitPromise:true,returnByValue:true});if(r.error||r.result?.exceptionDetails)throw Error(JSON.stringify(r));return r.result?.result?.value;};
await cdp('Page.enable');await cdp('Page.navigate',{url:`http://127.0.0.1:${port}/`});
for(let i=0;i<100;i++){const err=await ev('window.task18Error');if(err)throw Error(err);if(await ev('!!window.task18?.ready'))break;await sleep(100);}
if(!await ev('!!window.task18?.ready'))throw Error('Renderer unavailable');
fs.mkdirSync(path.join(dir,'qa'),{recursive:true});
for(const [name,p]of [['first',0],['middle',.5],['last',1]]){save('qa/background-'+name+'.png',await ev(`task18.backgroundPNG(${p})`));await ev(`task18.render(${p})`);save(name==='middle'?'qa/corridor-empty-middle.png':'corridor-empty-'+name+'.png',await ev('task18.png()'));}

async function encode({W,H,fps,bitrate,output,trackFile}){
const total=fps*3;
await ev(`(async()=>{const config={codec:'avc1.42002a',width:${W},height:${H},bitrate:${bitrate},framerate:${fps},latencyMode:'realtime',hardwareAcceleration:'prefer-software',avc:{format:'avc'}};if(!(await VideoEncoder.isConfigSupported(config)).supported)throw Error('H264 unavailable');const samples=[];let description,error;const b64=bytes=>{let s='';for(let i=0;i<bytes.length;i+=32768)s+=String.fromCharCode(...bytes.subarray(i,i+32768));return btoa(s);};const encoder=new VideoEncoder({output(c,m){const b=new Uint8Array(c.byteLength);c.copyTo(b);samples.push({timestamp:c.timestamp,key:c.type==='key',data:b64(b)});if(m.decoderConfig?.description)description=b64(new Uint8Array(m.decoderConfig.description));},error(e){error=e.message;}});encoder.configure(config);window.capture18={async frame(i){const track=task18.render(i/${total},${W},${H}),pixels=document.querySelector('canvas').getContext('2d').getImageData(0,0,${W},${H}),frame=new VideoFrame(pixels.data,{format:'RGBA',codedWidth:${W},codedHeight:${H},timestamp:Math.round(i*1e6/${fps}),duration:Math.round(1e6/${fps})});encoder.encode(frame,{keyFrame:i%${fps}===0});frame.close();if(encoder.encodeQueueSize>3)await encoder.flush();if(error)throw Error(error);return track;},async finish(){await encoder.flush();encoder.close();return{samples,description,config};}};})()`);
const frames=[];
for(let i=0;i<=total;i++){
const t=await ev(`capture18.frame(${i})`);frames.push({i,...t});
if(fps===60&&i%15===0)save('qa/green-'+String(i).padStart(3,'0')+'.png',await ev('task18.png()'));
if(i%fps===0)console.log(output+' '+i+'/'+total);
}
const encoded=await ev('capture18.finish()');if(encoded.samples.length!==total+1||!encoded.description)throw Error('Incomplete video');
const direct=await ev(`(async()=>{const from64=s=>{const v=atob(s),b=new Uint8Array(v.length);for(let i=0;i<v.length;i++)b[i]=v.charCodeAt(i);return b;};const c=document.createElement('canvas');c.width=${W};c.height=${H};const ctx=c.getContext('2d'),result={};let error;const decoder=new VideoDecoder({output(frame){const i=Math.round(frame.timestamp*${fps}/1e6);if([0,${fps*1.5},${total}].includes(i)){ctx.drawImage(frame,0,0);result[i]={timestamp:frame.timestamp,png:c.toDataURL('image/png')};}frame.close();},error(e){error=e.message;}});decoder.configure({codec:'avc1.42002a',codedWidth:${W},codedHeight:${H},description:from64(${JSON.stringify(encoded.description)}),hardwareAcceleration:'prefer-software'});for(const s of ${JSON.stringify(encoded.samples)})decoder.decode(new EncodedVideoChunk({type:s.key?'key':'delta',timestamp:s.timestamp,data:from64(s.data)}));await decoder.flush();decoder.close();if(error)throw Error(error);return result;})()`);
for(const [i,frame]of Object.entries(direct))save('qa/'+output.replace('.mp4','')+'-exact-frame-'+i+'.png',frame.png);
if(Object.keys(direct).length!==3)throw Error('Timestamp decoder did not produce checkpoints');
// MP4 muxer uses a 60k time scale. Adapt its fixed 60FPS sample delta for the web variant.
let mux=makeMux(W,H);if(fps!==60)mux=vm.runInNewContext(defs.replaceAll('1920',String(W)).replaceAll('1080',String(H)).replace('delta=1000','delta='+String(60000/fps))+';mux;',{Buffer});
const bytes=mux(encoded);fs.writeFileSync(path.join(dir,output),bytes);
const track={fps,width:W,height:H,frames,quadOrder:['top-left','top-right','bottom-right','bottom-left'],coordinateOrigin:'top-left; pixels; corners are NOT clamped to screen',durationSeconds:3,primaryQuad:'largest visible painting aperture; frameId changes as the neighbor becomes dominant; use instances for BOTH paintings',green:'#00FF00',apertureSourceQuad:await ev('task18.aperture')};
fs.writeFileSync(path.join(dir,trackFile),JSON.stringify(track,null,2));
const validation=await ev(`(async()=>{const s=atob(${JSON.stringify(bytes.toString('base64'))}),b=new Uint8Array(s.length);for(let i=0;i<s.length;i++)b[i]=s.charCodeAt(i);const v=document.createElement('video');v.muted=true;v.src=URL.createObjectURL(new Blob([b],{type:'video/mp4'}));await new Promise((r,j)=>{v.onloadedmetadata=r;v.onerror=j;});const c=document.createElement('canvas');c.width=${W};c.height=${H};const x=c.getContext('2d'),frames={};for(const t of [.00001,1.5,3.00001]){v.currentTime=t;await new Promise(r=>v.onseeked=r);x.drawImage(v,0,0);frames[t]=c.toDataURL('image/png');}return{width:v.videoWidth,height:v.videoHeight,duration:v.duration,frames};})()`);
if(validation.width!==W||validation.height!==H||Math.abs(validation.duration-(total+1)/fps)>.003)throw Error('Decoded MP4 invalid');
for(const [t,png]of Object.entries(validation.frames))save('qa/'+output.replace('.mp4','')+'-decoded-'+t.replace('.','-')+'.png',png);
return{output,trackFile,width:W,height:H,fps,frames:total+1,bytes:bytes.length,duration:validation.duration,bitrateTarget:bitrate,sha256:crypto.createHash('sha256').update(bytes).digest('hex'),decoded:true,timestampDecodedFrames:Object.keys(direct).map(Number),inputFramePixels:'explicit immutable RGBA CPU snapshot'};
}
const master=await encode({W:780,H:1688,fps:60,bitrate:4800000,output:'corridor-empty.mp4',trackFile:'frame-track.json'});
const web=await encode({W:540,H:1168,fps:30,bitrate:1800000,output:'corridor-empty-web.mp4',trackFile:'frame-track-web.json'});
const protectedFiles=JSON.parse(fs.readFileSync(path.join(dir,'protected-before.json'),'utf8').replace(/^\uFEFF/,''));
if(protectedFiles.some(f=>crypto.createHash('sha256').update(fs.readFileSync(f.path)).digest('hex').toUpperCase()!==f.sha256.toUpperCase()))throw Error('Approved files modified');
const report={master,web,approvedFilesUnchanged:true,writes:'docs/art/drafts/corridor-victory-v3-layered-20261031 only',greenSourceRGB:[0,255,0],codec:'H264 AVC baseline; software encoder'};
fs.writeFileSync(path.join(dir,'export-report.json'),JSON.stringify(report,null,2));console.log(JSON.stringify(report));
}finally{if(ws)ws.close();chrome.kill();server.close();}
}
main().catch(e=>{console.error(e.stack);server.close();process.exitCode=1;});
