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
    /*******************************/
    public TextMeshProUGUI Yut;
    public Yut_Field currentRoute;
    public AudioSource[] yutSound;

    public Button[] steps_button;
    public Button yes;
    public Button no;
    public TextMeshProUGUI[] steps_button_Text;

    public List<int> steps = new List<int>();
    private async Task DelayAsync(float seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }
    //bool ����
    bool isMoving;
    bool isBackdo = false;
    private bool chooseBindCalled = false;
    bool isFight = false;
    public bool isYutThrown = false;

    int chance = 0;
    int sum = 0;
    int enemy;
    public GameObject[] objectPrefab;
    int choose_step = 0;
    //������Ʈ �׵θ�
    private int bindedHorseIndex = -1; //�� ���� ���۽� �� ��ȣ ����
    Color original_Edge = Color.white;
    Color highligted_Edge = new Color(255f / 255f, 0f / 255f, 255f / 255f);
    private int selectedButtonIndex = -1;

    user[][] users;

    private void Update()
    {
        users = YutGameManager.Instance.GetUsers();
        MoveScript.GetData(YutGameManager.Instance.GetTurn(), YutGameManager.Instance.GetPlayerNumber(), users);
        choose_Player();
        check_Winner();
        AutoSelectClosestPlayerInArray();
        UIScript.GoalCounter(users);
        UIScript.timer();
    }
    void Start()
    {
        UIScript.StartText(YutGameManager.Instance.GetTurn());
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
        yes.onClick.AddListener(BindYes);
        no.onClick.AddListener(BindNo);

        for (int i = 0; i < steps_button.Length; i++)
        {
            int index = i; // ����: Ŭ���� ������ �ٱ� ������ ���� ���� ������ ���� ������ �� ����
            steps_button[i].onClick.AddListener(() => choose_steps(index));
        }
        //steps_button_Text ����
        for (int i = 0; i < 5; i++)
        {
            steps_button_Text[i] = steps_button[i].GetComponentInChildren<TextMeshProUGUI>();
        }
        for (int i = 0; i < 5; i++)
        {
            yutSound[i].mute = true;
        }
        UIScript.SetOutlineMaterial(Resources.Load<Material>("OutlineMaterial"));
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

            yutSound[sum].mute = false;
            yutSound[sum].Play();
        }
    }
    //�� ������ �Լ�
    private async void UpdateThrowResult(int value, string text)
    {
        if (!isYutThrown)
        {
            steps.Add(value);
            Debug.Log(steps[0]);
            Yut.text = text + "!";
            steps_button_Text[steps.Count - 1].text = text;
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
        UpdateThrowResult(1, "��");
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_back_do()
    {
        Debug.Log("�� ���� : �鵵");
        UpdateThrowResult(1, "�鵵");
        isBackdo = true;
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_gae()
    {
        Debug.Log("�� ���� : ��");
        UpdateThrowResult(2, "��");
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_girl()
    {
        Debug.Log("�� ���� : ��");
        UpdateThrowResult(3, "��");
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_yut()
    {
        Debug.Log("�� ���� : ��");
        UpdateThrowResult(4, "��");
    }
    public void throw_mo()
    {
        Debug.Log("�� ���� : ��");
        UpdateThrowResult(5, "��");
    }
    //�÷��̾� ����
    public void one()
    {
        YutGameManager.Instance.SetPlayerNumber(0);
        check_player();
        UIScript.SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
    }
    public void two()
    {
        YutGameManager.Instance.SetPlayerNumber(1);
        check_player();
        UIScript.SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
    }
    public void three()
    {
        YutGameManager.Instance.SetPlayerNumber(2);
        check_player();
        UIScript.SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
    }
    public void four()
    {
        YutGameManager.Instance.SetPlayerNumber(3);
        check_player();
        UIScript.SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
    }
    public void check_player()
    {
        if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].is_destroyed == true)
        {
            Yut.text = "player already goaled!";
        }
    }
    //�÷��̾� �� ����
    void clear_player(ref user u)
    {
        if (u.player != null)
        {
            Destroy(u.player);
            u.player = null;
        }
    }
    //�÷��̾� �� �����
    public void reset_player(ref user u, GameObject playerPrefab)
    {
        Quaternion prevRotation = u.player.transform.rotation;
        clear_player(ref u);
        u.player = Instantiate(playerPrefab);
        u.routePosition = 0;
        u.nowPosition = 0;
        u.lastPosition = 0;
        u.nextPos = Vector3.zero;
        u.goal = false;
        u.is_destroyed = false;

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
    /*Ű���� �Է����� �� ����
     1~4 : �� ����
    space : ��������*/
    void choose_Player()
    {
        if (steps.Count > 1 && isYutThrown == true)
        {
            if (isMoving)
            {
                Yut.text = "";
            }
            else
            {
                Yut.text = "�󸶳� �̵��� �� �����ϼ���!";
            }
        }
        if (isMoving == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                one();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                two();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                three();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                four();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Return) && isYutThrown == true)
            {
                Debug.Log("move" + YutGameManager.Instance.GetPlayerNumber());
                StartCoroutine(Move(steps[choose_step]));
            }
            else if (Input.GetKeyDown(KeyCode.Space) && isYutThrown == false)
            {
                throwYut();
            }
            //�̵��� steps left/right arrow�� ����            
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
                selectedButtonIndex = choose_step;
                UpdateButtonColors();
                match_Yut(steps[choose_step]);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Yut.text = "";
                if (choose_step < steps.Count - 1)
                {
                    choose_step++;
                }
                Debug.Log("steps=" + steps[choose_step]);
                foreach (int i in steps)
                {
                    match_Yut(i);
                }
                Yut.text = "Available" + Yut.text + "\nyou choose";
                selectedButtonIndex = choose_step;
                UpdateButtonColors();
                match_Yut(steps[choose_step]);
            }
        }
    }
    //��ư�� ��������� ������ �ȵ�
    public void move_Player()
    {
        if (isYutThrown)
        {
            Debug.Log("clicked");
            StartCoroutine(Move(steps[choose_step]));
        }
    }
    void match_Yut(int i)
    {
        switch (i)
        {
            case 1:
                if (isBackdo)
                {
                    Yut.text += " �鵵";
                }
                else
                {
                    Yut.text += " ��";
                }
                break;
            case 2:
                Yut.text += " ��";
                break;
            case 3:
                Yut.text += " ��";
                break;
            case 4:
                Yut.text += " ��";
                break;
            case 5:
                Yut.text += " ��";
                break;
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
    public void choose_steps(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < steps.Count && steps[buttonIndex] != null)
        {
            choose_step = buttonIndex;
            Debug.Log(buttonIndex);
            if (buttonIndex >= 0 && buttonIndex < steps_button.Length)
            {
                selectedButtonIndex = buttonIndex;
                UpdateButtonColors();
            }
        }
    }

    private void UpdateButtonColors()
    {
        for (int i = 0; i < steps_button.Length; i++)
        {
            Image buttonImage = steps_button[i].GetComponent<Image>();
            if (i == selectedButtonIndex)
            {
                buttonImage.color = highligted_Edge;
            }
            else
            {
                buttonImage.color = original_Edge;
            }
        }
    }
    //�� ��ư �ʱ�ȭ
    void clear_stepsButton()
    {
        for (int i = 0; i < 5; i++)
        {
            steps_button_Text[i].text = "";
            steps_button[i].image.color = original_Edge;
        }
    }
    IEnumerator Move(int chosed_step)
    {
        users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].lastPosition = users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition;
        if (YutGameManager.Instance.GetTurn() == 0)
        {
            enemy = 1;
        }
        else
        {
            enemy = 0;
        }
        UIScript.SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
        yield return new WaitForSeconds(1f);
        Yut.text = "";
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;


        while (chosed_step > 0)
        {
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition == 30 && chosed_step > 0)
            {
                Debug.Log("Goal");
                DestroyBindedHorses(YutGameManager.Instance.GetPlayerNumber());
                Destroy(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
                SetUserToGoal(ref users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()]);
                break;
            }

            //users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition = users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition;
            //�鵵 ���� ó��
            if (isBackdo == true)
            {
                Debug.Log("�鵵����ó��");
                int NowpositionSum = 0;
                for (int i = 0; i < Constants.HorseNumber; i++)
                {
                    NowpositionSum += users[YutGameManager.Instance.GetTurn()][i].nowPosition;
                }
                if (NowpositionSum == 0)
                {
                    Yut.text = "�̵��� �� �ִ� ���� �����ϴ�!";
                    yield return new WaitForSeconds(1f);
                    chosed_step = 0;
                    isMoving = false;
                    isBackdo = false;
                    ChangeTurn();
                    steps.RemoveAt(choose_step);
                    yield break;
                }
                else
                {
                    MoveScript.BackdoRoute(); //�鵵�̵�
                }
                isBackdo = false;
            }
            else
            {
                users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition++;
                MoveScript.NormalRoute(chosed_step); //�Ϲ��̵�
            }


            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition < currentRoute.childNodeList.Count)
            {
                users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nextPos = currentRoute.childNodeList[users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition].position;
            }

            while (MoveScript.MoveToNextNode(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nextPos)) { yield return null; }

            chosed_step--;
            users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition++;


            //���� ���� ��������
            //DefenseGameTrigger(chosed_step);


            yield return new WaitForSeconds(0.1f);
            while (isFight == true)
            {
                Yut.text = "";
                yield return new WaitForSeconds(0.1f);
            }
        }

        steps.RemoveAt(choose_step);

        steps_button_Text[choose_step].text = "";
        UpdateYutChoice();

        isMoving = false;
        sum = 0;
        Debug.Log("move all");
        users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition = users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition;
        Debug.Log(YutGameManager.Instance.GetTurn() + "�� �� " + YutGameManager.Instance.GetPlayerNumber() + "��° ���� ���� ��ġ : " + users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition + " ���� ��ġ : " + users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].lastPosition);
        BindHorse();
        FpsfightTrigger();
        if (bindedHorseIndex != -1)
        {
            yield return new WaitUntil(() => chooseBindCalled);
            chooseBindCalled = false;
        }
        SynchronizeBindedHorses(YutGameManager.Instance.GetPlayerNumber());

        if (chance == 0 && steps.Count() == 0)
        {
            ChangeTurn();
        }
        else if (chance > 0) { chance--; }
        //�� ���� ���� �̵� �׽�Ʈ ��
        //if (steps.Count == 0)
        //{
        //    isYutThrown = false;
        //    choose_step = 0;
        //}
    }
    //�̵� ������ ������Ʈ
    private void UpdateYutChoice()
    {
        for (int i = choose_step; i < steps.Count; i++)
        {
            steps_button_Text[i].text = steps_button_Text[i + 1].text;
        }
        if (steps.Count < steps_button_Text.Length)
        {
            steps_button_Text[steps.Count].text = "";
        }
        if (choose_step == steps.Count)
        {
            choose_step--;
        }
        foreach (int i in steps)
        {
            Debug.Log(i);
        }
    }
    //Fps Fight Ʈ����
    private void FpsfightTrigger()
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition == users[enemy][i].nowPosition)
            {
                //�̴� ���� ���� ���� ���� ���� ����
                Debug.Log("encounter");
                chance += (users[enemy][i].BindedHorse.Count + 1);
                reset_player(ref users[enemy][i], objectPrefab[enemy]);
                Yut.text = chance + " ���� ��ȸ�� �߰� ȹ��!";
                isYutThrown = false;
                choose_step = 0;
                //Fpsfight ����
                //SceneManager.LoadScene("Fpsfight");
            }
        }
    }
    //DefenseGame Ʈ����
    private IEnumerator DefenseGameTrigger(int chosed_step)
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition == users[enemy][i].nowPosition)
            {
                if (chosed_step > 0)
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
            choose_step = 0;
            isYutThrown = false;
            clear_stepsButton();
            UIScript.timeLeft = 60.0f;
        }
        else if (YutGameManager.Instance.GetTurn() == 1)
        {
            YutGameManager.Instance.SetTurn(0);
            choose_step = 0;
            isYutThrown = false;
            clear_stepsButton();
            UIScript.timeLeft = 60.0f;
        }
        YutGameManager.Instance.SetPlayerNumber(0);
        Yut.text = "player " + (YutGameManager.Instance.GetTurn() + 1) + " turn!";
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
                Yut.text = "���� �����ðڽ��ϱ�?";
                bindedHorseIndex = i;
                yes.gameObject.SetActive(true);
                no.gameObject.SetActive(true);
            }
        }
    }

    //���� ���ý� BindedHorse, is_bind ������Ʈ
    private void BindYes()
    {
        List<int> overlappedHorses = GetBindedHorses(YutGameManager.Instance.GetPlayerNumber());
        if (bindedHorseIndex < 0) return;

        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);

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
    private void BindNo()
    {
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
        bindedHorseIndex = -1;
        chooseBindCalled = true;
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
            users[YutGameManager.Instance.GetTurn()][bindedIndex].is_destroyed = true;
            SetUserToGoal(ref users[YutGameManager.Instance.GetTurn()][bindedIndex]);
        }
    }
    //������ ���鿡 ���� ���� �ʱ�ȭ
    public void SetUserToGoal(ref user u)
    {
        u.nextPos = Vector3.zero; // ���� ��ġ�� �ʿ� ����
        u.goal = true;
        u.is_destroyed = true;
        u.is_bind = false;
        u.BindedHorse?.Clear();
    }

    List<int> GetBindedHorses(int currentPlayer)
    {
        List<int> overlappedHorses = new List<int>();
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (currentPlayer == i) continue;
            if (users[YutGameManager.Instance.GetTurn()][currentPlayer].nowPosition == users[YutGameManager.Instance.GetTurn()][i].nowPosition &&
                users[YutGameManager.Instance.GetTurn()][i].goal != true)
            {
                overlappedHorses.Add(i);
            }
        }
        return overlappedHorses;
    }
    void AdjustPositionByStacking(int currentPlayer, List<int> overlappedHorses)
    {
        Vector3 basePosition = users[YutGameManager.Instance.GetTurn()][currentPlayer].player.transform.position;
        for (int i = 0; i < overlappedHorses.Count; i++)
        {
            users[YutGameManager.Instance.GetTurn()][overlappedHorses[i]].player.transform.position = basePosition + new Vector3(0, Constants.STACK_HEIGHT * (i + 1), 0);
        }
    }
}