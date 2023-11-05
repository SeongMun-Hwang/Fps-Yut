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
    /************스크립트***********/
    public UI UIScript;
    public MoveScript MoveScript;
    public setting SettingScript;
    /**************액션**************/
    public Action OnChangeTurnAction;
    public Action ShowDestination;
    public Action UpdateSteps;
    /*******************************/

    public TextMeshProUGUI Yut;
    public Yut_Field currentRoute;
    public setting setting;
    private async Task DelayAsync(float seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }
    //bool 변수
    public bool isMoving;
    public static bool isFight = false;
    public bool isYutThrown = false;
    bool shownDestination = false;
    int chance = 0;
    int sum = 0;
    public static int enemy;
    public GameObject[] objectPrefab;
    //오브젝트 테두리
    private int bindedHorseIndex = -1; //말 엎기 동작시 말 번호 저장
    public static int winner = -1;
    user[] users;
    horse horses;
    int fightenemy = -1;
    bool myturn = true;
    private void Update()
    {
        users = YutGameManager.Instance.GetUsers();
        horses = YutGameManager.Instance.GetNowHorse();
        MoveScript.GetData(YutGameManager.Instance.GetTurn(), YutGameManager.Instance.GetPlayerNumber(), YutGameManager.Instance.GetUsers());
        if (isYutThrown) { choose_Player(); }
    }
    //윷던지기 랜덤 함수 - 버튼에 연결
    public void throwYut()
    {
        setting.testSound();
        if (!isYutThrown)
        {
            //C_ThrowYut throwYutPacket = new C_ThrowYut();
            //Managers.Network.Send(throwYutPacket);
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
                Debug.Log("무슨상황이여");
                return 5;
        }
    }

    //윷 지정 던지기
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
        UpdateThrowResult(-1);
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

    public void throw_nak()
    {
        if (chance == 0)
        {
            isYutThrown = true;
        }
        else { chance--; }
    }
    //윷 결과 저장 함수
    private async void UpdateThrowResult(int value)
    {
        if (!isYutThrown)
        {
            UIScript.SetSteps(value); //steps리스트에 추가(UI.cs)
            if (value == 4 || value == 5) //윷, 모면 윷 던지기 함수 재호출
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
    //플레이어 선택
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
    //플레이어 말 재생성
    public void reset_player(horse u)
    {
        if (!u.goal)
        {
            Quaternion prevRotation = u.player.transform.rotation;
            Destroy(u.player);
            u.player = Instantiate(objectPrefab[u.Owner.turn]);
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
                    reset_player(users[u.Owner.turn].horses[bindedHorseIndex]); // 재귀적으로 호출
                }
                u.BindedHorse.Clear();
            }
            u.player.transform.position = u.player_start_position;
        }
    }
    /*키보드 입력으로 말 선택
     1~4 : 말 선택
    space : 윷던지기*/
    void choose_Player()
    {
        if (myturn)
        {
            if (!isMoving && !isFight) //말이 움직이지 않고
            {
                if (isYutThrown) //윷을 다 던졌으면
                {
                    ShowDestination.Invoke(); //이동 가능 위치 표시(UI.cs의 DestinationUI)
                }
                if (Input.GetKeyDown(KeyCode.Alpha1)) //1
                {
                    UIScript.DestroyDestination();
                    shownDestination = false;
                    one();
                    Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                    Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2)) //2
                {
                    UIScript.DestroyDestination();
                    shownDestination = false;
                    two();
                    Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                    Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3)) //3
                {
                    UIScript.DestroyDestination();
                    shownDestination = false;
                    three();
                    Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                    Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4)) //4
                {
                    UIScript.DestroyDestination();
                    shownDestination = false;
                    four();
                    Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                    Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
                }
                //엔터 입력 시 Move 코루틴 시작
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    Debug.Log("enter move" + YutGameManager.Instance.GetPlayerNumber());
                    move_Player();
                    //StartCoroutine(Move(UIScript.GetStep()));
                }
                //스페이스, 윷 던지기
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    throwYut();
                }
                //bool showDestination이 false이면 목적지 재계산후 출력. showDestinaiton은 말 선택을 바꿀때마다 false
                if (!shownDestination)
                {
                    ShowDestination.Invoke(); //이동 가능 위치 표시(UI.cs의 DestinationUI)
                    shownDestination = true;
                }
            }
        }
    }
    //버튼에 연결돼있음 날리면 안됨
    public void move_Player()
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

    //한 팀의 모든 플레이어 오브젝트가 null일 시 승리 선언
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

    //이동 동작의 모든 것
    IEnumerator Move(int LeftStep)
    {
        int turn = YutGameManager.Instance.GetTurn();
        int player_number = YutGameManager.Instance.GetPlayerNumber();
        horse nowUser = YutGameManager.Instance.GetNowHorse();
        //턴에 따라 적 변경, 3인 이상 가능하게 할거면 수정 필요
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
        //이동, step이 남아있는 동안
        while (LeftStep != 0 && !isFight)
        {
            //목적지가 30(마지막)이고, 그 이후에도 이동가능하면 오브젝트 파괴, 골인 처리 후 이동 탈출
            if (nowUser.routePosition == 30 && LeftStep > 0)
            {
                Debug.Log("Goal");
                DestroyBindedHorses(player_number);
                Destroy(nowUser.player);
                SetUserToGoal(nowUser);
                break;
            }

            //백도 예외 처리(움직일 수 있는 말이 없을 때)
            if (UIScript.GetStep() == -1)
            {
                Debug.Log("백도예외처리");
                int NowpositionSum = 0;
                //유저의 모든 말의 현재 위치를 더해도 0이면 == 필드위에 말이 없으면
                for (int i = 0; i < Constants.HorseNumber; i++)
                {
                    NowpositionSum += users[turn].horses[i].nowPosition;
                }
                if (NowpositionSum == 0)
                {
                    Yut.text = "이동할 수 있는 말이 없습니다!";
                    yield return new WaitForSeconds(1f);
                    LeftStep = 0;
                    isMoving = false;
                    UIScript.steps.Remove(-1);
                    ChangeTurn();
                    yield break;
                }
                //필드에 말이 있으면 BackdoRoute
                else
                {
                    int BackdoRoute = MoveScript.BackdoRoute(); //백도이동
                    nowUser.routePosition = BackdoRoute;
                }
            }
            //이동계산
            else
            {
                nowUser.routePosition++;
                int NormalRoute = MoveScript.NormalRoute(nowUser.nowPosition, nowUser.lastPosition);
                StartCoroutine(DefenseGameTrigger(LeftStep));

                while (isFight) // 싸우는 동안 일시정지
                {
                    yield return null;
                }
                isFight = false;

                if (winner == enemy)
                {
                    LeftStep = 0;
                    winner = -1;
                    isMoving = false;
                    nowUser.routePosition = nowUser.nowPosition;
                    UpdateSteps.Invoke();
                    if (UIScript.steps.Count == 0)
                    {
                        ChangeTurn();
                    }
                    yield break;
                }
                else
                {
                    if (NormalRoute != -1) // 특수 위치
                    {
                        nowUser.routePosition = NormalRoute;
                        Debug.Log("routePosition : " + nowUser.routePosition);
                        nowUser.nowPosition = NormalRoute;
                        Debug.Log("nowPosition : " + nowUser.nowPosition);
                    }
                    else // 평범한 이동
                    {
                        nowUser.nowPosition++;
                        Debug.Log("nowPosition : " + nowUser.nowPosition);
                    }
                }
                winner = -1;
            }


            if (nowUser.routePosition < currentRoute.childNodeList.Count)
            {
                nowUser.nextPos = currentRoute.childNodeList[nowUser.routePosition].position;
            }
            //다음 위치에 상대방이 있을 때

            //말 nextPos로 이동
            while (MoveScript.MoveToNextNode(nowUser.nextPos)) { yield return null; }
            if (LeftStep > 0)
            {
                LeftStep--;
            }
            else if (LeftStep == -1)
            {
                LeftStep++;
            }

            yield return new WaitForSeconds(0.1f);
            //FpsFight 진행 중이면
            while (isFight == true)
            {
                Yut.text = "";
                yield return new WaitForSeconds(0.1f);
            }
        }
        //LeftStep == 0 되서 나온거라 하나의 이동 끝
        UpdateSteps.Invoke(); //step 업데이트(UIcs _UpdateSteps())
        isMoving = false; //이동 끝났으니 bool 변수도 변경
        Debug.Log("move all");
        nowUser.nowPosition = nowUser.routePosition; //이동하면서 목적지에 도착했으니, 현재위치==도착지
        Debug.Log(turn + "의 턴 " + player_number + "번째 말의 현재 위치 : " + nowUser.nowPosition + " 이전 위치 : " + nowUser.lastPosition);
        nowUser.lastPosition = nowUser.nowPosition; //이전위치==현재위치
        BindHorse(); //말 묶기
        //if (nowUser.nowPosition == 22) { nowUser.nowPosition = 27; } //센터 두 개 통일
        SynchronizeBindedHorses(player_number); //묶인 말들 정보 통일
        /***********************fpsfight**************************/
        StartCoroutine(FpsfightTrigger());
        while (isFight)
        {
            yield return null;
        }
        isFight = false;
        Debug.Log("winner : " + winner);
        Debug.Log("turn : " + YutGameManager.Instance.GetTurn());
        StartCoroutine(UIScript.TurnOffFire());

        if (winner == turn)
        {
            chance += 1;
            Yut.text = chance + " 번의 기회를 추가 획득!";
            Debug.Log("fightenemy : " + fightenemy);
            reset_player(users[enemy].horses[fightenemy]);
            UIScript.choose_step = 0;
            isYutThrown = false;
        }
        else if (winner == enemy)
        {
            reset_player(horses);
        }
        winner = -1;
        fightenemy = -1;
        /**************************************************************/
        //턴 변경전 승자 체크
        check_Winner();
        //찬스 0이고, 스텝에 선택 가능한 이동 없으면 턴 변경
        if (chance == 0 && UIScript.steps.Count() == 0)
        {
            ChangeTurn();
        }
        //아니면 찬스--, 이동가능한 위치 다시 표시
        else if (chance > 0) { chance--; ShowDestination.Invoke(); }
    }

    //Fps Fight 트리거
    private IEnumerator FpsfightTrigger()
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (horses.nowPosition == users[enemy].horses[i].nowPosition)
            {
                if (!users[enemy].horses[i].goal)
                {
                    isFight = true;
                    Debug.Log("encounter");
                    fightenemy = i;
                    Yut.text = "먹으려면\n승리하세요!";
                    StartCoroutine(UIScript.TurnOnFire());
                    yield return new WaitForSeconds(1.0f);
                    Yut.text = "";
                    YutGameManager.Instance.StartHammerGame();
                    break;
                }
            }
        }
    }

    public void DoFight()
    {
        SceneManager.LoadScene("Fpsfight");
        StartCoroutine(UIScript.TurnOffFire());
    }
    //DefenseGame 트리거
    private IEnumerator DefenseGameTrigger(int LeftStep)
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            //이동 예상 경로에 상대가 존재하고
            if (horses.routePosition == users[enemy].horses[i].nowPosition)
            {
                //업은 말이 같거나 작으면
                //if (horses.BindedHorse.Count <= users[enemy].horses[i].BindedHorse.Count)
                //{
                    //이동 가능하면
                    if (LeftStep > 1)
                    {
                        isFight = true;

                        Debug.Log("defensegame");
                        Yut.text = "지나가려면\n 승리하세요!";

                        yield return new WaitForSeconds(1.0f);
                        StartCoroutine(LoadDefenseGameSceneAfterDelay(5f));
                        //SceneManager.LoadScene("Defense_Game");
                        YutGameManager.Instance.StartDefenseGame();
                    }
                //}
            }
        }
    }
    private IEnumerator LoadDefenseGameSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
    public void ChangeTurn()
    {
        if (UIScript.steps.Count != 0)
        {
            return;
        }
        else
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
            //changeTurn액션(UI.cs의 ChangeTurnUI)
            OnChangeTurnAction.Invoke();
        }

    }
    //말 업기
    private void BindHorse()
    {
        Debug.Log("말 묶기 실행");
        Debug.Log("현재 말 : " + YutGameManager.Instance.GetPlayerNumber());
        //자기 말들 위치 같은지 검사
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (YutGameManager.Instance.GetPlayerNumber() == i) continue;//자기 자신 건너뜀
            if (horses.nowPosition == users[YutGameManager.Instance.GetTurn()].horses[i].nowPosition &&
                horses.goal != true)
            {
                Debug.Log(YutGameManager.Instance.GetTurn() + "의 현재 윷의 위치 : " + horses.nowPosition);
                Debug.Log(YutGameManager.Instance.GetTurn() + "의 겹치는 윷의 위치 : " + horses.nowPosition);
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

                // 바인딩되어 있는 모든 말들의 리스트 생성
                List<int> allBindedHorses = new List<int>();
                allBindedHorses.Add(YutGameManager.Instance.GetPlayerNumber());
                allBindedHorses.AddRange(horses.BindedHorse);
                allBindedHorses.Add(bindedHorseIndex);
                allBindedHorses.AddRange(users[YutGameManager.Instance.GetTurn()].horses[bindedHorseIndex].BindedHorse);

                // 중복 항목 제거
                allBindedHorses = allBindedHorses.Distinct().ToList();

                // 모든 관련된 말들에 바인딩 정보 업데이트
                foreach (int horseIndex in allBindedHorses)
                {
                    users[YutGameManager.Instance.GetTurn()].horses[horseIndex].is_bind = true;
                    users[YutGameManager.Instance.GetTurn()].horses[horseIndex].BindedHorse = new List<int>(allBindedHorses);
                    users[YutGameManager.Instance.GetTurn()].horses[horseIndex].BindedHorse.Remove(horseIndex); // 자기 자신은 리스트에서 제외
                }
                bindedHorseIndex = -1;
                //말 오브젝트 쌓기
                AdjustPositionByStacking(YutGameManager.Instance.GetPlayerNumber(), overlappedHorses);
            }
        }
    }


    //묶인 말 이동후 묶인 말들 정보 동기화
    public void SynchronizeBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = horses.BindedHorse;

        horse mainHorse = users[YutGameManager.Instance.GetTurn()].horses[mainHorseIndex];

        foreach (int bindedIndex in bindedHorses)
        {
            Debug.Log("bindedindexx : " + bindedIndex);
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].routePosition = mainHorse.routePosition;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].nowPosition = mainHorse.nowPosition;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].lastPosition = mainHorse.lastPosition;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].nextPos = mainHorse.nextPos;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].goal = mainHorse.goal;
        }
    }
    //묶인 말들 동시 파괴(골인시)
    public void DestroyBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = users[YutGameManager.Instance.GetTurn()].horses[mainHorseIndex].BindedHorse;

        foreach (int bindedIndex in bindedHorses)
        {
            Destroy(users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].player);
            SetUserToGoal(users[YutGameManager.Instance.GetTurn()].horses[bindedIndex]);
        }
    }
    //골인한 말들에 대한 정보 초기화
    public void SetUserToGoal(horse u)
    {
        u.nextPos = Vector3.zero; // 다음 위치가 필요 없음
        u.goal = true;
        u.is_bind = false;
        u.BindedHorse?.Clear();
    }
    //말 업을 때 위치 조정
    void AdjustPositionByStacking(int currentPlayer, List<int> overlappedHorses)
    {
        Vector3 basePosition = horses.player.transform.position;
        for (int i = 0; i < overlappedHorses.Count; i++)
        {
            users[YutGameManager.Instance.GetTurn()].horses[overlappedHorses[i]].player.transform.position = basePosition + new Vector3(0, Constants.STACK_HEIGHT * (i + 1), 0);
        }
    }
}