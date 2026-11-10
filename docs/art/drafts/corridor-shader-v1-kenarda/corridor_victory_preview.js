// Local preview and GPU-backed checks. No network publishing or telemetry.
const fs=require('fs'),path=require('path'),http=require('http'),os=require('os'),crypto=require('crypto'),{spawn}=require('child_process');
const root=path.resolve(__dirname,'..'),port=8840,debugPort=9441;
const out=path.join(root,'docs/art/corridor-victory/qa');fs.mkdirSync(out,{recursive:true});
const sleep=ms=>new Promise(r=>setTimeout(r,ms));
const server=http.createServer((req,res)=>{
 const file=path.resolve(root,'.'+decodeURIComponent(new URL(req.url,'http://localhost').pathname));
 if(!file.startsWith(root+path.sep)){res.writeHead(403);res.end();return;}
 fs.readFile(file,(e,data)=>{if(e){res.writeHead(404);res.end();return;}res.setHeader('Content-Type',({'.html':'text/html; charset=utf-8','.js':'text/javascript; charset=utf-8','.png':'image/png','.jpg':'image/jpeg','.webm':'video/webm','.json':'application/json','.md':'text/plain; charset=utf-8','.cginc':'text/plain'})[path.extname(file)]||'application/octet-stream');res.setHeader('Cache-Control','no-store');res.end(data);});
});
const url=`http://127.0.0.1:${port}/docs/art/corridor-victory/index.html`;
server.listen(port,'127.0.0.1',()=>{console.log(url);if(process.argv.includes('--verify'))verify().catch(e=>{console.error(e.stack);process.exitCode=1;server.close();});});
async function verify(){
 const chrome=spawn('C:/Program Files/Google/Chrome/Application/chrome.exe',['--headless=new',`--remote-debugging-port=${debugPort}`,'--use-angle=d3d11','--enable-gpu','--ignore-gpu-blocklist',`--user-data-dir=${fs.mkdtempSync(path.join(os.tmpdir(),'rs-corridor-'))}`,'--no-first-run','about:blank'],{stdio:'ignore',windowsHide:true});
 let ws;
 try{
  for(let i=0;i<60;i++){try{await fetch(`http://127.0.0.1:${debugPort}/json/version`);break;}catch{}await sleep(250);}
  const target=await(await fetch(`http://127.0.0.1:${debugPort}/json/new?about:blank`,{method:'PUT'})).json();
  ws=new WebSocket(target.webSocketDebuggerUrl);await new Promise(r=>ws.addEventListener('open',r));let seq=0;const waiting=new Map(),errors=[];
  ws.addEventListener('message',e=>{const m=JSON.parse(e.data);if(m.id&&waiting.has(m.id)){waiting.get(m.id)(m);waiting.delete(m.id);}if(m.method==='Runtime.exceptionThrown')errors.push(m.params.exceptionDetails.text);});
  const cdp=(method,params={})=>new Promise(r=>{const id=++seq;waiting.set(id,r);ws.send(JSON.stringify({id,method,params}));});
  const ev=async expression=>{const r=await cdp('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true});if(r.result?.exceptionDetails)throw Error(r.result.exceptionDetails.text);return r.result?.result?.value;};
  await cdp('Runtime.enable');await cdp('Page.enable');await cdp('Emulation.setDeviceMetricsOverride',{width:1100,height:1000,deviceScaleFactor:1,mobile:false});await cdp('Page.navigate',{url});
  for(let i=0;i<160;i++){const e=await ev('window.corridorError');if(e)throw Error(e);if(await ev('!!window.corridorDemo?.ready'))break;await sleep(100);}
  if(!await ev('!!window.corridorDemo?.ready'))throw Error('Preview did not load');
  const states=[];let first,last;
  for(const p of [0,.001,.125,.25,.477777778,.483333333,.488888889,.494444444,.5,.505555556,.511111111,.516666667,.522222222,.75,.875,.999,1]){
   await ev(`corridorDemo.setProgress(${p})`);
   const png=Buffer.from((await ev('corridorDemo.png()')).split(',')[1],'base64');
   fs.writeFileSync(path.join(out,`frame-${String(p).replace('.','-')}.png`),png);
   const hash=crypto.createHash('sha256').update(png).digest('hex');states.push({progress:p,hash});if(p===0)first=hash;if(p===1)last=hash;
  }
  if(first!==last)throw Error('Initial and final composition differ');
  const entryDifference=await ev('corridorDemo.compare(0,.000001)'),exitDifference=await ev('corridorDemo.compare(.999999,1)');
  const temporalDirectory=path.join(out,'temporal');fs.mkdirSync(temporalDirectory,{recursive:true});
  for(let frame=0;frame<=180;frame++){
   await ev(`corridorDemo.setProgress(${frame/180})`);
   const thumbnail=await ev('corridorDemo.thumbnail()');
   fs.writeFileSync(path.join(temporalDirectory,`frame-${String(frame).padStart(3,'0')}.png`),Buffer.from(thumbnail.split(',')[1],'base64'));
  }
  await ev('corridorDemo.idle()');const idle=(await ev('corridorDemo.state')).frames;await sleep(700);if((await ev('corridorDemo.state')).frames!==idle)throw Error('Idle menu redraws continuously');
  await ev('corridorDemo.play()');if(await ev('corridorDemo.play()'))throw Error('Duplicate play was accepted');await sleep(3400);
  const ended=await ev('corridorDemo.state');if(ended.running||ended.completed!==1||ended.p!==1)throw Error('One-shot completion failed');
  await sleep(700);if((await ev('corridorDemo.state')).frames!==ended.frames)throw Error('Animation continues after completion');
  await ev("document.querySelector('#picture').checked=true;corridorDemo.setProgress(0)");const withPicture=await ev('corridorDemo.png()');await ev('corridorDemo.setProgress(1)');if(withPicture!==await ev('corridorDemo.png()'))throw Error('Dynamic painting endpoints differ');
  await ev("document.querySelector('#picture').checked=false;corridorDemo.setProgress(0)");
  const shot=await cdp('Page.captureScreenshot',{format:'png'});fs.writeFileSync(path.join(out,'preview-page.png'),Buffer.from(shot.result.data,'base64'));
  // A standalone three-second movie is also available for review without running the server.
  const movie=await ev(`(async()=>{const recorder=new MediaRecorder(document.querySelector('canvas').captureStream(60),{mimeType:'video/webm;codecs=vp9',videoBitsPerSecond:5000000});const chunks=[];recorder.ondataavailable=e=>chunks.push(e.data);const finished=new Promise(resolve=>recorder.onstop=async()=>{const reader=new FileReader();reader.onload=()=>resolve(reader.result);reader.readAsDataURL(new Blob(chunks,{type:'video/webm'}));});recorder.start();corridorDemo.play();await new Promise(r=>setTimeout(r,3200));recorder.stop();return finished;})()`);
  fs.writeFileSync(path.join(out,'corridor-victory.webm'),Buffer.from(movie.split(',')[1],'base64'));
  const gaps=ended.gaps.slice().sort((a,b)=>a-b);const result={status:'passed',endpointPixelsIdentical:first===last,idleStops:true,duplicateTriggerIgnored:true,completions:ended.completed,stoppedAfter3Seconds:true,dynamicPaintingEndpointsMatch:true,frameCount:ended.gaps.length,medianFrameMs:gaps[Math.floor(gaps.length*.5)],p95FrameMs:gaps[Math.floor(gaps.length*.95)],exceptions:errors,frames:states,scope:'Local WebGL preview of shared projection shader; desktop Chrome GPU, not an iPhone or Unity runtime measurement'};
  result.entryDifference=entryDifference;result.exitDifference=exitDifference;result.consecutiveFrames=181;
  if(errors.length)throw Error(JSON.stringify(errors));fs.writeFileSync(path.join(out,'report.json'),JSON.stringify(result,null,2));console.log(JSON.stringify({status:result.status,endpointPixelsIdentical:result.endpointPixelsIdentical,medianFrameMs:result.medianFrameMs,p95FrameMs:result.p95FrameMs,entryDifference,exitDifference,consecutiveFrames:181,exceptions:errors}));
 }finally{if(ws)ws.close();chrome.kill();server.close();}
}
