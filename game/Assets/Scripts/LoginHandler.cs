using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginHandler : MonoBehaviour
{
  
    public InputField EmailField; 
    public InputField PasswordField;
    public Button SignUpButton;
    public Button LoginButton;
    public Text LoginNotification;
    public Button backButton;

    private FirebaseManager firebase;

    void Start()
    {
        EmailField.onValueChanged.AddListener(ClearWarning);
        PasswordField.onValueChanged.AddListener(ClearWarning);
        LoginButton.onClick.AddListener(Login);
        SignUpButton.onClick.AddListener(SignUp);
        backButton.onClick.AddListener(LoadMainMenu);
        firebase = new FirebaseManager();
    }

    private void ClearWarning(string arg0)
    {
        LoginNotification.text = " ";
    }
    void LoadMainMenu() {
        SceneManager.LoadScene("main_menu");
    }

    async void Login()
    {
        string email = EmailField.text;
        string password = PasswordField.text;

        await firebase.SignIn(email, password);

        if (firebase.auth.CurrentUser != null)
        {
            SceneManager.LoadScene("pet");
        } else
        {
            LoginNotification.text = "Email/Password incorrect. Try again.";
        }
    }

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



