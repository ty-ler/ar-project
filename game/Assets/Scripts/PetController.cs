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

    private Vector3[] foodPositions;
    private APIHandler apiHandler;
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
        QuestionButton.gameObject.SetActive(false);
        firebase = new FirebaseManager();
        foods = GameObject.FindGameObjectsWithTag("Food");
        
        foodPositions = new Vector3[foods.Length];

        int index = 0;
        foreach (GameObject food in foods)
        {
            foodPositions[index] = food.transform.position;   
        }

        qaHandler = FindObjectOfType<QAHandler>();
        placed = false;
        validPlacementPose = false;
        wonGame = false;
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        pet.SetActive(false);
        petPlane.SetActive(false);
        HealthBar.fillAmount = startHealth;
        health = startHealth;

        ShowFood(false);

        timerText.SetText("Scanning...");

        loadClassData();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (validPlacementPose && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!placed && !wonGame)
            {
                QuestionButton.gameObject.SetActive(true);
                PlacePet();
                int index = 0;
                foreach (string solution in solutions)
                {
                    foods[index].SetActive(true);
                    index++;
                }
                PlaceFood();
                startTime = Time.time;
                timerGoing = true;
                timerText.SetText("");
            }
            else if (placed && !wonGame)
            {
                pet.GetComponent<CatMoveTo>().StartMove(placementPose.position);
            }
        }

        if (placed && timerGoing)
        {
            float t = Time.time - startTime;
            finishTime = t;
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f0");

            currentTimerText = minutes + ":" + seconds.PadLeft(2, '0');

            timerText.SetText(currentTimerText);
        }
    }

    void UpdatePlacementIndicator()
    {
        Vector3 placeIndPos = new Vector3(placementPose.position.x, placementPose.position.y + .001f, placementPose.position.z);

        if(!wonGame)
        {
            if (!placed)
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
        Vector3 screencenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var cameraForward = Camera.current.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

        if (!placed)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            arOrigin.Raycast(screencenter, hits, TrackableType.Planes);

            validPlacementPose = hits.Count > 0;

            if (validPlacementPose)
            {
                timerText.SetText("Tap to Place Pet");
                placementPose = hits[0].pose;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            } else
            {
                timerText.SetText("Scanning...");
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                if(hit.collider.gameObject.name == "Pet Plane")
                {
                    validPlacementPose = true;
                    placementPose.position = hit.point;
                    placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                }
            }
            else
            {
                validPlacementPose = false;
            }
        }
    }

    async void loadClassData()
    {

        classData = await firebase.get("teachers/" + teacherId + "/classes/" + classId);
        setupQuestions();
        nextQuestion();
    }

    void setupQuestions()
    {

        foreach (KeyValuePair<string, JToken> item in (JObject)classData["questions"])
        {
            JObject question = (JObject)item.Value;
            question["correct"] = false;
            questions.Add(question);
        }

        questions = new JArray(questions.OrderBy(obj => (int)obj["order"]));

        totalQuestionCount = questions.Count;
    }


    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            QuestionImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    void PlacePet()
    {
        placed = true;

        Vector3 placePosePos = new Vector3(placementPose.position.x, placementPose.position.y - 1f, placementPose.position.z + .5f);

        petPlanePos = placePosePos + placementPose.forward;

        placementPose.position = petPlanePos;

        petPlane = Instantiate(petPlane, petPlanePos, Quaternion.identity);
        pet = Instantiate(pet, petPlanePos, placementPose.rotation);
        petPlane.name = "Pet Plane";

        pet.SetActive(true);
        petPlane.SetActive(true);
    }

    void PlaceFood()
    {
        Vector3 catPos = petPlanePos;

        int index = 0;
        foreach (GameObject food in foods)
        {
            Debug.Log(food.transform.position.ToString() + ", " + foodPositions[index].ToString());
            food.transform.position = new Vector3(0, 0.1f, 0);
            bool validPos = false;
            Vector3 randomPosition = new Vector3(catPos.x + getRandomCoord(), catPos.y, catPos.z + getRandomCoord());
            food.transform.position += randomPosition;
            index++;
        }
    }

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
        return UnityEngine.Random.Range(-.9f, .9f);
    }
    public float OnDamage() {
        health = health - 0.1f;
        HealthBar.fillAmount = health;
        HealthBarPercentage.text = Math.Round((health / 1)*100) + "%";
        return health;
    }

    public void nextQuestion()
    {
        // If currentProblem = -1, the game has not initialized yet.
        currentProblem += 1;

        if (currentProblem == questions.Count-1)
        {
            lastProblem = true;
        }
        JObject question = (JObject)questions[currentProblem];

        string imageUrl = (string)question["imageUrl"];
        solutions = question["solutions"].ToObject<string[]>();
        correctSolutionIndex = (int)question["selectedSolution"];

        StartCoroutine(DownloadImage(imageUrl));

        int index = 0;
        foreach (string solution in solutions)
        {
            foods[index].GetComponent<FoodScript>().setValue(solution);
            foods[index].GetComponent<FoodScript>().valueIndex = index;

            index++;
        }

        if (placed)
        {
            ShowFood(false);
            index = 0;
            foreach (string solution in solutions)
            {
                foods[index].SetActive(true);
                index++;
            }
            PlaceFood();
        }
    }

    public void saveAttempt()
    {
        string studentId = firebase.auth.CurrentUser.UserId;
        string attemptId = Guid.NewGuid().ToString();

        Debug.Log(questions);
        JObject attemptData = new JObject();

        attemptData["finishTime"] = finishTime;
        attemptData["date"] = DateTime.UtcNow.ToString("s");
        attemptData["grade"] = Math.Floor((correctAnswerCount / totalQuestionCount) * 100);
        attemptData["questions"] = questions;

        firebase.set("students/" + studentId + "/classes/" + classId + "/attempts/" + attemptId, attemptData.ToString());
    }
}
