using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isMenuOpened;
    public GameObject menuUI;
    public GameObject scoreUI;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isMenuOpened == false)
        {
            scoreUI.SetActive(false);
            menuUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isMenuOpened = true;
            AudioListener.pause = true;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isMenuOpened == true)
        {
            scoreUI.SetActive(true);
            menuUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            isMenuOpened = false;
            AudioListener.pause = false;
        }
    }
    public void LeaveGame()
    {
        Application.Quit();
    }
}
