using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start_Menu : MonoBehaviour
{
    public void game_Start()
    {
        SceneManager.LoadScene("YutPlay");
    }
    public void Settings()
    {
        Debug.Log("setting button clicked");
    }
    public void game_Quit()
    {
        Application.Quit();
    }
}
