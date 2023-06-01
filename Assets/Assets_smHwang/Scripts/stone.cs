using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Threading.Tasks;

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
        public Vector3 nextPos;
        public Vector3 player_start_position;
        public bool goal;
        public bool is_destroyed;
    };

    public List<int> steps = new List<int>();
    private async Task DelayAsync(float seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }

    bool isMoving;
    int turn = 0;
    int sum = 0;
    int chance = 0;
    float time;
    bool isFight = false;
    int player_number;
    user[][] users;
    int enemy;
    public GameObject[] objectPrefab;
    bool isBackdo = false;
    int choose_step = 0;
    bool isYutThrown = false;
    //오브젝트 테두리
    Material originalMaterial;
    Material outlineMaterial;
    GameObject lastSelectedPlayer;

    // 턴이 바뀌기 전에 현재 선택된 플레이어의 테두리를 업데이트합니다.
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
        choose_Player();
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
            yut[i] = UnityEngine.Random.Range(0, 2);
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
    }
    public void throw_do()
    {
        steps.Add(1);
        Debug.Log(steps);
        Yut.text = "도!";
        isYutThrown = true;
    }
    public void throw_back_do()
    {
        steps.Add(1);
        Debug.Log(steps[0]);
        Yut.text = "백도!";
        isBackdo = true;
        isYutThrown = true;
    }
    public void throw_gae()
    {
        steps.Add(2);
        Debug.Log(steps[0]);
        Yut.text = "개!";
        isYutThrown = true;
    }
    public void throw_girl()
    {
        steps.Add(3);
        Debug.Log(steps[0]);
        Yut.text = "걸!";
        isYutThrown = true;
    }
    public async void throw_yut()
    {
        steps.Add(4);
        Debug.Log(steps[0]);
        Yut.text = "윷!";
        await DelayAsync(0.5f);
        Yut.text = "한 번 더!";
        if (Input.GetKeyDown(KeyCode.Space))
        {
            throwYut(); // Throw yut again
        }
    }

    public async void throw_mo()
    {
        steps.Add(5);
        Debug.Log(steps[0]);
        Yut.text = "모!";
        await DelayAsync(0.5f);
        Yut.text = "한 번 더!";
        if (Input.GetKeyDown(KeyCode.Space))
        {
            throwYut(); // Throw yut again
        }
    }

    public void one()
    //플레이어 선택
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
        u.nextPos = Vector3.zero;
        u.goal = false;
        u.is_destroyed = false;

        // 새로 생성된 player 게임 오브젝트의 위치와 회전을 이전 플레이어와 동일하게 설정합니다.
        u.player.transform.position = u.player_start_position;
    }

    /*키보드 입력으로 말 선택
     1~4 : 말 선택
    space : 윷던지기*/

    void choose_Player()
    {
        if (isMoving == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                one();
                Debug.Log("choose" + player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                two();
                Debug.Log("choose" + player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                three();
                Debug.Log("choose" + player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                four();
                Debug.Log("choose"+player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Return)&&isYutThrown==true)
            {
                Debug.Log("move"+player_number);
                StartCoroutine(Move(steps[choose_step]));
            }
            else if (Input.GetKeyDown(KeyCode.Space)&&isYutThrown==false)
            {
                throwYut();
            }
            //이동할 steps left/right arrow로 선택            
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Yut.text = "";
                if (choose_step > 0)
                {
                    choose_step--;
                }
                Debug.Log("steps=" + steps[choose_step]);
                foreach (int i in steps)
                {
                    match_Yut(i);
                }
                Yut.text = "Available" + Yut.text + "\nyou choose";
                match_Yut(steps[choose_step]);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Yut.text = "";
                if (choose_step < steps.Count-1)
                {
                    choose_step++;
                }
                Debug.Log("steps=" + steps[choose_step]);
                foreach (int i in steps)
                {
                    match_Yut(i);
                }
                Yut.text = "Available" + Yut.text + "\nyou choose";
                match_Yut(steps[choose_step]);
            }
        }
    }
    void match_Yut(int i)
    {
        switch (i)
        {
            case 1:
                if (isBackdo)
                {
                    Yut.text += " 백도";
                }
                else
                {
                    Yut.text += " 도";
                }
                break;
            case 2:
                Yut.text += " 개";
                break;
            case 3:
                Yut.text += " 걸";
                break;
            case 4:
                Yut.text += " 윷";
                break;
            case 5:
                Yut.text += " 모";
                break;
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
    IEnumerator Move(int chosed_step)
    {
        if (turn == 0)
        {
            enemy = 1;
        }
        else
        {
            enemy = 0;
        }
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

            while (chosed_step > 0)
            {
                if (users[turn][player_number].routePosition == 30 && chosed_step > 0)
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
                    else if (users[turn][player_number].nowPosition == 25)
                    {
                        users[turn][player_number].routePosition = 10;
                    }
                    else if (users[turn][player_number].nowPosition == 15 && users[turn][player_number].lastPosition == 24)
                    {
                        users[turn][player_number].routePosition = 24;
                    }
                    else if (users[turn][player_number].nowPosition == 1)
                    {
                        users[turn][player_number].routePosition = 30;
                    }
                    else
                    {
                        users[turn][player_number].routePosition = users[turn][player_number].nowPosition - 1;
                    }
                    users[turn][player_number].nowPosition = users[turn][player_number].routePosition;
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
                if (users[turn][player_number].nowPosition == 24 && chosed_step >= 1)
                {
                    users[turn][player_number].routePosition = 15;

                }
                if ((users[turn][player_number].lastPosition >= 15 && users[turn][player_number].lastPosition <= 19) || (users[turn][player_number].lastPosition >= 28 && users[turn][player_number].lastPosition <= 29))
                {
                    if ((users[turn][player_number].nowPosition == 19 || users[turn][player_number].nowPosition == 29) && chosed_step >= 1)
                    {
                        users[turn][player_number].routePosition = 30;
                    }
                }

                users[turn][player_number].nextPos = currentRoute.childNodeList[users[turn][player_number].routePosition].position;
                while (MoveToNextNode(users[turn][player_number].nextPos)) { yield return null; }
                chosed_step--;
                users[turn][player_number].nowPosition++;

                //상대방 말을 지나갈때
                for (int i = 0; i < 4; i++)
                {
                    if ((users[turn][player_number].routePosition == users[enemy][i].nowPosition) && chosed_step > 1)
                    {
                        Yut.text = "To pass, Win!";
                        yield return new WaitForSeconds(0.5f);
                        Debug.Log("Moving piece passed by an enemy piece.");
                        SceneManager.LoadScene("Defense_Game");
                    }
                }

                yield return new WaitForSeconds(0.1f);
                while (isFight == true)
                {
                    Yut.text = "";
                    yield return new WaitForSeconds(0.1f);
                }
            }

             steps.RemoveAt(choose_step);
            if (choose_step == steps.Count)
            {
                choose_step--;
                Debug.Log("if =" + choose_step);
            }

            Debug.Log("after remove" + steps.Count);
            foreach(int i in steps)
            {
                Debug.Log(i);
            }


            //말끼리 먹기 동작
            //for (int i = 0; i < 4; i++)
            //{
            //    if (users[turn][player_number].nowPosition == users[enemy][i].nowPosition)
            //    {
            //        미니 게임 없이 말을 먹을 때의 동작
            //        Debug.Log("encounter");
            //        reset_player(ref users[enemy][i], objectPrefab[enemy]);
            //        chance++;

            //        //Fpsfight 진행
            //        SceneManager.LoadScene("Fpsfight");
            //    }
            //}

            isMoving = false;
            sum = 0;
            Debug.Log("move all");

            //윷, 모 나왔을 시 턴 유지, 아니면 변경
            //if (chance > 0)
            //{
            //    chance--;
            //}
            if (steps.Count == 0)
            {
                if (turn == 0) { turn = 1; choose_step = 0; isYutThrown = false; }
                else if (turn == 1) { turn = 0; choose_step = 0; isYutThrown = false; }
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
