using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start_Menu : MonoBehaviour
{
    private void Update()
    {
        game_Start();
    }
    public void game_Start()
    {
        SceneManager.LoadScene("YutPlay");
    }
    public void Settings()
    {

    }
    public void game_Quit()
    {
        Application.Quit();
    }
}
