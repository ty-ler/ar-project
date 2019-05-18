using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using Firebase.Auth;

public class FirebaseManager
{
    public FirebaseAuth auth;
    public FirebaseDatabase database;    

    public FirebaseManager()
    {
        // Initialize database and authentication
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ar-project-d52cb.firebaseio.com/");
        auth = FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance;
    }
    
    // This method is not currently in use
    public async Task<bool> SignUp(string email, string password)
    {
        bool result = false;

        // Await the async CreateUser call
        await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return false;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return false;
            }

            // Firebase user has been created.
            FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            result = true;
            return true;
        });

        return result;
    }

    // Sign in using Firebase Authentication
    public async Task SignIn(string email, string password)
    {
        // Await the async SignIn 
        await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            
            FirebaseUser user = task.Result; // Get the logged in user
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                user.DisplayName, user.UserId);

            return;
        });
    }

    // Get data from Firebase path
    public async Task<JObject> get(string path)
    {

        // Await async get call
        return await database.GetReference(path).GetValueAsync().ContinueWith(task=> {
            if(task.IsFaulted || task.IsCanceled)
            {
                throw new FirebaseException(-1, "Task is faulted or canceled: " + task.Result.ToString());
            } else if(task.IsCompleted)
            {
                // Return the data as a JSON object
                return JObject.FromObject(task.Result.Value);
            }

            return null;
        });
    }

    // Set data at a Firebase path
    public void set(string path, string json)
    {
        // Set data using a json string. Use ToString() method on JObjects
        // to get a raw json string.
        database.GetReference(path).SetRawJsonValueAsync(json);
    }
}
