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
# Codex v2 (PM_MESAJ_12), used as soon as the files exist; names in the game on the right
for src, dst in (('Gameplay/Task6/bomb-v2-384.png', 'bomb-v2'), ('Gameplay/Task6/wall-repeat-v2.png', 'wall-stripe'),
                 ('Gameplay/Task6/board-frame-gold-v1.png', 'board-frame'), ('UI/HUD/hud-level-badge.png', 'level-badge'),
                 ('UI/HUD/hud-pill.png', 'hud-pill-normal'), ('UI/HUD/hud-pause-normal.png', 'pause-normal'),
                 ('UI/HUD/hud-pause-pressed.png', 'pause-pressed')):
    if os.path.exists(A + src): files[dst + '.png'] = A + src
files['hand-swipe.png'] = A + 'TutorialHand/Exports/hand-swipe-vertical-288.png'    # vertical export (PM_MESAJ_8)
files['hand-tap.png'] = A + 'TutorialHand/Exports/hand-tap-vertical-288.png'
files['skin-body.png'] = A + 'PieceSkin/body-256.png'
files['skin.json'] = A + 'PieceSkin/skin.json'
json.dump(borders, open('Assets/_Game/Editor/art_borders.json', 'w'), indent=0)
os.makedirs(OUT + 'audio', exist_ok=True)
for dst, s in files.items():
    if not os.path.exists(s): print('EKSİK', s); continue
    shutil.copyfile(s, OUT + dst)            # copyfile: keeps the destination .meta (import settings)
open(OUT + 'choice.txt', 'w').write(' '.join(f'{k}={v}' for k, v in CHOICE.items()) + '\n')
print(len(files), 'dosya;', open(OUT + 'choice.txt').read().strip())
