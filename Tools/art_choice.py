"""Copies the chosen Codex art into Assets/_Game/Presentation/Resources/Art/ (Art.cs reads it there).
The A/B choice lives ONLY here: change a letter in CHOICE (or run with one letter for all: python Tools/art_choice.py B),
then rebuild. #10/#11 have one version. Run: python Tools/art_choice.py"""
import os, shutil, sys

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
        'audio': {f'audio/{n}.wav': A + f'Audio/Option{c}/{n}.wav' for n in ('weee', 'piece', 'win', 'loss')},
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
json.dump(borders, open('Assets/_Game/Editor/art_borders.json', 'w'), indent=0)
os.makedirs(OUT + 'audio', exist_ok=True)
for dst, s in files.items():
    if not os.path.exists(s): print('EKSİK', s); continue
    shutil.copyfile(s, OUT + dst)            # copyfile: keeps the destination .meta (import settings)
# not used by the game (pressed states, replaced sets, HTML-only copies): kept out of Resources so the build stays small
UNUSED = ['settings-row-on', 'settings-row-off', 'switch-on', 'switch-off', 'sound-on', 'sound-off', 'close-normal', 'close-pressed',
          'rating-normal', 'rating-pressed', 'rating-selected', 'thumb-up-normal', 'thumb-up-pressed', 'thumb-up-selected',
          'thumb-down-normal', 'thumb-down-pressed', 'thumb-down-selected', 'name-input-normal', 'name-input-focus', 'progress-track',
          'progress-fill', 'rotate-phone', 'star-burst', 'star-filling-glow', 'play-pressed', 'primary-pressed', 'secondary-pressed',
          'profile-pressed', 'settings-pressed', 'reset-pressed', 'icon-reset-pressed', 'booster-empty', 'booster-locked', 'booster-out',
          'booster-active', 'booster-badge-stock', 'booster-badge-lock', 'booster-normal', 'bomb-counter-normal', 'bomb-counter-hot',
          'nail-counter', 'bomb-glow-normal', 'bomb-glow-hot', 'avatar-source', 'wall-repeat', 'wall-stripe', 'wall-corner', 'chain-cable',
          'chain-link', 'chain-disc-1', 'chain-disc-2', 'chain-disc-3', 'chain-disc-4', 'frame', 'button', 'board', 'board-hard',
          'hand-tap', 'joint-target-pulse', 'skin-body', 'icon-lock-card', 'board-tray-unused', 'reset-normal', 'icon-reset', 'pause-pressed']
for n in UNUSED:
    for ext in ('.png', '.png.meta'):
        if os.path.exists(OUT + n + ext): os.remove(OUT + n + ext)
open(OUT + 'choice.txt', 'w').write(' '.join(f'{k}={v}' for k, v in CHOICE.items()) + '\n')
print(len(files), 'dosya;', open(OUT + 'choice.txt').read().strip())
