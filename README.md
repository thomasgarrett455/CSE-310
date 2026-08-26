# The Daily Entry — AI Journal App

A web journaling app that pairs daily AI-generated reflection prompts with personal goal tracking.

Users register and log in, land on a journal page showing prompts generated fresh each night by the OpenAI API, write and save an entry against the prompt they chose, browse past entries on a month calendar, and create and complete goals. Sessions are cookie-based, passwords are hashed with Argon2, and everything (users, entries, goals, prompts) is persisted in MySQL.

## Team Members
Thomas Garrett, Daniel Wahlquist, Cam Woodward

## Features
* [x] **AI prompts** — a `node-cron` job runs nightly at midnight (America/Denver) and asks `gpt-4o` for a fresh set of reflective prompts across spiritual, mental, emotional, school/work, physical, relationship, and personal-growth categories, then stores them in MySQL
* [x] **Accounts & sessions** — registration, login, logout, and a `/me` endpoint that session-aware pages use to redirect anonymous visitors back to the login screen
* [x] **Journal entries** — write against a rotating prompt and save one entry per day
* [x] **Calendar** — month navigation with previews of days that have entries and a modal showing the full entry and the prompt it answered
* [x] **Goals** — create goals with a name and description, view them as cards, and toggle complete/incomplete
* [x] **Dark mode** — theme preference stored in `localStorage`

## Technologies Used

**Languages**
* JavaScript (ES modules — Node.js on the server, vanilla JS in the browser)
* HTML, CSS

**Backend** (`backend/`)
| Package | Purpose |
|---|---|
| `express` | HTTP API |
| `express-session` | cookie-based sessions |
| `argon2` | password hashing (argon2id) |
| `mysql2` | MySQL connection pool (promise API) |
| `openai` | prompt generation via `gpt-4o` |
| `node-cron` | nightly prompt-generation job |
| `dotenv` | configuration from environment variables |

**Frontend** (`front_end/`)
* Static HTML, hand-written CSS (split into `styles/frames/` and `styles/pages/`), and ES-module scripts under `scripts/`
* Font Awesome (CDN) for icons
* No build step and no frontend framework

**Data storage**
* MySQL — `users`, `journals`, `goals`, `prompts`

**Tools**
* Visual Studio Code (Live Server for the frontend)
* Node.js 18+
* Git / GitHub

## Project Structure
```
backend/
  server.js      Express app, all API routes, session config, cron schedule
  db.js          mysql2 connection pool built from .env
  services.js    getDailyPrompts() — calls OpenAI and inserts prompts
front_end/
  index.html     login / register
  journal.html   prompts, entry editor, goal list
  calendar.html  month view of past entries
  goals.html     goal cards
  about-us.html
  scripts/       auth, login, logout, main, calendar, goals, darkmode
  styles/        main.css + frames/ and pages/ partials
```

## Running Locally

### 1. Prerequisites
* Node.js 18 or newer
* A running MySQL server
* An OpenAI API key

### 2. Create the database
Create a database and the four tables the API queries:

```sql
CREATE DATABASE journal;
USE journal;

CREATE TABLE users (
  users_id      INT AUTO_INCREMENT PRIMARY KEY,
  username      VARCHAR(255) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  created_at    DATETIME
);

CREATE TABLE prompts (
  prompts_id INT AUTO_INCREMENT PRIMARY KEY,
  prompt     TEXT NOT NULL,
  date       DATE NOT NULL
);

CREATE TABLE journals (
  journals_id INT AUTO_INCREMENT PRIMARY KEY,
  content     TEXT,
  date        DATE NOT NULL,
  users_id    INT NOT NULL,
  prompts_id  INT,
  FOREIGN KEY (users_id)   REFERENCES users(users_id),
  FOREIGN KEY (prompts_id) REFERENCES prompts(prompts_id)
);

CREATE TABLE goals (
  goals_id     INT AUTO_INCREMENT PRIMARY KEY,
  name         VARCHAR(255),
  description  TEXT,
  status       TINYINT NOT NULL DEFAULT 0,
  users_id     INT NOT NULL,
  created_at   DATETIME,
  completed_by DATE,
  FOREIGN KEY (users_id) REFERENCES users(users_id)
);
```

### 3. Configure the backend
Create `backend/.env` (it is git-ignored):

```
DB_HOST=localhost
DB_USER=your_mysql_user
DB_PASSWORD=your_mysql_password
DB_NAME=journal
DB_PORT=3306
DB_CONNECTIONS=10
SESSION_SECRET=any_long_random_string
OPENAI_API_KEY=sk-...
```

### 4. Start the API
```bash
cd backend
npm install
node server.js      # listens on http://localhost:3000
```

### 5. Serve the frontend on port 5500
The backend only allows CORS from `http://localhost:5500`, so the frontend must be served from that port. In VS Code, open `front_end/` and start **Live Server** (default port 5500).

**Important:** the browser scripts call relative paths like `/api/login`, while Express registers those routes at the root (`/login`). In production an nginx reverse proxy strips the `/api` prefix; locally you need the same rewrite. With Live Server, add this to `.vscode/settings.json`:

```json
{
  "liveServer.settings.port": 5500,
  "liveServer.settings.proxy": {
    "enable": true,
    "baseUri": "/api",
    "proxyUri": "http://localhost:3000"
  }
}
```

Then open `http://localhost:5500/index.html`, register an account, and log in.

### 6. Seed today's prompts
The cron job only fires at midnight, so a fresh database has no prompts and the journal page will report none for today. Trigger a run manually:

```bash
curl http://localhost:3000/test-daily-job
```

## API Reference
All routes are served at the root of the Express app on port 3000 (the frontend reaches them through the `/api` proxy).

| Method | Route | Purpose |
|---|---|---|
| POST | `/register` | create an account |
| POST | `/login` | start a session |
| GET  | `/me` | current session username |
| POST | `/logout` | destroy the session |
| POST | `/journal_prompts` | today's prompts |
| POST | `/journal_entry` | save an entry |
| POST | `/get_journal_entry_dates` | dates that have entries |
| POST | `/get_journal_entry` | entry + prompt for one date |
| POST | `/name_current_goals` | names of incomplete goals |
| POST | `/goals_full` | all goals with dates and status |
| POST | `/add_goal` | create a goal |
| POST | `/update_goal_status` | mark complete/incomplete |
| GET  | `/test-daily-job` | manually run the prompt generator |

## Deployment Notes
The app is written to sit behind nginx on an EC2 instance, with nginx serving `front_end/` as static files and proxying `/api/*` to the Node process on port 3000. Before deploying, uncomment `app.set('trust proxy', 1)` in [server.js](backend/server.js) and set the session cookie's `secure` flag to `true` so cookies are only sent over HTTPS.
