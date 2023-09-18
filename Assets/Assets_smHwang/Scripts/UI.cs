using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI Goal_Status;

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
}
