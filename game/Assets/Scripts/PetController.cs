using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class PetController : MonoBehaviour
{
    public GameObject pet;
    public GameObject placementIndicator;

    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool validPlacementPose = false;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if(validPlacementPose && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlacePet();
        }
    }

    void UpdatePlacementIndicator()
    {
        if(validPlacementPose)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);

            pet.SetActive(true);
        } else
        {
            placementIndicator.SetActive(false);
        }
    }

    void UpdatePlacementPose()
    {
        var screencenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        var hits = new List<ARRaycastHit>();
        
        arOrigin.Raycast(screencenter, hits, TrackableType.Planes);

        validPlacementPose = hits.Count > 0;

        Debug.Log(validPlacementPose);

        if(validPlacementPose)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    void PlacePet()
    {
        Instantiate(pet, placementPose.position, Quaternion.identity);
    }
}
