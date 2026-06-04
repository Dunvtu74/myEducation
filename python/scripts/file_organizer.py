import os
import shutil
import sys
from pathlib import Path

EXTENSIONS = {
    "images": [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"],
    "video":  [".mp4", ".mov", ".avi", ".mkv"],
    "audio":  [".mp3", ".wav", ".flac", ".ogg"],
    "docs":   [".pdf", ".doc", ".docx", ".txt", ".xlsx", ".csv"],
    "code":   [".py", ".js", ".ts", ".cpp", ".c", ".h", ".html", ".css"],
    "archives": [".zip", ".tar", ".gz", ".rar", ".7z"],
}

def get_category(ext):
    for category, exts in EXTENSIONS.items():
        if ext.lower() in exts:
            return category
    return "other"

def organize(folder):
    folder = Path(folder)
    if not folder.is_dir():
        print(f"not a directory: {folder}")
        return

    moved = 0
    for file in folder.iterdir():
        if not file.is_file():
            continue
        category = get_category(file.suffix)
        dest = folder / category
        dest.mkdir(exist_ok=True)
        shutil.move(str(file), dest / file.name)
        moved += 1

    print(f"moved {moved} files")

if __name__ == "__main__":
    target = sys.argv[1] if len(sys.argv) > 1 else "."
    organize(target)
