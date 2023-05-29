using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_Piillar : MonoBehaviour
{
    public float speed = 10f;  // �����̴� �ӵ�
    private bool isStopped = false;  // �������� ������� ����
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ������Ʈ�� ȸ���� �����ϴ�.
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        // ���� �������� ������ �ʾҴٸ� x�� �������� �����Դϴ�.
        if (!isStopped)
        {
            rb.velocity = new Vector3(speed, rb.velocity.y, rb.velocity.z);
        }
        else
        {
            // �������� ���߾��ٸ� �ӵ��� 0���� �����մϴ�.
            rb.velocity = Vector3.zero;
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    // �ٸ� ������Ʈ�� �浹�ϸ� �������� ����ϴ�.
    //    isStopped = true;
    //}
}
