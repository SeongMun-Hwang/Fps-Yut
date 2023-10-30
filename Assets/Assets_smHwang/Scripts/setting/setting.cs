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
    public Slider volumeSlider;
    public AudioSource[] audioSources;
    public AudioClip[] myClips;
    private int EffectSoundIndex=0;
    private void Start()
    {
        for (int i = 0; i < myClips.Length; i++)
        {
            audioSources[i].clip = myClips[i];
        }
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        volumeSlider.onValueChanged.AddListener(SetVolume); // 슬라이더 값 변경 시 볼륨 조절
        volumeSlider.value = audioSources[0].volume;
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
        for(int i = 0; i < myClips.Length; i++)
        {
            audioSources[i].volume = volume;
        }
    }
    public void testSound()
    {
        Debug.Log(EffectSoundIndex);
        audioSources[EffectSoundIndex].Play();
        if (EffectSoundIndex == 4) { EffectSoundIndex = 0; }
        else { EffectSoundIndex++; }
    }
}
