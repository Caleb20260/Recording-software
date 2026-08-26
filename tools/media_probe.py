#!/usr/bin/env python3
"""Inspect media files and FFmpeg availability for Lubb Interactive Creator.

Uses only the Python standard library. FFprobe is optional; without it the
script can still verify whether FFmpeg is available on PATH.
"""
from __future__ import annotations

import argparse
import json
import shutil
import subprocess
from pathlib import Path


def executable_info(name: str) -> dict[str, object]:
    path = shutil.which(name)
    if not path:
        return {"available": False, "path": None, "version": None}

    version = None
    try:
        result = subprocess.run(
            [path, "-version"], capture_output=True, text=True, timeout=5, check=False
        )
        first_line = result.stdout.splitlines()
        version = first_line[0] if first_line else None
    except (OSError, subprocess.SubprocessError):
        pass
    return {"available": True, "path": path, "version": version}


def probe_media(file_path: Path) -> dict[str, object]:
    ffprobe = shutil.which("ffprobe")
    if not ffprobe:
        return {"available": False, "reason": "ffprobe was not found on PATH"}

    command = [
        ffprobe,
        "-v",
        "error",
        "-show_format",
        "-show_streams",
        "-of",
        "json",
        str(file_path),
    ]
    try:
        result = subprocess.run(command, capture_output=True, text=True, timeout=20, check=False)
    except (OSError, subprocess.SubprocessError) as error:
        return {"available": False, "reason": str(error)}
    if result.returncode != 0:
        return {"available": False, "reason": result.stderr.strip() or "ffprobe failed"}
    return {"available": True, "data": json.loads(result.stdout)}


def main() -> int:
    parser = argparse.ArgumentParser(description="Diagnose FFmpeg and inspect a media file.")
    parser.add_argument("file", nargs="?", type=Path, help="Optional media file to inspect")
    args = parser.parse_args()

    report: dict[str, object] = {
        "ffmpeg": executable_info("ffmpeg"),
        "ffprobe": executable_info("ffprobe"),
    }
    if args.file:
        if not args.file.is_file():
            parser.error(f"media file does not exist: {args.file}")
        report["media"] = probe_media(args.file)
    print(json.dumps(report, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
