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
    public AudioSource[] yutSound;

    private async Task DelayAsync(float seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }
    //bool 변수
    public bool isMoving;
    public bool isFight = false;
    public bool isYutThrown = false;
    bool shownDestination = false;
    int chance = 0;
    int sum = 0;
    int enemy;
    public GameObject[] objectPrefab;
    //오브젝트 테두리
    private int bindedHorseIndex = -1; //말 엎기 동작시 말 번호 저장

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
    //윷던지기 랜덤 함수 - 버튼에 연결
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
                    reset_player(users[YutGameManager.Instance.GetTurn()].horses[bindedHorseIndex], playerPrefab); // 재귀적으로 호출
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
        if (!isMoving) //말이 움직이지 않고
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
    //버튼에 연결돼있음 날리면 안됨
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
    //플레이어 오브젝트가 존재하지 않으면 배열 상 가장 가까운 존재하는 오브젝트 자동으로 선택
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
        while (LeftStep != 0)
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

            //백도 예외 처리
            if (UIScript.GetStep()== -1)
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
            //NormalRoute
            else
            {
                nowUser.routePosition++; //목적지++
                int NormalRoute = MoveScript.NormalRoute(nowUser.nowPosition, nowUser.lastPosition); //다음 목적지 계산
                //NormalRoute가 -1이 아니면 == 특수 위치면(코너, 중앙) routePosition에 계산한 NormalRoute 값 할당
                if (NormalRoute != -1)
                {
                    nowUser.routePosition = NormalRoute;
                    Debug.Log("routePosition : " + nowUser.routePosition);
                    nowUser.nowPosition = NormalRoute;
                    Debug.Log("nowPosition : " + nowUser.nowPosition);
                }
                //-1이면 == 평범한 이동이면 그냥 현재 위치++
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
            //이동 중 상대방 말을 지나갈때
            //DefenseGameTrigger(LeftStep);

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
        if (nowUser.nowPosition == 22) { nowUser.nowPosition = 27; } //센터 두 개 통일
        SynchronizeBindedHorses(player_number); //묶인 말들 정보 통일
        FpsfightTrigger(); //상대와 위치 같으면 Fpsfight 지금은 꺼놈
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
    private void FpsfightTrigger()
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (horses.nowPosition == users[enemy].horses[i].nowPosition)
            {
                if (!users[enemy].horses[i].goal)
                {
                    //불키기
                    UIScript.TurnOnFire();
                    //미니 게임 없이 말을 먹을 때의 동작
                    Debug.Log("encounter");
                    //묶여있는 말 개수만큼 추가로 찬스획득.
                    chance += (users[enemy].horses[i].BindedHorse.Count + 1);
                    //적말 리셋
                    reset_player(users[enemy].horses[i], objectPrefab[users[enemy].turn]);
                    Yut.text = chance + " 번의 기회를 추가 획득!";
                    UIScript.choose_step = 0;
                    //윷 다시 던지기 위한 bool 변수 설정
                    isYutThrown = false;
                    //Fpsfight 진행
                    //SceneManager.LoadScene("Fpsfight");
                    StartCoroutine(UIScript.TurnOffFire());
                }
            }
        }
    }
    //DefenseGame 트리거
    private void DefenseGameTrigger(int LeftStep)
    {
        Debug.Log("defensegame");
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            //이동 예상 경로에 상대가 존재하고
            if (horses.routePosition == users[enemy].horses[i].nowPosition)
            {
                //업은 말이 같거나 작으면
                if (horses.BindedHorse.Count <= users[enemy].horses[i].BindedHorse.Count)
                {
                    //이동 가능하면
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
        //changeTurn액션(UI.cs의 ChangeTurnUI)
        OnChangeTurnAction.Invoke();     
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
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].routePosition = mainHorse.routePosition;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].nowPosition = mainHorse.nowPosition;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].lastPosition = mainHorse.lastPosition;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].nextPos = mainHorse.nextPos;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].goal = mainHorse.goal;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].BindedHorse = mainHorse.BindedHorse;
            users[YutGameManager.Instance.GetTurn()].horses[bindedIndex].is_bind = mainHorse.is_bind;
        }
    }
    //묶인 말들 동시 파괴
    public void DestroyBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = users[YutGameManager.Instance.GetTurn()].horses[mainHorseIndex].BindedHorse;

        foreach (int bindedIndex in bindedHorses)
        {
            Destroy(users[YutGameManager.Instance.GetTurn()].horses[bindedHorseIndex].player);
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