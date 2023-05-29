using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_Piillar : MonoBehaviour
{
    public float speed = 10f;  // 움직이는 속도
    private bool isStopped = false;  // 움직임이 멈췄는지 여부
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 오브젝트의 회전을 막습니다.
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        // 아직 움직임이 멈추지 않았다면 x축 방향으로 움직입니다.
        if (!isStopped)
        {
            rb.velocity = new Vector3(speed, rb.velocity.y, rb.velocity.z);
        }
        else
        {
            // 움직임이 멈추었다면 속도를 0으로 설정합니다.
            rb.velocity = Vector3.zero;
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    // 다른 오브젝트와 충돌하면 움직임을 멈춥니다.
    //    isStopped = true;
    //}
}
