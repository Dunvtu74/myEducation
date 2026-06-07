import urllib.request
import re

HEADERS = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0 Safari/537.36",
    "Accept-Language": "ru-RU,ru;q=0.9",
}

def fetch_fl():
    url = "https://www.fl.ru/projects/?category=&kind=1"
    req = urllib.request.Request(url, headers=HEADERS)
    with urllib.request.urlopen(req, timeout=10) as r:
        html = r.read().decode("utf-8", errors="ignore")

    orders = []

    # ищем блоки проектов
    blocks = re.findall(
        r'data-id="(\d+)".*?<a[^>]+href="(/projects/\d+/[^"]+)"[^>]*>([^<]+)</a>.*?'
        r'(?:Бюджет|Budget)[^\d]*(\d[\d\s]*)',
        html, re.DOTALL
    )

    for pid, path, title, price in blocks[:20]:
        orders.append({
            "id": f"fl_{pid}",
            "title": title.strip(),
            "price": f"{price.strip()} руб.",
            "url": f"https://www.fl.ru{path}",
        })

    # если регулярка не поймала — fallback на заголовки
    if not orders:
        titles = re.findall(r'class="b-post__title[^"]*"[^>]*>\s*<a[^>]+href="(/projects/\d+/[^"]+)"[^>]*>([^<]+)</a>', html)
        ids = re.findall(r'/projects/(\d+)/', html)
        for i, (path, title) in enumerate(titles[:20]):
            pid = ids[i] if i < len(ids) else str(i)
            orders.append({
                "id": f"fl_{pid}",
                "title": title.strip(),
                "price": "не указан",
                "url": f"https://www.fl.ru{path}",
            })

    return orders
