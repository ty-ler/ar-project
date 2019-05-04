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

public class PetController : MonoBehaviour
{
    public GameObject pet;
    public GameObject placementIndicator;
    public GameObject petPlane;
    public GameObject[] foods;
    public TextMeshProUGUI timerText;
    public Image HealthBar;
    public Text HealthBarPercentage;
    public Text totalQuestions;
    public float correctAnswer;
    public bool timerGoing;
    public string currentTimerText;
    public bool wonGame;
    private float health;
    private float startHealth = 1f;
    public int correctAnswerCount = 0;
    public int totalAnswerCount = 0;

    private Vector3[] foodPositions;
    private APIHandler apiHandler;
    private QAHandler qaHandler;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool validPlacementPose;
    private bool placed;
    Vector3 petPlanePos;
    private float startTime;

    private JArray problems;
    private int currentProblem = -1;
    public bool lastProblem = false;

    void Start()
    {
        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ar-project-d52cb.firebaseio.com/");

        //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        //reference.Child("test").GetValueAsync().ContinueWith(task =>
        //{ 
        //    if(task.IsFaulted)
        //    {
        //        Debug.Log("Error!!!!!");
        //    } else if(task.IsCompleted)
        //    {
        //        DataSnapshot snapshot = task.Result;
                
        //        Debug.Log(snapshot.Value);
        //    } else if(task.IsCanceled)
        //    {
        //        Debug.Log("Task cancelled!!!!!");
        //    }
        //});

        foodPositions = new Vector3[foods.Length];

        int index = 0;
        foreach(GameObject food in foods)
        {
            foodPositions[index] = food.transform.position;
            index++;
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

        try
        {
            apiHandler = new APIHandler();
            JObject obj = apiHandler.get("/problems", null);
            Debug.Log(obj.ToString());
        } catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

        problems = new JArray();
        JObject p1 = new JObject();
        p1.Add("prompt", "What is the square root of 49?");
        p1.Add("solution", 7);
        p1.Add("possible_solutions", new JArray(new float[] { 7, 8, 6.5f }));

        JObject p2 = new JObject();
        p2.Add("prompt", "What is the square root of 9?");
        p2.Add("solution", 3);
        p2.Add("possible_solutions", new JArray(new float[] { 1, 2, 3 }));

        problems.Add(p1);
        problems.Add(p2);
        totalAnswerCount = problems.Count;
        totalQuestions.text = correctAnswerCount + "/" + totalAnswerCount;
        nextQuestion();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (validPlacementPose && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!placed && !wonGame) {
                PlacePet();
                ShowFood(true); 
                PlaceFood();
                //correctAnswer = 7;
                startTime = Time.time;
                timerGoing = true;
                timerText.SetText("");
            }
            else if(placed && !wonGame)
            {
                pet.GetComponent<CatMoveTo>().StartMove(placementPose.position);
            } 
        }

        if(placed && timerGoing)
        {
            float t = Time.time - startTime;
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f0");

            currentTimerText = minutes + ":" + seconds;
            timerText.SetText(currentTimerText);
        }

    }

    void UpdatePlacementIndicator()
    {
        Vector3 placeIndPos = new Vector3(placementPose.position.x, placementPose.position.y + .001f, placementPose.position.z);

        if(!placed)
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
        } else
        {
            if(validPlacementPose)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(placeIndPos, placementPose.rotation);
            } else
            {
                placementIndicator.SetActive(false);
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

    void PlacePet()
    {
        placed = true;

        Vector3 placePosePos = new Vector3(placementPose.position.x, placementPose.position.y - 1f, placementPose.position.z);

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
            food.transform.position = foodPositions[index];
            bool validPos = false;
            Vector3 randomPosition = new Vector3(catPos.x + getRandomCoord(), catPos.y, catPos.z + getRandomCoord());
            while (!validPos)
            {
                Collider[] collisions = Physics.OverlapSphere(randomPosition, .3f);
                foreach(Collider col in collisions)
                {
                    if(col.CompareTag("Food"))
                    {
                        validPos = false;
                    }
                    else
                    {
                        validPos = true;
                    }
                }

                if(!validPos)
                {
                    randomPosition = new Vector3(catPos.x + getRandomCoord(), catPos.y, catPos.z + getRandomCoord());
                }
            }

            if(validPos)
            {
                food.transform.position += randomPosition;
            }

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
        return UnityEngine.Random.Range(-1f, 1f);
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

        if (currentProblem == problems.Count-1)
        {
            lastProblem = true;
        }

        JObject problem = (JObject)problems[currentProblem];

        string prompt = (string)problem["prompt"];
        float solution = (float)problem["solution"];
        float[] possible_solutions = ((JArray)problem["possible_solutions"]).ToObject<float[]>();

        qaHandler.questionText.text = prompt;
        correctAnswer = solution; 

        int index = 0;
        foreach (Text solutionText in qaHandler.possible_solutions)
        {
            foods[index].GetComponent<FoodScript>().setValue(possible_solutions[index]); // Set the value on the actual GameObject
            solutionText.text = possible_solutions[index].ToString(); // Set the text of each option on the QAPanel
            index++;
        }

        if (placed)
        {
            ShowFood(true);
            PlaceFood();
        }
    }
}
