import sys
import re
from collections import Counter

def count_words(text):
    words = re.findall(r"[a-zA-Zа-яА-ЯёЁ]+", text.lower())
    return Counter(words)

if __name__ == "__main__":
    if len(sys.argv) > 1:
        with open(sys.argv[1], encoding="utf-8") as f:
            text = f.read()
    else:
        text = sys.stdin.read()

    counts = count_words(text)
    for word, n in counts.most_common(20):
        print(f"{n:>5}  {word}")
