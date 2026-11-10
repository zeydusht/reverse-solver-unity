# Original-art corridor reconstruction

The approved source is `Assets/_Game/Presentation/Resources/menu_bg.png` / `Assets/Art/Menu/corridor-v04-source.png`. Neither is modified.

`docs/art/build-corridor-victory-v2.py` extracts the original frame, columns, arches, distant paintings, wall patterns and carpet. Known visible pixels sample the approved source directly at runtime. Scene atlases supply silhouette/depth information and newly exposed areas. `arches.png` now packs wall/floor source-validity data; it is not visible artwork.

`join-repair-source.png` is the one built-in image_gen repair used for the previously unseen join. Only its narrow repaired area is back-projected onto the fixed column plane. It does not replace a whole animation frame. All frames use the same immutable assets; no independent frame generation or endpoint image crossfade is used. Exact prompts: `docs/art/corridor-victory-v2-prompts.json`.

Camera trajectory, duration, easing, start/end composition and win/menu behavior are unchanged. The complete previous working version is in `docs/art/backups/corridor-victory-v1-20261030/`. See `docs/art/corridor-victory/README.md` for review links and checks.
