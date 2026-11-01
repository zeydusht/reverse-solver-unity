"""Copies the chosen Codex art into Assets/_Game/Presentation/Resources/Art/ (Art.cs reads it there).
The A/B choice lives ONLY here: change a letter in CHOICE (or run with one letter for all: python Tools/art_choice.py B),
then rebuild. #10/#11 have one version. Run: python Tools/art_choice.py"""
import os, shutil, sys
import uuid

CHOICE = {'button': 'A', 'hand': 'A', 'intro': 'A', 'hard': 'A', 'frame': 'A', 'victory': 'A', 'defeat': 'A', 'board': 'A', 'audio': 'A'}
if len(sys.argv) > 1: CHOICE = {k: sys.argv[1].upper() for k in CHOICE}

A = 'Assets/Art/'
OUT = 'Assets/_Game/Presentation/Resources/Art/'
def src(group):
    c = CHOICE[group]; a = c == 'A'
    return {
        'button': {'button.png': A + ('Menu/Buttons/level-button-teal-option-a-888x138.png' if a else 'Menu/Buttons/level-button-orange-option-b-888x138.png')},
        'hand': {'hand-tap.png': A + f'TutorialHand/Options/option-{c.lower()}-tap-288.png', 'hand-swipe.png': A + f'TutorialHand/Options/option-{c.lower()}-swipe-288.png'},
        'intro': {f'intro-{n}.png': A + f'ObstacleIntros/Option{c}/{n}-888x592.png' for n in ('nail', 'bomb', 'chain', 'wall')},
        'hard': {'hard.png': A + ('HardWarning/option-a-baron-warning-888x592.png' if a else 'HardWarning/option-b-jiggle-warning-888x592.png')},
        'frame': {'frame.png': A + ('UI/PopupFrame/option-a-teal-gold-888x592.png' if a else 'UI/PopupFrame/option-b-cream-teal-888x592.png')},
        'victory': {'victory.png': A + ('Victory/option-a-single-jiggle-768.png' if a else 'Victory/option-b-three-jiggles-768.png')},
        'defeat': {f'defeat-{n}.png': A + f'Defeat/Option{c}/{n}-512.png' for n in ('bomb', 'time', 'no-moves')},
        # board base: A = walnut for every level, gold for hard levels (the B file is the hard frame)
        'board': {'board.png': A + 'Gameplay/BoardBase/option-a-walnut-teal-888.png', 'board-hard.png': A + 'Gameplay/BoardBase/option-b-hard-gold-888.png'},
        'audio': {f'audio/{n}.wav': A + f'Audio/Option{c}/{n}.wav' for n in ('weee', 'piece', 'win', 'loss', 'pop', 'popfull')},
    }[group]

files = {}
for g in CHOICE: files.update(src(g))
for n in ('bomb', 'nail', 'chain', 'wall'): files[f'icon-{n}.png'] = A + f'Gameplay/ObstacleIcons/{n}-128.png'
for n in ('scissors', 'wand', 'hammer', 'clock'): files[f'icon-{n}.png'] = A + f'Gameplay/BoosterIcons/{n}-128.png'

# Task 5 (one version): every Claude-drawn element; name = file name; 9-slice borders from Codex's manifest
import json
man = json.load(open(A + 'UI/Task5/manifest.json', encoding='utf-8'))
borders = {}
for it in man['assets']:
    name = os.path.basename(it['path'])
    if name in files: name = 'hud-' + name          # icon-clock (HUD) must not replace the booster's icon-clock
    files[name] = it['path']
    if it.get('slice'): borders[name] = it['slice']
for n in ('booster-active', 'joint-target', 'joint-target-pulse'): files[n + '.png'] = A + f'Gameplay/Task6/{n}.png'   # Task 6
# Task 6 board art (Zeyd 2026-10-25): obstacles, chain and wall on the board
for src, dst in (('nail-head-384', 'board-nail'), ('bomb-counter-384', 'board-bomb'), ('chain-link-repeat-96x48', 'chain-repeat'),
                 ('chain-endpoint-144', 'chain-end'), ('wall-repeat-96x48', 'wall-stripe'), ('wall-corner-96x96', 'wall-corner')):
    files[dst + '.png'] = A + f'Gameplay/Task6/{src}.png'
