using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LevelOne()
    {
        SceneManager.LoadScene("Level 1");
    }
    
    public void TestLevel()
    {
        SceneManager.LoadScene("DeveloperLevel");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
