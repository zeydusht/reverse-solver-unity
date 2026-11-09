"""Build intro films and two source-audio mixes. Only main() writes media."""

SES = "B"
CLIP2_ZOOM = 1.03
CLIP2_DY = 9  # Downward image displacement at 1080x1920.
CLIP2_ZOOM_S = 1.2
MUSIC_RMS_DB = -18
CLIP_TRIMS = {
    1: (0.0, 3.0), 2: (0.5, 3.5), 3: (2.04, 5.04),
    4: (0.54, 3.04), 5: (0.54, 3.04), 6: (0.79, 3.04),
    7: (0.54, 3.04), 8: (0.54, 3.04), 9: (0.0, 3.04),
    10: (0.0, 3.04), 11: (0.0, 3.04),
}
TRANSITIONS = {(1, 2): 0.45, (6, 7): 0.3}
CLIP4_FADE = 0.3
CRF = 26
PREVIEW_CRF = 20
FPS = 24
SAMPLE_RATE = 44100

import argparse
import json
from pathlib import Path
import re
import subprocess
import tempfile
import wave

import numpy as np
from imageio_ffmpeg import get_ffmpeg_exe

ROOT = Path(__file__).resolve().parents[2]
VIDEO_DIR = ROOT / "docs/art/task26/video"
AUDIO_DIR = ROOT / "Assets/_Game/Presentation/Resources/Art/audio"
SES_DIR = Path(__file__).with_name("ses")
OUT_DIR = ROOT / "Assets/Intro"
PARTS = {"A": tuple(range(1, 9)), "B": (9,), "C": (10, 11)}
# Exact source filenames; variant selection rotates separately for each part.
SOURCES = {
    "cloth": tuple(f"cloth{i}.ogg" for i in range(1, 5)),
    "bookFlip": tuple(f"bookFlip{i}.ogg" for i in range(1, 4)),
    "doorClose": tuple(f"doorClose_{i}.ogg" for i in range(1, 5)),
    "footstep": tuple(f"footstep{i:02d}.ogg" for i in range(4)),
    "dropLeather": ("dropLeather.ogg",),
    "handleSmallLeather": ("handleSmallLeather.ogg", "handleSmallLeather2.ogg"),
    "metalClick": ("metalClick.ogg",),
    "pleasing-bell": ("pleasing-bell.ogg",),
    "musicbox1_spooky_waltz": ("musicbox1_spooky_waltz.ogg",),
    "musicbox2_cute_tune": ("musicbox2_cute_tune.ogg",),
    "musicbox4_happy_polka": ("musicbox4_happy_polka.ogg",),
    **{name: (f"{name}.wav",) for name in ("piece", "pop", "weee", "win")},
}


def ffmpeg(args):
    result = subprocess.run(
        [get_ffmpeg_exe(), "-hide_banner", "-nostdin", "-y", *map(str, args)],
        stdout=subprocess.PIPE, stderr=subprocess.PIPE,
    )
    if result.returncode:
        raise RuntimeError(result.stderr.decode("utf-8", errors="replace")[-12000:])
    return result


def source_path(filename):
    return (AUDIO_DIR if filename.endswith(".wav") else SES_DIR) / filename


def read_audio(path, start=None, end=None, ratio=1.0):
    filters = []
    if start is not None:
        filters += [f"atrim=start={start}:end={end}", "asetpts=PTS-STARTPTS"]
    if ratio != 1.0:
        filters += [f"asetrate={SAMPLE_RATE * ratio}", f"aresample={SAMPLE_RATE}"]
    raw = ffmpeg(["-v", "error", "-i", path, "-map", "0:a:0", "-vn",
                  "-af", ",".join([f"aresample={SAMPLE_RATE}", *filters]),
                  "-ar", SAMPLE_RATE, "-ac", "2", "-c:a", "pcm_f32le",
                  "-f", "f32le", "pipe:1"]).stdout
    return np.frombuffer(raw, dtype="<f4").reshape(-1, 2).astype(np.float64)


