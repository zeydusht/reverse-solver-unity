"""Zips Builds/iOS (the Xcode project from IosBuild.Build) for the Mac: unix mode bits so macOS keeps the IL2CPP tools
and scripts executable. Run: python Tools/zip_ios.py 0.5.1  ->  Builds/ReverseSolver-iOS-0.5.1.zip"""
import os, sys, stat, time, zipfile

version = sys.argv[1]
src, top = 'Builds/iOS', f'ReverseSolver-iOS-{version}'
out = f'Builds/{top}.zip'
n = ex = 0
with zipfile.ZipFile(out, 'w', zipfile.ZIP_DEFLATED, compresslevel=6) as z:
    for root, dirs, files in os.walk(src):
        for f in files:
            p = os.path.join(root, f); rel = os.path.relpath(p, src).replace(os.sep, '/')
            info = zipfile.ZipInfo(top + '/' + rel)
            info.compress_type = zipfile.ZIP_DEFLATED
            info.create_system = 3   # unix, so the mode bits below are honoured by macOS Archive Utility
            extn = os.path.splitext(f.lower())[1]
            exe = extn in ('.sh', '.dylib', '.command') or ('Il2CppOutputProject/IL2CPP' in rel and extn == '')
            info.external_attr = (stat.S_IFREG | (0o755 if exe else 0o644)) << 16
            info.date_time = time.localtime(os.stat(p).st_mtime)[:6]
            with open(p, 'rb') as fh: z.writestr(info, fh.read())
            n += 1; ex += exe
print(out, n, 'dosya,', ex, 'çalıştırılabilir,', round(os.path.getsize(out) / 1e6), 'MB')
