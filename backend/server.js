//Express is a framework that makes it much easeir to handle apis
import express from 'express';

//This will allow us to easily make request to our database
import { pool } from "./db.js";

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

//This uses the .json method to parse incoming api requests into json format
app.use(express.json());

//API for registering a new user
app.post('/register', async (req, res) => {

});

//API for user login
app.post('/login', async (req, res) => {

});

//API for user logout
app.post('/logout', async (req, res) => {
    
});

//API to get the journal prompt from the LLM
app.post('/journal_prompt', async (req, res) => {

});

//API to get the names of current goals from the db
app.post('/name_current_goals', async (req, res) => {

});

//API to add a goal to the list of goals to the db
app.post('/add_goal', async (req, res) => {

});

//API to save journal entry to db 
app.post('/journal_entry', async (req, res) => {

});

//API to get journal entry dates from the db
app.post('/get_journal_entry_dates', async (req, res) => {

});

//API to get the journal entry from the selected date
app.post('/get_journal_entry', async (req, res) => {

});

//API to get the full list of current goal names and descriptions
app.post('/current_goals', async (req, res) => {

});

//This uses port 3000 to listen for api requests
app.listen(3000, () => {
    console.log('server running on port 3000');
});