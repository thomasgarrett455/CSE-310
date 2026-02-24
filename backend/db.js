//This is the library that will help us to connect to the db
import mysql from 'mysql2/promise';

//This helps us keep our database credentials safe
import dotenv from 'dotenv';

dotenv.config();

export const pool = mysql.createPool({
host: proccess.env.DB_HOST,
user: process.env.DB_USER,
password: process.env.DB_PASSWORD,
database: process.env.DB_NAME,
port: process.env.DB_PORT,
waitForConnections: true, 
connectionLimit: process.env.DB_CONNECTIONS
});