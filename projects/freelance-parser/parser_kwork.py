import urllib.request
import json

HEADERS = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0 Safari/537.36",
    "Accept": "application/json, text/plain, */*",
    "Referer": "https://kwork.ru/",
}

def fetch_kwork(category_id=None):
    url = "https://kwork.ru/api/wants/v2?page=1&per_page=20"
    if category_id:
        url += f"&category_id={category_id}"

    req = urllib.request.Request(url, headers=HEADERS)
    with urllib.request.urlopen(req, timeout=10) as r:
        data = json.loads(r.read())

    orders = []
    for item in data.get("data", {}).get("wants", []):
        orders.append({
            "id": f"kwork_{item['id']}",
            "title": item.get("name", ""),
            "price": f"{item.get('priceLimit', '?')} руб.",
            "url": f"https://kwork.ru/projects/{item['id']}/view",
        })
    return orders
