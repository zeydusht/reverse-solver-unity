"""Background music (Zeyd chose music-c, 57.6 s seamless loop): MP3 128 kbps for the web (served from GitHub Pages
/music/, never in the build) and OGG Vorbis for iOS (IosBuild copies it into Resources for that build only).
Run: python Tools/make_music.py   (needs: pip install lameenc soundfile numpy)"""
import lameenc, soundfile as sf, numpy as np
src = 'Assets/Audio/MusicCandidates/music-c.wav'
x, sr = sf.read(src, dtype='int16')
enc = lameenc.Encoder(); enc.set_bit_rate(128); enc.set_in_sample_rate(sr); enc.set_channels(2); enc.set_quality(2)
mp3 = enc.encode(x.tobytes()) + enc.flush()
open('Assets/Audio/Music/music.mp3', 'wb').write(mp3)
data = sf.read(src, dtype='float32')[0]
with sf.SoundFile('Assets/Audio/Music/music.ogg', 'w', sr, 2, format='OGG', subtype='VORBIS', compression_level=0.6) as f:
    for i in range(0, len(data), 44100): f.write(data[i:i + 44100])   # in blocks: one big write crashes libsndfile's Vorbis
import os
print('mp3', round(len(mp3) / 1e6, 2), 'MB; ogg', round(os.path.getsize('Assets/Audio/Music/music.ogg') / 1e6, 2), 'MB;', len(x), 'samples @', sr)
