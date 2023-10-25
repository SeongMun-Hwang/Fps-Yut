using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_Player : MonoBehaviour
{
    public float speed = 10.0f;  // 움직이는 속도
    private Rigidbody rb;

    private void Start()
    {
        // 오브젝트의 Rigidbody 컴포넌트를 가져옵니다.
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()  // 물리 계산이 들어가므로 FixedUpdate로 변경합니다.
    {
        // 입력을 받습니다.
        float moveHorizontal = 0;
        float moveVertical = 0;

        if (Input.GetKey(KeyCode.W))
        {
            moveVertical = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVertical = -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1;
        }


        // 움직일 방향을 계산합니다.
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // velocity를 직접 변경하여 움직이게 합니다.
        // rb.velocity = movement * speed;

        // 또는 AddForce를 사용하여 힘을 추가합니다.
        rb.AddForce(movement * speed);
    }
}
