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

    private void Start()
    {
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
        ChooseMove();
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
    }
    public void StartText(int turn)
    {
        Yut.text = "����!";
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
        Goal_Status.text = "�÷��̾�1 ���� ��: " + countArray[0] +
            "\n�÷��̾�2 ���� ��: " + countArray[1];
    }
    IEnumerator YieldReturnDelay(float time, String text)
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
    }
    public void choose_steps(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < steps.Count && steps[buttonIndex] != null)
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
        //�̵��� steps left/right arrow�� ����            
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
                    steps_button_Text[steps.Count - 1].text = "�鵵";
                    Yut.text = " �鵵";
                }
                else
                {
                    steps_button_Text[steps.Count - 1].text = "��";
                    Yut.text = " ��";
                }
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
    }
    public int GetStep()
    {
        return steps[choose_step];
    }
}