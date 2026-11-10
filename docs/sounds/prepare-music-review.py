"""Validate original background-music sketches and prepare a local listening page."""
from __future__ import annotations
import array
import hashlib
import html
import json
import math
import struct
import sys
import wave
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'Assets/Audio/MusicCandidates'
selection_path = OUT / 'selection.json'
selection = json.loads(selection_path.read_text(encoding='utf-8')) if selection_path.exists() else None
SPECS = [
    ('a', 'Şatonun Sırrı', 'Gizemli ve sıcak', 'Marimba, yumuşak yaylılar ve kısa flüt cevapları. Şatonun masalsı havasına yakın.', '40', '#3aa69d'),
    ('b', 'Tablo Koridoru', 'Zarif ve sakin', 'Yumuşak piyano valsi ve arp dokunuşları. Daha ağırbaşlı, rahat bir akış.', '33', '#bda5df'),
    ('c', 'Renkli Parçalar', 'Hafif ve neşeli', 'Ahşap tınılar, telli eşlik ve hafif ritim. Oyuna biraz daha hareket katar.', '05', '#e7b05d'),
    ('d', 'Gece Koleksiyonu', 'Düşsel ve huzurlu', 'Çan, arp ve uzun yumuşak akorlar. Vurmalısız, daha sakin bir büyü hissi.', '31', '#859fe0'),
]

def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()

def vlq(value):
    value = int(value)
    result = [value & 127]
    while value >> 7:
        value >>= 7
        result.insert(0, (value & 127) | 128)
    return bytes(result)

def midi_chunk(events, end_tick):
    events.sort(key=lambda e: (e[0], e[1]))
    data = bytearray()
    prev = 0
    for tick, priority, message in events:
        data += vlq(tick - prev) + message
        prev = tick
    data += vlq(max(0, end_tick - prev)) + b'\xff\x2f\x00'
    return b'MTrk' + struct.pack('>I', len(data)) + data

def write_midi(score, path):
    ppq = 480
    end_tick = score['bars'] * score['beatsPerBar'] * ppq
    tempo = round(60_000_000 / score['bpm'])
    chunks = [midi_chunk([
        (0, 0, b'\xff\x51\x03' + tempo.to_bytes(3, 'big')),
        (0, 0, b'\xff\x58\x04' + bytes([score['beatsPerBar'], 2, 24, 8])),
    ], end_tick)]
    programs = {'piano': 0, 'marimba': 12, 'pluck': 24, 'harp': 46,
                'celeste': 8, 'bass': 32, 'pad': 48, 'flute': 73}
    instruments = sorted({e['instrument'] for e in score['notes']})
    channel = 0
    for instrument in instruments:
        percussion = instrument in {'brush', 'wood', 'thump'}
        ch = 9 if percussion else channel
        if not percussion:
            channel += 1
        name = instrument.encode('ascii')
        events = [(0, 0, b'\xff\x03' + vlq(len(name)) + name)]
        if not percussion:
            events += [(0, 0, bytes([0xC0 | ch, programs[instrument]])),
                       (0, 0, bytes([0xB0 | ch, 91, 45]))]
        for event in score['notes']:
            if event['instrument'] != instrument:
                continue
            tick = round(event['beat'] * ppq)
            off = min(end_tick, tick + max(1, round(event['length'] * ppq)))
            pitch = {'brush': 42, 'wood': 37, 'thump': 36}.get(instrument, event['midi'])
            vel = max(12, min(110, round(event['velocity'] * 650)))
            events += [(tick, 2, bytes([0x90 | ch, pitch, vel])),
                       (off, 1, bytes([0x80 | ch, pitch, 0]))]
        chunks.append(midi_chunk(events, end_tick))
    path.write_bytes(b'MThd' + struct.pack('>IHHH', 6, 1, len(chunks), ppq) + b''.join(chunks))

