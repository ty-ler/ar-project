const express = require("express");
const auth = require("./auth.json");
const mysql = require("mysql");
const app = express();
const bodyParser = require("body-parser");
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());


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
        res.sendStatus(200); //success
        
      }
      else{
        res.sendStatus(203);
         //username and password does not match
      }
    }
    else{
      res.sendStatus(204);  //username doesnt exist     
    }
  }
  });
}
else{
  res.sendStatus(401);
}
});
app.post("/api/student_accounts", (req,res)=>{
  const req_access_token = req.body.access_token;
  const studentID= req.body.StudentID;
  const fullname = req.body.Fullname;
  const username = req.body.Username;
  const password = req.body.Password;
  const email = req.body.Email;
  const teacherID = req.body.TeacherID;

  console.log(req.body);
  
  
  if(req_access_token == access_token){
    var values = {
      "StudentID":studentID,
      "Name":fullname,
      "Username":username,
      "Password":password,
      "Email":email,
      "TeacherID":teacherID
    };
    console.log(values);
  
    connection.query('INSERT INTO student_accounts SET ?',values,function(error,results){
      
      if(error){
        console.log(error);
        res.send({
          "code":400,
          "failed":"failed to insert"
        })
      }else{
        res.send({
          "code":200,
          "success":"user registered sucessfully"
        });
      }
    });
  }

  app.post("/api/add_questions", (req,res)=>{
    const req_access_token = req.body.access_token;
    const problemID= req.body.ProblemID;
    const chapter = req.body.Chapter;
    const section = req.body.Section;
    const problem = req.body.Problem;
    const solution = req.body.Solution;
    const classID = req.body.ClassID;
    
    console.log(req.body);
    
    
    if(req_access_token == access_token){
      var values = {
        //"ProblemID":problemID, should auto increment
        //"ClassID":classID, should be a foreign key, might get rid of it anyways
        "Chapter":chapter,
        "Section":section,
        "Problem":problem,
        "Solution":solution
      };
      console.log(values);
    
      connection.query('INSERT INTO problems SET ?',values,function(error,results){
        
        if(error){
          console.log(error);
          res.send({
            "code":400,
            "failed":"failed to insert"
          })
        }else{
          res.send({
            "code":200,
            "success":"Problems added successfully."
          });
        }
      });
    }
});
  
})