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
    };

    bool isMoving;
    int turn = 0;
    int sum = 0;
    int chance = 0;
    float time;
    bool isFight = false;
    public int player_number = 1; 
    user[][] users;
    int enemy;
    public GameObject[] objectPrefab;
    void Start()
    {
        users = new user[2][];
        users[0] = new user[red_team.Length];
        users[1] = new user[blue_team.Length];
        if (isFight == false)
        {
            for(int i = 0; i < 4; i++)
            {
                users[0][i].player = red_team[i];
                users[1][i].player = blue_team[i];
                users[0][i].player_start_position = users[0][i].player.transform.position;
                users[1][i].player_start_position = users[1][i].player.transform.position;
            }
        }
        for (int i = 0; i < 5; i++)
        {
            yutSound[i].mute = true;
        }
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
    }

    public void throw_do()
    {
        users[turn][player_number].steps = 1;
        StartCoroutine(Move());
        Yut.text = "도!";
    }
    public void throw_back_do()
    {
        users[turn][player_number].steps = -1;
        StartCoroutine(Move());
        Yut.text = "백도!";
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
    }
    public void two()
    {
        player_number = 1;
    }
    public void three()
    {
        player_number = 2;
    }
    public void four()
    {
        player_number = 3;
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

        // 새로 생성된 player 게임 오브젝트의 위치와 회전을 이전 플레이어와 동일하게 설정합니다.
        u.player.transform.position = u.player_start_position;
    }



    IEnumerator Move()
    {              
        {
            yield return new WaitForSeconds(1f);
            Yut.text = "";
            if (isMoving)
            {
                yield break;
            }
            isMoving = true;

            users[turn][player_number].lastPosition = users[turn][player_number].routePosition;


            while (users[turn][player_number].steps > 0)
            {
                if (users[turn][player_number].routePosition == 30 && users[turn][player_number].steps > 0)
                {
                    Debug.Log("Goal");
                    Destroy(users[turn][player_number].player);
                    break;
                }

                users[turn][player_number].nowPosition = users[turn][player_number].routePosition;
                users[turn][player_number].routePosition++;

                if (users[turn][player_number].lastPosition == 5 && users[turn][player_number].nowPosition == 5)
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
            if (chance == 1)
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
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (users[turn][player_number].player.transform.position = Vector3.MoveTowards(users[turn][player_number].player.transform.position, goal, 8f * Time.deltaTime));
    }
}
