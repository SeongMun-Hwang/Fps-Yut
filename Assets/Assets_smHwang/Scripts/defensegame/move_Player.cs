using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class move_Player : MonoBehaviour
{
    public float speed = 10.0f;  // 움직이는 속도
    public Rigidbody rb;
    public Vector3 movement = new Vector3(0,0,0);

    float moveHorizontal = 0;
    float moveVertical = 0;
    float prevhorizontal = 0;
    float prevvertical = 0;

    private void Start()
    {
        // 오브젝트의 Rigidbody 컴포넌트를 가져옵니다.
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()  // 물리 계산이 들어가므로 FixedUpdate로 변경합니다.
    {
        // 입력을 받습니다.
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

        // 움직일 방향을 계산합니다.
        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // 또는 AddForce를 사용하여 힘을 추가합니다.
        rb.AddForce(movement * speed);
    }

    public void calcmovement(float posx, float posz)
    {
        movement.x = posx;
        movement.z = posz;
        movement.y = 0;
    }
}
