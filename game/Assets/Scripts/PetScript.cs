using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PetScript : MonoBehaviour
{
    public TextMeshProUGUI winText;

    private float correctAnswer;

    void Start()
    {
        correctAnswer = FindObjectOfType<PetController>().correctAnswer;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);
            float foodValue = other.gameObject.GetComponent<FoodScript>().value;
            Debug.Log(correctAnswer + ", " + foodValue);
            if(foodValue == correctAnswer)
            {
                Debug.Log("CORRECT ANSWER!!!!");
                winText.SetText("Correct Answer!");
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);
            float foodValue = other.gameObject.GetComponent<FoodScript>().value;
            Debug.Log(correctAnswer + ", " + foodValue);
            if (foodValue == correctAnswer)
            {
                Debug.Log("CORRECT ANSWER!!!!");
                winText.SetText("Correct Answer!");
            }
        }
    }
}
