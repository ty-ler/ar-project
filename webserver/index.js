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

app.get("/api/problems", (req, res) => {
  const req_access_token = req.query.access_token;
  
  if(req_access_token == access_token) {
    connection.query(`SELECT * FROM vrapp.problems`, (err, rows, fields) => {
      if(err) { 
        console.log(err);
      }

      res.json(rows);
    });
  } else {
    res.sendStatus(401);
  }
});
app.get("/api/student_accounts", (req, res) => {
  const req_access_token = req.query.access_token;
  const username = req.query.user;
  const password = req.query.pass;
  console.log(username);


  
  if(req_access_token == access_token) {
connection.query('SELECT * FROM student_accounts WHERE Username = ?',[username], function (error, results, fields) {
  if (error) {
    // console.log("error ocurred",error);
    res.send({
      "code":400,
      "failed":"error ocurred"
    })
  }else{
    // console.log('The solution is: ', results);
    if(results.length >0){
      if(results[0].Password == password){
        res.send({
          "code":200,
          "success":"login sucessful"
            });
      }
      else{
        res.send({
          "code":204,
          "success":"Username and password does not match"
            });
      }
    }
    else{
      res.send({
        "code":204,
        "success":"username does not exits"
          });
    }
  }
  });

  
}
else{
  res.sendStatus(401);
}
});
  

