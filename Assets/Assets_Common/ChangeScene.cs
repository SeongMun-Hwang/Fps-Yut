using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Update is called once per frame
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        //Screen.SetResolution(640, 480, false);
    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F1))
        //    SceneManager.LoadScene("start_Menu");
        //if (Input.GetKeyDown(KeyCode.F2))
        //    SceneManager.LoadScene("YutPlay");
        //if (Input.GetKeyDown(KeyCode.F3))
        //    SceneManager.LoadScene("Fpsfight");
        //if (Input.GetKeyDown(KeyCode.F4))
        //    SceneManager.LoadScene("Defense_Game");
        //if (Input.GetKeyDown(KeyCode.F5))
        //    SceneManager.LoadScene("setting");        
        //if (Input.GetKeyDown(KeyCode.F6))
        //    SceneManager.LoadScene("roomscene");
    }
}
