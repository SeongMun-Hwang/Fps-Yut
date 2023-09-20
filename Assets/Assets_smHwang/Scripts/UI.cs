using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI Goal_Status;
    public TextMeshProUGUI Yut;
    string DelayFunctionText;
    //���͸���
    private GameObject lastSelectedPlayer;
    private Material originalMaterial;
    private Material _outlineMaterial;

    public void StartText(int turn)
    {
        Yut.text = "����!";
        DelayFunctionText = "player " + (turn + 1) + " turn!";
        StartCoroutine(YieldReturnDelay(1.0f, DelayFunctionText));
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
    // �÷��̾� �׵θ� ������Ʈ
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
}