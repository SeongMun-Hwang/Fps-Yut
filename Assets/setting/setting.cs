using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class setting : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private void Start()
    {
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
    }

    public void SetResolution_full()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
    }

    public void SetResolution_1366x768()
    {
        Screen.SetResolution(1366, 768, FullScreenMode.Windowed);
    }

    public void SetResolution_1280x720()
    {
        Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
    }

    public void SetResolution_1920x1080()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
    }

    public void ChangeResolution(int value)
    {
        switch (value)
        {
            case 0: // Full Screen
                SetResolution_full();
                Debug.Log(value);
                break;
            case 1: // 1366x768
                Debug.Log(value);
                SetResolution_1366x768();
                break;
            case 2: // 1280x720
                Debug.Log(value);
                SetResolution_1280x720();
                break;
            case 3: // 1920x1080
                Debug.Log(value);
                SetResolution_1920x1080();
                break;
            default:
                Debug.LogError("Invalid resolution option.");
                break;
        }
    }
}