# Codex Task 7 (v2 board + HUD, 2026-10-26); names in the game on the right
for src, dst in (('bomb-v2-384', 'bomb-v2'), ('nail-head-v2-384', 'board-nail'), ('board-frame-gold-v1', 'board-frame'), ('hud-level-badge', 'level-badge'),
                 ('hud-icon-piece', 'icon-piece'),
                 ('hud-icon-clock', 'hud-icon-clock'), ('hud-pause-normal', 'pause-normal'), ('hud-pause-pressed', 'pause-pressed')):
    files[dst + '.png'] = A + f'Gameplay/Task7/{src}.png'
# Task 7 has no manifest: 9-slice borders from HANDOFF (frame corner >= 90 px) and the pill's round ends (84 px)
borders.update({'board-frame.png': [200, 200, 200, 200]})   # the ornament band is ~200 px wide
# Codex Task 10 booster set (PM_MESAJ_14; Task 8/9 not used): button, badge, icons
for src, dst in (('booster-button', 'booster-button'), ('booster-badge', 'booster-badge'), ('booster-scissors', 'icon-scissors'),
                 ('booster-wand', 'icon-wand'), ('booster-hammer', 'icon-hammer'), ('booster-clock', 'icon-clock')):
    files[dst + '.png'] = A + f'Gameplay/Task10/{src}.png'
# Codex Task 11 (PM_MESAJ_14): HUD v3, menu Level button, card panel and buttons, stars; 9-slice from HANDOFF
for src, dst in (('hud-level-badge', 'level-badge'), ('hud-pill', 'hud-pill-normal'), ('hud-pill-urgent', 'hud-pill-hot'),
                 ('hud-icon-piece', 'icon-piece'), ('hud-icon-clock', 'hud-icon-clock'), ('menu-play-button', 'play-normal'),
                 ('btn-primary', 'primary-normal'), ('btn-secondary', 'secondary-normal'), ('card-panel', 'popup-card'),
                 ('star-full', 'star-full'), ('star-empty', 'star-empty')):
    if os.path.exists(A + f'Gameplay/Task11/{src}.png'): files[dst + '.png'] = A + f'Gameplay/Task11/{src}.png'
for n in ('hud-pill-normal', 'hud-pill-hot', 'play-normal', 'primary-normal', 'secondary-normal'): borders[n + '.png'] = [72, 72, 72, 72]
borders['popup-card.png'] = [96, 96, 96, 96]
files['hand-swipe.png'] = A + 'TutorialHand/Exports/hand-swipe-vertical-288.png'    # vertical export (PM_MESAJ_8)
files['hand-tap.png'] = A + 'TutorialHand/Exports/hand-tap-vertical-288.png'
files['skin-body.png'] = A + 'PieceSkin/body-256.png'
files['skin.json'] = A + 'PieceSkin/skin.json'
# Codex Task 13 menu set (approved 2026-10-26): every sprite as m13-<name>; manifest sliceLTRB -> Unity L,B,R,T
m13 = json.load(open(A + 'Menu/Task13/manifest.json', encoding='utf-8'))
for it in m13['assets']:
    if it['name'].startswith('preview') or not it['path'].endswith('.png'): continue
    files['m13-' + it['name'] + '.png'] = it['path']
    l, r, t, b = it.get('sliceLTRB') or [0, 0, 0, 0]          # values are L,R,T,B (as HANDOFF), despite the key name
    if l or t or r or b: borders['m13-' + it['name'] + '.png'] = [l, b, r, t]
# approved Level button v2 (Assets/Art/UI/PlayButton): 684x336, 9-slice 92 px
for src, dst in (('menu-play-button-v2', 'level-button'), ('menu-play-button-v2-pressed', 'level-button-pressed')):
    files[dst + '.png'] = A + f'UI/PlayButton/{src}.png'; borders[dst + '.png'] = [92, 92, 92, 92]
