using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float LeftTime;
    public TextMeshProUGUI LeftTimeTxt;

    private void Start()
    {
        
    }
    private void Update()
    {
        LeftTime -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        int min = (int)((LeftTime / 60));
        int sec = (int)LeftTime % 60;
        LeftTimeTxt.text = string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);
    }
}
