using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginHandler : MonoBehaviour
{
  
    public InputField EmailField; 
    public InputField PasswordField;
    public Button LButton;
    public Text LoginNotification;
    public Button backButton;

    void Start()
    {
        EmailField.onValueChanged.AddListener(ClearWarning);
        PasswordField.onValueChanged.AddListener(ClearWarning);
        LButton.onClick.AddListener(LoginProcess);
        backButton.onClick.AddListener(LoadMainMenu);
    }

    private void ClearWarning(string arg0)
    {
        LoginNotification.text = " ";
    }
    void LoadMainMenu() {
        SceneManager.LoadScene("main_menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LoginProcess()
    {
        APIHandler api = new APIHandler();
        MainMenuController main = new MainMenuController();
        int status = api.Login(EmailField.text, PasswordField.text);

        if (status == 200)
        {
            SceneManager.LoadScene("pet");
        }
        if (status == 203)
        {
            LoginNotification.text = "Username and password are incorrect";


        }
        if (status == 204)
        {
            LoginNotification.text = "The entered username does not exist";
        }

    }
  
}



