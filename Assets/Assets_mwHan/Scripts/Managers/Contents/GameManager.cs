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
    public string loseplayer;

    public GameObject diedPanel;
    public TextMeshProUGUI WinPlayerTxt;
    public TextMeshProUGUI SceneChangeTimeTxt;
    public TextMeshProUGUI LeftTimeTxt; // 남은 시간 텍스트

    private void Start()
    {
    }
    private void Update()
    {

        if (isdead)
        {
            WaitTime -= Time.deltaTime;
        }
        else
        {
            LeftTime -= Time.deltaTime;
        }

        if (LeftTime <= 0)
        {
            // Determine the player with lower health
            PlayerController player1 = GameObject.Find("Player1").GetComponent<PlayerController>();
            PlayerController player2 = GameObject.Find("Enemy").GetComponent<PlayerController>();

            if (player1.Health < player2.Health)
                loseplayer = "Player1";
            else
                loseplayer = "Player2";

            GameOver();
        }
    }

    private void LateUpdate()
    {
        int min = (int)((LeftTime / 60));
        int sec = (int)LeftTime % 60;
        LeftTimeTxt.text = string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        int diesec = (int)WaitTime % 60;
        SceneChangeTimeTxt.text = string.Format("Please wait : ") + string.Format("{0:0}", diesec) + string.Format(" secs");
    }

    public void GameOver()
    {
        if (!isdead)
        {
            isdead = true;
            if(loseplayer == "Player1")
            {
                WinPlayerTxt.text = string.Format("{0} Win!!", "TestPlayer");
            }
            else WinPlayerTxt.text = string.Format("{0} Win!!", "Player1");
            diedPanel.SetActive(true);
            StartCoroutine(ChangeScene());
        }
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(WaitTime);
        SceneManager.LoadScene(0);
    }
}
