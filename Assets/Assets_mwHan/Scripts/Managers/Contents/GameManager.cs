using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MinitwoGameManager : MonoBehaviour
{
    MinitwoGameManager g_instance;
    public MinitwoGameManager Instance { get { Init(); return g_instance; } }

    float _lefttime; // 남은 시간
    float WaitTime = 3; // 씬 전환 시간

    public float LeftTime
    {
        get { return _lefttime; }
        set { Instance._lefttime = value; }
    } 

    #region
    public GameObject diedPanel;
    public TextMeshProUGUI WinPlayerTxt;
    public TextMeshProUGUI SceneChangeTimeTxt;
    public TextMeshProUGUI LeftTimeTxt; // 남은 시간 텍스트
    #endregion

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        LeftTime -= Time.deltaTime;

        if (LeftTime <= 0)
        {
            //GameOver();
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

    static void Init()
    {

    }

    //public void GameOver()
    //{
    //    if (!isdead)
    //    {
    //        isdead = true;
    //        if(loseplayer == "Player1")
    //        {
    //            WinPlayerTxt.text = string.Format("{0} Win!!", "TestPlayer");
    //        }
    //        else WinPlayerTxt.text = string.Format("{0} Win!!", "Player1");
    //        diedPanel.SetActive(true);
    //        StartCoroutine(ChangeScene());
    //    }
    //}

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(WaitTime);
        SceneManager.LoadScene(0);
    }
}
