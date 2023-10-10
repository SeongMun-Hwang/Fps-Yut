using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Constants
{
    public const int PlayerNumber = 2;
    public const int HorseNumber = 4;
    public const float STACK_HEIGHT = 1.2f;
}

public class YutGameManager : MonoBehaviour
{
    public static YutGameManager Instance { get; private set; }

    private int turn = 0;
    private int player_number;
    private user[] users;
    public GameObject[] red_team;
    public GameObject[] blue_team;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        users = new user[Constants.PlayerNumber];

        //���� ����
        for (int i = 0; i < Constants.PlayerNumber; i++)
        {
            users[i] = new();
        }
        AssignTurns(); // �� �� ���� �Ҵ�
        //�� ����
        for (int i = 0; i < Constants.PlayerNumber; i++)
        {
            users[i].horses = new List<horse>();
            for (int j = 0; j < Constants.HorseNumber; j++)
            {
                horse newHorse = new();
                users[i].horses.Add(newHorse);
            }
        }
        //user.turn�� ���� �ʱ�ȭ
        for (int j = 0; j < Constants.PlayerNumber; j++)
        {
            int userTurn = users[j].turn;
            for (int i = 0; i < Constants.HorseNumber; i++)
            {
                if (userTurn == 0) //����
                {
                    users[j].horses[i].player = red_team[i];
                    Color newColor = new Color(255f / 255f, 77f / 255f, 70f / 255f, 0.5f);
                    users[j].DestinationColor = newColor;
                }
                else if (userTurn == 1) //�Ķ�
                {
                    users[j].horses[i].player = blue_team[i];
                    Color newColor = new Color(77f / 255f, 77f / 255f, 255f / 255f);
                    users[j].DestinationColor = newColor;
                }
                users[j].horses[i].player_start_position = users[j].horses[i].player.transform.position;
                users[j].horses[i].is_bind = false;
                users[j].horses[i].BindedHorse = new List<int>();
                users[j].horses[i].FinalPosition = new List<int>();
            }
        }
    }

    public void SetTurnAndPlayerNumber(int t, int p_num, horse[][] u)
    {
        turn = t;
        player_number = p_num;
    }
    //get
    public int GetTurn()
    {
        return turn;
    }

    public int GetPlayerNumber()
    {
        return player_number;
    }
    public user[] GetUsers()
    {
        return users;
    }
    public user GetNowUsers()
    {
        return users[turn];
    }
    public List<horse> GetHorse()
    {
        return users[turn].horses;
    }
    public horse GetNowHorse()
    {
        return users[turn].horses[player_number];
    }
    public void SetTurn(int newTurn)
    {
        turn = newTurn;
    }

    public void SetPlayerNumber(int newPlayerNumber)
    {
        player_number = newPlayerNumber;
    }
    //���� �� ����
    private void AssignTurns()
    {
        int[] turnValues = new int[Constants.PlayerNumber];
        for (int i = 0; i < Constants.PlayerNumber; i++)
        {
            turnValues[i] = i;
        }

        Shuffle(turnValues);

        for (int i = 0; i < Constants.PlayerNumber; i++)
        {
            users[i].turn = turnValues[i];
        }
    }
    //�� �����ϰ� ����
    private void Shuffle(int[] array)
    {
        System.Random rng = new System.Random();
        int n = array.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
