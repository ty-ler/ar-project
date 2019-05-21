using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginHandler : MonoBehaviour
{
  
    public TMP_InputField EmailField; 
    public TMP_InputField PasswordField;
    public Button SignUpButton;
    public Button LoginButton;
    public TextMeshProUGUI LoginNotification;
    
    public static string studentId;

    private FirebaseManager firebase;

    void Start()
    {
        // Add event listeners to all buttons and fields
        EmailField.onValueChanged.AddListener(ClearWarning);
        PasswordField.onValueChanged.AddListener(ClearWarning);
        LoginButton.onClick.AddListener(Login);
        SignUpButton.onClick.AddListener(SignUp);

        firebase = new FirebaseManager();

        if(firebase.auth.CurrentUser != null)
        {
            // Sign out of the current user if 
            // Firebase preserved the sign in state
            // from previous sign in.
            Debug.Log("Signing out...");
            firebase.auth.SignOut();
        }
    }

    private void ClearWarning(string arg0)
    {
        // Clear login notifcation text
        LoginNotification.text = "";
    }

    async void Login()
    {
        string email = EmailField.text;
        string password = PasswordField.text;

        // Sign in using email and password
        await firebase.SignIn(email, password);

        // Check if sign in succeeded
        if (firebase.auth.CurrentUser != null)
        {
            studentId = firebase.auth.CurrentUser.UserId;

            // Load class picker scene
            SceneManager.LoadScene("class_picker");
        } else
        {
            // Notify failed login
            LoginNotification.text = "Email/Password incorrect. Try again.";
        }
    }

    // Unused method
    async void SignUp()
    {
        string email = EmailField.text;
        string password = PasswordField.text;

        bool result = await firebase.SignUp(email, password);

        Debug.Log(result);

        if(result)
        {
            LoginNotification.text = "Account created successfully!";
        } else
        {
            LoginNotification.text = "An error has occurred. Please try again.";
        }
    }
}



