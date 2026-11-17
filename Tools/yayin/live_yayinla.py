"""PM_81: GitHub Pages'teki live/ klasörünü (mağaza sürümünün okuduğu tek yer) Builds/live'dan yayınlar.
YALNIZ PM ONAYIYLA çalıştırılır: --onay "<PM onayının kısa notu>" olmadan hiçbir şey yapmaz.

    python Tools/yayin/live_yayinla.py --onay "PM 18 Kasım: 41-60 bölümler"   [--worktree <gh-pages çalışma kopyası>] [--kuru]

Builds/live, Unity'de LiveExport (Placeholder100 / FromSet) ile üretilir: levels.json, config.json, manifest.json,
paintings/. Betik: manifest'teki her dosyanın SHA-256'sını kontrol eder, gh-pages'in live/ klasörünü bununla değiştirir
(p23, p22 ve köke dokunmaz), "live: <sürüm>" diye commit eder ve gönderir. --kuru: yalnız kontrol, gönderme yok."""
import argparse, hashlib, json, os, shutil, subprocess, sys, tempfile

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
STAGE = os.path.join(ROOT, "Builds", "live")


def run(cmd, cwd):
    r = subprocess.run(cmd, cwd=cwd, capture_output=True, text=True)
    if r.returncode != 0:
        sys.exit(f"hata: {' '.join(cmd)}\n{r.stderr.strip()[-800:]}")
    return r.stdout.strip()


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--onay", required=True, help="PM onayının kısa notu (zorunlu)")
    ap.add_argument("--worktree", help="gh-pages çalışma kopyası (yoksa geçici olarak açılır)")
    ap.add_argument("--kuru", action="store_true", help="yalnız kontrol, yayın yok")
    a = ap.parse_args()
    if len(a.onay.strip()) < 3:
        sys.exit("PM onayı yazılmadı: yayın yok")
    m = json.load(open(os.path.join(STAGE, "manifest.json"), encoding="utf8"))
    for f in m["files"]:
        data = open(os.path.join(STAGE, f["path"]), "rb").read()
        if hashlib.sha256(data).hexdigest() != f["sha256"]:
            sys.exit(f"özet tutmuyor: {f['path']}")
    print(f"live {m['version']}: {len(m['files'])} dosya, özetler tamam")
    if a.kuru:
        return
    wt, temp = a.worktree, None
    if not wt:
        temp = tempfile.mkdtemp(prefix="rs-live-")
        wt = os.path.join(temp, "ghp")
        run(["git", "fetch", "-q", "origin", "gh-pages"], ROOT)
        run(["git", "worktree", "add", "-q", "--detach", wt, "origin/gh-pages"], ROOT)
    else:
        run(["git", "fetch", "-q", "origin", "gh-pages"], wt)
        run(["git", "checkout", "-q", "--detach", "origin/gh-pages"], wt)
    try:
        dst = os.path.join(wt, "live")
        if os.path.isdir(dst):
            shutil.rmtree(dst)
        shutil.copytree(STAGE, dst)
        run(["git", "add", "-A", "live"], wt)
        if not run(["git", "status", "--short", "live"], wt):
            print("değişiklik yok")
            return
        run(["git", "commit", "-q", "-m", f"live: {m['version']} ({a.onay})"], wt)
        run(["git", "-c", "http.postBuffer=524288000", "push", "-q", "origin", "HEAD:gh-pages"], wt)
        print("yayınlandı:", run(["git", "log", "--oneline", "-1"], wt))
    finally:
        if temp:
            run(["git", "worktree", "remove", "--force", wt], ROOT)
            shutil.rmtree(temp, ignore_errors=True)


if __name__ == "__main__":
    main()
