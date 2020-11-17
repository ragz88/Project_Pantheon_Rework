using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool paused = false;

    public GameObject pauseMenu;

    void Start()
    {
        paused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            paused = !paused;
        }

        if (paused) 
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
        }
        else if (!paused)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.visible = false;
        }
    }

    public void UnPause() 
    {
        paused = false;
    }

    public void ReloadLevel() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu() 
    {
        SceneManager.LoadScene("MainMenuTutorial");
        UnPause();
    }

    public void ExitGame() 
    {
        Application.Quit();
    }


}
