// Export-only utility. Captures the live browser renderer; does not edit game/preview files.
const fs=require('fs'),path=require('path'),os=require('os'),http=require('http'),crypto=require('crypto'),{spawn}=require('child_process');
const root=path.resolve(__dirname,'../../..');
const output=path.join(root,'Assets/Art/Menu/CorridorVictoryV2/corridor_victory_v2_preview.mp4');
const scratch=path.join(__dirname,'export');fs.mkdirSync(scratch,{recursive:true});
const port=9442,url='http://127.0.0.1:8840/docs/art/corridor-victory/index.html';
const software=process.argv.includes('--software');
const inspectOnly=process.argv.includes('--inspect-only');
const captureStatic=process.argv.includes('--capture-static');
const reviewFrames=process.argv.includes('--review-frames');
const sleep=ms=>new Promise(resolve=>setTimeout(resolve,ms));
const protectedFiles=[
 'Assets/_Game/Presentation/Ui/CorridorVictory.cs','Assets/_Game/Presentation/Ui/CorridorVictoryQueue.cs',
 'Assets/_Game/Presentation/GameRoot.cs','Assets/_Game/Presentation/Ui/Screens.cs',
 'Assets/_Game/Presentation/Resources/menu_bg.png','docs/art/corridor-victory/preview.js',
 'docs/art/corridor-victory/index.html','Tools/corridor_victory_preview.js',
 ...fs.readdirSync(path.join(root,'Assets/_Game/Presentation/Resources/CorridorVictory')).filter(n=>!n.endsWith('.meta')).map(n=>'Assets/_Game/Presentation/Resources/CorridorVictory/'+n)
];
const hash=file=>crypto.createHash('sha256').update(fs.readFileSync(path.join(root,file))).digest('hex');
const before=Object.fromEntries(protectedFiles.map(f=>[f,hash(f)]));
const u16=n=>{const b=Buffer.alloc(2);b.writeUInt16BE(n);return b;};
const u32=n=>{const b=Buffer.alloc(4);b.writeUInt32BE(n);return b;};
const zeros=n=>Buffer.alloc(n),str=s=>Buffer.from(s,'ascii');
const box=(name,...parts)=>{const data=Buffer.concat(parts);return Buffer.concat([u32(data.length+8),str(name),data]);};
const full=(name,flags,...parts)=>box(name,u32(flags),...parts);
const matrix=Buffer.concat([u32(0x10000),u32(0),u32(0),u32(0),u32(0x10000),u32(0),u32(0),u32(0),u32(0x40000000)]);
function mux(encoded){
 const samples=encoded.samples,scale=60000,delta=1000,duration=samples.length*delta;
 for(let i=1;i<samples.length;i++)if(samples[i].timestamp<=samples[i-1].timestamp)throw Error('Encoder reordered frames');
 const data=samples.map(s=>Buffer.from(s.data,'base64'));
 const ftyp=box('ftyp',str('isom'),u32(512),str('isomiso2avc1mp41'));
 function moov(offset){
  const mvhd=full('mvhd',0,u32(0),u32(0),u32(scale),u32(duration),u32(0x10000),u16(0x100),zeros(10),matrix,zeros(24),u32(2));
  const tkhd=full('tkhd',7,u32(0),u32(0),u32(1),u32(0),u32(duration),zeros(8),zeros(8),matrix,u32(1920*65536),u32(1080*65536));
  const mdhd=full('mdhd',0,u32(0),u32(0),u32(scale),u32(duration),u16(0x55c4),u16(0));
  const hdlr=full('hdlr',0,u32(0),str('vide'),zeros(12),str('Browser Corridor Victory V2\0'));
  const compressor=Buffer.alloc(32);compressor[0]=12;compressor.write('Browser H264',1,'ascii');
  const avc1=box('avc1',zeros(6),u16(1),zeros(16),u16(1920),u16(1080),u32(72*65536),u32(72*65536),u32(0),u16(1),compressor,u16(24),u16(65535),box('avcC',Buffer.from(encoded.description,'base64')),box('pasp',u32(1),u32(1)));
  const stsd=full('stsd',0,u32(1),avc1);
  const stts=full('stts',0,u32(1),u32(samples.length),u32(delta));
  const keys=samples.flatMap((s,i)=>s.key?[u32(i+1)]:[]);
  const stss=full('stss',0,u32(keys.length),...keys);
  const stsc=full('stsc',0,u32(1),u32(1),u32(samples.length),u32(1));
  const stsz=full('stsz',0,u32(0),u32(samples.length),...data.map(b=>u32(b.length)));
  const stco=full('stco',0,u32(1),u32(offset));
  const stbl=box('stbl',stsd,stts,stss,stsc,stsz,stco);
  const dinf=box('dinf',full('dref',0,u32(1),full('url ',1)));
  const minf=box('minf',full('vmhd',1,zeros(8)),dinf,stbl);
  return box('moov',mvhd,box('trak',tkhd,box('mdia',mdhd,hdlr,minf)));
 }
 const first=moov(0),movie=moov(ftyp.length+first.length+8);
 return Buffer.concat([ftyp,movie,box('mdat',...data)]);
}
async function main(){
 await fetch(url).then(r=>{if(!r.ok)throw Error('Existing browser preview is unavailable');});
 if(fs.existsSync(output)&&!inspectOnly&&!captureStatic&&!reviewFrames){
  const prior=path.join(scratch,'mp4-export-report.json');
  if(!software||!fs.existsSync(prior)||crypto.createHash('sha256').update(fs.readFileSync(output)).digest('hex')!==JSON.parse(fs.readFileSync(prior)).sha256)throw Error('Output already exists; preserving it: '+output);
 }
 const chrome=spawn('C:/Program Files/Google/Chrome/Application/chrome.exe',['--headless=new',`--remote-debugging-port=${port}`,'--use-angle=d3d11','--enable-gpu','--ignore-gpu-blocklist',`--user-data-dir=${fs.mkdtempSync(path.join(os.tmpdir(),'rs-mp4-export-'))}`,'--no-first-run','about:blank'],{stdio:'ignore',windowsHide:true});
 let ws;
 try{
  let available=false;for(let i=0;i<80;i++){try{await fetch(`http://127.0.0.1:${port}/json/version`);available=true;break;}catch{}await sleep(250);}if(!available)throw Error('Export browser did not start');
  const target=await(await fetch(`http://127.0.0.1:${port}/json/new?about:blank`,{method:'PUT'})).json();
  ws=new WebSocket(target.webSocketDebuggerUrl);await new Promise(r=>ws.addEventListener('open',r));
  let seq=0;const waiting=new Map();
  ws.addEventListener('message',e=>{const m=JSON.parse(e.data);if(waiting.has(m.id)){waiting.get(m.id)(m);waiting.delete(m.id);}});
  const cdp=(method,params={})=>new Promise(resolve=>{const id=++seq;waiting.set(id,resolve);ws.send(JSON.stringify({id,method,params}));});
  const ev=async expression=>{const r=await cdp('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true});if(r.result?.exceptionDetails)throw Error(JSON.stringify(r.result.exceptionDetails));return r.result?.result?.value;};
  await cdp('Page.enable');await cdp('Emulation.setDeviceMetricsOverride',{width:1920,height:1200,deviceScaleFactor:2,mobile:false});await cdp('Page.navigate',{url});
  for(let i=0;i<160;i++){const error=await ev('window.corridorError');if(error)throw Error(error);if(await ev('!!window.corridorDemo?.ready'))break;await sleep(100);}
  if(reviewFrames){
   for(const p of [0,.25,1.3/3,.5,1.75/3,2/3,.75,1]){
    await ev('corridorDemo.setProgress('+p+')');
    const png=await ev('corridorDemo.png()');
    fs.writeFileSync(path.join(scratch,'geometry-'+(p*3).toFixed(2).replace('.','-')+'.png'),Buffer.from(png.split(',')[1],'base64'));
   }
   console.log('Saved complete-move geometry checkpoints including 1.3, 1.5, 1.75 and 2 seconds.');return;
  }
  if(captureStatic){
   const png=await ev(`(()=>{const phone=document.querySelector('.phone');phone.style.width='512px';phone.style.height='768px';phone.style.aspectRatio='2/3';window.dispatchEvent(new Event('resize'));corridorDemo.setProgress(.000000001);return corridorDemo.png();})()`);
   const bytes=Buffer.from(png.split(',')[1],'base64');
   fs.writeFileSync(path.join(root,'Assets/Art/Menu/CorridorVictoryV2/Static.png'),bytes);
   fs.writeFileSync(path.join(root,'Assets/_Game/Presentation/Resources/CorridorVictory/Static.png'),bytes);
   console.log('Captured the actual scene at the reference camera as 1024x1536 static menu artwork.');return;
  }
  if(inspectOnly){
   const input=fs.readFileSync(output).toString('base64');
   const frames=await ev(`(async()=>{const binary=atob(${JSON.stringify(input)}),bytes=new Uint8Array(binary.length);for(let i=0;i<binary.length;i++)bytes[i]=binary.charCodeAt(i);const video=document.createElement('video');video.muted=true;video.src=URL.createObjectURL(new Blob([bytes],{type:'video/mp4'}));await new Promise((r,j)=>{video.onloadedmetadata=r;video.onerror=j;});const canvas=document.createElement('canvas');canvas.width=video.videoWidth;canvas.height=video.videoHeight;const ctx=canvas.getContext('2d');const result={};for(const time of [.001,1.3,1.5,1.75,2,2.5,3]){video.currentTime=time;await new Promise(r=>video.onseeked=r);ctx.drawImage(video,0,0);result[time]=canvas.toDataURL('image/png');}return result;})()`);
   for(const [time,png] of Object.entries(frames))fs.writeFileSync(path.join(scratch,'input-video-'+time.replace('.','-')+'.png'),Buffer.from(png.split(',')[1],'base64'));
   console.log('Decoded supplied MP4 at 0, 1.3, 1.5, 1.75, 2, 2.5 and 3 seconds.');return;
  }
  const capabilities=await ev(`({webCodecs:typeof VideoEncoder!=='undefined',mp4Recorder:MediaRecorder.isTypeSupported('video/mp4;codecs=avc1.64002a')})`);
  if(!capabilities.webCodecs)throw Error('This browser does not support frame-exact video encoding');
  console.log('Capturing actual WebGL canvas at fixed 1/60-second animation times.');
  const encoded=await ev(`(async()=>{
   const phone=document.querySelector('.phone');phone.style.width=(1080*390/844)+'px';phone.style.height='1080px';phone.style.flexShrink='0';window.dispatchEvent(new Event('resize'));
   corridorDemo.setProgress(0);
   const source=document.querySelector('canvas'),out=document.createElement('canvas');out.width=1920;out.height=1080;
   const ctx=out.getContext('2d',{alpha:false});ctx.imageSmoothingEnabled=true;ctx.imageSmoothingQuality='high';
   const toBase64=bytes=>{let s='';for(let i=0;i<bytes.length;i+=32768)s+=String.fromCharCode(...bytes.subarray(i,i+32768));return btoa(s);};
   let config;
   for(const codec of ${JSON.stringify(software?['avc1.42002a','avc1.4d002a','avc1.64002a']:['avc1.64002a','avc1.4d002a','avc1.42002a'])}){
    const candidate={codec,width:1920,height:1080,bitrate:24000000,framerate:60,latencyMode:'realtime',hardwareAcceleration:${JSON.stringify(software?'prefer-software':'prefer-hardware')},avc:{format:'avc'}};
    if((await VideoEncoder.isConfigSupported(candidate)).supported){config=candidate;break;}
   }
   if(!config)throw Error('1080p60 H.264 encoding is not supported');
   const samples=[];let description=null,encoderError=null;
   const encoder=new VideoEncoder({output(chunk,metadata){const bytes=new Uint8Array(chunk.byteLength);chunk.copyTo(bytes);samples.push({timestamp:chunk.timestamp,key:chunk.type==='key',data:toBase64(bytes)});if(metadata.decoderConfig?.description)description=toBase64(new Uint8Array(metadata.decoderConfig.description));},error(e){encoderError=e.message;}});
   encoder.configure(config);
   const draw=()=>{ctx.fillStyle='#000';ctx.fillRect(0,0,1920,1080);const w=1080*390/844;ctx.drawImage(source,(1920-w)/2,0,w,1080);};
   const references={};
   for(let i=0;i<=180;i++){
    corridorDemo.setProgress(i/180);draw();
    if(i===0||i===90||i===180)references[i]=out.toDataURL('image/png');
    const frame=new VideoFrame(out,{timestamp:Math.round(i*1000000/60),duration:Math.round(1000000/60)});
    encoder.encode(frame,{keyFrame:i%60===0});frame.close();
    if(encoder.encodeQueueSize>3)await encoder.flush();
    if(encoderError)throw Error(encoderError);
   }
   await encoder.flush();encoder.close();
   if(samples.length!==181||!description)throw Error('Incomplete encoded output');
   return{config,description,samples,references,sourceResolution:[source.width,source.height],frames:181};
  })()`);
  for(const [frame,png] of Object.entries(encoded.references))fs.writeFileSync(path.join(scratch,`reference-${frame}.png`),Buffer.from(png.split(',')[1],'base64'));
  const mp4=mux(encoded);
  const base64=mp4.toString('base64');
  const validation=await ev(`(async()=>{
   const binary=atob(${JSON.stringify(base64)}),bytes=new Uint8Array(binary.length);for(let i=0;i<binary.length;i++)bytes[i]=binary.charCodeAt(i);
   const video=document.createElement('video');video.muted=true;video.src=URL.createObjectURL(new Blob([bytes],{type:'video/mp4'}));
   await new Promise((resolve,reject)=>{video.onloadedmetadata=resolve;video.onerror=()=>reject(Error('MP4 decode failed'));});
   const decoded={width:video.videoWidth,height:video.videoHeight,duration:video.duration,frames:{}};
   const canvas=document.createElement('canvas');canvas.width=1920;canvas.height=1080;const ctx=canvas.getContext('2d');
   for(const [name,time] of [['0',.000001],['78',1.300001],['90',1.500001],['105',1.750001],['120',2.000001],['180',3.000001]]){
    video.currentTime=time;await new Promise((resolve,reject)=>{video.onseeked=resolve;video.onerror=()=>reject(Error('MP4 seek failed'));});
    ctx.drawImage(video,0,0);decoded.frames[name]=canvas.toDataURL('image/png');
   }
   URL.revokeObjectURL(video.src);return decoded;
  })()`);
  if(validation.width!==1920||validation.height!==1080||Math.abs(validation.duration-181/60)>.002)throw Error('MP4 dimensions or timing validation failed');
  for(const [frame,png] of Object.entries(validation.frames))fs.writeFileSync(path.join(scratch,`decoded-${frame}.png`),Buffer.from(png.split(',')[1],'base64'));
  const unchanged=protectedFiles.every(f=>hash(f)===before[f]);if(!unchanged)throw Error('Protected animation files changed');
  fs.writeFileSync(output,mp4);
  const report={output,resolution:[1920,1080],fps:60,frames:181,animationDurationSeconds:3,videoDurationSeconds:validation.duration,finalFrameHoldSeconds:1/60,codec:encoded.config.codec,bitrate:encoded.config.bitrate,sourceCanvas:encoded.sourceResolution,framing:'Original portrait composition centered with black side bars; no cropping or perspective change',capture:'Actual running V2 browser renderer, sampled at exact 1/60-second animation times',existingAnimationFilesUnchanged:unchanged,bytes:mp4.length,sha256:crypto.createHash('sha256').update(mp4).digest('hex'),browserDecodeValidated:true};
  fs.writeFileSync(path.join(scratch,'mp4-export-report.json'),JSON.stringify(report,null,2));console.log(JSON.stringify(report));
 }finally{if(ws)ws.close();chrome.kill();}
}
main().catch(error=>{console.error(error.stack);process.exitCode=1;});
