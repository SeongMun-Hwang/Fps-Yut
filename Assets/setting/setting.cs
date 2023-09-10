using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class setting : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public AudioSource audioSource;
    public Slider volumeSlider;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(0);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene(1);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.LoadScene("Fpsfight");
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.LoadScene("Defense_Game");
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene("setting");
        }
    }
    private void Start()
    {
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        volumeSlider.onValueChanged.AddListener(SetVolume); // 슬라이더 값 변경 시 볼륨 조절
        volumeSlider.value = audioSource.volume;
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
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
    public void testSound()
    {
        audioSource.Play();
    }
}
