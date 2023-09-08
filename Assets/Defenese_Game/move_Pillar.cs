using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Vector3[] initialPosition;
    private bool hasLaunched = false;

    private struct PillarState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }
    private PillarState[] initialPillarStates;

    void Start()
    {
        //pillar, edge 초기화
        GameObject[] pillars = GameObject.FindGameObjectsWithTag("pillar");
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
        foreach (GameObject pillar in GameObject.FindGameObjectsWithTag("pillar"))
        {
            Bounds adjustedBounds = pillar.GetComponent<Collider>().bounds;
            adjustedBounds.extents *= 1.0f; //충돌 판정 범위 상세 조정

            if (adjustedBounds.Intersects(player.GetComponent<Collider>().bounds))
            {
                Debug.Log("플레이어와 기둥이 충돌했습니다.");
                // 여기에 충돌 시 수행될 로직을 추가합니다.
            }
        }

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

        if (Input.GetKeyDown(KeyCode.Space)&&((pillar[0]|| pillar[1] || pillar[2] || pillar[3]) != false))
        {
            launch = true;
        }
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
    IEnumerator WaitHalfSecond()
    {
        yield return new WaitForSeconds(0.5f);
        // 0.5초 후에 수행할 작업을 여기에 추가합니다.
        if (pillar[0] && !isStopped[0]) pillar_Right();
        if (pillar[1] && !isStopped[1]) pillar_Left();
        if (pillar[2] && !isStopped[2]) pillar_Down();
        if (pillar[3] && !isStopped[3]) pillar_Up();

    }
    //공격 후 5초 뒤 기둥 위치 리셋
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

    void OnCollisionEnter(Collision collision)
    {
        if (this.CompareTag("pillar"))
        {
            if (collision.gameObject.CompareTag("edge"))
            {
                return;  // pillar는 edge와 충돌하면 안되므로 아무것도 하지 않습니다.
            }
        }
        else if (this.CompareTag("edge"))
        {
            if (!collision.gameObject.CompareTag("player"))
            {
                return;  // edge는 player 외의 다른 오브젝트와 충돌하면 아무것도 하지 않습니다.
            }
        }

        // 충돌 처리 로직
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