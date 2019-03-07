using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginHandler : MonoBehaviour
{
  
    public InputField EmailField; 
    public InputField PasswordField;
    public Button LButton;
    public Text LoginNotification;

    void Start()
    {
       LButton.onClick.AddListener(LoginProcess);
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
            main.LoadMainMenu();
            return;

        }
        if (status == 204)
        {
            LoginNotification.text = "Username and password does not match";
        }

    }


}



