using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Mail;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SignupHandler : MonoBehaviour
{
    public InputField studentID;
    public InputField Fullname;
    public InputField teacherID;
    public InputField Email;
    public InputField Username;
    public InputField Password;
    public Button SignUpButton;
    public Text WarningText;
    public Button BackButton;


    // Start is called before the first frame update
    void Start()
    {
        Email.onEndEdit.AddListener(IsValidEmail);
        BackButton.onClick.AddListener(LoadMainMenu);
        SignUpButton.onClick.AddListener(Signup);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void IsValidEmail(string email)
    {
        try 
        {
            MailAddress m = new MailAddress(email);
            WarningText.text = " ";
        }
        catch 
        {
            WarningText.text = "Invalid";
            WarningText.color = Color.red;
        }
    }
    void LoadMainMenu() 
    {
        SceneManager.LoadScene("main_menu");
    }
    async void Signup() {
        APIHandler api = new APIHandler();
        int status = await api.SignUpAsync(studentID.text,Fullname.text,Username.text,Password.text,Email.text,teacherID.text);

        if (status == 200) {
            SceneManager.LoadScene("main_menu");
        }
        if (status == 400) {
            WarningText.text = "failed to signup";
        }
    }
}
