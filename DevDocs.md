
# Developer Documentation

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

## Installation
### Android
After building from Unity, an APK file will be generated, usually in `<project-dir>/game/builds` (this folder does not exist in the repository and must be created). This APK file can be installed to a user's Android device through several means such as ADB.

### iOS
Distributing an iOS app may require an Apple Developer account. XCode can be used to build directly to an iOS device, but this may not be a viable method for many students who will require the app.

## Unity Development
### Installation
Visit https://unity3d.com/get-unity/download and download Unity Hub. Once opened, navigate to Installs and click Add in the top right. Select Unity 2019.03.0f6 or the latest version. Once installed, go to projects and click add. Select the project folder to add your project.

### Unity Layout
Unity operates by switching between "Scene" files. The left hand side is the *Hierarchy* which displays all of the objects in the scene. The center is the *Scene* view where you can click and drag assests into the scene. The right hand side is the *Inspector* which displays details on a selected object from the scene. The bottom left hand side displays the *Assets Folder* which contains all of the projects assests (another word for objects or related files). Lastly the bottom center shows the files selected from the assest folder.

### Using Unity
In order *to move* around in the scene, hold down right-click and move with the WASD keys. Scrolling the mouse wheel while holding down the right-click button will adjust the speed of your movement. 

To *center your view* directly over an object, select the object from the Hierarchy and press F while your cursor is over the scene panel.

Located underneath the File tab is a bar of useful tools used to maniuplate objects in a scene. The Hand Tool allows you to select objects and move the camera. The *Move Tool* allows you to drag a selected object along the X, Y, and Z axes. The *Rotate Tool* allows you to drag a selected object over the 3 axes. The *Scale Tool* allows the user to scale the object in the 3 axes. The *Rect Tool* allows you to scale multiple directions at the same time. The last tool combines the *Move, Rotate, and Scale tools*.

In order to group objects together so be moved/scaled together, right click on the Hierarchy and select Create Empty to create a blank object. Click and drag those objects into that blank object to combine them. When the parent object is moved/scaled, the child objects will move/scale with it.

### Running the project in Unity
Unity plays in scenes so in order to test your changes, you must start off by running the **login scene** then proceed as intended.

### Adding Accessories
Accessories are classified as either Head, Body, or Leg accessories. Both of the AR and NonAR pet prefabs (or models) contain accessory objects in their hierarchy. Double click on the assest in the bottom center to open up the prefab's scene. New accessories need to be manually added to the correct accessory prefab on the model, scaled, and placed. Once the accessory is properly placed on the pet model, tag that new accessory with the appropriate tag. The tags are spearated based off of the classification of the accessory (Head, Body, Leg) and the pricing (1-3 getting more expensive the higher the number). 

After the accessory has been tagged, it needs to be added to the Pet Script in the right hand Inspector. Adjust the size of the array to accomodate the new accessory, click and drag the object from the hierarchy to the inspector. The new accessory should now appear the in store.

For leg accessories, you have to specify the number of different accessories and then the number of each individual accessory for each leg. (Ex. 4 boots for a cow count as 1 leg accessory but all 4 boots need to be added). Keep the accessories for the legs the same name. **AR pets and NonAR pets are separate prefabs therefore have separate lists and need to be added to both models for it to appear.** You can copy the contents of one model to add and scale it to the other for easier placement.


