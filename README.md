AI Journal APP
## Team Members
Thomas Garrett, Daniel Wahlquist, Cam Woodward
## Software Description
A web journaling app (“The Daily Entry”): users register and log in, see AI-generated daily prompts, write journal entries, and manage personal goals. The UI is static HTML, CSS, and JavaScript under `front_end/`. The API is a Node.js (Express) server under `backend/` that uses a MySQL database for users, journal entries, goals, and stored prompts. Daily prompts are generated with the OpenAI API and written to the database on a schedule.

## Architecture
Programming Languages	
* JavaScript (Node.js backend and browser)
* HTML, CSS

Frameworks & major libraries	
* **Express** — HTTP API and sessions
* **mysql2** — MySQL connection pool
* **argon2** — password hashing
* **express-session** — cookie-based sessions
* **OpenAI** (Node SDK) — journal prompt generation (`gpt-4o`)
* **node-cron** — nightly job for new prompts
* **dotenv** — configuration from environment variables

Data Storage	
* **MySQL** — tables include `users`, `journals`, `goals`, `prompts` (and related keys as used by the API)

Development Tools	
* Visual Studio Code
* GitHub (required) — project and public repository
* Node.js — run the backend (`backend/server.js` listens on port **3000**)

**Configuration:** Create a `.env` in `backend/` (see `.gitignore`) with database settings (`DB_HOST`, `DB_USER`, `DB_PASSWORD`, `DB_NAME`, `DB_PORT`, `DB_CONNECTIONS`), `SESSION_SECRET`, and `OPENAI_API_KEY`. The server enables CORS for `http://localhost:5500` for local frontend development; the frontend calls routes under `/api/...`—use a dev server/proxy that maps those requests to the Express app if your setup requires it.

## Software Features
* [x] AI-generated journal prompts (OpenAI, stored in MySQL; nightly job; rotate prompts on the journal page)
* [x] MySQL persistence for users, entries, goals, and prompts
* [x] Frontend: login/register, session-aware pages, journal home with prompts and goal list, calendar with month navigation and entry previews/modal, goals page with cards and complete/incomplete status, dark mode (theme in `localStorage`), logout

## Team Communication
Text Message
## Team Responsibility

|Responsibility                      |Team Member(s)              |
|------------------------------------|----------------------------|
|Conducting Meetings                 |Thomas Garrett              |
|Maintaining Team Assignment List    |Thomas Garrett              |
|Ensuring GitHub is Working          |Daniel Wahlquist            |
|Maintaining Documentation           |Cam Woodward                |
|Create & Display Presentations      |Daniel Wahlquist            |
|Submit Team Assignments             |Thomas Garrett              |

## Reflections

# What the team Learned?
* Github Team Management
* How to work together using GitHub and communication techniques
* Github workflows!! I.e. creating branches, using pull requests, rebasing
* Using comments is important so others can easily scan code and understand what it’s meant to do
* Better code organization, and future prepping with modularity
# What can be Improved?
* Evening out the work that each teammate has so everyone can have equal amount of work
* Planning needs to be improved. At times we had little direction even though there was lots to do
* Team responsibility, with one of our team members not really doing much to help the project, that we eventually kicked out, but things may have gone smoother, if we had either, kicked him out earlier, or held him more accountable for his things
# Future Plans for this Project?
* Editing the journal
* History of editing
* Using email for registering users
* Ability to change password
* Integrating cloudflare to ensure users are human
* User profile page
* Add journals for past days



