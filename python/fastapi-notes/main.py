from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import Optional
import uuid

app = FastAPI()

notes = {}

class Note(BaseModel):
    title: str
    body: str
    tag: Optional[str] = None

class NoteUpdate(BaseModel):
    title: Optional[str] = None
    body: Optional[str] = None
    tag: Optional[str] = None

@app.get("/notes")
def get_notes():
    return list(notes.values())

@app.get("/notes/{note_id}")
def get_note(note_id: str):
    if note_id not in notes:
        raise HTTPException(status_code=404, detail="not found")
    return notes[note_id]

@app.post("/notes", status_code=201)
def create_note(note: Note):
    note_id = str(uuid.uuid4())
    notes[note_id] = {"id": note_id, **note.dict()}
    return notes[note_id]

@app.patch("/notes/{note_id}")
def update_note(note_id: str, data: NoteUpdate):
    if note_id not in notes:
        raise HTTPException(status_code=404, detail="not found")
    for field, value in data.dict(exclude_none=True).items():
        notes[note_id][field] = value
    return notes[note_id]

@app.delete("/notes/{note_id}", status_code=204)
def delete_note(note_id: str):
    if note_id not in notes:
        raise HTTPException(status_code=404, detail="not found")
    del notes[note_id]

