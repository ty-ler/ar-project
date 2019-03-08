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
    public Button BackButton;

    void Start() 
    {
        EmailField.onValueChanged.AddListener(ClearWarning);
        PasswordField.onValueChanged.AddListener(ClearWarning);
        LButton.onClick.AddListener(LoginProcess);
        BackButton.onClick.AddListener(LoadMainMenu);
    }

    private void ClearWarning(string arg0)
    {
        LoginNotification.text = " ";
    }

    // Update is called once per frame
    void Update() {

    }
    void LoginProcess() 
    {
        APIHandler api = new APIHandler();
        MainMenuController main = new MainMenuController();
        int status = api.Login(EmailField.text, PasswordField.text);

        if (status == 200) {
            main.LoadMainMenu();

        }
        if (status == 203) {
            LoginNotification.text = "Username and password are incorrect";


        }
        if (status == 204) {
            LoginNotification.text = "The entered username does not exist";
        }
    }
    void LoadMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }


}



