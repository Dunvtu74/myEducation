# telegram-bot

A simple Telegram bot built with aiogram 3. Lets users save short notes per session.

## Setup

```bash
pip install aiogram
```

Put your token in `bot.py` where it says `YOUR_TOKEN_HERE`, then:

```bash
python bot.py
```

## Commands

- `/start` — show help
- `/note <text>` — save a note
- `/notes` — list saved notes
- `/clear` — wipe notes

Notes are stored in memory and reset on restart.
