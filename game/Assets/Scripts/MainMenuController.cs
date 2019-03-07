using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MainMenuController : MonoBehaviour
{

    public Button LoginButton, RegisterButton;

    // Start is called before the first frame update
    void Start()
    {
        LoginButton.onClick.AddListener(LoadLoginScene);
        RegisterButton.onClick.AddListener(LoadRegisterScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLoginScene()
    {
        SceneManager.LoadScene("login");
    }

    public void LoadRegisterScene()
    {
        SceneManager.LoadScene("sign_up");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }
}
