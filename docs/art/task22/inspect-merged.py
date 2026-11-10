from pathlib import Path
import re,collections
p=Path('Assets/Art/Redesign_T22/inceleme.html');s=p.read_text(encoding='utf-8')
print('markers',[(k,s.count(k)) for k in ['T22_AI_EK_START','T22_AI_EK_END','id="i101"','id="ai-ek"']])
print('headings',re.findall(r'<h[12][^>]*>(.*?)</h[12]>',s))
print('duppairs',[(k,v) for k,v in collections.Counter(re.findall(r'<img[^>]+src="([^"]+)',s)).items() if v>1])
for m in re.finditer(r'101',s):
 if 'once-sonra' in s[m.start():m.start()+180]:print('context',s[max(0,m.start()-150):m.start()+100])
print('tail',s[-1000:])
