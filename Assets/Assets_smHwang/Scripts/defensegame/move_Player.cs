using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class move_Player : MonoBehaviour
{
    public float speed = 10.0f;  // �����̴� �ӵ�
    public Rigidbody rb;
    public Vector3 movement = new Vector3(0,0,0);

    float moveHorizontal = 0;
    float moveVertical = 0;
    float prevhorizontal = 0;
    float prevvertical = 0;

    private void Start()
    {
        // ������Ʈ�� Rigidbody ������Ʈ�� �����ɴϴ�.
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()  // ���� ����� ���Ƿ� FixedUpdate�� �����մϴ�.
    {
        // �Է��� �޽��ϴ�.
        if (Input.GetKey(KeyCode.W))
        {
            prevvertical = moveVertical;
            moveVertical = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            prevvertical = moveVertical;
            moveVertical = -1;
        }
        else
        {
            prevvertical = moveVertical;
            moveVertical = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            prevhorizontal = moveHorizontal;
            moveHorizontal = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            prevhorizontal = moveHorizontal;
            moveHorizontal = 1;
        }
        else
        {
            prevhorizontal = moveHorizontal;
            moveHorizontal = 0;
        }

        if (moveVertical != prevvertical || moveHorizontal != prevhorizontal)
        {
            C_DefMove movePacket = new C_DefMove();
            movePacket.Posinfo = new PositionInfo();
            movePacket.Posinfo.PosX = moveHorizontal;
            movePacket.Posinfo.PosZ = moveVertical;
            movePacket.Posinfo.PosY = 0;
            Managers.Network.Send(movePacket);
        }

        //calcmovement(moveHorizontal, moveVertical);

        // ������ ������ ����մϴ�.
        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // �Ǵ� AddForce�� ����Ͽ� ���� �߰��մϴ�.
        rb.AddForce(movement * speed);
    }

    public void calcmovement(float posx, float posz)
    {
        movement.x = posx;
        movement.z = posz;
        movement.y = 0;
    }
}
