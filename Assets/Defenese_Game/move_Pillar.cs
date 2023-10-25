using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using Google.Protobuf.Protocol;

public class move_Pillar : MonoBehaviour
{
    public float speed = 100f;  // 움직이는 속도
    private bool[] isStopped = new bool[4];  // 각 방향에 대해 움직임이 멈췄는지 추적합니다.
    public Rigidbody[] rbs;
    bool[] pillar = { false, false, false, false };
    bool launch;
    public GameObject player;
    public GameObject[] obstacle;
    public Renderer[] rend;
    private bool hasLaunched = false;
    public TextMeshProUGUI status_text;
    int count = 0;
    int round = 1;
    public float[] positions = new float[] { -22.5f, -7.5f, 7.5f, 22.5f };
    public Vector3 prevPosition = Vector3.zero;
    float originalY = 7.5f; // 원래 Y 좌표
    float targetY = 22.5f; // 목표 Y 좌표
    private struct PillarState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }
    private PillarState[] initialPillarStates;
    private bool playerCollidedWithPillar = false; //충돌 여부 체크

    private GameObject[] pillars;
    void Start()
    {
        Vector3 initialHitPoint; // 초기 hit 점의 위치
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            initialHitPoint = hit.point; // 초기 hit 점 저장
            Debug.Log(initialHitPoint);
        }
        pillars = GameObject.FindGameObjectsWithTag("pillar");
        status_text.text = "라운드 " + round + "!";
        //pillar, edge 초기화
        GameObject[] edges = GameObject.FindGameObjectsWithTag("edge"); 
        //충돌 무시 설정
        foreach (GameObject pillar in pillars)
        {
            foreach (GameObject edge in edges)
            {
                Physics.IgnoreCollision(pillar.GetComponent<Collider>(), edge.GetComponent<Collider>());
            }
        }
        //기둥 초기화를 위한 초기 위치 저장
        initialPillarStates = new PillarState[rbs.Length];
        for (int i = 0; i < rbs.Length; i++)
        {
            initialPillarStates[i] = new PillarState
            {
                position = rbs[i].transform.position,
                rotation = rbs[i].transform.rotation,
                velocity = rbs[i].velocity,
                angularVelocity = rbs[i].angularVelocity
            };
        }

        for (int i = 0; i < isStopped.Length; i++)
        {
            isStopped[i] = false;
        }
        //장애물 위치 랜덤 생성
        CreateObstacle();
    }
    bool IsAdjacentDiagonally(Vector3 pos1, Vector3 pos2)
    {
        return (pos1.x - pos2.x == 15f && pos1.z - pos2.z == 15f) ||   // top-right
               (pos1.x - pos2.x == -15f && pos1.z - pos2.z == 15f) ||  // top-left
               (pos1.x - pos2.x == 15f && pos1.z - pos2.z == -15f) ||  // bottom-right
               (pos1.x - pos2.x == -15f && pos1.z - pos2.z == -15f);   // bottom-left
    }

    void Update()
    {
        foreach (GameObject pillar in pillars)
        {
            //충돌판정
            Bounds adjustedBounds = pillar.GetComponent<Collider>().bounds;
            adjustedBounds.extents *= 1.0f; //충돌 판정 범위 상세 조정

            if (adjustedBounds.Intersects(player.GetComponent<Collider>().bounds))
            {
                //Debug.Log("플레이어와 기둥이 충돌했습니다.");
                // 여기에 충돌 시 수행될 로직을 추가합니다.
                status_text.text = "패배!";
                stone.winner = stone.enemy;
                StartCoroutine(delay());
            }
        }
        if (!launch && !hasLaunched)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ResetPillarAndColor();
                pillar[0] = true;
                StartCoroutine(MoveToTargetY(rend[0].transform, targetY, 1f));
                rend[0].material.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ResetPillarAndColor();
                pillar[1] = true;
                StartCoroutine(MoveToTargetY(rend[1].transform, targetY, 1f));
                rend[1].material.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ResetPillarAndColor();
                pillar[2] = true;
                StartCoroutine(MoveToTargetY(rend[2].transform, targetY, 1f));
                rend[2].material.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ResetPillarAndColor();
                pillar[3] = true;
                StartCoroutine(MoveToTargetY(rend[3].transform, targetY, 1f));
                rend[3].material.color = Color.red;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && pillar.Contains(true))
        {
            playerCollidedWithPillar = false;
            launch = true;
        }
    }
    //2초 대기 후 씬 이동
    IEnumerator delay()
    {
        ResetPillarAndColor();
        ResetPillarPosition();
        yield return new WaitForSeconds(2f);
        stone.isFight = false;
        //SceneManager.LoadScene("YutPlay");
        Debug.Log("isfight d : " + stone.isFight);
        Debug.Log("winner : " + stone.winner);
        Debug.Log("nowuser : " + YutGameManager.Instance.GetTurn());
        status_text.text = "";
        count = 0;
        round = 1;
        playerCollidedWithPillar = false;
        YutGameManager.Instance.StartMainGame();
        C_GameEndReady endreadyPacket = new C_GameEndReady();
        Managers.Network.Send(endreadyPacket);
    }
    //라운드 표시 함수
    void UpdateRound()
    {
        round++;
        CreateObstacle();
        status_text.text = "라운드 " + round + "!";
    }

    void ResetPillarAndColor()
    {
        Color initialColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);
        for (int i = 0; i < 4; i++)
        {
            pillar[i] = false;
            if (rend[i].transform.position.y != originalY)
            {
                StartCoroutine(MoveToTargetY(rend[i].transform, originalY, 1f)); // 1초 동안 이동
            }
            rend[i].material.color = initialColor; // 원하는 초기 색상으로 변경
        }
    }
    //0.5초 후 공격
    IEnumerator WaitHalfSecond()
    {
        yield return new WaitForSeconds(0.5f);
        if (pillar[0] && !isStopped[0]) pillar_Right();
        if (pillar[1] && !isStopped[1]) pillar_Left();
        if (pillar[2] && !isStopped[2]) pillar_Down();
        if (pillar[3] && !isStopped[3]) pillar_Up();

    }
    void ResetPillarPosition()
    {
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].transform.position = initialPillarStates[i].position;
            rbs[i].transform.rotation = initialPillarStates[i].rotation;
            rbs[i].velocity = initialPillarStates[i].velocity;
            rbs[i].angularVelocity = initialPillarStates[i].angularVelocity;
        }
        launch = false;
        hasLaunched = false;
    }
    //공격 후 5초 뒤 기둥 위치 리셋, count 증가
    IEnumerator ResetPillarPositionsAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        ResetPillarPosition();

        // pillar reset 후에 충돌이 발생하지 않았으면 count를 증가시킵니다.
        if (!playerCollidedWithPillar)
        {
            count++;
            if (count == 10)
            {
                stone.winner = YutGameManager.Instance.GetTurn();
                status_text.text = "승리!";
                count = 0;
                CreateObstacle();
                StartCoroutine(delay());
            }
            else
            {
                UpdateRound();
            }
        }
    }
    void FixedUpdate()
    {
        if (launch && !hasLaunched)
        {
            StartCoroutine(WaitHalfSecond());
            StartCoroutine(ResetPillarPositionsAfterDelay());
            hasLaunched = true;
        }
    }

    //각 입력에 따라 각 방향에 해당하는 기둥 공격
    void pillar_Right()
    {
        for (int i = 0; i < 4; i++)
        {
            rbs[i].constraints = RigidbodyConstraints.FreezePositionZ;
            rbs[i].velocity = new Vector3(speed, rbs[i].velocity.y, rbs[i].velocity.z);
        }
    }
    void pillar_Left()
    {
        for (int i = 4; i < 8; i++)
        {
            rbs[i].constraints = RigidbodyConstraints.FreezePositionZ;
            rbs[i].velocity = new Vector3(-speed, rbs[i].velocity.y, rbs[i].velocity.z);
        }
    }
    void pillar_Down()
    {
        for (int i = 8; i < 12; i++)
        {
            rbs[i].constraints = RigidbodyConstraints.FreezePositionX;
            rbs[i].velocity = new Vector3(rbs[i].velocity.x, rbs[i].velocity.y, -speed);
        }
    }
    void pillar_Up()
    {
        for (int i = 12; i < 16; i++)
        {
            rbs[i].constraints = RigidbodyConstraints.FreezePositionX;
            rbs[i].velocity = new Vector3(rbs[i].velocity.x, rbs[i].velocity.y, speed);
        }
    }
    public void CreateObstacle()
    {
        foreach (GameObject ob in obstacle)
        {
            Vector3 newPosition;
            do
            {
                //x축 랜덤
                float posX = positions[Random.Range(0, positions.Length)];

                //z축 랜덤
                float posZ = positions[Random.Range(0, positions.Length)];
                newPosition = new Vector3(posX, ob.transform.position.y, posZ);

            } while (IsAdjacentDiagonally(newPosition, prevPosition) ||
                     Mathf.Approximately(newPosition.x, prevPosition.x) ||
                     Mathf.Approximately(newPosition.z, prevPosition.z));

            ob.transform.position = newPosition;
            prevPosition = newPosition;
        }
    }
    IEnumerator MoveToTargetY(Transform objectTransform, float targetY, float duration)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = objectTransform.position;
        Vector3 targetPosition = new Vector3(originalPosition.x, targetY, originalPosition.z);

        while (elapsedTime < duration)
        {
            objectTransform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectTransform.position = targetPosition;
    }
}