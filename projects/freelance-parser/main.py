import time
import json
import os
from parser_kwork import fetch_kwork
from parser_fl import fetch_fl
from storage import load_seen, save_seen

INTERVAL = 300  # секунд между обновлениями

RED    = "\033[31m"
GREEN  = "\033[32m"
YELLOW = "\033[33m"
CYAN   = "\033[36m"
BOLD   = "\033[1m"
RESET  = "\033[0m"

def clear():
    os.system("cls" if os.name == "nt" else "clear")

def print_order(order, source):
    color = CYAN if source == "kwork" else YELLOW
    print(f"{color}{BOLD}[{source.upper()}]{RESET} {order['title']}")
    print(f"  {order['price']}")
    print(f"  {order['url']}")
    print()

def run():
    seen = load_seen()
    keywords = load_keywords()

    print(f"{BOLD}=== Freelance Parser ==={RESET}")
    if keywords:
        print(f"фильтр: {', '.join(keywords)}")
    print(f"обновление каждые {INTERVAL // 60} мин\n")

    while True:
        new_count = 0
        orders = []

        try:
            orders += [(o, "kwork") for o in fetch_kwork()]
        except Exception as e:
            print(f"{RED}kwork: {e}{RESET}")

        try:
            orders += [(o, "fl") for o in fetch_fl()]
        except Exception as e:
            print(f"{RED}fl.ru: {e}{RESET}")

        for order, source in orders:
            if order["id"] in seen:
                continue
            if keywords and not any(kw.lower() in order["title"].lower() for kw in keywords):
                continue
            print_order(order, source)
            seen.add(order["id"])
            new_count += 1

        save_seen(seen)

        if new_count == 0:
            print(f"{YELLOW}новых заказов нет{RESET}  [{time.strftime('%H:%M:%S')}]")
        else:
            print(f"{GREEN}новых: {new_count}{RESET}  [{time.strftime('%H:%M:%S')}]")

        print("-" * 40)
        time.sleep(INTERVAL)

def load_keywords():
    path = "keywords.txt"
    if not os.path.exists(path):
        return []
    with open(path, encoding="utf-8") as f:
        return [line.strip() for line in f if line.strip()]

if __name__ == "__main__":
    run()