def trim_filter(clip):
    start, end = CLIP_TRIMS[clip]
    return f"trim=start={start}:end={end},setpts=PTS-STARTPTS,fps={FPS}"


def frame_count(clip):
    result = ffmpeg(["-v", "error", "-i", VIDEO_DIR / f"{clip}.mp4",
                     "-map", "0:v:0", "-vf", trim_filter(clip), "-an",
                     "-progress", "pipe:1", "-f", "null", "-"])
    counts = re.findall(rb"(?:^|\n)frame=(\d+)", result.stdout)
    if not counts or int(counts[-1]) == 0:
        raise ValueError(f"Clip {clip} has no trimmed frames")
    return int(counts[-1])


def timeline(counts):
    plans = {}
    for part, clips in PARTS.items():
        cursor = 0.0
        entries = []
        for index, clip in enumerate(clips):
            overlap = TRANSITIONS.get((clips[index-1], clip), 0) if index else 0
            cursor -= overlap
            entries.append({"clip": clip, "in_seconds": CLIP_TRIMS[clip][0],
                            "out_seconds": CLIP_TRIMS[clip][1],
                            "start_seconds": cursor, "duration_seconds": counts[clip]/FPS,
                            "frames": counts[clip], "incoming_xfade_seconds": overlap})
            cursor += counts[clip]/FPS
        frames = round(cursor*FPS)
        plans[part] = {"frames": frames, "duration_seconds": frames/FPS, "clips": entries}
    if not 18 <= plans["A"]["duration_seconds"] <= 21:
        raise ValueError(f"A must be 18–21 s: {plans['A']['duration_seconds']:.3f} s")
    return plans


def build_video(part, plan, width, height, destination, crf):
    inputs, filters = [], []
    for index, entry in enumerate(plan["clips"]):
        clip = entry["clip"]
        inputs += ["-i", VIDEO_DIR / f"{clip}.mp4"]
        chain = (f"[{index}:v:0]{trim_filter(clip)},"
                 f"scale={width}:{height}:flags=lanczos,setsar=1")
        if clip == 2:
            progress = f"max(0,1-on/({FPS}*{CLIP2_ZOOM_S}))"
            zoom = f"1+({CLIP2_ZOOM}-1)*({progress})"
            # zoompan crop origin is moved upwards to shift the picture down.
            chain += (f",zoompan=z='{zoom}':x='iw/2-iw/zoom/2':"
                      f"y='ih/2-ih/zoom/2-{CLIP2_DY*height/1920}*({progress})/zoom':"
                      f"d=1:s={width}x{height}:fps={FPS}")
        if clip == 4:
            chain += f",fade=t=out:st={entry['duration_seconds']-CLIP4_FADE}:d={CLIP4_FADE}"
        filters.append(chain + f",format=yuv420p,settb=AVTB[v{clip}]")
    labels = [f"[v{c}]" for c in PARTS[part]]
    if part == "A":
        entries = {e["clip"]: e for e in plan["clips"]}
        for (left, right), duration in TRANSITIONS.items():
            pair_frames = round((entries[left]["duration_seconds"]
                                 + entries[right]["duration_seconds"]-duration)*FPS)
            filters.append(f"[v{left}][v{right}]xfade=transition=fade:duration={duration}:"
                           f"offset={entries[left]['duration_seconds']-duration},"
                           f"fps={FPS},trim=end_frame={pair_frames},"
                           f"setpts=PTS-STARTPTS[v{left}{right}]")
        labels = ["[v12]", "[v3]", "[v4]", "[v5]", "[v67]", "[v8]"]
    if len(labels) > 1:
        filters.append("".join(labels) + f"concat=n={len(labels)}:v=1:a=0[joined]")
        final = "[joined]"
    else:
        final = labels[0]
    filters.append(f"{final}fps={FPS},trim=end_frame={plan['frames']},"
                   "setpts=PTS-STARTPTS,format=yuv420p[outv]")
    ffmpeg(["-v", "error", *inputs, "-filter_complex_threads", "1",
            "-filter_complex", ";".join(filters), "-map", "[outv]", "-an",
            "-c:v", "libx264", "-crf", crf, "-preset", "veryfast",
            "-r", FPS, "-frames:v", plan["frames"], "-movflags", "+faststart", destination])


