//Express is a framework that makes it much easeir to handle apis
import express from 'express';

//This will allow us to easily make request to our database
import { pool } from "./db.js";

//This will allow us to hash paswords for greater account security
//We can also check passwords for login by using argon2.verify
import argon2 from 'argon2';

//Creates a variable named app that uses express
const app = express();

//This will allow requests that come from the frontend
//req = request, res = response, next processes request step by step 
app.use((req, res, next) => {
res.header("Access-Control-Allow-Origin", "http://127.0.0.1:5500");
res.header("Allow-Control-Access", "POST");
res.header("Access-Control-Allow-Headers", "Content-Type");

next();
});

//This is for the aws setup, don't unccoment this line until running on an ec2 instance with nginx
//app.set('trust proxy', 1);

//This uses the .json method to parse incoming api requests into json format
app.use(express.json());

//This is for the users cookies. It ensure security for user sessions
app.use(session({
    secret: process.env.SESSION_SECRET, //secret code to ensure session security
    resave: false, //If a session was not modified it will not be saved
    saveUninitialized: false, //cookie wont be created until data is stored
    cookie: {
        httpOnly: true,
        secure: true,
        maxAge: 1000 * 60 * 60
    }

}));

//This function is used to hash passwords for registration
async function secureHash(password) {
    try {
        const hash = await argon2.hash(password, {
            type: argon2.argon2id, 
            memoryCost: 2 ** 16,
            timeCost: 3,
            parallelism: 4
        });
        return hash;
    } catch (err) {
        console.error("Hashing failed:", err);
        throw new Error("Password hashing failed");
    }
}


//API for registering a new user
app.post('/register', async (req, res) => {
    pool.query = ""
});

//API for user login
app.post('/login', async (req, res) => {
    try {
        const { username, password } = req.body 
        if (!username || !password) {
            return res.status(400).json({error: "Username and password are required"});
        }

        const [rows] = await pool.query('SELECT * FROM users WHERE username = ?', [username]);
        if (!Array.isArray(rows) || rows.length ===0) {
            return res.status(401).json({ message: 'Invalid Credentials' });
        }
        const user = rows[0];
        const isPasswordValid = await argon2.verify(user.password, password);
        if (!isPasswordValid) {
            return res.status(401).json({ message: 'Invalid Credentials'});
        }
        res.json({ message: 'Login Successful'});
    } catch (error) {
        console.error("Login error: ", error);
        res.status(500).json({ error: "Internal server error"});
    }
});

//API for user logout
app.post('/logout', async (req, res) => {

});

//API to get the journal prompt from the LLM
app.post('/journal_prompt', async (req, res) => {

});

//API to get the names of current goals from the db
app.post('/name_current_goals', async (req, res) => {
    try{
        const { username } = req.body;
        if (!username) {
            return res.status(400).json({error: "Username not found"});
        }

        const [rows] = await pool.query(
            `SELECT goals.name
             FROM goals
             JOIN users
             ON goals.users_id = users.users_id
             WHERE users.username = ?`,
            [username],
        );
        if (!Array.isArray(rows) || rows.length === 0 ) {
            return res.status(401).json({ message: 'Invalid credentials'});
        }
        return res.status(200).json({ goals: rows });
    } catch (error) {
        console.error("Error fetching goal names", error)
        res.status(500).json({ error: "could not fetch goal names"})
    }
});

//API to add a goal to the list of goals to the db
app.post('/add_goal', async (req, res) => {

});

//API to save journal entry to db 
app.post('/journal_entry', async (req, res) => {
    try{
        const { username, content, prompt } = req.body;
        if (!username) {
            return res.status(400).json({error: "Username not found"});
        }

        const [rows] = await pool.query(
            `INSERT INTO journals (content, date, users_id, prompts_id)
            VALUES (
            ?,
            CURDATE(),
            (SELECT users_id FROM users WHERE username = ?),
            (SELECT prompt_id FROM prompts WHERE prompt = ?)
            ) `,
            [content, username, prompt],
        );
        if (!Array.isArray(rows) || rows.length === 0 ) {
            return res.status(401).json({ message: 'Invalid credentials'});
        }
        return res.status(200).json({ goals: rows });
    } catch (error) {
        console.error("Error saving journal entry", error)
        res.status(500).json({ error: "could not save journal entry"})
    }
});

//API to get journal entry dates from the db
app.post('/get_journal_entry_dates', async (req, res) => {
    try{
        const { username } = req.body;
        if (!username) {
            return res.status(400).json({error: "Username not found"});
        }

        const [rows] = await pool.query(
            `SELECT journals.date
             FROM journals
             JOIN users
             ON journals.users_id = users.users_id
             WHERE users.username = ?`,
            [username],
        );
        if (!Array.isArray(rows) || rows.length === 0 ) {
            return res.status(401).json({ message: 'Invalid credentials'});
        }
        return res.status(200).json({ goals: rows });
    } catch (error) {
        console.error("Error fetching goal names", error)
        res.status(500).json({ error: "could not fetch goal names"})
    }
});

//API to get the journal entry from the selected date
app.post('/get_journal_entry', async (req, res) => {
    try{
        const { username, date } = req.body;
        if (!username || !date) {
            return res.status(400).json({error: "Username or date not found"});
        }

        const [rows] = await pool.query(
            `SELECT journals.content
             FROM journals
             JOIN users
             ON journals.users_id = users.users_id
             WHERE users.username = ?
             AND date = ?`,
            [username, date],
        );
        if (!Array.isArray(rows) || rows.length === 0 ) {
            return res.status(401).json({ message: 'Invalid credentials'});
        }
        return res.status(200).json({ goals: rows });
    } catch (error) {
        console.error("Error fetching journal entries", error)
        res.status(500).json({ error: "could not fetch journal entries"})
    }
});

//API to get the full list of current goal names and descriptions
app.post('/current_goals', async (req, res) => {
    try{
        const { username } = req.body;
        if (!username) {
            return res.status(400).json({error: "Username not found"});
        }

        const [rows] = await pool.query(
            `SELECT goals.name, goals.description
             FROM goals
             JOIN users
             ON goals.users_id = users.users_id
             WHERE users.username = ?`,
            [username],
        );
        if (!Array.isArray(rows) || rows.length === 0 ) {
            return res.status(401).json({ message: 'Invalid credentials'});
        }
        return res.status(200).json({ goals: rows });
    } catch (error) {
        console.error("Error fetching goal names", error)
        res.status(500).json({ error: "could not fetch goal names"})
    }
});

//This uses port 3000 to listen for api requests
app.listen(3000, () => {
    console.log('server running on port 3000');
});