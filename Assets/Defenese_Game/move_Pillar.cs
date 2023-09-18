using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

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

        float[] positions = new float[] { -22.5f, -7.5f, 7.5f, 22.5f };
        Vector3 prevPosition = Vector3.zero;

        //장애물 위치 랜덤 생성
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

            } while (Mathf.Approximately(newPosition.x, prevPosition.x) || Mathf.Approximately(newPosition.z, prevPosition.z));

            ob.transform.position = newPosition;
            prevPosition = newPosition;
        }
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
                Debug.Log("플레이어와 기둥이 충돌했습니다.");
                // 여기에 충돌 시 수행될 로직을 추가합니다.
                status_text.text = "패배!";
                StartCoroutine(delay());
                playerCollidedWithPillar = true;
            }
        }
        if (!launch && !hasLaunched)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ResetPillarAndColor();
                pillar[0] = true;
                rend[0].material.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ResetPillarAndColor();
                pillar[1] = true;
                rend[1].material.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ResetPillarAndColor();
                pillar[2] = true;
                rend[2].material.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ResetPillarAndColor();
                pillar[3] = true;
                rend[3].material.color = Color.red;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && pillar.Contains(true))
        {
            playerCollidedWithPillar = false; // 이 부분을 추가합니다.
            launch = true;
        }
    }
    //2초 대기 후 씬 이동
    IEnumerator delay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("YutPlay");
    }
    //라운드 표시 함수
    void UpdateRoundInfo()
    {
        round++;
        status_text.text = "라운드 " + round + "!";
    }

    void ResetPillarAndColor()
    {
        Color initialColor = new Color(243f / 255f, 171f / 255f, 6f / 255f);
        for (int i = 0; i < 4; i++)
        {
            pillar[i] = false;
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
    //공격 후 5초 뒤 기둥 위치 리셋, count 증가
    IEnumerator ResetPillarPositionsAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].transform.position = initialPillarStates[i].position;
            rbs[i].transform.rotation = initialPillarStates[i].rotation;
            rbs[i].velocity = initialPillarStates[i].velocity;
            rbs[i].angularVelocity = initialPillarStates[i].angularVelocity;
        }
        launch = false;
        hasLaunched = false;

        // pillar reset 후에 충돌이 발생하지 않았으면 count를 증가시킵니다.
        if (!playerCollidedWithPillar)
        {
            count++;
            if (count == 3)
            {
                status_text.text = "승리!";
                count = 0;
                StartCoroutine(delay());
            }
            else
            {
                UpdateRoundInfo();
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
}