# PM54 Job C: continue offer

The scene references `Assets/_Game/Levels/redesign.json`: 40 timers, minimum
50 s, median 65 s, maximum 170 s; eight are below 60 s. The clock booster
adds 20 s. There was no coin system. Continue uses 20 s below 60 s and 30 s
otherwise, with a 1000 starting balance and 50 coins per win. These are
parameters on the pure `ContinueRules` class.

`GameRoot` retains the failed record without committing any loss side effects.
The existing loss sound, explosion/flash and delay run first. At the result-card
deadline the offer appears. Decline/visibility withdrawal commits the original
loss once; accept revives the same session and drops that provisional record.
The final plays row retains its original row_id. Restart/menu withdraw any
pending offer. Hidden continued attempts also retain a provisional final quit
event, removed on return, following the plays queue's existing hide pattern.

Resource keys: `hud40/headers/continue`, `devam_v2/{panel,close-button,
icon-close,continue-clock,continue-coin,continue-primary}` and
`jigo/uzgun/01` through `06` (the copied Jigo_v3 frames).
The panel fits a 340 x 470 pt region on 375 x 667, with 48 pt X,
64 pt purchase target and optional 44 pt decline target.

Flags: `&devam=1` previews Time at level start; its purchase/X only close it,
including when the displayed balance is insufficient. `&devamhayir=1` shows
the optional text decline; `&coin=5000` persists the startup balance.
All are gated by Art.T54, non-pt1 and non-PlayOverride. `art=eski` has no
offer and receives/spends no coins. Preview decisions produce no events.

The separate persistent `continue` queue uses the intro sender pattern;
missing-table 4xx responses park rows. Debug/editor sending is disabled.
No plays columns were added: only a continued win's existing result changes
to `won_after_continue`. Local first-try wins exclude it; progression, best
stars, win totals and coin rewards include it.

SQL audit: the three model views need no changes. The old standalone
005 query 2 classifies `result <> 'win'` as loss; a commented-out replacement
using explicit bomb/time outcomes is included in `015_continue.sql`.

Validation: EditMode coverage added in `ContinueTests.cs`, including ladder,
boundary, revival, record replacement, stars, column parity, local stats,
coins and queue parking. Static source/asset checks and whitespace checks
performed. Unity, builds and database operations were not run as instructed;
golden/EditMode execution and device visual verification remain unverified.
Assumption: final events use zero price/seconds and the balance immediately
before that final event; only actual purchase decisions carry a purchase price.
