using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PetScript : MonoBehaviour
{
    public TextMeshProUGUI winText;

    private PetController petController;
    void Start()
    {
        winText.SetText("");
        petController = FindObjectOfType<PetController>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("HITTING: " + other.ToString() + ", Tag: " + other.tag);
        if(other.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);
            float foodValue = other.gameObject.GetComponent<FoodScript>().value;
            if (foodValue == petController.correctAnswer) {
                if(petController.lastProblem)
                {
                    winText.SetText("Game Finished!\nTime: " + petController.currentTimerText);
                    petController.timerGoing = false;
                    petController.timerText.gameObject.SetActive(false);
                    petController.wonGame = true;
                }
                else
                {
                   petController.nextQuestion();
                }
            }
            else
                petController.OnDamage();
        }
    }
}