records = []
for ident, title, mood, description, picture, accent in SPECS:
    score_path = OUT / f'score-{ident}.json'
    score = json.loads(score_path.read_text(encoding='utf-8'))
    wav_path = OUT / f'music-{ident}.wav'
    with wave.open(str(wav_path), 'rb') as wav:
        assert wav.getparams()[:3] == (2, 2, 44100), 'Expected stereo 16-bit 44100-Hz PCM'
        assert wav.getcomptype() == 'NONE'
        frames = wav.getnframes()
        pcm = array.array('h', wav.readframes(frames))
    if sys.byteorder != 'little':
        pcm.byteswap()
    expected = round(score['bars'] * score['beatsPerBar'] * 60 / score['bpm'] * 44100)
    assert frames == expected == score['frames']
    peak = max(abs(v) for v in pcm)
    assert 100 < peak < 32760, 'Empty or clipping file'
    rms = math.sqrt(sum(v * v for v in pcm) / len(pcm)) / 32768
    rms_db = 20 * math.log10(rms)
    assert abs(rms_db - score['rmsDbfs']) < .02
    dc = sum(pcm) / len(pcm) / 32768
    assert abs(dc) < .001
    seam = max(abs(pcm[0] - pcm[-2]), abs(pcm[1] - pcm[-1])) / 32768
    assert seam < .025, 'Loop join has an unexpectedly large sample step'
    midi_path = OUT / f'music-{ident}.mid'
    write_midi(score, midi_path)
    samples = pcm[::64]
    peaks = []
    for i in range(120):
        part = samples[i * len(samples) // 120:(i + 1) * len(samples) // 120]
        peaks.append(round(math.sqrt(sum(v*v for v in part) / max(1, len(part))) / 32768, 5))
    rec = dict(id=ident, title=title, mood=mood, description=description,
               wav=wav_path.name, midi=midi_path.name, score=score_path.name,
               bpm=score['bpm'], meter=f"{score['beatsPerBar']}/4", bars=score['bars'],
               key=score['key'], duration=frames / 44100, bytes=wav_path.stat().st_size,
               sampleRate=44100, channels=2, bitDepth=16,
               rmsDbfs=round(rms_db, 4), peakDbfs=round(20*math.log10(peak/32768), 4),
               dcOffset=round(dc, 8), seamJump=round(seam, 8),
               noteCount=len(score['notes']), wavSha256=digest(wav_path),
               midiSha256=digest(midi_path), scoreSha256=digest(score_path),
               waveformRms=peaks, picture=picture, accent=accent)
    records.append(rec)
    print(f"{ident.upper()}: {rec['duration']:.2f}s, {rec['bpm']} BPM, {rec['meter']}, RMS {rms_db:.2f}, peak {rec['peakDbfs']:.2f}, seam {seam:.6f}")

manifest = dict(title='Reverse Solver background music candidates',
                selectedCandidate=selection['candidate'] if selection else None,
                method='Original note arrangements rendered with a custom procedural synthesizer; no third-party recordings or samples',
                structure='24 bars per candidate; three related 8-bar phrases with instrumentation and melodic variation',
                loop='One exact musical period; voice tails and stereo ambience wrap circularly; no intro/outro fade',
                verification='PCM headers, complete frame counts, peaks, RMS, DC, numerical loop-join step and hashes checked; browser listening remains subjective',
                gameIntegration=False, candidates=records,
                build='docs/sounds/build-music-candidates.ps1',
                reviewBuild='docs/sounds/prepare-music-review.py',
                review='docs/sounds/music-candidates.html')
(OUT / 'manifest.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')

cards = []
for r in records:
    ident = r['id']
    chosen = bool(selection and selection['candidate'] == ident)
    badge = ' <span style="font-size:12px;color:#e5bb72">✓ Seçildi</span>' if chosen else ''
    duration = f"{int(r['duration'])//60}:{int(r['duration'])%60:02d}"
    max_rms = max(r['waveformRms']) or 1
    bars = ''.join(f'<rect x="{i*4}" y="{18-16*p/max_rms:.2f}" width="2" height="{max(2,32*p/max_rms):.2f}" rx="1"/>' for i,p in enumerate(r['waveformRms']))
    cards.append(f'''<article class="card" style="--accent:{r['accent']}">
<div class="cover"><img src="../../Assets/Art/Paintings/painting-{r['picture']}.jpg" alt=""><span class="letter">{ident.upper()}</span><span class="mood">{html.escape(r['mood'])}</span></div>
<div class="body"><h2>{html.escape(r['title'])}{badge}</h2><p class="desc">{html.escape(r['description'])}</p>
<div class="meta">{r['bpm']} BPM <span>·</span> {r['meter']} <span>·</span> {duration}</div>
<svg class="wave" viewBox="0 0 480 36" aria-hidden="true">{bars}</svg>
<audio id="music-{ident}" controls loop preload="metadata" src="../../Assets/Audio/MusicCandidates/{r['wav']}"></audio>
<div class="actions"><button class="play" data-id="{ident}">Dinle</button><button class="join" data-id="{ident}">Döngü geçişini dinle</button><a href="../../Assets/Audio/MusicCandidates/{r['wav']}" download>WAV indir ↓</a></div>
</div></article>''')
page = '''<!doctype html>
<html lang="tr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>Reverse Solver · Müzik adayları</title>
<style>
:root{color-scheme:dark;--bg:#102e36;--cream:#f8ebd0;--muted:#b6c8c6}*{box-sizing:border-box}body{margin:0;background:radial-gradient(ellipse at top,#1c5059,var(--bg) 70%);color:var(--cream);font:16px/1.55 "Segoe UI",system-ui,sans-serif}main{max-width:1100px;margin:auto;padding:44px 24px 60px}.eyebrow{letter-spacing:.18em;text-transform:uppercase;color:#e5bb72;font-size:12px;font-weight:700}h1{font-size:clamp(30px,5vw,48px);line-height:1.15;margin:10px 0 16px;letter-spacing:-.035em}.intro{color:var(--muted);max-width:680px;margin-bottom:24px}.toolbar{display:flex;gap:22px;align-items:center;flex-wrap:wrap;padding:15px 20px;background:#ffffff08;border:1px solid #ffffff12;border-radius:14px;margin:26px 0}.toolbar label{display:flex;align-items:center;gap:10px;font-size:14px}input[type=range]{accent-color:#e5bb72;width:150px}input[type=checkbox]{accent-color:#61c3b8}.toolbar button{margin-left:auto;background:transparent;color:var(--cream);border:1px solid #ffffff38;padding:8px 12px;border-radius:8px}.grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:24px}.card{overflow:hidden;border-radius:20px;background:#143841;border:1px solid #ffffff14;box-shadow:0 12px 30px #0002;transition:border-color .2s}.card.active{border-color:var(--accent);box-shadow:0 0 0 2px color-mix(in srgb,var(--accent) 20%,transparent)}.cover{height:156px;position:relative;overflow:hidden}.cover img{width:100%;height:100%;object-fit:cover;object-position:center 40%;filter:brightness(.7)}.cover:after{content:"";position:absolute;inset:0;background:linear-gradient(transparent 20%,#143841df)}.letter{position:absolute;top:16px;left:18px;z-index:1;display:grid;place-items:center;width:42px;height:42px;border-radius:12px;background:var(--accent);color:#102e36;font-size:24px;font-weight:800}.mood{position:absolute;left:22px;bottom:10px;z-index:1;color:#fff;font-weight:600}.body{padding:18px 22px 22px}h2{font-size:24px;margin:0 0 8px;letter-spacing:-.02em}.desc{color:var(--muted);margin:0 0 14px;min-height:50px;font-size:14px}.meta{font-size:12px;color:var(--accent);font-weight:650}.meta span{padding:0 8px;color:#9aafaf}.wave{width:100%;height:38px;fill:var(--accent);opacity:.65;margin:14px 0 6px}audio{width:100%;height:38px}.actions{display:flex;gap:10px;align-items:center;flex-wrap:wrap;margin-top:14px}button,a{font:inherit}button{cursor:pointer;min-height:38px}.actions button{border:0;border-radius:9px;padding:9px 13px;font-size:13px;font-weight:700}.play{background:var(--accent);color:#102e36}.join{background:#ffffff0b;color:var(--cream);border:1px solid #ffffff22!important}.actions a{color:var(--muted);font-size:12px;text-decoration:none;margin-left:auto}.footer{color:#a4bcba;font-size:13px;margin-top:28px}button:focus-visible,a:focus-visible{outline:3px solid #fff;outline-offset:3px}@media(max-width:720px){main{padding:28px 16px}.grid{grid-template-columns:1fr}.toolbar{gap:16px}.cover{height:145px}.desc{min-height:0}}@media(prefers-reduced-motion:reduce){*{transition:none!important}}
</style></head><body><main>
<div class="eyebrow">Reverse Solver · 4 özgün aday</div><h1>Oyunun arkasında hangisi çalsın?</h1>
<p class="intro">Dört farklı hava, sözsüz melodiler. Her parçanın sonu başlangıcına bağlanır; döngüyü açık bırakarak oyunda sürekli nasıl hissettireceğini deneyebilirsin.</p>
<div class="toolbar"><label>Ses <input id="volume" type="range" min="0" max="1" step=".01" value=".55" aria-label="Ses düzeyi"></label><label><input id="loop" type="checkbox" checked> Sürekli döngü</label><button id="stop">Hepsini durdur</button></div>
<section class="grid">__CARDS__</section>
<p class="footer">Beğendiğin adayı A, B, C veya D diye söyle. Bunlar bu oyun için hazırlanmış özgün beste ve sentez taslaklarıdır; seçtiğimiz yönü birlikte geliştirebiliriz.</p>
</main><script>
const players=[...document.querySelectorAll('audio')],vol=document.querySelector('#volume'),loop=document.querySelector('#loop');
players.forEach(p=>{p.volume=Number(vol.value);p.addEventListener('play',()=>{players.forEach(q=>{if(q!==p)q.pause()});p.closest('.card').classList.add('active');p.closest('.card').querySelector('.play').textContent='Duraklat'});p.addEventListener('pause',()=>{p.closest('.card').classList.remove('active');p.closest('.card').querySelector('.play').textContent='Dinle'});p.addEventListener('error',()=>{p.closest('.card').querySelector('.desc').textContent='Ses dosyası açılamadı. Bu sayfayı repo içindeki docs/sounds klasöründen aç veya WAV bağlantısını kullan.'})});
vol.addEventListener('input',()=>players.forEach(p=>p.volume=Number(vol.value)));loop.addEventListener('change',()=>players.forEach(p=>p.loop=loop.checked));
document.querySelectorAll('.play').forEach(b=>b.addEventListener('click',()=>{const p=document.querySelector('#music-'+b.dataset.id);if(p.paused)p.play().catch(()=>{});else p.pause()}));
document.querySelectorAll('.join').forEach(b=>b.addEventListener('click',()=>{const p=document.querySelector('#music-'+b.dataset.id);loop.checked=true;players.forEach(q=>q.loop=true);const start=()=>{p.currentTime=Math.max(0,p.duration-5);p.play().catch(()=>{})};if(p.readyState>=1)start();else p.addEventListener('loadedmetadata',start,{once:true})}));
document.querySelector('#stop').addEventListener('click',()=>players.forEach(p=>{p.pause();p.currentTime=0}));
</script></body></html>'''.replace('__CARDS__', ''.join(cards))
(ROOT / 'docs/sounds/music-candidates.html').write_text(page, encoding='utf-8')
print('Listening page and manifest ready.')
