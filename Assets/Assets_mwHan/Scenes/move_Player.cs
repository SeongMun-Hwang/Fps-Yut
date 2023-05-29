using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_Player : MonoBehaviour
{
    public float speed = 5.0f;  // �����̴� �ӵ�
    private Rigidbody rb;

    private void Start()
    {
        // ������Ʈ�� Rigidbody ������Ʈ�� �����ɴϴ�.
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // �Է��� �޽��ϴ�.
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // ������ ������ ����մϴ�.
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // velocity�� ���� �����Ͽ� �����̰� �մϴ�.
        // rb.velocity = movement * speed;

        // �Ǵ� AddForce�� ����Ͽ� ���� �߰��մϴ�.
        rb.AddForce(movement * speed);
    }
}
