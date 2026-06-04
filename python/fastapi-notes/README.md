# fastapi-notes

Simple notes REST API built with FastAPI. Supports creating, reading, updating and deleting notes. No database, just in-memory storage — enough to see how FastAPI routing and Pydantic models work.

## Running

```bash
pip install fastapi uvicorn
uvicorn main:app --reload
```

## Endpoints

- `GET /notes` — list all notes
- `GET /notes/{id}` — get one note
- `POST /notes` — create note
- `PATCH /notes/{id}` — update fields
- `DELETE /notes/{id}` — delete note

## Example

```bash
curl -X POST http://localhost:8000/notes   -H 'Content-Type: application/json'   -d '{"title": "test", "body": "hello world"}'
```
