# Codex rules (Reverse Solver)

- Browser tests: create Chrome/Edge `--user-data-dir` profiles ONLY outside the repo (e.g. `fs.mkdtempSync(path.join(os.tmpdir(), 'rs-chrome-'))`), and delete them when done. Never inside `docs/`, `Assets/` or any repo folder.
- Never write crash dumps (`*.dmp`, `Crashpad/`), browser caches, Python venvs or pip caches into the repo. Tool caches go to `%TEMP%`.
- Never print, log or save API keys or other secrets. A pre-commit hook (`.githooks/pre-commit`) blocks profiles, dumps, files >10 MB and key patterns.
- Do not read `Library/`, `Logs/`, `Builds/`, `*.csproj`.
