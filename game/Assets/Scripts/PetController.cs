using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Linq;

public class PetController : MonoBehaviour
{
    public GameObject pet;
    public GameObject placementIndicator;
    public GameObject petPlane;
    public GameObject[] foods;
    public RawImage QuestionImage;
    public GameObject QuestionPanel;
    public Button QuestionButton;
    public TextMeshProUGUI timerText;
    public Image HealthBar;
    public Text HealthBarPercentage;
    public TextMeshProUGUI totalQuestions;
    public int correctSolutionIndex;
    public bool timerGoing;
    public string currentTimerText;
    public bool wonGame;
    private float health;
    private float startHealth = 1f;
    private float finishTime;
    public float correctAnswerCount = 0;
    public float totalQuestionCount = 0;

    private QAHandler qaHandler;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool validPlacementPose;
    private bool placed;
    Vector3 petPlanePos;
    private float startTime;
    public static string classId;
    public static string teacherId;
    private FirebaseManager firebase;
    private JObject classData;
    private string[] solutions;

    public JArray questions = new JArray();
    public int currentProblem = -1;
    public bool lastProblem = false;

    void Start()
    {
        // Get firebase manager instance
        firebase = new FirebaseManager();

        // Get necessary GameObjects and Scripts
        foods = GameObject.FindGameObjectsWithTag("Food");
        qaHandler = FindObjectOfType<QAHandler>();
        arOrigin = FindObjectOfType<ARSessionOrigin>();

        // Initialization
        placed = false;
        validPlacementPose = false;
        wonGame = false;

        // TODO: Remove healthbar code
        HealthBar.fillAmount = startHealth;
        health = startHealth;

        // Hide all 3d objects in scene
        ShowFood(false);
        pet.SetActive(false);
        petPlane.SetActive(false);

        // Show Scanning text
        timerText.SetText("Scanning...");

        // Hide View Question Button
        QuestionButton.gameObject.SetActive(false);

        // Load Class Data
        loadClassData();
    }

    // Update is ran once per frame
    void Update()
    {
        // Update the location of the placement indicator
        UpdatePlacementPose();

        // Handle whether to hide or show the placement indicator
        UpdatePlacementIndicator();

        if (validPlacementPose && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Check if touching
            if (!placed && !wonGame)
            {
                // If the pet is not placed and you have not won the game,
                // place the pet and food down and start the timer
                QuestionButton.gameObject.SetActive(true);
                PlacePet();

                int index = 0;
                foreach (string solution in solutions)
                {
                    // Only show the amount of food necessary for the
                    // current question
                    foods[index].SetActive(true);
                    index++;
                }

                // Calculate the positions of the food on the plane
                PlaceFood();

                // Set start time to the current time
                startTime = Time.time;
                timerGoing = true;
                timerText.SetText("");
            }
            else if (placed && !wonGame)
            {
                // If the cat is placed, get the move script from the
                // pet and move the pet to the new position
                pet.GetComponent<CatMoveTo>().StartMove(placementPose.position);
            }
        }

        if (placed && timerGoing)
        {
            // Get difference of current time and the start time 
            // for the current timer time
            float t = Time.time - startTime;
            finishTime = t;
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f0");

            currentTimerText = minutes + ":" + seconds.PadLeft(2, '0');

            // Update the timer text
            timerText.SetText(currentTimerText);
        }
    }

    void UpdatePlacementIndicator()
    {
        // Move placement indicator up .001 so that it will appear above the plane
        Vector3 placeIndPos = new Vector3(placementPose.position.x, placementPose.position.y + .001f, placementPose.position.z);
        
        if(!wonGame)
        {
            if (!placed)
            {
                // If the pet is not placed yet, constantly check
                // if the current placement pose is valid
                if (validPlacementPose)
                {
                    // If it is, show the indicator and update its position 
                    // with the new position
                    placementIndicator.SetActive(true);
                    placementIndicator.transform.SetPositionAndRotation(placeIndPos, placementPose.rotation);
                }
                else
                {
                    // Hide the indicator
                    placementIndicator.SetActive(false);
                }
            }
            else
            {
                if (validPlacementPose)
                {
                    placementIndicator.SetActive(true);
                    placementIndicator.transform.SetPositionAndRotation(placeIndPos, placementPose.rotation);
                }
                else
                {
                    placementIndicator.SetActive(false);
                }
            }
        }
    }

