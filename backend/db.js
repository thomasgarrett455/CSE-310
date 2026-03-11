//This is the library that will help us to connect to the db
import mysql from 'mysql2/promise';

//This helps us keep our database credentials safe
import dotenv from 'dotenv';

//This allows us to use the .env file to grab the credentials we need for the db
dotenv.config();

//This creates a variable named pool that is exported to our server and used to query the db
export const pool = mysql.createPool({
host: process.env.DB_HOST,
user: process.env.DB_USER,
password: process.env.DB_PASSWORD,
database: process.env.DB_NAME,
port: process.env.DB_PORT,
waitForConnections: true, 
connectionLimit: process.env.DB_CONNECTIONS
});