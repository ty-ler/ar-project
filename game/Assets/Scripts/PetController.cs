using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class PetController : MonoBehaviour
{
    public GameObject pet;
    public GameObject placementIndicator;
    public GameObject petPlane;
    public GameObject[] foods;
    public TextMeshProUGUI timerText;

    public float correctAnswer;
    public bool timerGoing;
    public string currentTimerText;
    public bool wonGame;

    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool validPlacementPose;
    private bool placed;
    Vector3 petPlanePos;
    private float startTime;


    void Start()
    {
        placed = false;
        validPlacementPose = false;
        wonGame = false;
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        pet.SetActive(false);
        petPlane.SetActive(false);

        ShowFood(false);

        timerText.SetText("Scanning...");
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (validPlacementPose && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!placed && !wonGame) {
                PlacePet();
                PlaceFood();
                ShowFood(true);
                correctAnswer = 7;
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

        Vector3 placePosePos = placementPose.position;

        petPlanePos = new Vector3(placePosePos.x, placePosePos.y - 1f, placePosePos.z) + placementPose.forward;

        placementPose.position = petPlanePos;

        petPlane = Instantiate(petPlane, petPlanePos, Quaternion.identity);
        pet = Instantiate(pet, petPlanePos, placementPose.rotation);
        petPlane.name = "Pet Plane";

        pet.SetActive(true);
        petPlane.SetActive(true);
    }

    void PlaceFood()
    {
        Vector3 catPos = placementPose.position;

        GameObject cherry = foods[0];
        GameObject cake = foods[1];
        GameObject hamburger = foods[2];

        cherry.GetComponent<FoodScript>().setValue(6.5f);
        cake.GetComponent<FoodScript>().setValue(8f);
        hamburger.GetComponent<FoodScript>().setValue(7f);

        foreach(GameObject food in foods)
        {
            bool validPos = false;
            Vector3 randomPosition = new Vector3(catPos.x + getRandomCoord(), catPos.y, catPos.z + getRandomCoord());
            while (!validPos)
            {
                Collider[] collisions = Physics.OverlapSphere(randomPosition, 3f);
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
}
