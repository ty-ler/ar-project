# Documentation
## Usage
### Teacher App
#### Login
![Login](https://i.imgur.com/Xapr9nh.png)

#### Add a Student
Go to the `Students` tab:

![enter image description here](https://i.imgur.com/USsthCr.png)

Click `New Student` and enter in a students information:

![enter image description here](https://i.imgur.com/CVOB1ww.png)

Click `Confirm`. Now the student has been added to the pool of all students:

![enter image description here](https://i.imgur.com/rNS1itT.png)

#### Add a Class
Go to the `Classes` tab, click the `Add Class` button. Enter a class name:

![enter image description here](https://i.imgur.com/bhHqPcq.png)

Click `Confirm`. A new class named `Algebra 1` has been created under the currently logged in teacher:

![enter image description here](https://i.imgur.com/Nxv833j.png)

#### Add Students to Class
Select a class from the `Classes` page. Click the `Add Student` button. Select a student you wish to be  added to a class:  

![enter image description here](https://i.imgur.com/ACCy3VN.png)

The selected student has now been added to the class:

![enter image description here](https://i.imgur.com/Azlg12u.png)

## Development
Start by cloning the project to your machine with:  `git clone https://github.com/ty-ler/ar-project.git`

### Firebase
Before starting the teacher web app or game, you must first set up a Firebase account and project instance [here](https://firebase.google.com/).

After setting up a Firebase project, go to the `Project Overview` page and click on the `Web` icon to add a web app to your Firebase.  Go through the steps to add a web app until you are directed to add the Firebase SDK.

You should be presented with some html scripts to add to your page. We only want the JSON object from the `firebaseConfig` variable. Copy only the JSON object.

Go to `<project-dir>/teacher-app/src/`. Create a file called `firebase.json`. Open the file and paste in the JSON object you copied from Firebase. Make sure to change the keys in the object to be surrounded by `"`'s. Save the file.

The file should look something like: 
```
{
	"apiKey": API_KEY_HERE,
	"authDomain": AUTH_DOMAIN_HERE,
	...
}
```

Go back to the `Project Overview` page in Firebase and this time add a Unity app. Go through the steps again to register the app. Download both the `google-services.json` and `GoogleService-Info.plist` files and place them in the `<project-dir>/game/Assets` folder.

Go back to Firebase and click on `Database` on the left side of the page. Scroll down to `Realtime Database` section and create a database started in `test mode` (Do **NOT** choose Cloud Firestore). Now that you have created the database for the project,  you can add your first teacher user.

Click `Authentication` on the left side of the page. Click `Set up sign-in method`  and set up an `Email/Password` sign-in provider. Do **NOT** enable `Email link (passwordless sign-in)`.

Click the `Users` tab at the top of the page. Click `Add user`, and enter in the credentials for a teacher. Once you have added a teacher user, you are now ready to set up the teacher app.

### Teacher Web App
When you first clone the project, you must install all the node packages used. To do this, navigate to `<project-dir>/teacher-app` run the following command: `npm install --save`. This will fetch all the necessary dependencies.

Once all of the dependencies are installed, you can start the teacher web app by running `npm start`. When you see the login screen, enter in the credentials you just created in Firebase.

### Mobile AR App
Open Unity and open the project from the folder `<project-dir>/game`. 

After all the assets have been imported, select `File > Build Settings`. Choose which platform you wish to build the game for and click `Switch Platform`. Configure the options for your respective build target. 

Make sure your device is connected to your computer. Click `Build and Run`.  The game will build and then install to your phone.
