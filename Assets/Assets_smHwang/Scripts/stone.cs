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
    public const float STACK_HEIGHT = 0.5f;
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
        public List<int> BindedeHorse;
    };

    public List<int> steps = new List<int>();
    private async Task DelayAsync(float seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }

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
    //오브젝트 테두리
    Material originalMaterial;
    Material outlineMaterial;
    GameObject lastSelectedPlayer;
    private int bindedHorseIndex = -1; //말 엎기 동작시 말 번호 저장
    Color original_Edge = Color.white;
    Color highligted_Edge = new Color(255f / 255f, 0f / 255f, 255f / 255f);
    private int selectedButtonIndex = -1;
    private bool chooseBindCalled = false;
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
        choose_Player();
        check_Winner();
        AutoSelectClosestPlayerInArray();
    }
    void Start()
    {
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
                users[j][i].BindedeHorse = new List<int>();
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
        UpdateThrowResult(1, "도");
        isYutThrown = true;
    }
    public void throw_back_do()
    {
        UpdateThrowResult(1, "백도");
        isBackdo = true;
        isYutThrown = true;
    }
    public void throw_gae()
    {
        UpdateThrowResult(2, "개");
        isYutThrown = true;
    }
    public void throw_girl()
    {
        UpdateThrowResult(3, "걸");
        isYutThrown = true;
    }
    public void throw_yut()
    {
        UpdateThrowResult(4, "윷");
    }
    public void throw_mo()
    {
        UpdateThrowResult(5, "모");
    }
    //플레이어 선택
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
                Debug.Log("choose" + player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                two();
                Debug.Log("choose" + player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                three();
                Debug.Log("choose" + player_number);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                four();
                Debug.Log("choose" + player_number);
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
        //if (turn == 0)
        //{
        //    enemy = 1;
        //}
        //else
        //{
        //    enemy = 0;
        //}

        SetOutline(users[turn][player_number].player);
        yield return new WaitForSeconds(1f);
        Yut.text = "";
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;

        users[turn][player_number].lastPosition = users[turn][player_number].routePosition;

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

            users[turn][player_number].nowPosition = users[turn][player_number].routePosition;
            users[turn][player_number].routePosition++;

            //백도 예외 처리
            if (isBackdo == true && steps.Count < 2)
            {
                int NowpositionSum = 0;
                for (int i = 0; i < Constants.HorseNumber; i++)
                {
                    NowpositionSum += users[turn][i].nowPosition;
                }
                if (NowpositionSum == 0)
                {
                    Yut.fontSize = 20f;
                    Yut.text = "이동할 수 있는 말이 없습니다!";
                    yield return new WaitForSeconds(1f);
                    Yut.fontSize = 30f;
                    chosed_step = 0;
                    isMoving = false;
                    isBackdo = false;
                    ChangeTurn();
                    steps.RemoveAt(choose_step);
                    yield break;
                }
                else
                {
                    BackdoRoute(); //백도이동
                }
            }
            else
            {
                NormalRoute(chosed_step); //일반이동
            }


            if (users[turn][player_number].routePosition < currentRoute.childNodeList.Count)
            {
                users[turn][player_number].nextPos = currentRoute.childNodeList[users[turn][player_number].routePosition].position;
            }
            else
            {
                // 처리 방법이 필요한 경우 여기에 코드를 추가. 예를 들면, 오류 메시지를 출력하거나 다른 루틴을 실행.
                Debug.LogError("routePosition is out of range!");
            }

            while (MoveToNextNode(users[turn][player_number].nextPos)) { yield return null; }
            chosed_step--;
            users[turn][player_number].nowPosition++;

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
        BindHorse();
        if (bindedHorseIndex != -1)
        {
            yield return new WaitUntil(() => chooseBindCalled);
            chooseBindCalled = false;
        }
        //FpsfightTrigger();

        isMoving = false;
        sum = 0;
        Debug.Log("move all");

        //윷, 모 나왔을 시 턴 유지, 아니면 변경
        //if (chance > 0)
        //{
        //    chance--;
        //}

        //if (steps.Count == 0)
        //{
        //    if (turn == 0) { turn = 1; choose_step = 0; isYutThrown = false; clear_stepsButton(); }
        //    else if (turn == 1) { turn = 0; choose_step = 0; isYutThrown = false; clear_stepsButton(); }
        //    Yut.text = "player " + (turn + 1) + " turn!";
        //}
        if (steps.Count == 0)
        {
            isYutThrown = false;
            choose_step = 0;
        }
        player_number = 0;

        users[turn][player_number].lastPosition = users[turn][player_number].nowPosition;
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
            if (users[turn][player_number].nowPosition == users[enemy][i].nowPosition)
            {
                //미니 게임 없이 말을 먹을 때의 동작
                //Debug.Log("encounter");
                //reset_player(ref users[enemy][i], objectPrefab[enemy]);
                //chance++;

                //Fpsfight 진행
                SceneManager.LoadScene("Fpsfight");
            }
        }
    }
    //DefenseGame 트리거
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
        isBackdo = false;
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
        if (users[turn][player_number].nowPosition == 24 && chosed_step >= 1)
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
    //플레이어 이동함수
    bool MoveToNextNode(Vector3 goal)
    {
        if (users[turn][player_number].player != null)
        {
            bool isMovingPlayer = goal != (users[turn][player_number].player.transform.position = Vector3.MoveTowards(users[turn][player_number].player.transform.position, goal, 8f * Time.deltaTime));

            if (users[turn][player_number].is_bind)
            {
                foreach (int bindedIndex in users[turn][player_number].BindedeHorse)
                {
                    if (users[turn][bindedIndex].player != null)
                    {
                        users[turn][bindedIndex].player.transform.position = Vector3.MoveTowards(users[turn][bindedIndex].player.transform.position, goal, 8f * Time.deltaTime);
                    }
                }
            }

            return isMovingPlayer;
        }
        SynchronizeBindedHorses(player_number);
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
        }
        else if (turn == 1)
        {
            turn = 0;
            choose_step = 0;
            isYutThrown = false;
            clear_stepsButton();
        }
        Yut.text = "player " + (turn + 1) + " turn!";
    }
    private void BindHorse()
    {
        List<int> overlappedHorses = GetBindedHorses(player_number);

        for (int i = 0; i < overlappedHorses.Count; i++)
        {
            Yut.text = "윷을 엎으시겠습니까?";
            Debug.Log("playernumber: " + player_number);
            Debug.Log("index: " + overlappedHorses[i]);
            bindedHorseIndex = overlappedHorses[i];
            yes.gameObject.SetActive(true);
            no.gameObject.SetActive(true);
        }

        AdjustPositionByStacking(player_number, overlappedHorses);
    }

    //묶기 선택시 BindedHorse, is_bind 업데이트
    private void BindYes()
    {
        if (bindedHorseIndex < 0) return;

        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);

        // 바인딩되어 있는 모든 말들의 리스트 생성
        List<int> allBindedHorses = new List<int>();
        allBindedHorses.Add(player_number);
        allBindedHorses.AddRange(users[turn][player_number].BindedeHorse);
        allBindedHorses.Add(bindedHorseIndex);
        allBindedHorses.AddRange(users[turn][bindedHorseIndex].BindedeHorse);

        // 중복 항목 제거
        allBindedHorses = allBindedHorses.Distinct().ToList();

        // 모든 관련된 말들에 바인딩 정보 업데이트
        foreach (int horseIndex in allBindedHorses)
        {
            users[turn][horseIndex].is_bind = true;
            users[turn][horseIndex].BindedeHorse = new List<int>(allBindedHorses);
            users[turn][horseIndex].BindedeHorse.Remove(horseIndex); // 자기 자신은 리스트에서 제외
        }
        chooseBindCalled = true;
        bindedHorseIndex = -1;
    }

    private void BindNo()
    {
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
        chooseBindCalled = true;
    }
    //묶인 말 이동후 묶인 말들 정보 동기화
    public void SynchronizeBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = users[turn][mainHorseIndex].BindedeHorse;

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
    //묶일 말들 동시 파괴
    public void DestroyBindedHorses(int mainHorseIndex)
    {
        List<int> bindedHorses = users[turn][mainHorseIndex].BindedeHorse;

        foreach (int bindedIndex in bindedHorses)
        {
            Destroy(users[turn][bindedIndex].player);
            users[turn][bindedIndex].is_destroyed = true;
            SetUserToGoal(ref users[turn][bindedIndex]);
        }
    }
    //골인한 말들에 대한 정보 초기화
    public void SetUserToGoal(ref user u)
    {
        u.nextPos = Vector3.zero; // 다음 위치가 필요 없음
        u.goal = true;
        u.is_destroyed = true;
        u.is_bind = false;
        u.BindedeHorse?.Clear();
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

        // 현재 말의 위치는 기본 위치에 따라 y축을 조정합니다.
        users[turn][currentPlayer].player.transform.position = basePosition + new Vector3(0, Constants.STACK_HEIGHT * overlappedHorses.Count, 0);

        for (int i = 0; i < overlappedHorses.Count; i++)
        {
            // 겹치는 다른 말들의 위치도 y축을 기반으로 조정합니다.
            users[turn][overlappedHorses[i]].player.transform.position = basePosition + new Vector3(0, Constants.STACK_HEIGHT * (i + 1), 0);
        }
    }

    Vector3 CalculatePosition(Vector3 center, float distance, float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        float x = center.x + distance * Mathf.Cos(angleInRadians);
        float z = center.z + distance * Mathf.Sin(angleInRadians);
        return new Vector3(x, center.y, z);
    }
}