    void UpdatePlacementPose()
    {
        // Get the center of the screen
        Vector3 screencenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        // Get the forward of the camera ?
        var cameraForward = Camera.current.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

        if (!placed)
        {
            // If the pet has not been placed, we must search
            // for a real-life plane to put the pet on

            // Create an empty list of possible raycast hits
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            // Raycast from the center of the screen out.
            // If the raycast hits a game object and finds
            // a suitable plane, it will register it in the 
            // hits List we just created.
            arOrigin.Raycast(screencenter, hits, TrackableType.Planes);

            // Update whether the placement pose is valid
            // depending on if the raycast found any hits
            validPlacementPose = hits.Count > 0;

            if (validPlacementPose)
            {
                // If it is valid, show option to place pet
                timerText.SetText("Tap to Place Pet");
                placementPose = hits[0].pose;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            } else
            {
                // Else, reset to scanning
                timerText.SetText("Scanning...");
            }
        }
        else
        {
            // If the pet has been placed down, we raycast to the "grass"
            // plane to determine where to move the pet on the grass

            RaycastHit hit;
            
            // Raycast from the middle of the screen, supplying the RaycastHit variable
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                if(hit.collider.gameObject.name == "Pet Plane")
                {
                    // If the raycast object hit is the pet plane,
                    // set the valid placement pose to be true
                    validPlacementPose = true;

                    // Update the placement position and rotation with the 
                    // hit's point
                    placementPose.position = hit.point;
                    placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                }
            }
            else
            {
                // The current position is not valid
                validPlacementPose = false;
            }
        }
    }

    async void loadClassData()
    {
        // Load a teachers class data 
        classData = await firebase.get("teachers/" + teacherId + "/classes/" + classId);

        // Initialize questions
        setupQuestions();

        // Start the game by loading the first question
        nextQuestion();
    }

    void setupQuestions()
    {
        // Loop through each question in the class questions
        foreach (KeyValuePair<string, JToken> item in (JObject)classData["questions"])
        {
            JObject question = (JObject)item.Value;

            // Set the question to incorrect by default
            question["correct"] = false;
            
            // Add question into a JArray
            questions.Add(question);
        }

        // Update questions JArray sorted by question order
        questions = new JArray(questions.OrderBy(obj => (int)obj["order"]));

        // Set total question count
        totalQuestionCount = questions.Count;
    }

    // Used to download and display an image
    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);

        // Make request
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            // If the request completes, set the image to the question image
            QuestionImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    void PlacePet()
    {
        placed = true;

        // This is the location where the grass plane will spawn.
        // We set it to spawn y-1 and z+.5 (down and forward) because otherwise the 
        // plane it detected does not appear to rest on the 
        // actual plane in "real life"
        Vector3 placePosePos = new Vector3(placementPose.position.x, placementPose.position.y - 1f, placementPose.position.z + .5f);

        // Add the current forward in an attempt to combat the same issue
        // discussed above.
        petPlanePos = placePosePos + placementPose.forward;

        placementPose.position = petPlanePos;

        // Place the plane down in the scene
        petPlane = Instantiate(petPlane, petPlanePos, Quaternion.identity);

        // Place the pet down on the plane
        pet = Instantiate(pet, petPlanePos, placementPose.rotation);

        // Set the planes name
        petPlane.name = "Pet Plane";
        
        // Show plane and pet
        pet.SetActive(true);
        petPlane.SetActive(true);
    }

    // Calculate random positions for food
    void PlaceFood()
    {
        // Get the pets position
        Vector3 catPos = petPlanePos;

        int index = 0;

        // Loop through each food 
        foreach (GameObject food in foods)
        {
            // Reset the foods position for some reason
            food.transform.position = new Vector3(0, 0.1f, 0);

            // Get random position
            Vector3 randomPosition = new Vector3(catPos.x + getRandomCoord(), catPos.y, catPos.z + getRandomCoord());

            // Add the random position to the current position.
            food.transform.position += randomPosition;

            index++;
        }
    }

    // Hide or Show all food in the scene
    void ShowFood(bool show)
    {
        foreach(GameObject food in foods)
        {
            if(show)
            {
                food.SetActive(true);
            } else
            {
                food.SetActive(false);
            }
        }
    }

    float getRandomCoord()
    {
        // Get a random number from (-.9, .9)
        return UnityEngine.Random.Range(-.9f, .9f);
    }

    // Not in use
    public float OnDamage() {
        health = health - 0.1f;
        HealthBar.fillAmount = health;
        HealthBarPercentage.text = Math.Round((health / 1)*100) + "%";
        return health;
    }

    // Get the next question
    public void nextQuestion()
    {
        // If currentProblem = -1, the game has not initialized yet.
        currentProblem += 1;

        if (currentProblem == questions.Count-1)
        {
            // Check if the current question is the last one
            lastProblem = true;
        }
        
        // Get the current question
        JObject question = (JObject)questions[currentProblem];

        // Get all data from questions
        string imageUrl = (string)question["imageUrl"];
        solutions = question["solutions"].ToObject<string[]>();
        correctSolutionIndex = (int)question["selectedSolution"];

        // Download question image
        StartCoroutine(DownloadImage(imageUrl));

        int index = 0;
        foreach (string solution in solutions)
        {
            // Associate food with question solutions
            foods[index].GetComponent<FoodScript>().setValue(solution);
            foods[index].GetComponent<FoodScript>().valueIndex = index;

            index++;
        }


        if (placed)
        {
            // If the plane has already been placed,
            // hide all food in the scene from the last quesiton
            ShowFood(false);
            index = 0;
            foreach (string solution in solutions)
            {
                // Only reshow food that is relevant for nex tquestion
                foods[index].SetActive(true);
                index++;
            }
            
            // Recalculate new random positions for the food
            PlaceFood();
        }
    }

    public void saveAttempt()
    {
        // Get the current logged in studentId
        string studentId = firebase.auth.CurrentUser.UserId;

        // Generate new id for this new attempt
        string attemptId = Guid.NewGuid().ToString();

        // Create an object to hold the attempt data
        JObject attemptData = new JObject();

        // Fill in attempt data
        attemptData["finishTime"] = finishTime;
        attemptData["date"] = DateTime.UtcNow.ToString("s"); // Get ISO date string
        attemptData["grade"] = Math.Floor((correctAnswerCount / totalQuestionCount) * 100); // Calculate grade
        attemptData["questions"] = questions;

        // Send new attempt data to Firebase database
        firebase.set("students/" + studentId + "/classes/" + classId + "/attempts/" + attemptId, attemptData.ToString());
    }
}
