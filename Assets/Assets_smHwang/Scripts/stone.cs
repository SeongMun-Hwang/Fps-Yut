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
    public setting SettingScript;
    /*******************************/
    public TextMeshProUGUI Yut;
    public Yut_Field currentRoute;
    public AudioSource[] yutSound;

    public Button yes;
    public Button no;

    private async Task DelayAsync(float seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }
    //bool 변수
    public bool isMoving;
    public bool isBackdo = false;
    private bool chooseBindCalled = false;
    public bool isFight = false;
    public bool isYutThrown = false;

    int chance = 0;
    int sum = 0;
    int enemy;
    public GameObject[] objectPrefab;
    //오브젝트 테두리
    private int bindedHorseIndex = -1; //말 엎기 동작시 말 번호 저장

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
        for (int i = 0; i < 5; i++)
        {
            yutSound[i].mute = true;
        }
        UIScript.SetOutlineMaterial(Resources.Load<Material>("OutlineMaterial"));
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
            int RandomSound = UnityEngine.Random.Range(0, 5);
            yutSound[RandomSound].mute = false;
            yutSound[RandomSound].Play();
        }
    }
    //윷 던지기 함수
    private async void UpdateThrowResult(int value)
    {
        if (!isYutThrown)
        {
            UIScript.SetSteps(value);
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
        UpdateThrowResult(1);
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_back_do()
    {
        Debug.Log("윷 지정 : 백도");
        UpdateThrowResult(1);
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
        UpdateThrowResult(2);
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_girl()
    {
        Debug.Log("윷 지정 : 걸");
        UpdateThrowResult(3);
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    public void throw_yut()
    {
        Debug.Log("윷 지정 : 윷");
        UpdateThrowResult(4);
    }
    public void throw_mo()
    {
        Debug.Log("윷 지정 : 모");
        UpdateThrowResult(5);
    }
    //플레이어 선택
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
        if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].goal == true)
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
                StartCoroutine(Move(UIScript.GetStep()));
            }
            else if (Input.GetKeyDown(KeyCode.Space) && isYutThrown == false)
            {
                throwYut();
            }
            
        }
    }
    //버튼에 연결돼있음 날리면 안됨
    public void move_Player()
    {
        if (isYutThrown)
        {
            Debug.Log("clicked");
            StartCoroutine(Move(UIScript.GetStep()));
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
                UIScript.SetOutline(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].player);
            }
        }
    }



    //말 버튼 초기화
    IEnumerator Move(int LeftStep)
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


        while (LeftStep > 0)
        {
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition == 30 && LeftStep > 0)
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
                    LeftStep = 0;
                    isMoving = false;
                    isBackdo = false;
                    ChangeTurn();
                    UIScript.steps.RemoveAt(UIScript.choose_step);
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
                MoveScript.NormalRoute(LeftStep); //일반이동
            }


            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition < currentRoute.childNodeList.Count)
            {
                users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nextPos = currentRoute.childNodeList[users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].routePosition].position;
            }

            while (MoveScript.MoveToNextNode(users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nextPos)) { yield return null; }

            LeftStep--;
            users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition++;


            //상대방 말을 지나갈때
            //DefenseGameTrigger(LeftStep);


            yield return new WaitForSeconds(0.1f);
            while (isFight == true)
            {
                Yut.text = "";
                yield return new WaitForSeconds(0.1f);
            }
        }

        UIScript.steps.RemoveAt(UIScript.choose_step);

        UIScript.steps_button_Text[UIScript.choose_step].text = "";
        UIScript.UpdateYutChoice();

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

        if (chance == 0 && UIScript.steps.Count() == 0)
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
                UIScript.choose_step = 0;
                isYutThrown = false;
                //Fpsfight 진행
                //SceneManager.LoadScene("Fpsfight");
            }
        }
    }
    //DefenseGame 트리거
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
            UIScript.choose_step = 0;
            isYutThrown = false;
            UIScript.clear_stepsButton();
            UIScript.timeLeft = 60.0f;
        }
        else if (YutGameManager.Instance.GetTurn() == 1)
        {
            YutGameManager.Instance.SetTurn(0);
            UIScript.choose_step = 0;
            isYutThrown = false;
            UIScript.clear_stepsButton();
            UIScript.timeLeft = 60.0f;
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
            SetUserToGoal(ref users[YutGameManager.Instance.GetTurn()][bindedIndex]);
        }
    }
    //골인한 말들에 대한 정보 초기화
    public void SetUserToGoal(ref user u)
    {
        u.nextPos = Vector3.zero; // 다음 위치가 필요 없음
        u.goal = true;
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