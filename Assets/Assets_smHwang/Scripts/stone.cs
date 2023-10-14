using Google.Protobuf.Protocol;
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
    public bool isFight = false;
    public bool isYutThrown = false;
    bool shownDestination = false;
    int chance = 0;
    int sum = 0;
    int enemy;
    public GameObject[] objectPrefab;
    //������Ʈ �׵θ�
    private int bindedHorseIndex = -1; //�� ���� ���۽� �� ��ȣ ����

    user[] users;
    horse horses;
    private void Update()
    {
        users = YutGameManager.Instance.GetUsers();
        horses = YutGameManager.Instance.GetNowHorse();
        MoveScript.GetData(YutGameManager.Instance.GetTurn(), YutGameManager.Instance.GetPlayerNumber(), YutGameManager.Instance.GetUsers());
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
    //�������� ���� �Լ� - ��ư�� ����
    public void throwYut()
    {
        if (!isYutThrown)
        {
            C_ThrowYut throwYutPacket = new C_ThrowYut();
            Managers.Network.Send(throwYutPacket);
        }
    }

    public void HandleThrowYut(YutResult result)
    {
        sum = convertYutResult(result);
        Debug.Log(result);
        Debug.Log(sum);

        switch (sum)
        {
            case -1:
                throw_back_do();
                break;
            case 0:
                throw_mo();
                break;
            case 1:
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
            case 5:
                throw_nak();
                break;
        }
        int RandomSound = UnityEngine.Random.Range(0, 5);
        yutSound[RandomSound].mute = false;
        yutSound[RandomSound].Play();

        ShowDestination.Invoke();
    }

    private int convertYutResult(YutResult result)
    {
        switch (result)
        {
            case YutResult.Backdo:
                return -1;
            case YutResult.Mo:
                return 0;
            case YutResult.Do:
                return 1;
            case YutResult.Gae:
                return 2;
            case YutResult.Geol:
                return 3;
            case YutResult.Yut:
                return 4;
            case YutResult.Nak:
                return 5;
            default:
                Debug.Log("������Ȳ�̿�");
                return 5;
        }
    }

    //�� ���� ������
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
        UpdateThrowResult(-1);
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
    
    public void throw_nak()
    {
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    //�� ��� ���� �Լ�
    private async void UpdateThrowResult(int value)
    {
        if (!isYutThrown)
        {
            UIScript.SetSteps(value); //steps����Ʈ�� �߰�(UI.cs)
            if (value == 4 || value == 5) //��, ��� �� ������ �Լ� ��ȣ��
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
        if (horses.goal == true)
        {
            Yut.text = "player already goaled!";
        }
    }
    //�÷��̾� �� �����
    public void reset_player(horse u, GameObject playerPrefab)
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
                    reset_player(users[YutGameManager.Instance.GetTurn()].horses[bindedHorseIndex], playerPrefab); // ��������� ȣ��
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
        if (!isMoving) //���� �������� �ʰ�
        {
            if (isYutThrown) //���� �� ��������
            {
                ShowDestination.Invoke(); //�̵� ���� ��ġ ǥ��(UI.cs�� DestinationUI)
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) //1
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                one();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) //2
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                two();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) //3
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                three();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4)) //4
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                four();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            //���� �Է� �� Move �ڷ�ƾ ����
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("enter move" + YutGameManager.Instance.GetPlayerNumber());
                move_Player();
                //StartCoroutine(Move(UIScript.GetStep()));
            }
            //�����̽�, �� ������
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                throwYut();
            }
            //bool showDestination�� false�̸� ������ ������ ���. showDestinaiton�� �� ������ �ٲܶ����� false
            if (!shownDestination)
            {
                ShowDestination.Invoke(); //�̵� ���� ��ġ ǥ��(UI.cs�� DestinationUI)
                shownDestination = true;
            }
        }
    }
    //��ư�� ��������� ������ �ȵ�
    public void move_Player()
    {
        if (isYutThrown)
        {
            horse horses = YutGameManager.Instance.GetNowHorse();

        C_YutMove yutmovePacket = new C_YutMove();
        yutmovePacket.UseResult = UIScript.choose_step;
        yutmovePacket.MovedYut = YutGameManager.Instance.GetPlayerNumber();
        yutmovePacket.MovedPos = horses.FinalPosition[UIScript.choose_step];
        Managers.Network.Send(yutmovePacket);

        Debug.Log("step : " + yutmovePacket.UseResult);
        Debug.Log("Horse num :  " + yutmovePacket.MovedYut);
        Debug.Log("destination : " + yutmovePacket.MovedPos);

        //if (isYutThrown)
        //{
        //    Debug.Log("move");
        //    StartCoroutine(Move(UIScript.GetStep()));
        //}
         handleMovePlayer(); }
    }

    public void handleMovePlayer()
    {
        if (isYutThrown)
        {
            Debug.Log("step : " + UIScript.choose_step);
            Debug.Log("Horse num :  " + YutGameManager.Instance.GetPlayerNumber());
            Debug.Log("destination : " + horses.FinalPosition[UIScript.choose_step]);

            Debug.Log("move");
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
            if (users[0].horses[i].player != null)
            {
                redTeamAllNull = false;
            }
            if (users[1].horses[i].player != null)
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
        if (users[YutGameManager.Instance.GetTurn()].horses[YutGameManager.Instance.GetPlayerNumber()].player == null)
        {
            int closestPlayerNumber = -1;

            for (int i = 0; i < Constants.HorseNumber; i++)
            {
                int nextPlayerNumber = (YutGameManager.Instance.GetPlayerNumber() + i) % Constants.HorseNumber;

                if (users[YutGameManager.Instance.GetTurn()].horses[nextPlayerNumber] != null &&
                    users[YutGameManager.Instance.GetTurn()].horses[nextPlayerNumber].goal != true)
                {
                    closestPlayerNumber = nextPlayerNumber;
                    break;
                }
            }

            if (closestPlayerNumber != -1)
            {
                YutGameManager.Instance.SetPlayerNumber(closestPlayerNumber);
                Debug.Log("close:"+ closestPlayerNumber);
                UIScript.SetOutline(users[YutGameManager.Instance.GetTurn()].horses[YutGameManager.Instance.GetPlayerNumber()].player);
            }
        }
    }
    //�̵� ������ ��� ��
    IEnumerator Move(int LeftStep)
    {
        int turn = YutGameManager.Instance.GetTurn();
        int player_number = YutGameManager.Instance.GetPlayerNumber();
        horse nowUser = YutGameManager.Instance.GetNowHorse();
        //�Ͽ� ���� �� ����, 3�� �̻� �����ϰ� �ҰŸ� ���� �ʿ�
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
        //�̵�, step�� �����ִ� ����
        while (LeftStep != 0)
        {
            //�������� 30(������)�̰�, �� ���Ŀ��� �̵������ϸ� ������Ʈ �ı�, ���� ó�� �� �̵� Ż��
            if (nowUser.routePosition == 30 && LeftStep > 0)
            {
                Debug.Log("Goal");
                DestroyBindedHorses(player_number);
                Destroy(nowUser.player);
                SetUserToGoal(nowUser);
                break;
            }

            //�鵵 ���� ó��
            if (UIScript.GetStep()== -1)
            {
                Debug.Log("�鵵����ó��");
                int NowpositionSum = 0;
                //������ ��� ���� ���� ��ġ�� ���ص� 0�̸� == �ʵ����� ���� ������
                for (int i = 0; i < Constants.HorseNumber; i++)
                {
                    NowpositionSum += users[turn].horses[i].nowPosition;
                }
                if (NowpositionSum == 0)
                {
                    Yut.text = "�̵��� �� �ִ� ���� �����ϴ�!";
                    yield return new WaitForSeconds(1f);
                    LeftStep = 0;
                    isMoving = false;
                    ChangeTurn();
                    yield break;
                }
                //�ʵ忡 ���� ������ BackdoRoute
                else
                {
                    int BackdoRoute = MoveScript.BackdoRoute(); //�鵵�̵�
                    nowUser.routePosition = BackdoRoute;
                }
            }
            //NormalRoute
            else
            {
                nowUser.routePosition++; //������++
                int NormalRoute = MoveScript.NormalRoute(nowUser.nowPosition, nowUser.lastPosition); //���� ������ ���
                //NormalRoute�� -1�� �ƴϸ� == Ư�� ��ġ��(�ڳ�, �߾�) routePosition�� ����� NormalRoute �� �Ҵ�
                if (NormalRoute != -1)
                {
                    nowUser.routePosition = NormalRoute;
                    Debug.Log("routePosition : " + nowUser.routePosition);
                    nowUser.nowPosition = NormalRoute;
                    Debug.Log("nowPosition : " + nowUser.nowPosition);
                }
                //-1�̸� == ����� �̵��̸� �׳� ���� ��ġ++
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
            //�� nextPos�� �̵�
            while (MoveScript.MoveToNextNode(nowUser.nextPos)) { yield return null; }
            if (LeftStep > 0)
            {
                LeftStep--;
            }
            else if (LeftStep == -1)
            {
                LeftStep++;
            }
            //�̵� �� ���� ���� ��������
            //DefenseGameTrigger(LeftStep);

            yield return new WaitForSeconds(0.1f);
            //FpsFight ���� ���̸�
            while (isFight == true)
            {
                Yut.text = "";
                yield return new WaitForSeconds(0.1f);
            }
        }
        //LeftStep == 0 �Ǽ� ���°Ŷ� �ϳ��� �̵� ��
        UpdateSteps.Invoke(); //step ������Ʈ(UIcs _UpdateSteps())
        isMoving = false; //�̵� �������� bool ������ ����
        Debug.Log("move all");
        nowUser.nowPosition = nowUser.routePosition; //�̵��ϸ鼭 �������� ����������, ������ġ==������
        Debug.Log(turn + "�� �� " + player_number + "��° ���� ���� ��ġ : " + nowUser.nowPosition + " ���� ��ġ : " + nowUser.lastPosition);
        nowUser.lastPosition = nowUser.nowPosition; //������ġ==������ġ
        BindHorse(); //�� ����
        if (nowUser.nowPosition == 22) { nowUser.nowPosition = 27; } //���� �� �� ����
        SynchronizeBindedHorses(player_number); //���� ���� ���� ����
        FpsfightTrigger(); //���� ��ġ ������ Fpsfight ������ ����
        //�� ������ ���� üũ
        check_Winner();
        //���� 0�̰�, ���ܿ� ���� ������ �̵� ������ �� ����
        if (chance == 0 && UIScript.steps.Count() == 0)
        {
            ChangeTurn();
        }
        //�ƴϸ� ����--, �̵������� ��ġ �ٽ� ǥ��
        else if (chance > 0) { chance--; ShowDestination.Invoke(); }
    }

    //Fps Fight Ʈ����
    private void FpsfightTrigger()
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (horses.nowPosition == users[enemy].horses[i].nowPosition)
            {
                if (!users[enemy].horses[i].goal)
                {
                    //��Ű��
                    UIScript.TurnOnFire();
                    //�̴� ���� ���� ���� ���� ���� ����
                    Debug.Log("encounter");
                    //�����ִ� �� ������ŭ �߰��� ����ȹ��.
                    chance += (users[enemy].horses[i].BindedHorse.Count + 1);
                    //���� ����
                    reset_player(users[enemy].horses[i], objectPrefab[users[enemy].turn]);
                    Yut.text = chance + " ���� ��ȸ�� �߰� ȹ��!";
                    UIScript.choose_step = 0;
                    //�� �ٽ� ������ ���� bool ���� ����
                    isYutThrown = false;
                    //Fpsfight ����
                    //SceneManager.LoadScene("Fpsfight");
                    StartCoroutine(UIScript.TurnOffFire());
                }
            }
        }
    }
    //DefenseGame Ʈ����
    private void DefenseGameTrigger(int LeftStep)
    {
        Debug.Log("defensegame");
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            //�̵� ���� ��ο� ��밡 �����ϰ�
            if (horses.routePosition == users[enemy].horses[i].nowPosition)
            {
                //���� ���� ���ų� ������
                if (horses.BindedHorse.Count <= users[enemy].horses[i].BindedHorse.Count)
                {
                    //�̵� �����ϸ�
                    if (LeftStep > 0)
                    {
                        Yut.text = "To pass, Win!";
                        //yield return new WaitForSeconds(1f
                        StartCoroutine(LoadDefenseGameSceneAfterDelay(1f));
                        SceneManager.LoadScene("Defense_Game");
                    }
                }
            }
        }
    }
    private IEnumerator LoadDefenseGameSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
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
        //changeTurn�׼�(UI.cs�� ChangeTurnUI)
        OnChangeTurnAction.Invoke();     
    }
    //�� ����
    private void BindHorse()
    {
        Debug.Log("�� ���� ����");
        Debug.Log("���� �� : " + YutGameManager.Instance.GetPlayerNumber());
        //�ڱ� ���� ��ġ ������ �˻�
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (YutGameManager.Instance.GetPlayerNumber() == i) continue;//�ڱ� �ڽ� �ǳʶ�
            if (horses.nowPosition == users[YutGameManager.Instance.GetTurn()].horses[i].nowPosition &&
                horses.goal != true)
            {
                Debug.Log(YutGameManager.Instance.GetTurn() + "�� ���� ���� ��ġ : " + horses.nowPosition);
                Debug.Log(YutGameManager.Instance.GetTurn() + "�� ��ġ�� ���� ��ġ : " + horses.nowPosition);
                bindedHorseIndex = i;
                List<int> overlappedHorses = new List<int>();
                for (int j = 0; j < Constants.HorseNumber; j++)
                {
                    int currentPlayer = YutGameManager.Instance.GetPlayerNumber();
                    if (currentPlayer == j) continue;
                    if (horses.nowPosition == users[YutGameManager.Instance.GetTurn()].horses[j].nowPosition &&
                        users[YutGameManager.Instance.GetTurn()].horses[j].goal != true)
                    {
                        overlappedHorses.Add(j);
                    }
                }
                if (bindedHorseIndex < 0) return;

                // ���ε��Ǿ� �ִ� ��� ������ ����Ʈ ����
                List<int> allBindedHorses = new List<int>();
                allBindedHorses.Add(YutGameManager.Instance.GetPlayerNumber());
                allBindedHorses.AddRange(horses.BindedHorse);
                allBindedHorses.Add(bindedHorseIndex);
                allBindedHorses.AddRange(users[YutGameManager.Instance.GetTurn()].horses[bindedHorseIndex].BindedHorse);

                // �ߺ� �׸� ����
                allBindedHorses = allBindedHorses.Distinct().ToList();

                // ��� ���õ� ���鿡 ���ε� ���� ������Ʈ
                foreach (int horseIndex in allBindedHorses)
                {
                    users[YutGameManager.Instance.GetTurn()].horses[horseIndex].is_bind = true;
                    users[YutGameManager.Instance.GetTurn()].horses[horseIndex].BindedHorse = new List<int>(allBindedHorses);
                    users[YutGameManager.Instance.GetTurn()].horses[horseIndex].BindedHorse.Remove(horseIndex); // �ڱ� �ڽ��� ����Ʈ���� ����
                }
                bindedHorseIndex = -1;
                //�� ������Ʈ �ױ�
                AdjustPositionByStacking(YutGameManager.Instance.GetPlayerNumber(), overlappedHorses);
            }
        }
    }


    //���� �� �̵��� ���� ���� ���� ����ȭ
    public void SynchronizeBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = horses.BindedHorse;

        horse mainHorse = users[YutGameManager.Instance.GetTurn()].horses[mainHorseIndex];

        foreach (int bindedIndex in bindedHorses)
        {
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].routePosition = mainHorse.routePosition;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].nowPosition = mainHorse.nowPosition;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].lastPosition = mainHorse.lastPosition;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].nextPos = mainHorse.nextPos;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].goal = mainHorse.goal;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].BindedHorse = mainHorse.BindedHorse;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].is_bind = mainHorse.is_bind;
        }
    }
    //���� ���� ���� �ı�
    public void DestroyBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = users[YutGameManager.Instance.GetTurn()].horses[mainHorseIndex].BindedHorse;

        foreach (int bindedIndex in bindedHorses)
        {
            Destroy(users[YutGameManager.Instance.GetTurn()].horses[bindedHorseIndex].player);
            SetUserToGoal(users[YutGameManager.Instance.GetTurn()].horses[bindedIndex]);
        }
    }
    //������ ���鿡 ���� ���� �ʱ�ȭ
    public void SetUserToGoal(horse u)
    {
        u.nextPos = Vector3.zero; // ���� ��ġ�� �ʿ� ����
        u.goal = true;
        u.is_bind = false;
        u.BindedHorse?.Clear();
    }
    //�� ���� �� ��ġ ����
    void AdjustPositionByStacking(int currentPlayer, List<int> overlappedHorses)
    {
        Vector3 basePosition = horses.player.transform.position;
        for (int i = 0; i < overlappedHorses.Count; i++)
        {
            users[YutGameManager.Instance.GetTurn()].horses[overlappedHorses[i]].player.transform.position = basePosition + new Vector3(0, Constants.STACK_HEIGHT * (i + 1), 0);
        }
    }
}