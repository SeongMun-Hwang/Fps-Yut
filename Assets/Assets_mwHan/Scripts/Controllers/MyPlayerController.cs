using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    float _mouseRotationY;
    float _mouseRotationX;

    [SerializeField]
    protected float lookSensitivity; // 카메라 민감도


    // Start is called before the first frame update
    new void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        Transform hammerTransform = transform.Find("Main Camera/Weapon Camera/WeaponHolder/Hammer");
        if (hammerTransform != null)
        {
            WeaponController weaponController = hammerTransform.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.owner = gameObject;
            }
            else
            {
                Debug.LogWarning("Hammer 개체에 WeaponController 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("Hammer 개체를 찾을 수 없습니다.");
        }

        applySpeed = Speed;
        InvokeRepeating("UpdatePlayerInfo", 0f, 0.2f);
        InvokeRepeating("SendPlayerMoving", 0f, 0.2f);
        // 초기화
    }

    // Update is called once per frame
    new void Update()
    {
        if (!isdead)
        {
            if (!isKnockedBack)
            {
                GetKey();
                TryRun();
                TryJump();
                Move();
                IsGround();
            }
            else
            {
                knockbackTimer -= Time.deltaTime;
                if (knockbackTimer <= 0f)
                {
                    isKnockedBack = false;
                }
            }
            GetMouseRotation();
            CameraRotation();
            CharacterRotation();

            //base.Update();
        }
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void TryRun() // 쉬프트 키 누르면 달릴 수 있게
    {
        if (Input.GetKey(KeyCode.LeftShift)) // 쉬프트 누르면 달림
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancle();
        }
    }

    private void GetKey()
    {
        MoveDirX = Input.GetAxisRaw("Horizontal"); // 좌우 오른쪽 방향키 1, 왼쪽 -1, 안누르면 0
        MoveDirZ = Input.GetAxisRaw("Vertical"); // 정면, 뒤
    }

    protected void Move(Vector3 targetPosition)
    {
        Vector3 moveDirection = targetPosition - transform.position;
        float distance = moveDirection.magnitude;

        if (distance > 0.1f)
        {
            Vector3 normalizedDirection = moveDirection.normalized;
            Vector3 targetVelocity = normalizedDirection * Speed;

            // Apply smooth movement using Rigidbody's velocity
            myRigid.velocity = targetVelocity;
        }
        else
        {
            // Stop the player when the target position is reached
            myRigid.velocity = Vector3.zero;
        }
    }

    private void GetMouseRotation() // 상하 카메라 회전
    {
        _mouseRotationX = Input.GetAxisRaw("Mouse Y"); // 상하 카메라 회전
        CameraRotationX = _mouseRotationX * lookSensitivity; // 마우스 움직였을 때 카메라 천천히 움직이게 해줌

        _mouseRotationY = Input.GetAxisRaw("Mouse X"); // 좌우 마우스 회전
        CameraRotationY = _mouseRotationY * lookSensitivity;
    }
    //protected void CameraRotation() // 상하 카메라 회전
    //{
    //    currentCameraRotationX -= CameraRotationX;
    //    currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // 카메라 각도 최대값 고정

    //    theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f); //localEulerAngles : Rotation x,y,z

    //}

    //protected void CharacterRotation() // 좌우 카메라 회전. (캐릭터도 같이 회전됨)
    //{
    //    Vector3 _characterRotationY = new Vector3(0f, CameraRotationY, 0f);
    //    myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    //    // 실제 유니티 내부에 회전은 quaternion 사용함. 우리가 구한 vector(euler)를 quaternion으로 바꿔주는 과정
    //}

    void UpdatePlayerInfo()
    {
        PlayerPos = transform.position;
        PlayerRot = transform.rotation;
    }

    void SendPlayerMoving()
    {
        C_Move movePacket = new C_Move();
        C_Rotation rotationPacket = new C_Rotation();
        movePacket.PosInfo = PosInfo;
        rotationPacket.RotInfo = RotInfo;
        Managers.Network.Send(movePacket);
        Managers.Network.Send(rotationPacket);
    }
}

