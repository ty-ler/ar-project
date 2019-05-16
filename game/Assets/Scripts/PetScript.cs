using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PetScript : MonoBehaviour
{
    public TextMeshProUGUI winText;

    private PetController petController;
    private float health;
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
            int foodValueIndex = other.gameObject.GetComponent<FoodScript>().valueIndex;
            Debug.Log("FOOD INDEX: " + foodValueIndex + ", CORRECT INDEX: " + petController.correctSolutionIndex);
            if (foodValueIndex == petController.correctSolutionIndex) {
                petController.correctAnswerCount++;
                petController.totalQuestions.text = petController.correctAnswerCount + "/" + petController.totalQuestionCount;
                if (petController.lastProblem)
                {
                    winText.SetText("Game Finished!\nTime: " + petController.currentTimerText + "\nCorrect Answers: " + petController.totalQuestions.text);
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
            {
                //petController.nextQuestion();
                health = petController.OnDamage();
                if (health == 0.0f)
                {
                    winText.SetText("Game Over!\nTime: " + petController.currentTimerText + "/nCorrect Answers:" + petController.totalQuestions);
                    petController.timerGoing = false;
                    petController.timerText.gameObject.SetActive(false);
                    petController.wonGame = true;
                }
            }
                
        }
    }
}
