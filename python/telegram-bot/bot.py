import asyncio
import logging
from aiogram import Bot, Dispatcher, types, F
from aiogram.filters import CommandStart, Command
from aiogram.types import Message

logging.basicConfig(level=logging.INFO)

BOT_TOKEN = "YOUR_TOKEN_HERE"

bot = Bot(token=BOT_TOKEN)
dp = Dispatcher()

user_notes = {}

@dp.message(CommandStart())
async def start(message: Message):
    await message.answer(
        f"Hi {message.from_user.first_name}.\n\n"
        "/note <text> — save a note\n"
        "/notes — show all notes\n"
        "/clear — delete all notes"
    )

@dp.message(Command("note"))
async def save_note(message: Message):
    text = message.text.removeprefix("/note").strip()
    if not text:
        await message.answer("Write something after /note")
        return
    uid = message.from_user.id
    user_notes.setdefault(uid, []).append(text)
    await message.answer("Saved.")

@dp.message(Command("notes"))
async def show_notes(message: Message):
    uid = message.from_user.id
    notes = user_notes.get(uid, [])
    if not notes:
        await message.answer("No notes yet.")
        return
    text = "\n".join(f"{i+1}. {n}" for i, n in enumerate(notes))
    await message.answer(text)

@dp.message(Command("clear"))
async def clear_notes(message: Message):
    user_notes.pop(message.from_user.id, None)
    await message.answer("Cleared.")

@dp.message(F.text)
async def echo(message: Message):
    await message.answer(message.text)

async def main():
    await dp.start_polling(bot)

if __name__ == "__main__":
    asyncio.run(main())
