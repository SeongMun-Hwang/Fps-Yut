using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
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
    public GameObject Setting;
    public GameObject MainCanvas;
    private Camera mainCamera;
    public TextMeshProUGUI ManagerText;
    public Action ActionSetPlayerNumber;
    public bool allPlayerEnter = false;
    //loading image
    public GameObject loadingSpinner;
    public float rotateSpeed = 100f;

    private void Awake()
    {
        loadingSpinner.SetActive(false);
        MainGame.SetActive(false);
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
                users[j].horses[i].Owner = users[j];
            }
        }
        StartMainGame();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                // Check if the clicked object is in the red_team array
                int index = Array.IndexOf(red_team, clickedObject);
                if (index != -1)
                {
                    SetPlayerNumber(index);
                    return; // Exit after setting the player number
                }
                // Check if the clicked object is in the blue_team array
                index = Array.IndexOf(blue_team, clickedObject);
                if (index != -1)
                {
                    SetPlayerNumber(index);
                }
            }
        }
        if (allPlayerEnter)
        {
            loadingSpinner.SetActive(false);
            MainGame.SetActive(true);
        }
        else if(!allPlayerEnter)
        {
            loadingSpinner.SetActive(true);
            MainGame.SetActive(false);
            loadingSpinner.GetComponent<Image>().transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
            ManagerText.text = "플레이어 대기중";
        }
        if (Input.GetKeyDown(KeyCode.Space) && allPlayerEnter == false)
        {
            ManagerText.text = "시작!";
            allPlayerEnter = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartSetting();
        }
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
        ManagerText.text = (player_number + 1) + " 번 플레이어 선택!";
        ActionSetPlayerNumber.Invoke();
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
        MainCanvas.SetActive(true);
        if (DefenseGame.activeSelf) { DefenseGame.SetActive(false); }
        if (HammerGame.activeSelf) { HammerGame.SetActive(false); }
        if (Setting.activeSelf) { Setting.SetActive(false); }
        Vector3 v = new Vector3(0.0f, 0.5f, -5.48f);
        mainCamera.transform.position = new Vector3(-4.11f, 17.02f, -4.5f);
        transform.LookAt(v);
    }
    public void StartDefenseGame()
    {
        Vector3 v = new Vector3(0.0f, 0.5f, -5.48f);
        MainGame.SetActive(false);
        DefenseGame.SetActive(true);
        MainCanvas.SetActive(false);
        mainCamera.transform.position = new Vector3(0.0f, 94.3f, -37.6f);
        transform.LookAt(v);
        StartCoroutine(MinigameDelay());
    }
    public void StartHammerGame()
    {
        MainGame.SetActive(false);
        HammerGame.SetActive(true);
        MainCanvas.SetActive(false);
        StartCoroutine(MinigameDelay());
    }
    public void StartSetting()
    {
        if (Setting.activeSelf)
        {
            Setting.SetActive(false);
        }
        else
        {
            Setting.SetActive(true);
        }
    }
    private IEnumerator MinigameDelay()
    {
        yield return new WaitForSeconds(3.0f);

        C_GameReady readyPacket = new C_GameReady();
        Managers.Network.Send(readyPacket);
    }
}
