const fs = require("fs");
const hat = require("hat");

var auth = {
  access_token: hat()
};

fs.writeFileSync("./auth.json", JSON.stringify(auth));
fs.writeFileSync("../game/Assets/Resources/auth.json", JSON.stringify(auth));