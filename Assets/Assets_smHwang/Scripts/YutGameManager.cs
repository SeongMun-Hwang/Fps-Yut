using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private user[][] users;
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
        users = new user[Constants.PlayerNumber][];
        for (int i = 0; i < Constants.PlayerNumber; i++)
        {
            users[i] = new user[Constants.HorseNumber];
            for (int j = 0; j < Constants.HorseNumber; j++)
            {
                users[i][j] = new user();  // user 클래스의 인스턴스를 생성 (user 클래스의 생성자가 필요하다면 이것도 추가)
            }
        }
        for (int j = 0; j < Constants.PlayerNumber; j++)
        {
            for (int i = 0; i < Constants.HorseNumber; i++)
            {
                users[0][i].player = red_team[i];
                users[1][i].player = blue_team[i];
                users[j][i].player_start_position = users[j][i].player.transform.position;
                users[j][i].is_bind = false;
                users[j][i].BindedHorse = new List<int>();
                users[j][i].FinalPosition = new List<int>();
            }
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
    public user GetNowUser()
    {
        return users[turn][player_number];
    }
    public void SetTurn(int newTurn)
    {
        turn = newTurn;
    }

    public void SetPlayerNumber(int newPlayerNumber)
    {
        player_number = newPlayerNumber;
    }
}
