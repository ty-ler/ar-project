const express = require("express");
const auth = require("./auth.json");
const mysql = require("mysql");
const app = express();


const db_name = "vrapp";
const db_hostname = "ateamdb.cpvc4dn7yohj.us-east-1.rds.amazonaws.com";
const db_username = "acatal2";
const db_password = "Towson123";
const access_token = auth.access_token;
const PORT = 1337;

const connection = mysql.createConnection({
  host: db_hostname,
  user: db_username,
  password: db_password,
  database: db_name
});

app.listen(PORT, () => {
  console.log(`Now listening on port ${PORT}`);
});

app.get("/api/:table_name", (req, res) => {
  const req_access_token = req.query.access_token;
  
  if(req_access_token == access_token) {
    connection.query(`SELECT * FROM vrapp.${req.params.table_name}`, (err, rows, fields) => {
      if(err) { 
        console.log(err);
      }

      res.json(rows);
    });
  } else {
    res.sendStatus(401);
  }
});
