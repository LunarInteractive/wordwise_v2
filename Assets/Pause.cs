using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public GameObject micButton;
    public bool isPaused;

    public void PauseGame(){
        Time.timeScale = 0f;
        isPaused = true;
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        micButton.SetActive(false);
    }

    public void ResumeGame(){
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        micButton.SetActive(true);
    }
}
