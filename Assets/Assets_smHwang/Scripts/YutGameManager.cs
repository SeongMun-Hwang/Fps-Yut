using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const int PlayerNumber = 2;
    public const int HorseNumber = 4;
    public const float STACK_HEIGHT = 1f;
}

public class YutGameManager : MonoBehaviour
{
    public static YutGameManager Instance { get; private set; }

    private int turn = 0;
    private int player_number;
    private user[][] users;

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
    }

    public void SetTurnAndPlayerNumber(int t, int p_num, user[][] u)
    {
        turn = t;
        player_number = p_num;
        users = u;
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

    public user[][] GetUsers()
    {
        return users;
    }

    //set
    public void SetTurn(int newTurn)
    {
        turn = newTurn;
    }

    public void SetPlayerNumber(int newPlayerNumber)
    {
        player_number = newPlayerNumber;
    }
}
