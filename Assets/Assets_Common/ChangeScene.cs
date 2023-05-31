using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Update is called once per frame

    private void Start()
    {
        Screen.SetResolution(640, 480, false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            SceneManager.LoadScene(0);
        if (Input.GetKeyDown(KeyCode.F2))
            SceneManager.LoadScene(1);
        if (Input.GetKeyDown(KeyCode.F3))
            SceneManager.LoadScene(2);
    }
}
