using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_Player : MonoBehaviour
{
    public float speed = 10.0f;  // �����̴� �ӵ�
    private Rigidbody rb;

    private void Start()
    {
        // ������Ʈ�� Rigidbody ������Ʈ�� �����ɴϴ�.
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()  // ���� ����� ���Ƿ� FixedUpdate�� �����մϴ�.
    {
        // �Է��� �޽��ϴ�.
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

        // ������ ������ ����մϴ�.
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // velocity�� ���� �����Ͽ� �����̰� �մϴ�.
        // rb.velocity = movement * speed;

        // �Ǵ� AddForce�� ����Ͽ� ���� �߰��մϴ�.
        rb.AddForce(movement * speed);
    }
}
