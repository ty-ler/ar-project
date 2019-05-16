using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class ClassPickerController : MonoBehaviour
{
    public TextMeshProUGUI StudentNameText;
    public GameObject ListContainer;
    public GameObject ClassButton;

    private FirebaseManager firebase;
    private string studentName;
    public static JObject studentData;

    // Start is called before the first frame update
    async void Start()
    {
        firebase = new FirebaseManager();
        await getStudentData();
        setStudentName();
        setupStudentClasses();
    }

    async Task getStudentData()
    {
        string studentId = firebase.auth.CurrentUser.UserId;

        studentData = await firebase.get("students/" + studentId);

    }

    void setStudentName()
    {
        studentName = studentData["firstName"] + " " + studentData["lastName"];
        StudentNameText.text = studentName;
    }

    void setupStudentClasses()
    {
        if(studentData["classes"] != null)
        {
            Vector3 buttonPosition = new Vector3(0, 50, 0);
            int classButtonY = 400;
            foreach (KeyValuePair <string, JToken> item in (JObject)studentData["classes"])
            {
                GameObject classButton = Instantiate(ClassButton, ListContainer.transform);
                classButton.transform.SetParent(ListContainer.transform);
                classButton.transform.position += new Vector3(0, classButtonY, 0);
                classButton.GetComponentInChildren<TextMeshProUGUI>().text = item.Value["name"].ToString();
                classButton.GetComponent<Button>().onClick.AddListener(delegate { loadPetController(item.Value["id"].ToString(), item.Value["teacherId"].ToString()); });
                classButtonY -= 110;
            }
        } else
        {

        }
    }

    void loadPetController(string classId, string teacherId)
    {
        PetController.classId = classId;
        PetController.teacherId = teacherId;
        SceneManager.LoadScene("pet");
    }
}
