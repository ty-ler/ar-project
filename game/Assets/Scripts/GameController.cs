using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class GameController : MonoBehaviour
{
    public GameObject placementIndicator;
    public GameObject laser;
    public Camera helperCamera;
    public Camera arCamera;

    private ARSessionOrigin AROrigin;
    private Pose placementPose;
    private bool validPlacementPose = false;
    private WaitForSeconds shotDuration = new WaitForSeconds(.1f);
    private LineRenderer laserLine;

    // Start is called before the first frame update
    void Start()
    {

        AROrigin = FindObjectOfType<ARSessionOrigin>();
        laserLine = laser.GetComponent<LineRenderer>();
        //laser.SetActive(false);
        laserLine.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        Vector3 cameraCenter = Camera.main.transform.position;
        Vector3 shootPoint = new Vector3(cameraCenter.x, cameraCenter.y - 10, cameraCenter.z);
        laserLine.SetPosition(0, shootPoint);
        laserLine.SetPosition(1, Camera.main.transform.forward);

        if (Input.touchCount > 0)
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
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        Ray rayToPlayerPos = Camera.main.ScreenPointToRay(screenCenter);

        var arhits = new List<ARRaycastHit>();
        var physhits = new List<RaycastHit>();
        AROrigin.Raycast(screenCenter, arhits, TrackableType.Planes);
        
        validPlacementPose = arhits.Count > 0;

        if (validPlacementPose)
        {
            placementPose = arhits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private IEnumerator ShotEffect()
    {
        laser.SetActive(true);
        laserLine.enabled = true;

        yield return shotDuration;

        laser.SetActive(false);
        laserLine.enabled = false;
    }

    private void ShootRay()
    {
        //StartCoroutine(ShotEffect());
        Debug.Log("shooting!");

        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f));

        Vector3 cameraCenter = Camera.main.transform.position;
        Vector3 shootPoint = new Vector3(cameraCenter.x, cameraCenter.y - 10, cameraCenter.z);
        laserLine.SetPosition(0, shootPoint);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit)) {
            laserLine.SetPosition(1, hit.point);
            Debug.Log(hit.transform.name);
        } else
        {
            laserLine.SetPosition(1, Camera.main.transform.forward);
        }
    }
}
