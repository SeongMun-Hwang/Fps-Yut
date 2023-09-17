using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Threading.Tasks;
using System.Linq;

static class Constants
{
    public const int PlayerNumber = 2;
    public const int HorseNumber = 4;
    public const float STACK_HEIGHT = 1f;
}

public class stone : MonoBehaviour
{
    public Yut_Field currentRoute;
    public AudioSource[] yutSound;
    public TextMeshProUGUI Yut;
    public GameObject[] red_team;
    public GameObject[] blue_team;
    public Button[] steps_button;
    public Button yes;
    public Button no;
    public TextMeshProUGUI[] steps_button_Text;
    public TextMeshProUGUI Goal_Status;
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
        public bool is_bind;
        public List<int> BindedHorse;
    };

    public List<int> steps = new List<int>();
    private async Task DelayAsync(float seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }
    int chance = 0;
    bool isMoving;
    int turn = 0;
    int sum = 0;
    float time;
    bool isFight = false;
    int player_number;
    user[][] users;
    int enemy;
    public GameObject[] objectPrefab;
    bool isBackdo = false;
    int choose_step = 0;
    bool isYutThrown = false;
    //������Ʈ �׵θ�
    Material originalMaterial;
    Material outlineMaterial;
    GameObject lastSelectedPlayer;
    private int bindedHorseIndex = -1; //�� ���� ���۽� �� ��ȣ ����
    Color original_Edge = Color.white;
    Color highligted_Edge = new Color(255f / 255f, 0f / 255f, 255f / 255f);
    private int selectedButtonIndex = -1;
    private bool chooseBindCalled = false;
    string DelayFunctionText;
    //Ÿ�̸�
    public TextMeshProUGUI timerText; // Ÿ�̸� ���� ǥ���� Text ������Ʈ ����
    public float startTime = 60.0f; // Ÿ�̸��� ���� �ð� (60��)
    private float timeLeft;
    // �÷��̾� �׵θ� ������Ʈ
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
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            if (!isYutThrown)
            {
                ChangeTurn();
            }
        }
        choose_Player();
        check_Winner();
        AutoSelectClosestPlayerInArray();
        GoalCounter(users);
    }
    void Start()
    {
        timeLeft = startTime;
        Yut.text = "����!";
        DelayFunctionText = "player " + (turn + 1) + " turn!";
        StartCoroutine(YieldReturnDelay(1.0f, DelayFunctionText));
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
        yes.onClick.AddListener(BindYes);
        no.onClick.AddListener(BindNo);
        users = new user[Constants.PlayerNumber][];
        users[0] = new user[Constants.HorseNumber];
        users[1] = new user[Constants.HorseNumber];

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
        //users �ʱ�ȭ

        for (int j = 0; j < Constants.PlayerNumber; j++)
        {
            for (int i = 0; i < Constants.HorseNumber; i++)
            {
                users[0][i].player = red_team[i];
                users[1][i].player = blue_team[i];
                users[j][i].player_start_position = users[j][i].player.transform.position;
                users[j][i].is_destroyed = false;
                users[j][i].is_bind = false;
                users[j][i].BindedHorse = new List<int>();
            }
        }


        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            Debug.Log("user" + i + ": " + users[0][i].nowPosition);
        }
        for (int i = 0; i < 5; i++)
        {
            yutSound[i].mute = true;
        }
        outlineMaterial = Resources.Load<Material>("OutlineMaterial");
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
                //StartCoroutine(YieldReturnDelay(1.0f, DelayFunctionText));
            }
        }
        if (isMoving == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                one();
                Debug.Log("choose" + player_number);
                Yut.text = (player_number + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                two();
                Debug.Log("choose" + player_number);
                Yut.text = (player_number + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                three();
                Debug.Log("choose" + player_number);
                Yut.text = (player_number + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                four();
                Debug.Log("choose" + player_number);
                Yut.text = (player_number + 1) + " ��° �� ����!";
            }
            else if (Input.GetKeyDown(KeyCode.Return) && isYutThrown == true)
            {
                Debug.Log("move" + player_number);
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
        if (users[turn][player_number].player == null)
        {
            int closestPlayerNumber = -1;

            for (int i = 0; i < Constants.HorseNumber; i++)
            {
                int nextPlayerNumber = (player_number + i) % Constants.HorseNumber;

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
        users[turn][player_number].lastPosition = users[turn][player_number].nowPosition;
        if (turn == 0)
        {
            enemy = 1;
        }
        else
        {
            enemy = 0;
        }
        SetOutline(users[turn][player_number].player);
        yield return new WaitForSeconds(1f);
        Yut.text = "";
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;


        while (chosed_step > 0)
        {
            if (users[turn][player_number].routePosition == 30 && chosed_step > 0)
            {
                Debug.Log("Goal");
                DestroyBindedHorses(player_number);
                Destroy(users[turn][player_number].player);
                SetUserToGoal(ref users[turn][player_number]);
                break;
            }

            //users[turn][player_number].nowPosition = users[turn][player_number].routePosition;
            //�鵵 ���� ó��
            if (isBackdo == true)
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
                    chosed_step = 0;
                    isMoving = false;
                    isBackdo = false;
                    ChangeTurn();
                    steps.RemoveAt(choose_step);
                    yield break;
                }
                else
                {
                    BackdoRoute(); //�鵵�̵�
                }
                isBackdo = false;
            }
            else
            {
                users[turn][player_number].routePosition++;
                NormalRoute(chosed_step); //�Ϲ��̵�
            }


            if (users[turn][player_number].routePosition < currentRoute.childNodeList.Count)
            {
                users[turn][player_number].nextPos = currentRoute.childNodeList[users[turn][player_number].routePosition].position;
            }

            while (MoveToNextNode(users[turn][player_number].nextPos)) { yield return null; }

            chosed_step--;
            users[turn][player_number].nowPosition++;


            //���� ���� ��������//DefenseGameTrigger(chosed_step);


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
        users[turn][player_number].nowPosition = users[turn][player_number].routePosition;
        Debug.Log(turn + "�� �� " + player_number + "��° ���� ���� ��ġ : " + users[turn][player_number].nowPosition + " ���� ��ġ : " + users[turn][player_number].lastPosition);
        BindHorse();
        FpsfightTrigger();
        if (bindedHorseIndex != -1)
        {
            yield return new WaitUntil(() => chooseBindCalled);
            chooseBindCalled = false;
        }
        SynchronizeBindedHorses(player_number);
        player_number = 0;

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
            if (users[turn][player_number].nowPosition == users[enemy][i].nowPosition)
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
            if (users[turn][player_number].routePosition == users[enemy][i].nowPosition)
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
    private void BackdoRoute()
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
    }
    private void NormalRoute(int chosed_step)
    {
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
        if (users[turn][player_number].nowPosition == 24 &&
            users[turn][player_number].lastPosition == 24 && chosed_step >= 1)
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
    }
    //�÷��̾� �̵��Լ�
    bool MoveToNextNode(Vector3 goal)
    {
        if (users[turn][player_number].player != null)
        {
            // ���� ��ġ���� x, z ���� �������� y ���� �������� �ʽ��ϴ�.
            Vector3 startPos = users[turn][player_number].player.transform.position;
            Vector3 newGoal = new Vector3(goal.x, startPos.y, goal.z);

            bool isMovingPlayer = Vector3.Distance(newGoal, startPos) > 0.001f; // �̵��� �ʿ䰡 �ִ��� Ȯ��
            if (isMovingPlayer)
            {
                users[turn][player_number].player.transform.position = Vector3.MoveTowards(startPos, newGoal, 8f * Time.deltaTime);
            }

            if (users[turn][player_number].is_bind)
            {
                foreach (int bindedIndex in users[turn][player_number].BindedHorse)
                {
                    if (users[turn][bindedIndex].player != null)
                    {
                        Vector3 bindedStartPos = users[turn][bindedIndex].player.transform.position;
                        Vector3 bindedGoal = new Vector3(goal.x, bindedStartPos.y, goal.z);
                        users[turn][bindedIndex].player.transform.position = Vector3.MoveTowards(bindedStartPos, bindedGoal, 8f * Time.deltaTime);
                    }
                }
            }
            return isMovingPlayer;
        }
        return false;
    }

    void ChangeTurn()
    {
        if (turn == 0)
        {
            turn = 1;
            choose_step = 0;
            isYutThrown = false;
            clear_stepsButton();
            timeLeft = 60.0f;
        }
        else if (turn == 1)
        {
            turn = 0;
            choose_step = 0;
            isYutThrown = false;
            clear_stepsButton();
            timeLeft = 60.0f;
        }
        Yut.text = "player " + (turn + 1) + " turn!";
    }
    private void BindHorse()
    {
        Debug.Log("�� ���� ����");
        Debug.Log("���� �� : " + player_number);
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (player_number == i) continue;
            if (users[turn][player_number].nowPosition == users[turn][i].nowPosition &&
                users[turn][i].goal != true)
            {
                Debug.Log(turn + "�� ���� ���� ��ġ : " + users[turn][player_number].nowPosition);
                Debug.Log(turn + "�� ��ġ�� ���� ��ġ : " + users[turn][i].nowPosition);
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
        List<int> overlappedHorses = GetBindedHorses(player_number);
        if (bindedHorseIndex < 0) return;

        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);

        // ���ε��Ǿ� �ִ� ��� ������ ����Ʈ ����
        List<int> allBindedHorses = new List<int>();
        allBindedHorses.Add(player_number);
        allBindedHorses.AddRange(users[turn][player_number].BindedHorse);
        allBindedHorses.Add(bindedHorseIndex);
        allBindedHorses.AddRange(users[turn][bindedHorseIndex].BindedHorse);

        // �ߺ� �׸� ����
        allBindedHorses = allBindedHorses.Distinct().ToList();

        // ��� ���õ� ���鿡 ���ε� ���� ������Ʈ
        foreach (int horseIndex in allBindedHorses)
        {
            users[turn][horseIndex].is_bind = true;
            users[turn][horseIndex].BindedHorse = new List<int>(allBindedHorses);
            users[turn][horseIndex].BindedHorse.Remove(horseIndex); // �ڱ� �ڽ��� ����Ʈ���� ����
        }
        bindedHorseIndex = -1;
        AdjustPositionByStacking(player_number, overlappedHorses);
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
        List<int> bindedHorses = users[turn][mainHorseIndex].BindedHorse;

        user mainHorse = users[turn][mainHorseIndex];

        foreach (int bindedIndex in bindedHorses)
        {
            users[turn][bindedIndex].routePosition = mainHorse.routePosition;
            users[turn][bindedIndex].nowPosition = mainHorse.nowPosition;
            users[turn][bindedIndex].lastPosition = mainHorse.lastPosition;
            users[turn][bindedIndex].nextPos = mainHorse.nextPos;
            users[turn][bindedIndex].goal = mainHorse.goal;
        }
    }
    //���� ���� ���� �ı�
    public void DestroyBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = users[turn][mainHorseIndex].BindedHorse;

        foreach (int bindedIndex in bindedHorses)
        {
            Destroy(users[turn][bindedIndex].player);
            users[turn][bindedIndex].is_destroyed = true;
            SetUserToGoal(ref users[turn][bindedIndex]);
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
            if (users[turn][currentPlayer].nowPosition == users[turn][i].nowPosition &&
                users[turn][i].goal != true)
            {
                overlappedHorses.Add(i);
            }
        }
        return overlappedHorses;
    }
    void AdjustPositionByStacking(int currentPlayer, List<int> overlappedHorses)
    {
        Vector3 basePosition = users[turn][currentPlayer].player.transform.position;
        for (int i = 0; i < overlappedHorses.Count; i++)
        {
            users[turn][overlappedHorses[i]].player.transform.position = basePosition + new Vector3(0, Constants.STACK_HEIGHT * (i + 1), 0);
        }
    }
    IEnumerator YieldReturnDelay(float time, String text)
    {
        yield return new WaitForSeconds(time);
        Yut.text = text;
        DelayFunctionText = "";
    }
    void UpdateTimerText()
    {
        int minutes = (int)timeLeft / 60;
        int seconds = (int)timeLeft % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void GoalCounter(user[][] users)
    {
        int[] countArray = new int[2];

        for (int i = 0; i < 2; i++)
        {
            foreach (var user in users[i])
            {
                if (!user.goal)
                {
                    countArray[i]++;
                }
            }
        }
        Goal_Status.text = "�÷��̾�1 ���� ��: "+countArray[0]+
            "\n�÷��̾�2 ���� ��: "+countArray[1];
    }
}