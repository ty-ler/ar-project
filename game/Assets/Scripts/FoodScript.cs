using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodScript : MonoBehaviour
{
    public float value { get; set; }
    public TextMeshPro foodText;

    void Update()
    {
        foodText.transform.LookAt(Camera.main.transform);
        foodText.transform.Rotate(Vector3.up - new Vector3(0, 180, 0));
    }

    public void setValue(float val)
    {
        value = val;
        foodText.SetText(val.ToString());
    }
}
