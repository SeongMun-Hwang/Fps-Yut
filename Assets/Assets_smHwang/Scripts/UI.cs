using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI : MonoBehaviour
{
    //스크립트
    public stone stone;
    public MoveScript MoveScript;
    //텍스트
    public TextMeshProUGUI Goal_Status;
    public TextMeshProUGUI Yut;
    string DelayFunctionText;
    //메터리얼
    private GameObject lastSelectedPlayer;
    private Material originalMaterial;
    private Material _outlineMaterial;
    //타이머
    public TextMeshProUGUI timerText;
    public float timeLeft;
    public float startTime = 60.0f; // 타이머의 시작 시간 (60초)
    //버튼
    Color original_Edge = Color.white;
    Color highligted_Edge = new Color(255f / 255f, 0f / 255f, 255f / 255f);
    public Button[] steps_button;
    public TextMeshProUGUI[] steps_button_Text;
    public int selectedButtonIndex = -1;
    public List<int> steps = new List<int>();
    public int choose_step = 0;
    private bool _isBackdo;
    //최종위치 표시
    public GameObject instance;
    List<GameObject> DestiantionObject = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < steps_button.Length; i++)
        {
            int index = i; // 이유: 클로저 때문에 바깥 변수를 직접 쓰면 마지막 값이 고정될 수 있음
            steps_button[i].onClick.AddListener(() => choose_steps(index));
        }
        //steps_button_Text 연결
        for (int i = 0; i < 5; i++)
        {
            steps_button_Text[i] = steps_button[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    private void Update()
    {
        ChooseMove();
        if (steps.Count > 1 && stone.isYutThrown == true)
        {
            if (stone.isMoving)
            {
                Yut.text = "";
            }
            else
            {
                Yut.text = "얼마나 이동할 지 선택하세요!";
            }
        }
        //이동하면 모든 예상 위치 오브젝트 파괴
        if (stone.isMoving)
        {
            DestoryPredictedPosition();
        }
    }
    public void StartText(int turn)
    {
        Yut.text = "시작!";
        DelayFunctionText = "player " + (turn + 1) + " turn!";
        StartCoroutine(YieldReturnDelay(1.0f, DelayFunctionText));
        timeLeft = startTime;
    }
    public void GoalCounter(user[][] users)
    {
        int[] countArray = new int[2];

        for (int i = 0; i < 2; i++)
        {
            foreach (var user in users[i])
            {
                if (!user.goal)
                {
                    countArray[i]++;
                }
            }
        }
        Goal_Status.text = "플레이어1 남은 말: " + countArray[0] +
            "\n플레이어2 남은 말: " + countArray[1];
    }
    IEnumerator YieldReturnDelay(float time, String text)
    {
        yield return new WaitForSeconds(time);
        Yut.text = text;
        DelayFunctionText = "";
    }
    // 플레이어 오브젝트
    public void SetOutline(GameObject player)
    {
        if (lastSelectedPlayer != null)
        {
            lastSelectedPlayer.GetComponent<Renderer>().material = originalMaterial;
        }

        if (player != null)
        {
            originalMaterial = player.GetComponent<Renderer>().material;
            player.GetComponent<Renderer>().material = _outlineMaterial;
        }

        lastSelectedPlayer = player;
    }
    public void SetOutlineMaterial(Material OutlineMaterial)
    {
        _outlineMaterial = OutlineMaterial;
    }
    //타이머
    public void UpdateTimerText()
    {
        int minutes = (int)timeLeft / 60;
        int seconds = (int)timeLeft % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void timer()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            if (!stone.isYutThrown)
            {
                stone.ChangeTurn();
            }
        }
    }
    //이동 선택 버튼
    public void UpdateButtonColors()
    {
        for (int i = 0; i < steps_button.Length; i++)
        {
            Image buttonImage = steps_button[i].GetComponent<Image>();
            if (i == selectedButtonIndex)
            {
                buttonImage.color = highligted_Edge;
            }
            else
            {
                buttonImage.color = original_Edge;
            }
        }
    }
    public void clear_stepsButton()
    {
        for (int i = 0; i < 5; i++)
        {
            steps_button_Text[i].text = "";
            steps_button[i].image.color = original_Edge;
        }
    }
    public void choose_steps(int buttonIndex)
    {
        user[][] users = YutGameManager.Instance.GetUsers();
        if (buttonIndex >= 0 && buttonIndex < steps.Count)
        {
            choose_step = buttonIndex;
            Debug.Log(buttonIndex);
            if (buttonIndex >= 0 && buttonIndex < steps_button.Length)
            {
                selectedButtonIndex = buttonIndex;
                UpdateButtonColors();
            }
        }
    }
    public void UpdateYutChoice()
    {
        for (int i = choose_step; i < steps.Count; i++)
        {
            steps_button_Text[i].text = steps_button_Text[i + 1].text;
        }
        if (steps.Count < steps_button_Text.Length)
        {
            steps_button_Text[steps.Count].text = "";
        }
        if (choose_step == steps.Count)
        {
            choose_step--;
        }
        foreach (int i in steps)
        {
            Debug.Log(i);
        }
    }
    public void SetSteps(int value, bool isBackdo)
    {
        steps.Add(value);
        _isBackdo = isBackdo;
        Debug.Log("_isBackdo : " + _isBackdo);
        Debug.Log("isBackdo : " + isBackdo);
        match_Yut(value);
    }
    public void ChooseMove()
    {
        //이동할 steps left/right arrow로 선택            
            if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Yut.text = "";
            if (choose_step > 0)
            {
                choose_step--;
            }
            Debug.Log("steps=" + steps[choose_step]);
            Yut.text = "Available" + Yut.text + "\nyou choose";
            selectedButtonIndex = choose_step;
            UpdateButtonColors();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Yut.text = "";
            if (choose_step < steps.Count - 1)
            {
                choose_step++;
            }
            Debug.Log("steps=" + steps[choose_step]);
            Yut.text = "Available" + Yut.text + "\nyou choose";
            selectedButtonIndex = choose_step;
            UpdateButtonColors();
        }
    }
    void match_Yut(int i)
    {
        switch (i)
        {
            case 1:
                if (_isBackdo)
                {
                    steps_button_Text[steps.Count - 1].text = "백도";
                    Yut.text = " 백도";
                }
                else
                {
                    steps_button_Text[steps.Count - 1].text = "도";
                    Yut.text = " 도";
                }
                break;
            case 2:
                steps_button_Text[steps.Count - 1].text = "개";
                Yut.text = " 개";
                break;
            case 3:
                steps_button_Text[steps.Count - 1].text = "걸";
                Yut.text = " 걸";
                break;
            case 4:
                steps_button_Text[steps.Count - 1].text = "윷";
                Yut.text = " 윷";
                break;
            case 5:
                steps_button_Text[steps.Count - 1].text = "모";
                Yut.text = " 모";
                break;
        }
    }
    public int GetStep()
    {
        return steps[choose_step];
    }

    public void CalculateFinalPosition(bool isBackdo)
    {
        user user = YutGameManager.Instance.GetNowUser();
        foreach (var stepsLeft in steps)
        {
            int tempStepsLeft = stepsLeft;
            int finalPosition = user.nowPosition;

            while (tempStepsLeft > 0)
            {
                int NormalRoute;
                if (isBackdo && tempStepsLeft == 1)
                {
                    finalPosition = MoveScript.BackdoRoute();
                    isBackdo = false;
                }
                else
                {
                    NormalRoute = MoveScript.NormalRoute(user.nowPosition,finalPosition);
                    Debug.Log(NormalRoute);
                    if (NormalRoute != -1)
                    {
                        finalPosition = NormalRoute;
                        tempStepsLeft--;
                    }
                    else if(NormalRoute==-1)
                    {
                        finalPosition++;
                        tempStepsLeft--;
                    }
                }
                // 경로를 벗어나는지 확인
                if (finalPosition >= stone.currentRoute.childNodeList.Count)
                {
                    finalPosition = stone.currentRoute.childNodeList.Count - 1; // 경로의 끝
                }

            }
            user.FinalPosition.Add(finalPosition);
        }
    }

    public void ShowFinalDestination()
    {
        int turn = YutGameManager.Instance.GetTurn();
        user users = YutGameManager.Instance.GetNowUser();

        foreach (var destination in users.FinalPosition)
        {
            Transform targetNode = stone.currentRoute.childNodeList[destination];
            Vector3 targetPosition = targetNode.position;

            targetPosition.y += 0.5f;

            GameObject instantiatedObj = Instantiate(stone.objectPrefab[turn], targetPosition, Quaternion.identity);
            DestiantionObject.Add(instantiatedObj);
            Renderer rend = instantiatedObj.GetComponent<Renderer>();
            if (rend != null)
            {
                if (turn == 0)
                {
                    Color newColor = new Color(255f / 255f, 77f / 255f, 70f / 255f, 0.5f);
                    rend.material.color = newColor;
                }
                else if (turn == 1)
                {
                    Color newColor = new Color(77f / 255f, 77f / 255f, 255f / 255f);
                    rend.material.color = newColor;
                }
            }
        }
    }
    public void DestoryPredictedPosition()
    {
        foreach (var obj in DestiantionObject)
        {
            Destroy(obj);
        }
        DestiantionObject.Clear();
    }
}