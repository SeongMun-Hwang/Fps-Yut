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
    /************스크립트***********/
    public UI UIScript;
    public MoveScript MoveScript;
    /*******************************/
    public TextMeshProUGUI Yut;
    public Yut_Field currentRoute;
    public AudioSource[] yutSound;
    public GameObject[] red_team;
    public GameObject[] blue_team;
    public Button[] steps_button;
    public Button yes;
    public Button no;
    public TextMeshProUGUI[] steps_button_Text;

    public List<int> steps = new List<int>();
    private async Task DelayAsync(float seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }
    int chance = 0;
    bool isMoving;
    int sum = 0;
    float time;
    bool isFight = false;
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
    private int bindedHorseIndex = -1; //말 엎기 동작시 말 번호 저장
    Color original_Edge = Color.white;
    Color highligted_Edge = new Color(255f / 255f, 0f / 255f, 255f / 255f);
    private int selectedButtonIndex = -1;
    private bool chooseBindCalled = false;
    //타이머
    public TextMeshProUGUI timerText; // 타이머 값을 표시할 Text 컴포넌트 참조
    public float startTime = 60.0f; // 타이머의 시작 시간 (60초)
    private float timeLeft;
    
    // 플레이어 테두리 업데이트
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
        MoveScript.GetData(YutGameManager.Instance.GetTurn(), YutGameManager.Instance.GetPlayerNumber(), users);
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
        UIScript.GoalCounter(users);
    }
    void Start()
    {
        UIScript.StartText(YutGameManager.Instance.GetTurn());
        timeLeft = startTime;
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
        yes.onClick.AddListener(BindYes);
        no.onClick.AddListener(BindNo);
        users = new user[Constants.PlayerNumber][];
        users[0] = new user[Constants.HorseNumber];
        users[1] = new user[Constants.HorseNumber];

        for (int i = 0; i < steps_button.Length; i++)
        {
            int index = i; // 이유: 클로저 때문에 바깥 변수를 직접 쓰면 마지막 값이 고정될 수 있음
            steps_button[i].onClick.AddListener(() => choose_steps(index));
        }
        //steps_button_Text 연결
        for (int i = 0; i < 5; i++)
        {
            steps_button_Text[i] = steps_button[i].GetComponentInChildren<TextMeshProUGUI>();
        }
        //users 초기화

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
    //윷던지기 랜덤 함수
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
    //윷 던지기 함수
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
                Yut.text = "한 번 더!";
                sum = 0;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    throwYut();
                }
            }
        }
    }
    //개발자용 지정 던지기
    public void throw_do()
    {
        Debug.Log("윷 지정 : 도");
        UpdateThrowResult(1, "도");
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_back_do()
    {
        Debug.Log("윷 지정 : 백도");
        UpdateThrowResult(1, "백도");
        isBackdo = true;
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_gae()
    {
        Debug.Log("윷 지정 : 개");
        UpdateThrowResult(2, "개");
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_girl()
    {
        Debug.Log("윷 지정 : 걸");
        UpdateThrowResult(3, "걸");
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_yut()
    {
        Debug.Log("윷 지정 : 윷");
        UpdateThrowResult(4, "윷");
    }
    public void throw_mo()
    {
        Debug.Log("윷 지정 : 모");
        UpdateThrowResult(5, "모");
    }
    //플레이어 선택
    public void one()
    {
        YutGameManager.Instance.SetPlayerNumber(0);
        check_player();
        SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
    }
    public void two()
    {
        YutGameManager.Instance.SetPlayerNumber(1);
        check_player();
        SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
    }
    public void three()
    {
        YutGameManager.Instance.SetPlayerNumber(2);
        check_player();
        SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
    }
    public void four()
    {
        YutGameManager.Instance.SetPlayerNumber(3);
        check_player();
        SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
    }
    public void check_player()
    {
        if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].is_destroyed == true)
        {
            Yut.text = "player already goaled!";
        }
    }
    //플레이어 말 삭제
    void clear_player(ref user u)
    {
        if (u.player != null)
        {
            Destroy(u.player);
            u.player = null;
        }
    }
    //플레이어 말 재생성
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
                reset_player(ref users[enemy][bindedHorseIndex], playerPrefab); // 재귀적으로 호출
            }
            u.BindedHorse.Clear();
        }
        u.player.transform.position = u.player_start_position;
    }
    /*키보드 입력으로 말 선택
     1~4 : 말 선택
    space : 윷던지기*/
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
                Yut.text = "얼마나 이동할 지 선택하세요!";
            }
        }
        if (isMoving == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                one();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                two();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                three();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                four();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
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
    //버튼에 연결돼있음 날리면 안됨
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
    //플레이어 오브젝트가 존재하지 않으면 배열 상 가장 가까운 존재하는 오브젝트 자동으로 선택
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
                SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
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
    //말 버튼 초기화
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
        SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
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
            //백도 예외 처리
            if (isBackdo == true)
            {
                Debug.Log("백도예외처리");
                int NowpositionSum = 0;
                for (int i = 0; i < Constants.HorseNumber; i++)
                {
                    NowpositionSum += users[YutGameManager.Instance.GetTurn()][i].nowPosition;
                }
                if (NowpositionSum == 0)
                {
                    Yut.text = "이동할 수 있는 말이 없습니다!";
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
                    MoveScript.BackdoRoute(); //백도이동
                }
                isBackdo = false;
            }
            else
            {
                users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition++;
                MoveScript.NormalRoute(chosed_step); //일반이동
            }


            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition < currentRoute.childNodeList.Count)
            {
                users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nextPos = currentRoute.childNodeList[users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition].position;
            }

            while (MoveScript.MoveToNextNode(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nextPos)) { yield return null; }

            chosed_step--;
            users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition++;


            //상대방 말을 지나갈때
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
        Debug.Log(YutGameManager.Instance.GetTurn() + "의 턴 " + YutGameManager.Instance.GetPlayerNumber() + "번째 말의 현재 위치 : " + users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition + " 이전 위치 : " + users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].lastPosition);
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
        //턴 변경 없이 이동 테스트 시
        //if (steps.Count == 0)
        //{
        //    isYutThrown = false;
        //    choose_step = 0;
        //}
    }
    //이동 선택지 업데이트
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
    //Fps Fight 트리거
    private void FpsfightTrigger()
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition == users[enemy][i].nowPosition)
            {
                //미니 게임 없이 말을 먹을 때의 동작
                Debug.Log("encounter");
                chance += (users[enemy][i].BindedHorse.Count + 1);
                reset_player(ref users[enemy][i], objectPrefab[enemy]);
                Yut.text = chance + " 번의 기회를 추가 획득!";
                isYutThrown = false;
                choose_step = 0;
                //Fpsfight 진행
                //SceneManager.LoadScene("Fpsfight");
            }
        }
    }
    //DefenseGame 트리거
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

    void ChangeTurn()
    {
        if (YutGameManager.Instance.GetTurn() == 0)
        {
            YutGameManager.Instance.SetTurn(1);
            choose_step = 0;
            isYutThrown = false;
            clear_stepsButton();
            timeLeft = 60.0f;
        }
        else if (YutGameManager.Instance.GetTurn() == 1)
        {
            YutGameManager.Instance.SetTurn(0);
            choose_step = 0;
            isYutThrown = false;
            clear_stepsButton();
            timeLeft = 60.0f;
        }
        YutGameManager.Instance.SetPlayerNumber(0);
        Yut.text = "player " + (YutGameManager.Instance.GetTurn() + 1) + " turn!";
    }
    private void BindHorse()
    {
        Debug.Log("말 묶기 실행");
        Debug.Log("현재 말 : " + YutGameManager.Instance.GetPlayerNumber());
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (YutGameManager.Instance.GetPlayerNumber() == i) continue;
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition == users[YutGameManager.Instance.GetTurn()][i].nowPosition &&
                users[YutGameManager.Instance.GetTurn()][i].goal != true)
            {
                Debug.Log(YutGameManager.Instance.GetTurn() + "의 현재 윷의 위치 : " + users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition);
                Debug.Log(YutGameManager.Instance.GetTurn() + "의 겹치는 윷의 위치 : " + users[YutGameManager.Instance.GetTurn()][i].nowPosition);
                Yut.text = "윷을 업으시겠습니까?";
                bindedHorseIndex = i;
                yes.gameObject.SetActive(true);
                no.gameObject.SetActive(true);
            }
        }
    }

    //묶기 선택시 BindedHorse, is_bind 업데이트
    private void BindYes()
    {
        List<int> overlappedHorses = GetBindedHorses(YutGameManager.Instance.GetPlayerNumber());
        if (bindedHorseIndex < 0) return;

        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);

        // 바인딩되어 있는 모든 말들의 리스트 생성
        List<int> allBindedHorses = new List<int>();
        allBindedHorses.Add(YutGameManager.Instance.GetPlayerNumber());
        allBindedHorses.AddRange(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].BindedHorse);
        allBindedHorses.Add(bindedHorseIndex);
        allBindedHorses.AddRange(users[YutGameManager.Instance.GetTurn()][bindedHorseIndex].BindedHorse);

        // 중복 항목 제거
        allBindedHorses = allBindedHorses.Distinct().ToList();

        // 모든 관련된 말들에 바인딩 정보 업데이트
        foreach (int horseIndex in allBindedHorses)
        {
            users[YutGameManager.Instance.GetTurn()][horseIndex].is_bind = true;
            users[YutGameManager.Instance.GetTurn()][horseIndex].BindedHorse = new List<int>(allBindedHorses);
            users[YutGameManager.Instance.GetTurn()][horseIndex].BindedHorse.Remove(horseIndex); // 자기 자신은 리스트에서 제외
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
    //묶인 말 이동후 묶인 말들 정보 동기화
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
    //묶인 말들 동시 파괴
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
    //골인한 말들에 대한 정보 초기화
    public void SetUserToGoal(ref user u)
    {
        u.nextPos = Vector3.zero; // 다음 위치가 필요 없음
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
    void UpdateTimerText()
    {
        int minutes = (int)timeLeft / 60;
        int seconds = (int)timeLeft % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}