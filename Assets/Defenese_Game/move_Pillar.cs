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

    public Renderer[] rend;

    void Start()
    {
        for (int i = 0; i < isStopped.Length; i++)
        {
            isStopped[i] = false;
        }

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].constraints = RigidbodyConstraints.FreezeRotation;
            rbs[i].constraints = RigidbodyConstraints.FreezePositionY;
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

        if (Input.GetKeyDown(KeyCode.Space))
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
    void FixedUpdate()
    {
        // 스페이스바가 눌린 후, 움직임 시작
        if (launch)
        {
            StartCoroutine(WaitHalfSecond());
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    foreach (Rigidbody rb in rbs)
    //    {
    //        if (collision.gameObject == player)
    //        {
    //            Debug.Log("collision with Player");
    //        }
    //    }
    //}

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

