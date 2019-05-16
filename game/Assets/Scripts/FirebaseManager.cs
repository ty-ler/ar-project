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
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ar-project-d52cb.firebaseio.com/");
        auth = FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance;
    }

    public async Task<bool> SignUp(string email, string password)
    {
        bool result = false;

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

    public async Task SignIn(string email, string password)
    {
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

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            return;
        });
    }

    public async Task<JObject> get(string path)
    {
        return await database.GetReference(path).GetValueAsync().ContinueWith(task=> {
            if(task.IsFaulted || task.IsCanceled)
            {
                throw new FirebaseException(-1, "Task is faulted or canceled: " + task.Result.ToString());
            } else if(task.IsCompleted)
            {
                return JObject.FromObject(task.Result.Value);
            }

            return null;
        });
    }
}
