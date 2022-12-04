using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float LeftTime; // 남은 시간
    public bool isdead; 
    public float WaitTime; // 씬 전환 시간

    public GameObject diedPanel;
    public TextMeshProUGUI SceneChangeTimeTxt;
    public TextMeshProUGUI LeftTimeTxt; // 남은 시간 텍스트

    private void Start()
    {
        
    }
    private void Update()
    {

        if (isdead) WaitTime -= Time.deltaTime;
        else { 
            LeftTime -= Time.deltaTime; 
        }
    }

    private void LateUpdate()
    {
        int min = (int)((LeftTime / 60));
        int sec = (int)LeftTime % 60;
        LeftTimeTxt.text = string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        int diesec = (int)WaitTime % 60;
        SceneChangeTimeTxt.text = string.Format("Please wait : ") + string.Format("{0:0}", diesec) + string.Format("secs");
    }

    public void GameOver()
    {
        isdead = true;
        diedPanel.SetActive(true);
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }
}