def silence(seconds):
    return np.zeros((round(seconds*SAMPLE_RATE), 2), dtype=np.float64)


def fit_audio(sound, length):
    fitted = np.zeros((length, 2), dtype=np.float64)
    fitted[:min(length, len(sound))] = sound[:length]
    return fitted


def rms_gain(sound, db, cap_db=None):
    rms = float(np.sqrt(np.mean(sound**2))) if sound.size else 0
    gain = 10**(db/20)/rms if rms else 1.0
    return min(gain, 10**(cap_db/20)) if cap_db is not None else gain


def fade(sound, seconds, incoming, equal_power=False):
    n = min(len(sound), round(seconds*SAMPLE_RATE))
    if not n:
        return
    ramp = np.linspace(0, 1, n)
    if equal_power:
        ramp = np.sin(ramp*np.pi/2)
    if incoming:
        sound[:n] *= ramp[:, None]
    else:
        sound[-n:] *= ramp[::-1, None]


def overlay(bus, sound, at):
    first = round(at*SAMPLE_RATE)
    source, dest = max(0, -first), max(0, first)
    length = min(len(sound)-source, len(bus)-dest)
    if length > 0:
        bus[dest:dest+length] += sound[source:source+length]


def soft_limit(sound):
    ceiling = 10**(-1/20)
    magnitude = np.abs(sound)
    knee = ceiling*0.8
    above = magnitude > knee
    sound = sound.copy()
    sound[above] = np.sign(sound[above]) * (
        knee + (ceiling-knee)*np.tanh((magnitude[above]-knee)/(ceiling-knee)))
    return sound.astype(np.float32)