# Codex Task 14 (approved in-game set, PM_MESAJ_15): replaces the Task 7-12 board/HUD/booster/card art
T14 = A + 'Gameplay/Task14/'
for src, dst in (('hud-level-badge', 'level-badge'), ('hud-moves-pill', 'hud-moves-pill'), ('hud-time-pill', 'hud-time-pill'),
                 ('hud-time-pill-urgent', 'hud-time-pill-urgent'), ('hud-pause-normal', 'pause-normal'), ('hud-pause-pressed', 'pause-pressed'),
                 ('booster-badge', 'booster-badge'), ('booster-bar', 'booster-bar'), ('board-frame-gold', 'board-frame'),
                 ('bomb-384', 'bomb-v2'), ('nail-head-384', 'board-nail'), ('chain-link-repeat-96x48', 'chain-repeat'),
                 ('chain-endpoint-144', 'chain-end'), ('wall-repeat-96x48', 'wall-stripe'), ('wall-corner-96x96', 'wall-corner'),
                 ('card-panel', 'popup-card'), ('btn-primary', 'primary-normal'), ('btn-secondary', 'secondary-normal'),
                 ('btn-danger', 'danger-normal'), ('star-full', 'star-full'), ('star-empty', 'star-empty'),
                 ('loss-reason-bomb', 'loss-reason-bomb'), ('loss-reason-time', 'loss-reason-time')):
    files[dst + '.png'] = T14 + src + '.png'
for b in ('scissors', 'wand', 'hammer', 'clock'):
    for st in ('normal', 'pressed', 'locked', 'zero'): files[f'booster-{b}-{st}.png'] = T14 + f'booster-{b}-{st}.png'
# Task 14 manifest gives L,R,T,B (as HANDOFF); Unity wants L,B,R,T
t14 = {a['file']: a for a in json.load(open(T14 + 'manifest.json', encoding='utf-8-sig'))}
for src, dst in (('hud-moves-pill', 'hud-moves-pill'), ('hud-time-pill', 'hud-time-pill'), ('hud-time-pill-urgent', 'hud-time-pill-urgent'),
                 ('booster-bar', 'booster-bar'), ('board-frame-gold', 'board-frame'), ('card-panel', 'popup-card'),
                 ('btn-primary', 'primary-normal'), ('btn-secondary', 'secondary-normal'), ('btn-danger', 'danger-normal')):
    l, r, t, b = t14[src + '.png']['sliceLTRB']; borders[dst + '.png'] = [l, b, r, t]
# Codex Task 15 (PM_MESAJ_16 B): red badge, wall v3, taller HUD pills with separate icons, short tall card buttons,
# five avatars, the Jiggles counter icon, the painting test. NOT taken (Zeyd): its board frame and boxed booster bar.
T15 = A + 'Gameplay/Task15/'
for src, dst in (('booster-badge-v2', 'booster-badge'), ('wall-repeat-v3', 'wall-stripe'), ('wall-corner-v3', 'wall-corner'),
                 ('hud-moves-pill-v2', 'hud-moves-pill'), ('hud-time-pill-v2', 'hud-time-pill'), ('hud-time-pill-urgent-v2', 'hud-time-pill-urgent'),
                 ('hud-icon-piece-v2', 'icon-piece'), ('hud-icon-stopwatch-v2', 'hud-icon-clock'),
                 ('btn-primary-v2', 'btn2-primary'), ('btn-primary-v2-pressed', 'btn2-primary-pressed'),
                 ('btn-secondary-v2', 'btn2-secondary'), ('btn-secondary-v2-pressed', 'btn2-secondary-pressed'),
                 ('avatar-blue', 'avatar-blue'), ('avatar-pink', 'avatar-pink'), ('avatar-red', 'avatar-red'),
                 ('avatar-green', 'avatar-green'), ('avatar-purple', 'avatar-purple'), ('jiggle-counter-icon', 'jiggle-counter'),
                 ('painting-test-01', 'painting-test-01')):
    files[dst + '.png'] = T15 + src + '.png'
# Codex Task 17 (PM_MESAJ_20): coloured v3 chains, links and locks per pair (gold, teal, purple, coral)
for c in ('altin', 'turkuaz', 'mor', 'mercan'):
    for k in ('link', 'lock'): files[f'chain-{k}-v3-{c}.png'] = A + f'Gameplay/Task17/chain-{k}-v3-{c}.png'
