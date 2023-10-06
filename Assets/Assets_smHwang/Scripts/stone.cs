using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Threading.Tasks;
using System.Linq;

public class stone : MonoBehaviour
{
    /************��ũ��Ʈ***********/
    public UI UIScript;
    public MoveScript MoveScript;
    public setting SettingScript;
    /**************�׼�**************/
    public Action OnChangeTurnAction;
    public Action ShowDestination;
    public Action UpdateSteps;
    /*******************************/

    public TextMeshProUGUI Yut;
    public Yut_Field currentRoute;
    public AudioSource[] yutSound;

    private async Task DelayAsync(float seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }
    //bool ����
    public bool isMoving;
    public bool isBackdo = false;
    private bool chooseBindCalled = false;
    public bool isFight = false;
    public bool isYutThrown = false;
    bool shownDestination = false;
    int chance = 0;
    int sum = 0;
    int enemy;
    public GameObject[] objectPrefab;
    //������Ʈ �׵θ�
    private int bindedHorseIndex = -1; //�� ���� ���۽� �� ��ȣ ����

    user[][] users;

    private void Update()
    {
        users = YutGameManager.Instance.GetUsers();
        MoveScript.GetData(YutGameManager.Instance.GetTurn(), YutGameManager.Instance.GetPlayerNumber(), users);
        if (isYutThrown) { choose_Player(); }
        AutoSelectClosestPlayerInArray();
    }
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            yutSound[i].mute = true;
        }
        //UIScript.SetOutlineMaterial(Resources.Load<Material>("OutlineMaterial"));
    }
    //�������� ���� �Լ�
    public void throwYut()
    {
        if (!isYutThrown)
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
            int RandomSound = UnityEngine.Random.Range(0, 5);
            yutSound[RandomSound].mute = false;
            yutSound[RandomSound].Play();
        }
        ShowDestination.Invoke();
    }
    public bool GetBackdo()
    {
        return isBackdo;
    }
    //�� ������ �Լ�
    private async void UpdateThrowResult(int value)
    {
        if (!isYutThrown)
        {
            Debug.Log("before : " + isBackdo);
            UIScript.SetSteps(value, isBackdo);
            if (value == 4 || value == 5)
            {
                await DelayAsync(0.5f);
                Yut.text = "�� �� ��!";
                sum = 0;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    throwYut();
                }
            }
        }
    }
    //�����ڿ� ���� ������
    public void throw_do()
    {
        Debug.Log("�� ���� : ��");
        UpdateThrowResult(1);
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_back_do()
    {
        Debug.Log("�� ���� : �鵵");
        isBackdo = true;
        UpdateThrowResult(1);
        Debug.Log("stone : " + isBackdo);
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_gae()
    {
        Debug.Log("�� ���� : ��");
        UpdateThrowResult(2);
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_girl()
    {
        Debug.Log("�� ���� : ��");
        UpdateThrowResult(3);
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_yut()
    {
        Debug.Log("�� ���� : ��");
        UpdateThrowResult(4);
    }
    public void throw_mo()
    {
        Debug.Log("�� ���� : ��");
        UpdateThrowResult(5);
    }
    //�÷��̾� ����
    public void one()
    {
        YutGameManager.Instance.SetPlayerNumber(0);
        check_player();
    }
    public void two()
    {
        YutGameManager.Instance.SetPlayerNumber(1);
        check_player();
    }
    public void three()
    {
        YutGameManager.Instance.SetPlayerNumber(2);
        check_player();
    }
    public void four()
    {
        YutGameManager.Instance.SetPlayerNumber(3);
        check_player();
    }
    public void check_player()
    {
        if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].goal == true)
        {
            Yut.text = "player already goaled!";
        }
    }
    //�÷��̾� �� �����
    public void reset_player(ref user u, GameObject playerPrefab)
    {
        if (!u.goal)
        {
            Quaternion prevRotation = u.player.transform.rotation;
            Destroy(u.player);
            u.player = Instantiate(playerPrefab);
            u.routePosition = 0;
            u.nowPosition = 0;
            u.lastPosition = 0;
            u.nextPos = Vector3.zero;
            u.goal = false;

            if (u.is_bind)
            {
                u.is_bind = false;
                foreach (int bindedHorseIndex in u.BindedHorse)
                {
                    reset_player(ref users[enemy][bindedHorseIndex], playerPrefab); // ��������� ȣ��
                }
                u.BindedHorse.Clear();
            }
            u.player.transform.position = u.player_start_position;
        }
    }
    /*Ű���� �Է����� �� ����
     1~4 : �� ����
    space : ��������*/
    void choose_Player()
    {
        if (!isMoving)
        {
            if (isYutThrown)
            {
                user user = YutGameManager.Instance.GetNowUser();
                user.FinalPosition.Clear();
                UIScript.CalculateDestination(isBackdo);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                one();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                two();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                three();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                four();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Return) && isYutThrown == true)
            {
                Debug.Log("move" + YutGameManager.Instance.GetPlayerNumber());
                StartCoroutine(Move(UIScript.GetStep()));
            }
            else if (Input.GetKeyDown(KeyCode.Space) && isYutThrown == false)
            {
                throwYut();
            }
            if (!shownDestination)
            {
                UIScript.CalculateDestination(isBackdo);
                UIScript.ShowDestination();
                shownDestination = true;
            }
        }
    }
    //��ư�� ��������� ������ �ȵ�
    public void move_Player()
    {
        if (isYutThrown)
        {
            Debug.Log("clicked");
            StartCoroutine(Move(UIScript.GetStep()));
        }
    }

    //�� ���� ��� �÷��̾� ������Ʈ�� null�� �� �¸� ����
    void check_Winner()
    {
        bool redTeamAllNull = true;
        bool blueTeamAllNull = true;

        for (int i = 0; i < Constants.HorseNumber; i++)
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
        if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player == null)
        {
            int closestPlayerNumber = -1;

            for (int i = 0; i < Constants.HorseNumber; i++)
            {
                int nextPlayerNumber = (YutGameManager.Instance.GetPlayerNumber() + i) % Constants.HorseNumber;

                if (users[YutGameManager.Instance.GetTurn()][nextPlayerNumber].player != null)
                {
                    closestPlayerNumber = nextPlayerNumber;
                    break;
                }
            }

            if (closestPlayerNumber != -1)
            {
                YutGameManager.Instance.SetPlayerNumber(closestPlayerNumber);
                UIScript.SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
            }
        }
    }
    IEnumerator Move(int LeftStep)
    {
        int turn = YutGameManager.Instance.GetTurn();
        int player_number = YutGameManager.Instance.GetPlayerNumber();
        user nowUser = YutGameManager.Instance.GetNowUser();
        if (turn == 0)
        {
            enemy = 1;
        }
        else
        {
            enemy = 0;
        }
        yield return new WaitForSeconds(1f);
        Yut.text = "";
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;


        while (LeftStep > 0)
        {
            if (nowUser.routePosition == 30 && LeftStep > 0)
            {
                Debug.Log("Goal");
                DestroyBindedHorses(player_number);
                Destroy(nowUser.player);
                SetUserToGoal(ref nowUser);
                break;
            }

            //nowUser.nowPosition = nowUser.routePosition;
            //�鵵 ���� ó��
            if (isBackdo == true && UIScript.GetStep() == 1)
            {
                Debug.Log("�鵵����ó��");
                int NowpositionSum = 0;
                for (int i = 0; i < Constants.HorseNumber; i++)
                {
                    NowpositionSum += users[turn][i].nowPosition;
                }
                if (NowpositionSum == 0)
                {
                    Yut.text = "�̵��� �� �ִ� ���� �����ϴ�!";
                    yield return new WaitForSeconds(1f);
                    LeftStep = 0;
                    isMoving = false;
                    isBackdo = false;
                    ChangeTurn();
                    yield break;
                }
                else
                {
                    int BackdoRoute = MoveScript.BackdoRoute(); //�鵵�̵�
                    nowUser.routePosition = BackdoRoute;
                }
                isBackdo = false;
            }
            //NormalRoute
            else
            {
                nowUser.routePosition++;
                int NormalRoute = MoveScript.NormalRoute(nowUser.nowPosition, nowUser.lastPosition);
                if (NormalRoute != -1)
                {
                    nowUser.routePosition = NormalRoute;
                    Debug.Log("routePosition : " + nowUser.routePosition);
                    nowUser.nowPosition = NormalRoute;
                    Debug.Log("nowPosition : " + nowUser.nowPosition);
                }
                else
                {
                    nowUser.nowPosition++;
                    Debug.Log("nowPosition : " + nowUser.nowPosition);
                }
            }

            if (nowUser.routePosition < currentRoute.childNodeList.Count)
            {
                nowUser.nextPos = currentRoute.childNodeList[nowUser.routePosition].position;
            }

            while (MoveScript.MoveToNextNode(nowUser.nextPos)) { yield return null; }

            LeftStep--;

            //���� ���� ��������
            //DefenseGameTrigger(LeftStep);

            yield return new WaitForSeconds(0.1f);
            while (isFight == true)
            {
                Yut.text = "";
                yield return new WaitForSeconds(0.1f);
            }
        }

        UpdateSteps.Invoke();
        isMoving = false;
        sum = 0;
        Debug.Log("move all");
        nowUser.nowPosition = nowUser.routePosition;
        Debug.Log(turn + "�� �� " + player_number + "��° ���� ���� ��ġ : " + nowUser.nowPosition + " ���� ��ġ : " + nowUser.lastPosition);
        nowUser.lastPosition = nowUser.nowPosition;
        BindHorse();
        FpsfightTrigger();
        if (bindedHorseIndex != -1)
        {
            yield return new WaitUntil(() => chooseBindCalled);
            chooseBindCalled = false;
        }
        SynchronizeBindedHorses(player_number);
        //�� ������ ���� üũ
        check_Winner();

        if (chance == 0 && UIScript.steps.Count() == 0)
        {
            ChangeTurn();
        }
        else if (chance > 0) { chance--; }
        ShowDestination.Invoke();
    }

    //Fps Fight Ʈ����
    private void FpsfightTrigger()
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition == users[enemy][i].nowPosition)
            {
                if (!users[enemy][i].goal)
                {
                    //�̴� ���� ���� ���� ���� ���� ����
                    Debug.Log("encounter");
                    chance += (users[enemy][i].BindedHorse.Count + 1);
                    reset_player(ref users[enemy][i], objectPrefab[enemy]);
                    Yut.text = chance + " ���� ��ȸ�� �߰� ȹ��!";
                    UIScript.choose_step = 0;
                    isYutThrown = false;
                    //Fpsfight ����
                    //SceneManager.LoadScene("Fpsfight");
                }
            }
        }
    }
    //DefenseGame Ʈ����
    private IEnumerator DefenseGameTrigger(int LeftStep)
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition == users[enemy][i].nowPosition)
            {
                if (LeftStep > 0)
                {
                    Yut.text = "To pass, Win!";
                    yield return new WaitForSeconds(1f);
                    SceneManager.LoadScene("Defense_Game");
                }
            }
        }
    }
    public void ChangeTurn()
    {
        if (YutGameManager.Instance.GetTurn() == 0)
        {
            YutGameManager.Instance.SetTurn(1);
            isYutThrown = false;
        }
        else if (YutGameManager.Instance.GetTurn() == 1)
        {
            YutGameManager.Instance.SetTurn(0);
            isYutThrown = false;
        }
        YutGameManager.Instance.SetPlayerNumber(0);
        Yut.text = "player " + (YutGameManager.Instance.GetTurn() + 1) + " turn!";
        OnChangeTurnAction.Invoke();     
    }
    private void BindHorse()
    {
        Debug.Log("�� ���� ����");
        Debug.Log("���� �� : " + YutGameManager.Instance.GetPlayerNumber());
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (YutGameManager.Instance.GetPlayerNumber() == i) continue;
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition == users[YutGameManager.Instance.GetTurn()][i].nowPosition &&
                users[YutGameManager.Instance.GetTurn()][i].goal != true)
            {
                Debug.Log(YutGameManager.Instance.GetTurn() + "�� ���� ���� ��ġ : " + users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition);
                Debug.Log(YutGameManager.Instance.GetTurn() + "�� ��ġ�� ���� ��ġ : " + users[YutGameManager.Instance.GetTurn()][i].nowPosition);
                bindedHorseIndex = i;
                List<int> overlappedHorses =new List<int>();
                for (int j = 0; j < Constants.HorseNumber; j++)
                {
                    int currentPlayer = YutGameManager.Instance.GetPlayerNumber();
                    if (currentPlayer == j) continue;
                    if (users[YutGameManager.Instance.GetTurn()][currentPlayer].nowPosition == users[YutGameManager.Instance.GetTurn()][j].nowPosition &&
                        users[YutGameManager.Instance.GetTurn()][j].goal != true)
                    {
                        overlappedHorses.Add(j);
                    }
                }
                if (bindedHorseIndex < 0) return;

                // ���ε��Ǿ� �ִ� ��� ������ ����Ʈ ����
                List<int> allBindedHorses = new List<int>();
                allBindedHorses.Add(YutGameManager.Instance.GetPlayerNumber());
                allBindedHorses.AddRange(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].BindedHorse);
                allBindedHorses.Add(bindedHorseIndex);
                allBindedHorses.AddRange(users[YutGameManager.Instance.GetTurn()][bindedHorseIndex].BindedHorse);

                // �ߺ� �׸� ����
                allBindedHorses = allBindedHorses.Distinct().ToList();

                // ��� ���õ� ���鿡 ���ε� ���� ������Ʈ
                foreach (int horseIndex in allBindedHorses)
                {
                    users[YutGameManager.Instance.GetTurn()][horseIndex].is_bind = true;
                    users[YutGameManager.Instance.GetTurn()][horseIndex].BindedHorse = new List<int>(allBindedHorses);
                    users[YutGameManager.Instance.GetTurn()][horseIndex].BindedHorse.Remove(horseIndex); // �ڱ� �ڽ��� ����Ʈ���� ����
                }
                bindedHorseIndex = -1;
                AdjustPositionByStacking(YutGameManager.Instance.GetPlayerNumber(), overlappedHorses);
                chooseBindCalled = true;
            }
        }
    }

    //���� �� �̵��� ���� ���� ���� ����ȭ
    public void SynchronizeBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = users[YutGameManager.Instance.GetTurn()][mainHorseIndex].BindedHorse;

        user mainHorse = users[YutGameManager.Instance.GetTurn()][mainHorseIndex];

        foreach (int bindedIndex in bindedHorses)
        {
            users[YutGameManager.Instance.GetTurn()][bindedIndex].routePosition = mainHorse.routePosition;
            users[YutGameManager.Instance.GetTurn()][bindedIndex].nowPosition = mainHorse.nowPosition;
            users[YutGameManager.Instance.GetTurn()][bindedIndex].lastPosition = mainHorse.lastPosition;
            users[YutGameManager.Instance.GetTurn()][bindedIndex].nextPos = mainHorse.nextPos;
            users[YutGameManager.Instance.GetTurn()][bindedIndex].goal = mainHorse.goal;
        }
    }
    //���� ���� ���� �ı�
    public void DestroyBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = users[YutGameManager.Instance.GetTurn()][mainHorseIndex].BindedHorse;

        foreach (int bindedIndex in bindedHorses)
        {
            Destroy(users[YutGameManager.Instance.GetTurn()][bindedIndex].player);
            SetUserToGoal(ref users[YutGameManager.Instance.GetTurn()][bindedIndex]);
        }
    }
    //������ ���鿡 ���� ���� �ʱ�ȭ
    public void SetUserToGoal(ref user u)
    {
        u.nextPos = Vector3.zero; // ���� ��ġ�� �ʿ� ����
        u.goal = true;
        u.is_bind = false;
        u.BindedHorse?.Clear();
    }
    //�� ���� �� ��ġ ����
    void AdjustPositionByStacking(int currentPlayer, List<int> overlappedHorses)
    {
        Vector3 basePosition = users[YutGameManager.Instance.GetTurn()][currentPlayer].player.transform.position;
        for (int i = 0; i < overlappedHorses.Count; i++)
        {
            users[YutGameManager.Instance.GetTurn()][overlappedHorses[i]].player.transform.position = basePosition + new Vector3(0, Constants.STACK_HEIGHT * (i + 1), 0);
        }
    }
}