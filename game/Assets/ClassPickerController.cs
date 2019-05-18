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

    async void Start()
    {
        firebase = new FirebaseManager();

        // Get the current students data
        await getStudentData();

        // Initialize scene
        setStudentName();
        setupStudentClasses();
    }

    async Task getStudentData()
    {
        string studentId = firebase.auth.CurrentUser.UserId;

        // Get student data from Firebase database
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
            // Initial Y position
            int classButtonY = 400;

            // Loop through each class in student data
            foreach (KeyValuePair <string, JToken> item in (JObject)studentData["classes"])
            {
                // Generate button
                GameObject classButton = Instantiate(ClassButton, ListContainer.transform);
                classButton.transform.SetParent(ListContainer.transform);
                classButton.transform.position += new Vector3(0, classButtonY, 0);

                // Set class buttons text to class name
                classButton.GetComponentInChildren<TextMeshProUGUI>().text = item.Value["name"].ToString();

                // Add click listenter to button
                // Had to use delegate here to add 
                // parameters to the method call
                classButton.GetComponent<Button>().onClick.AddListener(delegate { loadPetController(item.Value["id"].ToString(), item.Value["teacherId"].ToString()); });

                // Set new Y position for the next button
                classButtonY -= 110;
            }
        }
    }

    void loadPetController(string classId, string teacherId)
    {
        // Setup Id's in the PetController object
        PetController.classId = classId;
        PetController.teacherId = teacherId;

        // Load the scene
        SceneManager.LoadScene("pet");
    }
}
