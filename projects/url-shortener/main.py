from fastapi import FastAPI, HTTPException
from fastapi.responses import RedirectResponse
from pydantic import BaseModel, HttpUrl
import hashlib
import time

app = FastAPI()

db = {}

class URLRequest(BaseModel):
    url: HttpUrl
    alias: str = None

def make_code(url: str) -> str:
    raw = url + str(time.time())
    return hashlib.md5(raw.encode()).hexdigest()[:6]

@app.post("/shorten")
def shorten(req: URLRequest):
    code = req.alias or make_code(str(req.url))
    if code in db:
        if db[code] != str(req.url):
            raise HTTPException(status_code=409, detail="alias already taken")
    db[code] = str(req.url)
    return {"short": f"http://localhost:8000/{code}", "code": code}

@app.get("/{code}")
def redirect(code: str):
    if code not in db:
        raise HTTPException(status_code=404, detail="not found")
    return RedirectResponse(url=db[code])

@app.get("/api/links")
def list_links():
    return [{"code": k, "url": v} for k, v in db.items()]
