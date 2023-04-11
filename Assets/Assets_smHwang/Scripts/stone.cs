using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class stone : MonoBehaviour
{
    public Yut_Field currentRoute;
    public AudioSource[] yutSound;
    public TextMeshProUGUI Yut;
    public GameObject[] red_team;
    public GameObject[] blue_team;

    public struct user
    {
        public GameObject player;
        public int routePosition;
        public int nowPosition;
        public int lastPosition;      
        public int steps;
        public Vector3 nextPos;
        public Vector3 player_start_position;
        public bool goal;
        public bool is_destroyed;
    };

    bool isMoving;
    int turn = 0;
    int sum = 0;
    int chance = 0;
    float time;
    bool isFight = false;
    public int player_number = 0; 
    user[][] users;
    int enemy;
    public GameObject[] objectPrefab;
    bool isBackdo = false;

    //오브젝트 테두리
    Material originalMaterial;
    Material outlineMaterial;
    GameObject lastSelectedPlayer;

    void SetOutline(GameObject player)
    {
        if (lastSelectedPlayer != null)
        {
            lastSelectedPlayer.GetComponent<Renderer>().material = originalMaterial;
        }

        if (player != null)
        {
            originalMaterial = player.GetComponent<Renderer>().material;
            player.GetComponent<Renderer>().material = outlineMaterial;
        }

        lastSelectedPlayer = player;
    }

    private void Update()
    {
        keyboard_Input();
        check_Winner();
        AutoSelectClosestPlayerInArray();
    }
    void Start()
    {
        users = new user[2][];
        users[0] = new user[red_team.Length];
        users[1] = new user[blue_team.Length];

        if (isFight == false)
        {
            for(int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    users[0][i].player = red_team[i];
                    users[1][i].player = blue_team[i];
                    users[j][i].player_start_position = users[j][i].player.transform.position;
                    users[j][i].is_destroyed = false;
                }
            }
            
        }
        for (int i = 0; i < 5; i++)
        {
            yutSound[i].mute = true;
        }
        outlineMaterial = Resources.Load<Material>("OutlineMaterial");
    }

    public void throwYut()
    {
        int[] yut = new int[4];
        for (int i = 0; i < 4; i++)
        {
            yut[i] = Random.Range(0, 2);
            sum += yut[i];
        }
        switch (sum)
        {
            case 0:
                throw_mo();
                break;
            case 1:
                if (yut[3] == 1)
                {
                    throw_back_do();
                    break;
                }
                throw_do();
                break;
            case 2:
                throw_gae();
                break;
            case 3:
                throw_girl();
                break;
            case 4:
                throw_yut();
                break;
        }
        yutSound[sum].mute = false;
        yutSound[sum].Play();
        StartCoroutine(Move());
        // 턴이 바뀌기 전에 현재 선택된 플레이어의 테두리를 업데이트합니다.
    }

    public void throw_do()
    {
        users[turn][player_number].steps = 1;
        StartCoroutine(Move());
        Yut.text = "도!";
    }
    public void throw_back_do()
    {
        users[turn][player_number].steps = 1;
        StartCoroutine(Move());
        Yut.text = "백도!";
        isBackdo = true;
    }
    public void throw_gae()
    {
        users[turn][player_number].steps = 2;
        StartCoroutine(Move());
        Yut.text = "개!";
    }
    public void throw_girl()
    {
        users[turn][player_number].steps = 3;
        StartCoroutine(Move());
        Yut.text = "걸!";
    }
    public void throw_yut()
    {
        users[turn][player_number].steps = 4;
        chance++;
        Yut.text = "윷!";
        StartCoroutine(Move());
    }
    public void throw_mo()
    {
        users[turn][player_number].steps = 5;
        chance++;
        Yut.text = "모!";
        StartCoroutine(Move());
    }
    
    //플레이어 선택
    public void one()
    {
        player_number = 0;
        check_player();
        SetOutline(users[turn][player_number].player);
    }
    public void two()
    {
        player_number = 1;
        check_player();
        SetOutline(users[turn][player_number].player);
    }
    public void three()
    {
        player_number = 2;
        check_player();
        SetOutline(users[turn][player_number].player);
    }
    public void four()
    {
        player_number = 3;
        check_player();
        SetOutline(users[turn][player_number].player);
    }
    public void check_player()
    {
        if (users[turn][player_number].is_destroyed == true)
        {
            Yut.text = "player already goaled!";
        }
    }

    void clear_player(ref user u) //player 말 삭제
    {
        if (u.player != null)
        {
            Destroy(u.player);
            u.player = null;
        }
    }
    //삭제한 플레이어 말 재생성
    public void reset_player(ref user u, GameObject playerPrefab)
    {
        // 먼저 현재 user에 대한 리소스를 해제하기 전에 이전 player 정보를 저장합니다.
        Quaternion prevRotation = u.player.transform.rotation;

        // 먼저 현재 user에 대한 리소스를 해제합니다.
        clear_player(ref u);

        // 초기값 설정
        u.player = Instantiate(playerPrefab); // player 게임 오브젝트를 생성합니다.
        u.routePosition = 0;
        u.nowPosition = 0;
        u.lastPosition = 0;
        u.steps = 0;
        u.nextPos = Vector3.zero;
        u.goal = false;
        u.is_destroyed = false;

        // 새로 생성된 player 게임 오브젝트의 위치와 회전을 이전 플레이어와 동일하게 설정합니다.
        u.player.transform.position = u.player_start_position;
    }

    /*키보드 입력으로 말 선택
     1~4 : 말 선택
    space : 윷던지기*/
    void keyboard_Input()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            one();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            two();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            three();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            four();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            throwYut();
        }
    }
    //한 팀의 모든 플레이어 오브젝트가 null일 시 승리 선언
    void check_Winner()
    {
        bool redTeamAllNull = true;
        bool blueTeamAllNull = true;

        for (int i = 0; i < 4; i++)
        {
            if (users[0][i].player != null)
            {
                redTeamAllNull = false;
            }
            if (users[1][i].player != null)
            {
                blueTeamAllNull = false;
            }
        }

        if (redTeamAllNull)
        {
            Yut.text = "Red Team is the winner!";
        }
        else if (blueTeamAllNull)
        {
            Yut.text = "Blue Team is the winner!";
        }
        else
        {
            Yut.text = "";
        }
    }


    //플레이어 오브젝트가 존재하지 않으면 배열 상 가장 가까운 존재하는 오브젝트 자동으로 선택
    void AutoSelectClosestPlayerInArray()
    {
        if (users[turn][player_number].player == null)
        {
            int closestPlayerNumber = -1;

            for (int i = 0; i < users[turn].Length; i++)
            {
                int nextPlayerNumber = (player_number + i) % users[turn].Length;

                if (users[turn][nextPlayerNumber].player != null)
                {
                    closestPlayerNumber = nextPlayerNumber;
                    break;
                }
            }

            if (closestPlayerNumber != -1)
            {
                player_number = closestPlayerNumber;
                SetOutline(users[turn][player_number].player);
            }
        }
    }
    IEnumerator Move()
    {              
        {
            SetOutline(users[turn][player_number].player);
            yield return new WaitForSeconds(1f);
            Yut.text = "";
            if (isMoving)
            {
                yield break;
            }
            isMoving = true;

            users[turn][player_number].lastPosition = users[turn][player_number].routePosition;


            while (users[turn][player_number].steps != 0)
            {
                if (users[turn][player_number].routePosition == 30 && users[turn][player_number].steps > 0)
                {
                    Debug.Log("Goal");
                    Destroy(users[turn][player_number].player);
                    users[turn][player_number].is_destroyed = true;
                    break;
                }

                users[turn][player_number].nowPosition = users[turn][player_number].routePosition;
                users[turn][player_number].routePosition++;

                //백도 예외 처리
                if (isBackdo == true)
                {
                    if (users[turn][player_number].nowPosition == 20)
                    {
                        users[turn][player_number].routePosition = 5;
                    }
                    else if(users[turn][player_number].nowPosition == 25)
                    {
                        users[turn][player_number].routePosition = 10;
                    }
                    else if(users[turn][player_number].nowPosition==15&& users[turn][player_number].lastPosition==24)
                    {
                        users[turn][player_number].routePosition = 24;
                    }
                    else
                    {
                        users[turn][player_number].routePosition = users[turn][player_number].nowPosition - 1;
                    }
                    isBackdo = false;
                }
                else if (users[turn][player_number].lastPosition == 5 && users[turn][player_number].nowPosition == 5)
                {
                    users[turn][player_number].routePosition = 20;
                }
                else if (users[turn][player_number].lastPosition == 10 && users[turn][player_number].nowPosition == 10)
                {
                    users[turn][player_number].routePosition = 25;
                }
                else if (users[turn][player_number].lastPosition == 22 && users[turn][player_number].nowPosition == 22) //center
                {
                    users[turn][player_number].routePosition = 28;
                }
                else if (users[turn][player_number].lastPosition == 24 && users[turn][player_number].nowPosition == 24)
                {
                    users[turn][player_number].routePosition = 15;
                }
                if (users[turn][player_number].nowPosition == 24 && users[turn][player_number].steps >= 1)
                {
                    users[turn][player_number].routePosition = 15;

                }
                if ((users[turn][player_number].lastPosition >= 15 && users[turn][player_number].lastPosition <= 19) || (users[turn][player_number].lastPosition >= 28 && users[turn][player_number].lastPosition <= 29))
                {
                    if ((users[turn][player_number].nowPosition == 19 || users[turn][player_number].nowPosition == 29) && users[turn][player_number].steps >= 1)
                    {
                        users[turn][player_number].routePosition = 30;
                    }
                }

                users[turn][player_number].nextPos = currentRoute.childNodeList[users[turn][player_number].routePosition].position;
                while (MoveToNextNode(users[turn][player_number].nextPos)) { yield return null; }

                //start fps fight if position same
                //if ((users[0].nowPosition == users[1].nowPosition) && users[1].routePosition != 0)
                //{
                //    Yut.text = "Encounter!!";
                //    isFight = true;
                //    yield return new WaitForSeconds(1f);
                //    SceneManager.LoadScene("Fpsfight");
                //}
                
                yield return new WaitForSeconds(0.1f);
                while (isFight == true)
                {
                    Yut.text = "";
                    yield return new WaitForSeconds(0.1f);
                }
                users[turn][player_number].steps--;
                users[turn][player_number].nowPosition++;
            }
            //말끼리 먹기 동작
            if (turn == 0)
            {
                enemy = 1;
            }
            else
            {
                enemy = 0;
            }
            Debug.Log("me -- nowposition : " + users[turn][player_number].nowPosition + " lastposition : " + users[turn][player_number].lastPosition);
            Debug.Log("enemy -- nowposition : " + users[enemy][player_number].nowPosition + " lastposition : " + users[enemy][player_number].lastPosition);
            for (int i = 0; i < 4; i++)
            {
                if (users[turn][player_number].nowPosition == users[enemy][i].nowPosition)
                {
                    Debug.Log("encounter");
                    reset_player(ref users[enemy][i], objectPrefab[enemy]);
                }
            }

            isMoving = false;
            sum = 0;
            Debug.Log("move all");
           
            
            //윷, 모 나왔을 시 턴 유지, 아니면 변경
            if (chance > 0)
            {
                chance--;
            }
            else
            {
                if (turn == 0) { turn = 1; }
                else if (turn == 1) { turn = 0; }
                Yut.text = "player " + (turn + 1) + " turn!";
            }
            player_number = 0;
        }
        users[turn][player_number].lastPosition = users[turn][player_number].nowPosition;
    }

    bool MoveToNextNode(Vector3 goal)
    {
        if (users[turn][player_number].player != null)
        {
            return goal != (users[turn][player_number].player.transform.position = Vector3.MoveTowards(users[turn][player_number].player.transform.position, goal, 8f * Time.deltaTime));
        }
        return false;
    }
}