for n in ('hud-moves-pill', 'hud-time-pill', 'hud-time-pill-urgent'): borders[n + '.png'] = [84, 84, 84, 84]
for n in ('btn2-primary', 'btn2-primary-pressed', 'btn2-secondary', 'btn2-secondary-pressed'): borders[n + '.png'] = [92, 92, 92, 92]
# Task20 uses actual L,T,R,B, unlike the older manifests above. Preserve its exact dimensions.
t20 = json.load(open(A + 'Menu/Task20/manifest.json', encoding='utf-8-sig'))
for it in t20:
    if it.get('isRuntime') is False or it.get('category') == 'preview': continue
    name = 't20-' + it['file']
    files[name] = A + 'Menu/Task20/' + it['file']
    l, t, r, b = it.get('sliceLTRB') or [0, 0, 0, 0]
    if l or t or r or b: borders[name] = [l, b, r, t]
# Painting titles stay separate from the remotely downloaded pictures.
names = []
for batch in range(1, 5):
    for p in json.load(open(A + f'Paintings/manifest-batch-{batch}.json', encoding='utf-8-sig'))['paintings']:
        names.append({'level': p['level'], 'tr': p.get('nameTr', p['theme']), 'en': p.get('nameEn', p.get('themeEn', p['theme']))})
json.dump({'paintings': names}, open('Assets/_Game/Presentation/Resources/painting_names.json', 'w', encoding='utf-8'), ensure_ascii=False, indent=2)
# Big sprites whose sides are not multiples of 4 cannot be block-compressed (WebGL keeps them raw, MBs each):
# pad them with transparent pixels at the right/bottom to a multiple of 4 and move the right/bottom 9-slice border
import struct, zlib
def png_size(path):
    with open(path, 'rb') as f: return struct.unpack('>II', f.read(24)[16:24])
def pad_png(src, dst, pw, ph):
    b = open(src, 'rb').read(); i = 8; idat = b''
    while i < len(b):
        n = struct.unpack('>I', b[i:i+4])[0]; t = b[i+4:i+8]; d = b[i+8:i+8+n]
        if t == b'IHDR': w, h, bd, ct = struct.unpack('>IIBB', d[:10])
        if t == b'IDAT': idat += d
        i += 12 + n
    assert bd == 8 and ct == 6, src
    raw = zlib.decompress(idat); st = w * 4; prev = bytearray(st); k2 = 0; rows = []
    for y in range(h):
        f = raw[k2]; line = bytearray(raw[k2+1:k2+1+st]); k2 += 1 + st
        if f:
            for x in range(st):
                a = line[x-4] if x >= 4 else 0; c = prev[x-4] if x >= 4 else 0; up = prev[x]
                if f == 1: line[x] = (line[x] + a) & 255
                elif f == 2: line[x] = (line[x] + up) & 255
                elif f == 3: line[x] = (line[x] + ((a + up) >> 1)) & 255
                else:
                    pa = abs(up - c); pb = abs(a - c); pc = abs(a + up - 2 * c)
                    line[x] = (line[x] + (a if pa <= pb and pa <= pc else up if pb <= pc else c)) & 255
        rows.append(bytes(line)); prev = line
    W2, H2 = w + pw, h + ph
    body = b''.join(b'\x00' + r + b'\x00' * (4 * pw) for r in rows) + (b'\x00' + b'\x00' * (4 * W2)) * ph
    def chunk(t, d): return struct.pack('>I', len(d)) + t + d + struct.pack('>I', zlib.crc32(t + d) & 0xffffffff)
    open(dst, 'wb').write(b'\x89PNG\r\n\x1a\n' + chunk(b'IHDR', struct.pack('>IIBBBBB', W2, H2, 8, 6, 0, 0, 0)) +
                          chunk(b'IDAT', zlib.compress(body, 6)) + chunk(b'IEND', b''))
PAD = {}
for dst, src in files.items():
    if not src.endswith('.png') or not os.path.exists(src): continue
    w, h = png_size(src)
    if (w * h >= 300 * 300 or dst.startswith('t20-')) and (w % 4 or h % 4):
        PAD[dst] = ((-w) % 4, (-h) % 4)
        if dst in borders: borders[dst] = [borders[dst][0], borders[dst][1] + PAD[dst][1], borders[dst][2] + PAD[dst][0], borders[dst][3]]
