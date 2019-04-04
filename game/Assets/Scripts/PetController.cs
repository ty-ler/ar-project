using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;

public class PetController : MonoBehaviour
{
    public GameObject pet;
    public GameObject placementIndicator;
    public GameObject petPlane;

    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool validPlacementPose = false;
    private int maxCatAllowed = 1;
    private int currentNumberOfCats = 0;
    private bool placed = false;
    Vector3 petPlanePos;
    public GameObject fruit;
    public GameObject food1;
    public GameObject food2;
    public Text TextField;
   

    public float distance = -5;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        pet.SetActive(false);
        petPlane.SetActive(false);
        fruit.AddComponent<MeshCollider>();
        food1.AddComponent<MeshCollider>();
        food2.AddComponent<MeshCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if(validPlacementPose && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (currentNumberOfCats < maxCatAllowed) {
                PlacePet();
                PlaceFood();
                placementIndicator.SetActive(false);
                currentNumberOfCats++;
            }
            else 
            {
                pet.GetComponent<CatMoveTo>().StartMove(placementPose.position);
            }

        }

    }

    void UpdatePlacementIndicator()
    {
        if(!placed)
        {
            if (validPlacementPose)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
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
                placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
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
                placementPose = hits[0].pose;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }
        else
        {
            RaycastHit hit;
            Debug.Log("Raycasting!");
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                Debug.Log("Hit!");
                validPlacementPose = true;
                placementPose.position = hit.point;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
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

        petPlanePos = new Vector3(placePosePos.x, placePosePos.y, placePosePos.z);

        petPlane = Instantiate(petPlane, placePosePos, placementPose.rotation);
        pet = Instantiate(pet, placePosePos, placementPose.rotation);
        pet.SetActive(true);
        petPlane.SetActive(true);
    }
    void PlaceFood()
    {
        Vector3 back = new Vector3(0, 0.1f, 0.35f);
        Vector3 right = new Vector3(0.35f, 0.2f, 0);
        Vector3 left = new Vector3(-0.35f, 0.15f, 0);




        Instantiate(fruit, pet.transform.position + back, transform.rotation);
            Instantiate(food1, pet.transform.position + right, transform.rotation);
            Instantiate(food2, pet.transform.position + left, transform.rotation);




    }
    public void OnCollisionEnter(Collider pet)
    {
       TextField.text = "Hello!";
    }
}
