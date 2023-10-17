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
/// <summary>
/// 0.0 0.5 -5.48
/// </summary>
public class YutGameManager : MonoBehaviour
{
    public static YutGameManager Instance { get; private set; }

    private int turn = 0;
    public int player_number;
    private user[] users;
    public GameObject[] red_team;
    public GameObject[] blue_team;
    public GameObject MainGame;
    public GameObject HammerGame;
    public GameObject DefenseGame;
    private Camera mainCamera;
    private void Awake()
    {
        mainCamera = Camera.main;
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

        //유저 생성
        for (int i = 0; i < Constants.PlayerNumber; i++)
        {
            users[i] = new();
        }
        AssignTurns(); // 턴 값 랜덤 할당
        //말 생성
        for (int i = 0; i < Constants.PlayerNumber; i++)
        {
            users[i].horses = new List<horse>();
            for (int j = 0; j < Constants.HorseNumber; j++)
            {
                horse newHorse = new();
                users[i].horses.Add(newHorse);
            }
        }
        //user.turn에 따라 초기화
        for (int j = 0; j < Constants.PlayerNumber; j++)
        {
            int userTurn = users[j].turn;
            for (int i = 0; i < Constants.HorseNumber; i++)
            {
                if (userTurn == 0) //빨강
                {
                    users[j].horses[i].player = red_team[i];
                    Color newColor = new Color(255f / 255f, 77f / 255f, 70f / 255f, 0.5f);
                    users[j].DestinationColor = newColor;
                }
                else if (userTurn == 1) //파랑
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
        StartMainGame();
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
    //랜덤 턴 지정
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
    //턴 랜덤하게 셔플
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
    public void StartMainGame()
    {
        stone.isFight = false;
        MainGame.SetActive(true);
        if (DefenseGame.activeSelf) { DefenseGame.SetActive(false); }
        if (HammerGame.activeSelf) { HammerGame.SetActive(false); }
        Vector3 v = new Vector3(0.0f, 0.5f, -5.48f);
        mainCamera.transform.position = new Vector3(-4.11f, 17.02f, -4.5f);
        transform.LookAt(v);
    }
    public void StartDefenseGame()
    {
        Vector3 v = new Vector3(0.0f, 0.5f, -5.48f);
        //MainGame.SetActive(false);
        DefenseGame.SetActive(true);
        mainCamera.transform.position = new Vector3(0.0f, 94.3f, -37.6f);
        transform.LookAt(v);
    }
    public void StartHammerGame()
    {
        MainGame.SetActive(false);
        HammerGame.SetActive(true);
    }
}
