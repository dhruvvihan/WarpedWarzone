using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Include this if you want to load a different scene on timeout

public class TimerScript : MonoBehaviour
{
    public Text timerText;
    private float timeRemaining = 600f;
    public string exitSceneName = "Menu"; // Name of the scene to load when time runs out

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            timeRemaining = 0;
            LeaveMatch(); // Call method to handle leaving the match
        }
    }

    private void UpdateTimerText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeRemaining);
        timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void LeaveMatch()
    {
        // Optionally: display a "Time's up!" message or show a UI element
        Debug.Log("Time's up! Leaving match...");

        // Load a specified exit scene
        SceneManager.LoadScene(exitSceneName);

        // Additional cleanup logic if needed, like disabling player controls or saving data
    }
}