def mix_part(mix_name, part, plan, cache):
    bus = silence(plan["duration_seconds"])
    entries = {e["clip"]: e for e in plan["clips"]}
    events, rotation = [], {}

    def load(filename, ratio=1.0):
        key = (filename, ratio)
        if key not in cache:
            cache[key] = read_audio(source_path(filename), ratio=ratio)
        return cache[key]

    def add(clip, at, name, gain=1.0, ratio=1.0):
        variants = SOURCES[name]
        index = rotation.get(name, 0)
        rotation[name] = index+1
        filename = variants[index % len(variants)]
        sound = load(filename, ratio).copy()*gain
        fade(sound, 0.005, True)
        fade(sound, 0.03, False)
        start = entries[clip]["start_seconds"]+at
        overlay(bus, sound, start)
        events.append({"clip": clip, "sound": filename,
                       "source": source_path(filename).relative_to(ROOT).as_posix(),
                       "clip_time_seconds": at, "part_time_seconds": start,
                       "gain": gain, "pitch_ratio": ratio})

    def end(clip, before):
        return entries[clip]["duration_seconds"]-before

    if mix_name == "A":
        clips = list(entries)
        for index, clip in enumerate(clips):
            entry = entries[clip]
            sound = read_audio(VIDEO_DIR / f"{clip}.mp4", *CLIP_TRIMS[clip])
            gain = rms_gain(sound, -20, cap_db=12)
            sound = fit_audio(sound*gain, round(entry["duration_seconds"]*SAMPLE_RATE))
            incoming = entry["incoming_xfade_seconds"]
            outgoing = TRANSITIONS.get((clip, clips[index+1]), 0) if index+1 < len(clips) else 0
            fade(sound, incoming or 0.03, True, bool(incoming))
            fade(sound, outgoing or 0.03, False, bool(outgoing))
            if clip == 4:
                fade(sound, CLIP4_FADE, False)
            overlay(bus, sound, entry["start_seconds"])
            events.append({"clip": clip, "source": f"docs/art/task26/video/{clip}.mp4",
                           "trim": CLIP_TRIMS[clip], "gain": gain,
                           "part_time_seconds": entry["start_seconds"],
                           "incoming_xfade_seconds": incoming, "outgoing_xfade_seconds": outgoing})
        if 5 in entries:
            add(5, end(5, 0.35), "piece", 0.3)
        if 10 in entries:
            add(10, 0.4, "weee", 0.3)
            add(11, end(11, 1.7), "win", 0.4)
        return soft_limit(bus), {"cues": events, "music": []}

    if part == "A":
        for at in (0.2, 0.7, 1.2):
            add(3, at, "cloth", 0.6)
        for at in (1.2, 1.5, 1.8):
            add(3, at, "pop", 0.6, 0.8)
        for at in np.arange(0.2, end(4, 1.0)+1e-8, 0.45):
            add(4, float(at), "footstep", 0.22)
        add(4, end(4, 0.6), "doorClose", 0.6)
        add(5, 0, "cloth", 0.6)
        for at in (0.7, 1.0, 1.3, 1.6):
            add(5, at, "bookFlip", 0.18)
        add(5, end(5, 0.35), "piece", 0.7)
        add(6, 0.25, "dropLeather", 0.7)
        add(6, end(6, 0.6), "handleSmallLeather", 0.6)
        for index, at in enumerate(np.arange(0.2, end(7, 0), 0.5)):
            add(7, float(at), "footstep", 0.3*0.7**index)
        add(8, end(8, 0.9), "pleasing-bell", 0.15, 1.3)
    elif part == "B":
        add(9, 0.2, "pleasing-bell", 0.5)
    else:
        add(10, 0.4, "weee", 0.7)
        for at, ratio in zip((0.6, 0.85, 1.1, 1.35), (0.96, 1.02, 0.99, 1.05)):
            add(10, at, "pop", 0.5, ratio)
        for at in (1.9, 2.2, 2.5):
            add(10, at, "piece", 0.65)
        for at in (0.5, 1.0):
            add(11, at, "metalClick", 0.4)
        add(11, end(11, 1.7), "win", 0.8)
    peak = float(np.max(np.abs(bus))) if bus.size else 0
    effect_gain = 10**(-3/20)/peak if peak else 1
    bus *= effect_gain
    music, music_events = music_bus(part, plan, load)
    return soft_limit(bus+music), {"cues": events, "effects_bus_gain": effect_gain,
                                  "effects_peak_dbfs": -3 if peak else None,
                                  "music": music_events}


def music_bus(part, plan, load):
    bus = silence(plan["duration_seconds"])
    entries = {e["clip"]: e for e in plan["clips"]}
    events = []

    def track(name, start, points):
        # Repeat only decoded source music if it is shorter than its cue.
        length = len(bus)-round(start*SAMPLE_RATE)
        source = load(SOURCES[name][0])
        if not len(source):
            raise ValueError(f"Empty music: {name}")
        sound = np.tile(source, (int(np.ceil(length/len(source))), 1))[:length].copy()
        times = np.arange(length)/SAMPLE_RATE+start
        shape = np.interp(times, [p[0] for p in points], [p[1] for p in points])
        # Measure sounding source at a unit envelope BEFORE applying its shape.
        sounding = sound[shape > 0]
        gain = rms_gain(sounding, MUSIC_RMS_DB)
        overlay(bus, sound*gain*shape[:, None], start)
        events.append({"sound": SOURCES[name][0], "part_time_seconds": start,
                       "envelope": points, "unit_envelope_rms_dbfs": MUSIC_RMS_DB,
                       "gain": gain})

    duration = plan["duration_seconds"]
    if part == "A":
        change = entries[2]["start_seconds"]+0.5
        start7 = entries[7]["start_seconds"]
        end7 = start7+entries[7]["duration_seconds"]
        start8 = entries[8]["start_seconds"]
        # Clip 7's first half reaches 0.5, then its second half reaches 0.08.
        middle7 = (start7+end7)/2
        track("musicbox2_cute_tune", 0, [(0, 1), (change, 1), (change+1, 0), (duration, 0)])
        track("musicbox1_spooky_waltz", change,
              [(change, 0), (change+1, 1), (start7, 1),
               (middle7, 0.5), (start8, 0.08), (duration, 0.08)])
    elif part == "B":
        track("musicbox2_cute_tune", 0, [(0, 0), (duration, 0.4)])
    else:
        track("musicbox4_happy_polka", 0, [(0, 1), (duration-0.4, 1), (duration, 0)])
    return bus, events