json.dump(borders, open('Assets/_Game/Editor/art_borders.json', 'w'), indent=0)
os.makedirs(OUT + 'audio', exist_ok=True)
for dst, s in files.items():
    if not os.path.exists(s): print('EKSİK', s); continue
    if dst in PAD: pad_png(s, OUT + dst, *PAD[dst])
    else: shutil.copyfile(s, OUT + dst)      # copyfile: keeps the destination .meta (import settings)
# Stable initial import settings even when the art sync runs without a licensed Unity Editor.
for it in t20:
    if it.get('isRuntime') is False or it.get('category') == 'preview': continue
    path = OUT + 't20-' + it['file'] + '.meta'
    if os.path.exists(path): continue
    l, t, r, b = it.get('sliceLTRB') or [0, 0, 0, 0]
    px, py = it.get('pivot', [.5, .5])
    guid = uuid.uuid5(uuid.NAMESPACE_URL, 'reverse-solver/' + path).hex
    with open(path, 'w', encoding='utf-8') as meta:
        meta.write(f'''fileFormatVersion: 2
guid: {guid}
TextureImporter:
  serializedVersion: 13
  internalIDToNameTable: []
  externalObjects: {{}}
  mipmaps:
    enableMipMap: 0
    sRGBTexture: 1
  isReadable: 0
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    wrapU: 1
    wrapV: {0 if it['file'] == 'levels-path-repeat.png' else 1}
    wrapW: 1
  nPOTScale: 0
  maxTextureSize: 2048
  spriteMode: 1
  spriteMeshType: 0
  alignment: 9
  spritePivot: {{x: {px}, y: {py}}}
  spritePixelsToUnits: {1 if it['file'].startswith('scene-') else 3}
  spriteBorder: {{x: {l}, y: {b}, z: {r}, w: {t}}}
  alphaUsage: 1
  alphaIsTransparency: 1
  textureType: 8
  textureShape: 1
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    textureFormat: -1
    textureCompression: 0
    crunchedCompression: 0
    overridden: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    secondaryTextures: []
    nameFileIdTable: {{}}
  userData:
  assetBundleName:
  assetBundleVariant:
''')
# not used by the game (pressed states, replaced sets, HTML-only copies): kept out of Resources so the build stays small
UNUSED = ['booster-bar', 'm13-bottom-nav', 'm13-nav-active', 'm13-nav-home', 'm13-nav-collection', 'm13-nav-jiggles', 'm13-nav-trophy', 'settings-row-on', 'settings-row-off', 'switch-on', 'switch-off', 'sound-on', 'sound-off', 'close-normal', 'close-pressed',
          'rating-normal', 'rating-pressed', 'rating-selected', 'thumb-up-normal', 'thumb-up-pressed', 'thumb-up-selected',
          'thumb-down-normal', 'thumb-down-pressed', 'thumb-down-selected', 'name-input-normal', 'name-input-focus', 'progress-track',
          'progress-fill', 'rotate-phone', 'star-burst', 'star-filling-glow', 'play-pressed', 'primary-pressed', 'secondary-pressed',
          'profile-pressed', 'settings-pressed', 'reset-pressed', 'icon-reset-pressed', 'booster-empty', 'booster-locked', 'booster-out',
          'booster-active', 'booster-badge-stock', 'booster-badge-lock', 'booster-normal', 'bomb-counter-normal', 'bomb-counter-hot',
          'nail-counter', 'bomb-glow-normal', 'bomb-glow-hot', 'avatar-source', 'wall-repeat', 'chain-cable',
          'chain-link', 'chain-disc-1', 'chain-disc-2', 'chain-disc-3', 'chain-disc-4', 'frame', 'button', 'board', 'board-hard',
          'hand-tap', 'joint-target-pulse', 'skin-body', 'icon-lock-card', 'board-tray-unused', 'reset-normal', 'icon-reset', 'pause-pressed']
for n in UNUSED:
    for ext in ('.png', '.png.meta'):
        if os.path.exists(OUT + n + ext): os.remove(OUT + n + ext)
open(OUT + 'choice.txt', 'w').write(' '.join(f'{k}={v}' for k, v in CHOICE.items()) + '\n')
print(len(files), 'dosya;', open(OUT + 'choice.txt').read().strip())
