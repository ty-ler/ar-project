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

    // Method that is called once the cat hits another GameObject
    void OnTriggerEnter(Collider other)
    {
        // Check if the GameObject was a Food
        if (other.CompareTag("Food"))
        {
            other.gameObject.SetActive(false); // Hide the food

            int foodValueIndex = other.gameObject.GetComponent<FoodScript>().valueIndex;
            petController.questions[petController.currentProblem]["studentSelection"] = foodValueIndex; // Set the index of the solution which the student chose

            // Check if the selected index was the same as the correct solution index
            if (foodValueIndex == petController.correctSolutionIndex) {
                // Increment correct answer count and set current problem as correct
                petController.correctAnswerCount++;
                petController.questions[petController.currentProblem]["correct"] = true;

                HandleAnswerSelected();
            }
            else
            {
                HandleAnswerSelected();
            }

            // Update correct question counter
            petController.totalQuestions.text = petController.correctAnswerCount + "/" + petController.totalQuestionCount;
        }
    }

    void HandleAnswerSelected()
    {
        if (petController.lastProblem)
        {
            // Update correct question counter
            petController.totalQuestions.text = petController.correctAnswerCount + "/" + petController.totalQuestionCount; 

            // Display win game message
            winText.SetText("Game Finished!\nTime: " + petController.currentTimerText + "\nCorrect Answers: " + petController.totalQuestions.text); 

            // Stop and hide the timer
            petController.timerGoing = false; 
            petController.timerText.gameObject.SetActive(false);

            // End the game
            petController.wonGame = true;
            
            // Schedule a notifcation for 24 hours
            NotificationsManager.ScheduleNotifcation("Your pet is getting hungry!", "Tap to start answering questions to feed them!", 5, 1440); // 1440 mintues = 24 hours

            // Save attempt in the Database
            petController.saveAttempt();
        }
        else
        {
            // If problem isn't last problem, load next one
            petController.nextQuestion();
        }
    }
}
