from django.shortcuts import render, get_object_or_404
from .models import Note
from django.http import HttpResponse

# Create your views here.
#список заметок
def notes_list(request):
  notes = Note.objects.all()
  html = "<h1>Список заметок</h1>"
  for note in notes:
    html += f"<p><a href='/notes/{note.id}'>{note.title}</a><p>"
    return render(request, 'notes/index.html', {'notes': notes })
  
#детальна страница одной заметки
def note_detail(request, note_id):
  note = get_object_or_404(Note, pk=note_id)
  return render(request, 'notes/detail.html', {'note': note })

#render() — возвращает HTML-шаблон с данными
