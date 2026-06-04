# url-shortener

A URL shortener API on FastAPI. In-memory storage, no database needed to run it.

## Run

```bash
pip install fastapi uvicorn
uvicorn main:app --reload
```

## Usage

Shorten a URL:
```bash
curl -X POST http://localhost:8000/shorten \
  -H "Content-Type: application/json" \
  -d "{\"url\": \"https://github.com/Dunvtu74\"}"
```

Response:
```json
{"short": "http://localhost:8000/a3f9c1", "code": "a3f9c1"}
```

Open `http://localhost:8000/a3f9c1` in a browser — it redirects to the original URL.

Custom alias:
```bash
curl -X POST http://localhost:8000/shorten \
  -d "{\"url\": \"https://github.com\", \"alias\": \"gh\"}" \
  -H "Content-Type: application/json"
```

List all links: `GET /api/links`
