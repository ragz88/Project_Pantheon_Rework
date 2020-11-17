using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialMainMenu : MonoBehaviour
{

    void Start() 
    {
        Cursor.visible = true;
    }

    public float delay;

    public void LoadSceneAfterDelay(string scene)
    {
        StartCoroutine(sceneLoadDelay(delay, scene));
    }
    

    public void ExitGame()
    {
        Application.Quit();
    }

   

    public IEnumerator sceneLoadDelay(float delay, string scene) 
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(scene);
    }
    
}