def write_wav(path, sound):
    pcm = np.rint(np.clip(sound, -1, 1)*32767).astype("<i2")
    with wave.open(str(path), "wb") as wav:
        wav.setnchannels(2)
        wav.setsampwidth(2)
        wav.setframerate(SAMPLE_RATE)
        wav.writeframes(pcm.tobytes())


def join_preview(videos, audio, destination, high=False):
    inputs = []
    for path in videos:
        inputs += ["-i", path]
    filters = ";".join(f"[{i}:v]setpts=PTS-STARTPTS[v{i}]" for i in range(3))
    filters += ";[v0][v1][v2]concat=n=3:v=1:a=0[outv]"
    ffmpeg(["-v", "error", *inputs, "-i", audio,
            "-filter_complex_threads", "1", "-filter_complex", filters,
            "-map", "[outv]", "-map", "3:a:0", "-c:v", "libx264",
            "-crf", PREVIEW_CRF if high else CRF, "-preset", "veryfast",
            "-r", FPS, "-pix_fmt", "yuv420p", "-c:a", "aac",
            "-b:a", "160k" if high else "128k", "-ar", SAMPLE_RATE,
            "-ac", "2", "-movflags", "+faststart", destination])


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--ses", choices=("A", "B"), default=SES)
    selected = parser.parse_args().ses
    print("Sound folder: " + ", ".join(sorted(p.name for p in SES_DIR.iterdir()
                                               if p.is_file())), flush=True)
    required = [VIDEO_DIR / f"{clip}.mp4" for clip in CLIP_TRIMS]
    required += [source_path(name) for names in SOURCES.values() for name in names]
    missing = [str(path) for path in required if not path.is_file()]
    if missing:
        raise FileNotFoundError("Missing inputs: " + ", ".join(missing))
    for clip, (start, end) in CLIP_TRIMS.items():
        if not 0 <= start < end:
            raise ValueError(f"Invalid trim for clip {clip}")
    plans = timeline({clip: frame_count(clip) for clip in CLIP_TRIMS})
    print(f"A duration: {plans['A']['duration_seconds']:.3f} s (required 18–21 s)", flush=True)
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    VIDEO_DIR.mkdir(parents=True, exist_ok=True)
    sizes, cache, sounds_used = {}, {}, {"A": {}, "B": {}}
    videos, high_res = [], []

    def record(path):
        sizes[path.relative_to(ROOT).as_posix()] = path.stat().st_size

    with tempfile.TemporaryDirectory(prefix="kurgu-") as temporary:
        temp = Path(temporary)
        sounds = {"A": [], "B": []}
        for part, plan in plans.items():
            video = OUT_DIR / f"intro{part}.mp4"
            build_video(part, plan, 720, 1280, video, CRF)
            videos.append(video)
            record(video)
            high = temp / f"intro{part}-1080.mp4"
            build_video(part, plan, 1080, 1920, high, 0)
            high_res.append(high)
            for mix_name in ("A", "B"):
                sound, details = mix_part(mix_name, part, plan, cache)
                sounds[mix_name].append(sound)
                sounds_used[mix_name][part] = details
                if mix_name == selected:
                    wav = temp / f"intro{part}.wav"
                    write_wav(wav, sound)
                    audio = OUT_DIR / f"intro{part}.mp3"
                    ffmpeg(["-v", "error", "-i", wav, "-t", plan["duration_seconds"],
                            "-ac", "2", "-ar", SAMPLE_RATE, "-c:a", "libmp3lame",
                            "-b:a", "96k", audio])
                    record(audio)
        joined_audio = {}
        for mix_name in ("A", "B"):
            wav = temp / f"mix-{mix_name}.wav"
            write_wav(wav, np.concatenate(sounds[mix_name]))
            joined_audio[mix_name] = wav
            comparison = VIDEO_DIR / f"intro_ses_{mix_name}.mp4"
            join_preview(videos, wav, comparison)
            record(comparison)
        preview = VIDEO_DIR / "intro_onizleme_v2.mp4"
        join_preview(high_res, joined_audio[selected], preview, high=True)
        record(preview)
    verification = {}
    total_frames = sum(plan["frames"] for plan in plans.values())
    for filename in sizes:
        path = ROOT / filename
        if path.suffix == ".mp3":
            expected = round(plans[path.stem[-1]]["duration_seconds"]*SAMPLE_RATE)
            actual = len(read_audio(path))
            gap = (actual-expected)/SAMPLE_RATE
            if abs(actual-expected) > 1:
                raise ValueError(f"Decoded audio length mismatch: {filename}: {gap} s")
            verification[filename] = {"decoded_samples": actual, "gap_seconds": gap}
        else:
            result = ffmpeg(["-v", "error", "-i", path, "-map", "0:v:0", "-an",
                             "-progress", "pipe:1", "-f", "null", "-"])
            frames = int(re.findall(rb"(?:^|\n)frame=(\d+)", result.stdout)[-1])
            expected = plans[path.stem[-1]]["frames"] if path.parent == OUT_DIR else total_frames
            if frames != expected:
                raise ValueError(f"Decoded video length mismatch: {filename}: {frames} != {expected}")
            verification[filename] = {"decoded_frames": frames, "duration_seconds": frames/FPS}
    total = sum(size for name, size in sizes.items() if name.startswith("Assets/Intro/"))
    report = {
        "parts": plans, "trims": CLIP_TRIMS, "selected_mix": selected,
        "transitions": [{"from": a, "to": b, "seconds": seconds}
                        for (a, b), seconds in TRANSITIONS.items()],
        "clip4_fade_seconds": CLIP4_FADE,
        "clip2_zoom": {"zoom": CLIP2_ZOOM, "dy_at_1080": CLIP2_DY,
                       "seconds": CLIP2_ZOOM_S, "interpolation": "linear"},
        "sounds_used_per_mix": sounds_used,
        "library_available": sorted(p.name for p in SES_DIR.iterdir() if p.is_file()),
        "music_rms_dbfs": MUSIC_RMS_DB, "file_sizes_bytes": sizes,
        "assets_intro_total_bytes": total, "fps": FPS, "sample_rate": SAMPLE_RATE,
        "verification": verification,
        "crf": CRF, "preview_crf": PREVIEW_CRF,
        "timing_note": "Trims use decoded 24 fps frames; crossfades use requested seconds. "
                       "Final part lengths are rounded to the nearest frame. MP3 may include codec padding.",
    }
    Path(__file__).with_name("kurgu_ozet.json").write_text(
        json.dumps(report, ensure_ascii=False, indent=2)+"\n", encoding="utf-8")
    for part, plan in plans.items():
        print(f"{part}: {plan['duration_seconds']:.3f} s; selected mix {selected}")
    print(f"Intro total: {total:,} B; preview {preview.stat().st_size:,} B")
    for name, size in sizes.items():
        print(f"{name}: {size:,} B")
    print("Verified all output frames and gapless MP3 samples; no gaps between concatenated parts.")


if __name__ == "__main__":
    main()
