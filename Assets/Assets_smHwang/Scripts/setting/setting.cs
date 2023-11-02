using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class setting : MonoBehaviour
{
    public stone stone;
    public TMP_Dropdown resolutionDropdown;
    public Slider effectSoundsSlider;
    public Slider backGroundSlider;
    public AudioSource[] effectSounds;
    public AudioSource backgroundSounds;
    private void Start()
    {
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        effectSoundsSlider.onValueChanged.AddListener(SetEffectSounds); // 슬라이더 값 변경 시 볼륨 조절
        effectSoundsSlider.value = effectSounds[0].volume;
        backGroundSlider.onValueChanged.AddListener(SetBackgroundSounds); // 슬라이더 값 변경 시 볼륨 조절
        backGroundSlider.value = backgroundSounds.volume;
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
    public void SetEffectSounds(float volume)
    {
        Debug.Log("Setting volume to: " + volume);
        for (int i = 0; i < effectSounds.Length; i++)
        {
            effectSounds[i].volume = volume;
        }
    }
    public void SetBackgroundSounds(float volume)
    {
        backgroundSounds.volume = volume;
    }
    public void testSound()
    {
        int RandomSound = UnityEngine.Random.Range(0, 5);
        effectSounds[RandomSound].mute = false;
        effectSounds[RandomSound].Play();
    }
    public void TurnOnBackGroundMusic()
    {
        backgroundSounds.mute = false;
        backgroundSounds.Play();
        backgroundSounds.loop = true;
    }
}
