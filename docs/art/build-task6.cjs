const fs = require('fs'), path = require('path'), cp = require('child_process');
const root = path.resolve(__dirname, '../..');
function compressPNG(b) {
  const z=require('zlib'), chunks=[]; let ids=[];
  function chunk(type,data){const t=Buffer.from(type),n=Buffer.alloc(4),crc=Buffer.alloc(4);n.writeUInt32BE(data.length);let c=0xffffffff;for(const v of Buffer.concat([t,data])){c^=v;for(let i=0;i<8;i++)c=(c>>>1)^((c&1)?0xedb88320:0);}crc.writeUInt32BE((c^0xffffffff)>>>0);return Buffer.concat([n,t,data,crc]);}
  for(let i=8;i<b.length;){const n=b.readUInt32BE(i),t=b.toString('ascii',i+4,i+8),d=b.subarray(i+8,i+8+n);if(t==='IDAT')ids.push(d);else {if(t==='IEND')chunks.push(chunk('IDAT',z.deflateSync(z.inflateSync(Buffer.concat(ids)),{level:9})));chunks.push(chunk(t,d));}i+=n+12;}
  return Buffer.concat([b.subarray(0,8),...chunks]);
}
const out = path.join(root, 'Assets/Art/Gameplay/Task6');
fs.mkdirSync(out, {recursive:true});
const base = fs.readFileSync(path.join(root,'Assets/Art/Gameplay/Task5/booster-normal.svg'),'utf8');
const active = base.replace('stroke="#123f46" stroke-width="5"','stroke="#f9cf66" stroke-width="5"').replace('stroke="#baded3"','stroke="#fff2b2"').replace('</svg>', '<rect x="15" y="14" width="150" height="139" rx="36" fill="none" stroke="#f9cf66" stroke-opacity=".45" stroke-width="5"/></svg>');
function target(pulse) {
  const r = pulse ? 24 : 20;
  return `<svg xmlns="http://www.w3.org/2000/svg" width="66" height="66" viewBox="0 0 66 66"><defs><radialGradient id="halo"><stop stop-color="#f9cf66" stop-opacity=".8"/><stop offset="1" stop-color="#f9cf66" stop-opacity="0"/></radialGradient><linearGradient id="gold" x2="0" y2="1"><stop stop-color="${pulse?'#fff9e9':'#fff2b2'}"/><stop offset=".4" stop-color="#f9cf66"/><stop offset="1" stop-color="#f39449"/></linearGradient></defs><circle cx="33" cy="33" r="32" fill="url(#halo)"/><circle cx="33" cy="35" r="${r}" fill="#19252d"/><circle cx="33" cy="32" r="${r}" fill="url(#gold)" stroke="#936329" stroke-width="2"/><circle cx="33" cy="32" r="${r-4}" fill="none" stroke="#fff4da" stroke-width="2" stroke-opacity="${pulse?1:.7}"/></svg>`;
}
const assets = [
  {name:'booster-active',width:180,height:180,svg:active},
  {name:'joint-target',width:66,height:66,svg:target(false)},
  {name:'joint-target-pulse',width:66,height:66,svg:target(true)},
  {name:'loader-bg',width:750,height:1334,source:'data:image/png;base64,'+fs.readFileSync(path.join(root,'Assets/Art/Gameplay/Task5/game-background.png')).toString('base64')}
];
for(const a of assets) if(a.svg) fs.writeFileSync(path.join(out,a.name+'.svg'),a.svg);
const html = `<!doctype html><meta charset="utf-8"><script>const assets=${JSON.stringify(assets)};Promise.all(assets.map(a=>new Promise((ok,fail)=>{const im=new Image();im.onerror=fail;im.onload=()=>{const c=document.createElement('canvas');c.width=a.width;c.height=a.height;const x=c.getContext('2d');if(a.source){x.fillStyle='#19252d';x.fillRect(0,0,c.width,c.height);x.filter='blur(22px)';x.drawImage(im,-30,-54,810,1442);x.filter='none';x.fillStyle='rgba(12,25,33,.68)';x.fillRect(0,0,c.width,c.height);}else x.drawImage(im,0,0);const p=document.createElement('pre');p.dataset.name=a.name;p.textContent=c.toDataURL('image/png').split(',')[1];document.body.append(p);ok()};im.src=a.source||'data:image/svg+xml;base64,'+btoa(a.svg)}))).then(()=>document.title='DONE')</script>`;
const render=path.join(__dirname,'task6-render.html');fs.writeFileSync(render,html);
const result=cp.spawnSync('C:/Program Files/Google/Chrome/Application/chrome.exe',['--headless','--disable-gpu','--no-sandbox','--disable-background-networking','--user-data-dir='+path.join(__dirname,'.chrome-task6'),'--virtual-time-budget=10000','--dump-dom','file:///'+render.replaceAll('\\','/')],{encoding:'utf8',maxBuffer:20*1024*1024});
if(!result.stdout.includes('<title>DONE</title>')) throw Error('Rasterization failed: '+result.stderr.slice(-1000));
for(const a of assets){const m=result.stdout.match(new RegExp('<pre data-name="'+a.name+'">([^<]+)</pre>'));if(!m)throw Error(a.name);fs.writeFileSync(path.join(out,a.name+'.png'),compressPNG(Buffer.from(m[1],'base64')));}
const manifest={version:1,scale:3,sliceOrder:'left,bottom,right,top (Unity px)',origin:'PNG top-left; Unity border order as specified',assets:assets.map(({name,width,height})=>({name,width,height,path:'Assets/Art/Gameplay/Task6/'+name+'.png',slice:null}))};
fs.writeFileSync(path.join(out,'manifest.json'),JSON.stringify(manifest,null,2)+'\n');
fs.writeFileSync(path.join(out,'review.html'),`<!doctype html><html lang="tr"><meta charset="utf-8"><title>GÃ¶rev 6</title><style>body{background:#19252d;color:#fff4da;font:16px system-ui;margin:30px}section{display:inline-block;vertical-align:top;margin:16px;padding:16px;background:repeating-conic-gradient(#385158 0% 25%,#294047 0% 50%) 0/20px 20px}img{display:block;max-width:300px}small{display:block}</style><h1>GÃ¶rev 6 Â· Son eksikler</h1><section>GÃ¶rev 5 normal<img src="../Task5/booster-normal.png" width="180"></section>${assets.map(a=>`<section>${a.name}<img src="${a.name}.png" width="${a.width===750?300:a.width}"><small>${a.width} Ã— ${a.height} px Â· 9-slice yok</small></section>`).join('')}</html>`);
for(const a of assets){const b=fs.readFileSync(path.join(out,a.name+'.png'));if(b.readUInt32BE(16)!==a.width||b.readUInt32BE(20)!==a.height)throw Error('Dimensions: '+a.name);if(a.name==='loader-bg'&&b.length>250000)throw Error('Loader budget: '+b.length);console.log(a.name,a.width+'x'+a.height,b.length+' bytes');}
fs.unlinkSync(render);



