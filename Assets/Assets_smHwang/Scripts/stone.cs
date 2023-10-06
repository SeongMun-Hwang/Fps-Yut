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
    public bool isBackdo = false;
    private bool chooseBindCalled = false;
    public bool isFight = false;
    public bool isYutThrown = false;
    bool shownDestination = false;
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
        ShowDestination.Invoke();
    }
    public bool GetBackdo()
    {
        return isBackdo;
    }
    //윷 던지기 함수
    private async void UpdateThrowResult(int value)
    {
        if (!isYutThrown)
        {
            Debug.Log("before : " + isBackdo);
            UIScript.SetSteps(value, isBackdo);
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
    //플레이어 말 재생성
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
                    reset_player(ref users[enemy][bindedHorseIndex], playerPrefab); // 재귀적으로 호출
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
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                two();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UIScript.DestroyDestination();
                shownDestination = false;
                three();
                Debug.Log("choose" + YutGameManager.Instance.GetPlayerNumber());
                Yut.text = (YutGameManager.Instance.GetPlayerNumber() + 1) + " 번째 말 선택!";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                UIScript.DestroyDestination();
                shownDestination = false;
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
            if (!shownDestination)
            {
                UIScript.CalculateDestination(isBackdo);
                UIScript.ShowDestination();
                shownDestination = true;
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
            //백도 예외 처리
            if (isBackdo == true && UIScript.GetStep() == 1)
            {
                Debug.Log("백도예외처리");
                int NowpositionSum = 0;
                for (int i = 0; i < Constants.HorseNumber; i++)
                {
                    NowpositionSum += users[turn][i].nowPosition;
                }
                if (NowpositionSum == 0)
                {
                    Yut.text = "이동할 수 있는 말이 없습니다!";
                    yield return new WaitForSeconds(1f);
                    LeftStep = 0;
                    isMoving = false;
                    isBackdo = false;
                    ChangeTurn();
                    yield break;
                }
                else
                {
                    int BackdoRoute = MoveScript.BackdoRoute(); //백도이동
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

            //상대방 말을 지나갈때
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
        Debug.Log(turn + "의 턴 " + player_number + "번째 말의 현재 위치 : " + nowUser.nowPosition + " 이전 위치 : " + nowUser.lastPosition);
        nowUser.lastPosition = nowUser.nowPosition;
        BindHorse();
        FpsfightTrigger();
        if (bindedHorseIndex != -1)
        {
            yield return new WaitUntil(() => chooseBindCalled);
            chooseBindCalled = false;
        }
        SynchronizeBindedHorses(player_number);
        //턴 변경전 승자 체크
        check_Winner();

        if (chance == 0 && UIScript.steps.Count() == 0)
        {
            ChangeTurn();
        }
        else if (chance > 0) { chance--; }
        ShowDestination.Invoke();
    }

    //Fps Fight 트리거
    private void FpsfightTrigger()
    {
        for (int i = 0; i < Constants.HorseNumber; i++)
        {
            if (users[YutGameManager.Instance.GetTurn()][YutGameManager.Instance.GetPlayerNumber()].nowPosition == users[enemy][i].nowPosition)
            {
                if (!users[enemy][i].goal)
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
        }
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
    //말 업을 때 위치 조정
    void AdjustPositionByStacking(int currentPlayer, List<int> overlappedHorses)
    {
        Vector3 basePosition = users[YutGameManager.Instance.GetTurn()][currentPlayer].player.transform.position;
        for (int i = 0; i < overlappedHorses.Count; i++)
        {
            users[YutGameManager.Instance.GetTurn()][overlappedHorses[i]].player.transform.position = basePosition + new Vector3(0, Constants.STACK_HEIGHT * (i + 1), 0);
        }
    }
}