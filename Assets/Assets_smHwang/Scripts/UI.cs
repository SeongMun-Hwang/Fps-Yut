using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI : MonoBehaviour
{
    //��ũ��Ʈ
    public stone stone;
    public MoveScript MoveScript;
    //�ؽ�Ʈ
    public TextMeshProUGUI Goal_Status;
    public TextMeshProUGUI Yut;
    string DelayFunctionText;
    //���͸���
    private GameObject lastSelectedPlayer;
    private Material originalMaterial;
    private Material _outlineMaterial;
    //Ÿ�̸�
    public TextMeshProUGUI timerText;
    public float timeLeft;
    public float startTime = 60.0f; // Ÿ�̸��� ���� �ð� (60��)
    //��ư
    Color original_Edge = Color.white;
    Color highligted_Edge = new Color(255f / 255f, 0f / 255f, 255f / 255f);
    public Button[] steps_button;
    public TextMeshProUGUI[] steps_button_Text;
    public int selectedButtonIndex = -1;
    public List<int> steps = new List<int>();
    public int choose_step = 0;
    private bool _isBackdo;
    //������ġ ǥ��
    public GameObject instance;
    public List<GameObject> DestiantionObject = new List<GameObject>();
    //�Ҳ�
    public ParticleSystem[] Fire;
    //����
    public Light DirectionalLight;
    private void ChangeTurnUI()
    {
        choose_step = 0;
        timeLeft = 60.0f;
        clear_stepsButton();
    }
    //������ �����ֱ� �׼�
    private void DestinationUI()
    {
        DestroyDestination();
        CalculateDestination();
        ShowDestination();
    }
    private void _UpdateSteps()
    {
        steps.RemoveAt(choose_step);
        steps_button_Text[choose_step].text = "";
        UpdateYutChoice();
    }
    private void Start()
    {
        //�׼ǿ� ���� ���� ����
        stone.OnChangeTurnAction = ChangeTurnUI;
        stone.ShowDestination = DestinationUI;
        stone.UpdateSteps = _UpdateSteps;

        StartText(YutGameManager.Instance.GetTurn());
        for (int i = 0; i < steps_button.Length; i++)
        {
            int index = i; // ����: Ŭ���� ������ �ٱ� ������ ���� ���� ������ ���� ������ �� ����
            steps_button[i].onClick.AddListener(() => choose_steps(index));
        }
        //steps_button_Text ����
        for (int i = 0; i < 5; i++)
        {
            steps_button_Text[i] = steps_button[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    private void Update()
    {
        timer();
        GoalCounter(YutGameManager.Instance.GetHorse());
        ChooseMove();
        SetOutline(YutGameManager.Instance.GetNowHorse().player);
        if (steps.Count > 1 && stone.isYutThrown == true)
        {
            if (stone.isMoving)
            {
                Yut.text = "";
            }
            else
            {
                Yut.text = "�󸶳� �̵��� �� �����ϼ���!";
            }
        }
        //�̵��ϸ� ��� ���� ��ġ ������Ʈ �ı�
        if (stone.isMoving)
        {
            DestroyDestination();
        }
    }
    public void StartText(int turn)
    {
        Yut.text = "����!";
        DelayFunctionText = "player " + (turn + 1) + " turn!";
        StartCoroutine(YieldReturnDelay(1.0f, DelayFunctionText));
        timeLeft = startTime;
    }
    public void GoalCounter(List<horse> horses)
    {
        int[] countArray = new int[2];

        for (int i = 0; i < 2; i++)
        {
            foreach (var horse in horses)
            {
                if (!horse.goal)
                {
                    countArray[i]++;
                }
            }
        }
        Goal_Status.text = "�÷��̾�1 ���� ��: " + countArray[0] +
            "\n�÷��̾�2 ���� ��: " + countArray[1];
    }
    public IEnumerator YieldReturnDelay(float time, String text)
    {
        yield return new WaitForSeconds(time);
        Yut.text = text;
        DelayFunctionText = "";
    }
    // �÷��̾� ������Ʈ
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
    //Ÿ�̸�
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
    //�̵� ���� ��ư
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
        steps.Clear();
    }
    public void choose_steps(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < steps.Count)
        {
            choose_step = buttonIndex;
            Debug.Log("buttonindex : "+buttonIndex);
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
    public void SetSteps(int value)
    {
        steps.Add(value);
        //_isBackdo = isBackdo;
        //Debug.Log("_isBackdo : " + _isBackdo);
        //Debug.Log("isBackdo : " + isBackdo);
        match_Yut(value);
    }
    public void ChooseMove()
    {
        if (steps.Count > 0)
        {
            //�̵��� steps left/right arrow�� ����            
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Yut.text = "";
                if (choose_step > 0)
                {
                    choose_step--;
                }
                Debug.Log("steps=" + steps[choose_step]);
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
                selectedButtonIndex = choose_step;
                UpdateButtonColors();
            }
        }
    }
    void match_Yut(int i)
    {
        switch (i)
        {
            case -1:
                steps_button_Text[steps.Count - 1].text = "�鵵";
                Yut.text = " �鵵";
                break;
            case 1:
                steps_button_Text[steps.Count - 1].text = "��";
                Yut.text = " ��";
                break;
            case 2:
                steps_button_Text[steps.Count - 1].text = "��";
                Yut.text = " ��";
                break;
            case 3:
                steps_button_Text[steps.Count - 1].text = "��";
                Yut.text = " ��";
                break;
            case 4:
                steps_button_Text[steps.Count - 1].text = "��";
                Yut.text = " ��";
                break;
            case 5:
                steps_button_Text[steps.Count - 1].text = "��";
                Yut.text = " ��";
                break;
        }
        StartCoroutine(YieldReturnDelay(1.0f, ""));
    }
    public int GetStep()
    {
        return steps[choose_step];
    }
    //������ ���
    public void CalculateDestination()
    {
        horse horses = YutGameManager.Instance.GetNowHorse();
        //steps ����Ʈ�� �ִ� ���鿡 ���� ���� ��� �� FinalDestination ����Ʈ�� �߰�
        foreach (var stepsLeft in steps)
        {
            int tempStepsLeft = stepsLeft;
            int finalPosition = horses.nowPosition;

            while (tempStepsLeft > 0)
            {
                int NormalRoute;
                if (tempStepsLeft == -1)
                {
                    finalPosition = MoveScript.BackdoRoute();
                }
                else
                {
                    NormalRoute = MoveScript.NormalRoute(horses.nowPosition, finalPosition);
                    if (NormalRoute != -1)
                    {
                        finalPosition = NormalRoute;
                        tempStepsLeft--;
                    }
                    else if (NormalRoute == -1)
                    {
                        finalPosition++;
                        tempStepsLeft--;
                    }
                }
                // ��θ� ������� Ȯ��
                if (finalPosition >= stone.currentRoute.childNodeList.Count)
                {
                    finalPosition = stone.currentRoute.childNodeList.Count - 1; // ����� ��
                }

            }
            horses.FinalPosition.Add(finalPosition);
        }
    }
    //FinalDestination ��ġ�鿡 ������Ʈ ����
    public void ShowDestination()
    {
        int turn = YutGameManager.Instance.GetTurn();
        horse horses = YutGameManager.Instance.GetNowHorse();
        user nowUser = YutGameManager.Instance.GetNowUsers();

        foreach (var destination in horses.FinalPosition)
        {
            Transform targetNode = stone.currentRoute.childNodeList[destination];
            Vector3 targetPosition = targetNode.position;

            targetPosition.y += 0.5f;

            GameObject instantiatedObj = Instantiate(stone.objectPrefab[turn], targetPosition, Quaternion.identity);
            DestiantionObject.Add(instantiatedObj);
            Renderer rend = instantiatedObj.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = nowUser.DestinationColor;
            }
        }
    }
    //������ ������Ʈ ��� �ı�, ����Ʈ �ʱ�ȭ
    public void DestroyDestination()
    {
        foreach (var obj in DestiantionObject)
        {
            Destroy(obj);
        }
        horse horses = YutGameManager.Instance.GetNowHorse();
        horses.FinalPosition.Clear();
        DestiantionObject.Clear();
    }
    public void TurnOnFire()
    {
        for(int i = 0; i < 4; i++)
        {
            Fire[i].Play();
        }
        DirectionalLight.intensity = 0.1f;
    }
    public IEnumerator TurnOffFire()
    {
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < 4; i++)
        {
            Fire[i].Stop();
        }
        DirectionalLight.intensity = 1.0f;
    }
}