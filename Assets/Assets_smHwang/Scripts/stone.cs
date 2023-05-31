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
        //public int steps;
        public Vector3 nextPos;
        public Vector3 player_start_position;
        public bool goal;
        public bool is_destroyed;
    };
    int steps;
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

    //������Ʈ �׵θ�
    Material originalMaterial;
    Material outlineMaterial;
    GameObject lastSelectedPlayer;

    // ���� �ٲ�� ���� ���� ���õ� �÷��̾��� �׵θ��� ������Ʈ�մϴ�.
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
    }

    public void throw_do()
    {
        steps = 1;
        //StartCoroutine(Move());
        Yut.text = "��!";
    }
    public void throw_back_do()
    {
        steps = 1;
        //StartCoroutine(Move());
        Yut.text = "�鵵!";
        isBackdo = true;
    }
    public void throw_gae()
    {
        steps = 2;
        //StartCoroutine(Move());
        Yut.text = "��!";
    }
    public void throw_girl()
    {
        steps = 3;
        //StartCoroutine(Move());
        Yut.text = "��!";
    }
    public void throw_yut()
    {
        steps = 4;
        chance++;
        Yut.text = "��!";
        //StartCoroutine(Move());
    }
    public void throw_mo()
    {
        steps = 5;
        chance++;
        Yut.text = "��!";
        //StartCoroutine(Move());
    }
    
    //�÷��̾� ����
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

    void clear_player(ref user u) //player �� ����
    {
        if (u.player != null)
        {
            Destroy(u.player);
            u.player = null;
        }
    }
    //������ �÷��̾� �� �����
    public void reset_player(ref user u, GameObject playerPrefab)
    {
        // ���� ���� user�� ���� ���ҽ��� �����ϱ� ���� ���� player ������ �����մϴ�.
        Quaternion prevRotation = u.player.transform.rotation;

        // ���� ���� user�� ���� ���ҽ��� �����մϴ�.
        clear_player(ref u);

        // �ʱⰪ ����
        u.player = Instantiate(playerPrefab); // player ���� ������Ʈ�� �����մϴ�.
        u.routePosition = 0;
        u.nowPosition = 0;
        u.lastPosition = 0;       
        u.nextPos = Vector3.zero;
        u.goal = false;
        u.is_destroyed = false;

        // ���� ������ player ���� ������Ʈ�� ��ġ�� ȸ���� ���� �÷��̾�� �����ϰ� �����մϴ�.
        u.player.transform.position = u.player_start_position;
    }

    /*Ű���� �Է����� �� ����
     1~4 : �� ����
    space : ��������*/

    void choose_Player()
    {
        if (isMoving == false)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                one();
                Debug.Log("choose" + player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                two();
                Debug.Log("choose" + player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                three();
                Debug.Log("choose" + player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
            {
                four();
                Debug.Log("choose"+player_number);
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("move"+player_number);
                StartCoroutine(Move());
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                throwYut();
            }
        }
    }
    //�� ���� ��� �÷��̾� ������Ʈ�� null�� �� �¸� ����
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


    //�÷��̾� ������Ʈ�� �������� ������ �迭 �� ���� ����� �����ϴ� ������Ʈ �ڵ����� ����
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


            while (steps != 0)
            {
                if (users[turn][player_number].routePosition == 30 && steps > 0)
                {
                    Debug.Log("Goal");
                    Destroy(users[turn][player_number].player);
                    users[turn][player_number].is_destroyed = true;
                    break;
                }

                users[turn][player_number].nowPosition = users[turn][player_number].routePosition;
                users[turn][player_number].routePosition++;

                //�鵵 ���� ó��
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
                    else if(users[turn][player_number].nowPosition==15 && users[turn][player_number].lastPosition==24)
                    {
                        users[turn][player_number].routePosition = 24;
                    }
                    else if(users[turn][player_number].nowPosition == 1)
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
                if (users[turn][player_number].nowPosition == 24 && steps >= 1)
                {
                    users[turn][player_number].routePosition = 15;

                }
                if ((users[turn][player_number].lastPosition >= 15 && users[turn][player_number].lastPosition <= 19) || (users[turn][player_number].lastPosition >= 28 && users[turn][player_number].lastPosition <= 29))
                {
                    if ((users[turn][player_number].nowPosition == 19 || users[turn][player_number].nowPosition == 29) && steps >= 1)
                    {
                        users[turn][player_number].routePosition = 30;
                    }
                }

                users[turn][player_number].nextPos = currentRoute.childNodeList[users[turn][player_number].routePosition].position;
                while (MoveToNextNode(users[turn][player_number].nextPos)) { yield return null; }

                //���� ���� ��������
                for (int i = 0; i < 4; i++)
                {
                    if ((users[turn][player_number].routePosition == users[enemy][i].nowPosition)&& steps>1)
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
                steps--;
                users[turn][player_number].nowPosition++;
            }

            //������ �Ա� ����
            //for (int i = 0; i < 4; i++)
            //{
            //    if (users[turn][player_number].nowPosition == users[enemy][i].nowPosition)
            //    {
            //        �̴� ���� ���� ���� ���� ���� ����
            //        Debug.Log("encounter");
            //        reset_player(ref users[enemy][i], objectPrefab[enemy]);
            //        chance++;

            //        //Fpsfight ����
            //        SceneManager.LoadScene("Fpsfight");
            //    }
            //}


            isMoving = false;
            sum = 0;
            Debug.Log("move all");
                  
            //��, �� ������ �� �� ����, �ƴϸ� ����
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
