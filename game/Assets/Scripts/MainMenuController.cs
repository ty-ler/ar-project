using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    void LoadLoginScene()
    {
        SceneManager.LoadScene("login");
    }

    void LoadRegisterScene()
    {
        SceneManager.LoadScene("register");
    }
}
