using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_Pillar : MonoBehaviour
{
    public float speed = 100f;  // �����̴� �ӵ�
    private bool[] isStopped = new bool[4];  // �� ���⿡ ���� �������� ������� �����մϴ�.
    public Rigidbody[] rbs;
    bool[] pillar = { false, false, false, false };
    bool launch;
    public GameObject player;
    public GameObject[] obstacle;
    public Renderer[] rend;
    private Vector3[] initialPosition;

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

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].constraints = RigidbodyConstraints.FreezeRotation;
            rbs[i].constraints = RigidbodyConstraints.FreezePositionY;
        }

        float[] positions = new float[] { -22.5f, -7.5f, 7.5f, 22.5f };
        Vector3 prevPosition = Vector3.zero;

        foreach (GameObject ob in obstacle) // obstacle �迭�� �� ���ҿ� ����...
        {
            Vector3 newPosition;

            do
            {
                // X�� ������ �����ϰ� �����մϴ�.
                float posX = positions[Random.Range(0, positions.Length)];

                // Z�� ������ �����ϰ� �����մϴ�.
                float posZ = positions[Random.Range(0, positions.Length)];

                // ���õ� ���� �߽��� ������Ʈ�� ��ġ�� �����մϴ�.
                newPosition = new Vector3(posX, ob.transform.position.y, posZ);

            } while (Mathf.Approximately(newPosition.x, prevPosition.x) || Mathf.Approximately(newPosition.z, prevPosition.z));

            ob.transform.position = newPosition;
            prevPosition = newPosition;
        }
    }

    void Update()
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
            rend[i].material.color = initialColor; // ���ϴ� �ʱ� �������� ����
        }
    }
    IEnumerator WaitHalfSecond()
    {
        yield return new WaitForSeconds(0.5f);
        // 0.5�� �Ŀ� ������ �۾��� ���⿡ �߰��մϴ�.
        if (pillar[0] && !isStopped[0]) pillar_Right();
        if (pillar[1] && !isStopped[1]) pillar_Left();
        if (pillar[2] && !isStopped[2]) pillar_Down();
        if (pillar[3] && !isStopped[3]) pillar_Up();

    }
    //���� �� 5�� �� ��� ��ġ ����
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
    }



    void FixedUpdate()
    {
        // �����̽��ٰ� ���� ��, ������ ����
        if (launch)
        {
            StartCoroutine(WaitHalfSecond());
            StartCoroutine(ResetPillarPositionsAfterDelay());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("Collision with Player");
        }
    }

    //�� �Է¿� ���� �� ���⿡ �ش��ϴ� ��� ����
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

