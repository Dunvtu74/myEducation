import csv
import sys
from collections import defaultdict

def analyze(filepath):
    rows = []
    with open(filepath, newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            rows.append(row)

    if not rows:
        print("файл пустой")
        return

    print(f"строк: {len(rows)}")
    print(f"колонки: {list(rows[0].keys())}")

    # пробуем найти числовые колонки и посчитать среднее
    for col in rows[0]:
        try:
            vals = [float(r[col]) for r in rows if r[col].strip()]
            avg = sum(vals) / len(vals)
            print(f"{col}: min={min(vals):.2f}  max={max(vals):.2f}  avg={avg:.2f}")
        except ValueError:
            pass

def generate_sample():
    with open("sample.csv", "w", newline="", encoding="utf-8") as f:
        w = csv.writer(f)
        w.writerow(["name", "age", "score"])
        w.writerows([
            ["Alice", 22, 88],
            ["Bob", 25, 74],
            ["Carol", 23, 95],
            ["Dan", 21, 61],
            ["Eve", 24, 82],
        ])
    print("создан sample.csv")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        analyze(sys.argv[1])
    else:
        generate_sample()
        analyze("sample.csv")