import json
import os

PATH = "seen.json"

def load_seen():
    if not os.path.exists(PATH):
        return set()
    with open(PATH, encoding="utf-8") as f:
        return set(json.load(f))

def save_seen(seen):
    with open(PATH, "w", encoding="utf-8") as f:
        json.dump(list(seen), f)
