using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PetScript : MonoBehaviour
{
    public TextMeshProUGUI winText;

    private float correctAnswer;
    private PetController petController;
    private QAHandler HealthBar;
    void Start()
    {
        winText.SetText("");
        petController = FindObjectOfType<PetController>();
        HealthBar = GetComponent<QAHandler>();
        correctAnswer = petController.correctAnswer;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);
            float foodValue = other.gameObject.GetComponent<FoodScript>().value;
            if (foodValue == correctAnswer) {
                winText.SetText("Correct Answer!\nTime: " + petController.currentTimerText);
                petController.timerGoing = false;
                petController.timerText.gameObject.SetActive(false);
                petController.wonGame = true;
            }
            else
                HealthBar.OnDamage();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);    
            float foodValue = other.gameObject.GetComponent<FoodScript>().value;
            Debug.Log(correctAnswer + ", " + foodValue);
            if (foodValue == correctAnswer) {
                winText.SetText("Correct Answer!");
            }
            else
                HealthBar.OnDamage();
        }
    }
}
