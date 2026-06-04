import urllib.request
import json
import sys

def get_weather(city):
    url = f"https://wttr.in/{city}?format=j1"
    try:
        with urllib.request.urlopen(url, timeout=5) as r:
            data = json.loads(r.read())
    except Exception as e:
        print(f"error: {e}")
        return

    current = data["current_condition"][0]
    desc = current["weatherDesc"][0]["value"]
    temp_c = current["temp_C"]
    feels = current["FeelsLikeC"]
    humidity = current["humidity"]
    wind = current["windspeedKmph"]

    print(f"{city}")
    print(f"{desc}, {temp_c}°C (feels like {feels}°C)")
    print(f"humidity: {humidity}%  wind: {wind} km/h")

if __name__ == "__main__":
    city = " ".join(sys.argv[1:]) if len(sys.argv) > 1 else "Moscow"
    get_weather(city)
