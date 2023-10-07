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
        for (int i = 0; i < Constants.PlayerNumber; i++)
        {
            users[i] = new user();
            users[i].horses = new List<horse>();
            for (int j = 0; j < Constants.HorseNumber; j++)
            {
                horse newHorse = new horse();
                users[i].horses.Add(newHorse);
            }
        }
        for (int j = 0; j < Constants.PlayerNumber; j++)
        {
            for (int i = 0; i < Constants.HorseNumber; i++)
            {
                users[0].horses[i].player = red_team[i];
                users[1].horses[i].player = blue_team[i];
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
}
