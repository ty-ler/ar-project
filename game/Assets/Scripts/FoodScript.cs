using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodScript : MonoBehaviour
{
    public string value { get; set; }
    public int valueIndex { get; set; }
    public TextMeshPro foodText;

    void Update()
    {
        // Make sure the food's value text is always facing the player
        foodText.transform.LookAt(Camera.main.transform);

        // Flip the food's value text in the correct direction
        foodText.transform.Rotate(Vector3.up - new Vector3(0, 180, 0));
    }

    // Used to set the value show above the food
    public void setValue(string val)
    {
        value = val;
        foodText.SetText(val);
    }
}
