const fs=require('fs'),path=require('path');const {control}=JSON.parse(fs.readFileSync(path.join(__dirname,'session.json'),'utf8'));
fetch('http://127.0.0.1:'+control,{method:'POST',body:process.argv[2]||fs.readFileSync(0,'utf8')}).then(x=>x.text()).then(console.log).catch(console.error);
