using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class GameController : MonoBehaviour
{
    public GameObject placementIndicator;

    private ARSessionOrigin AROrigin;
    private Pose placementPose;
    private bool validPlacementPose = false;

    // Start is called before the first frame update
    void Start()
    {
        AROrigin = FindObjectOfType<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if(Input.touchCount > 0)
        {
            ShootRay();
        }
    }

    private void UpdatePlacementIndicator()
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
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        Ray rayToPlayerPos = Camera.current.ScreenPointToRay(screenCenter);

        var arhits = new List<ARRaycastHit>();
        var physhits = new List<RaycastHit>();
        AROrigin.Raycast(screenCenter, arhits, TrackableType.Planes);
        
        validPlacementPose = arhits.Count > 0;

        if (validPlacementPose)
        {
            Debug.Log(arhits.Count);
            placementPose = arhits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void ShootRay()
    {
        Debug.Log("shooting!");
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        RaycastHit hit;
        if (Physics.Raycast(Camera.current.transform.position, Camera.current.transform.forward, out hit)) {
            Debug.Log(hit.transform.name);
        }
        Debug.Log(hit.transform);
    }
}
