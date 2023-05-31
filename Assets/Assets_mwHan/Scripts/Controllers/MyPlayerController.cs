using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    float _mouseRotationY;
    float _mouseRotationX;

    [SerializeField]
    protected float lookSensitivity; // ī�޶� �ΰ���


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
                Debug.LogWarning("Hammer ��ü�� WeaponController ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("Hammer ��ü�� ã�� �� �����ϴ�.");
        }

        applySpeed = Speed;
        InvokeRepeating("UpdatePlayerInfo", 0f, 0.2f);
        InvokeRepeating("SendPlayerMoving", 0f, 0.2f);
        // �ʱ�ȭ
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

    private void TryRun() // ����Ʈ Ű ������ �޸� �� �ְ�
    {
        if (Input.GetKey(KeyCode.LeftShift)) // ����Ʈ ������ �޸�
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
        MoveDirX = Input.GetAxisRaw("Horizontal"); // �¿� ������ ����Ű 1, ���� -1, �ȴ����� 0
        MoveDirZ = Input.GetAxisRaw("Vertical"); // ����, ��
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

    private void GetMouseRotation() // ���� ī�޶� ȸ��
    {
        _mouseRotationX = Input.GetAxisRaw("Mouse Y"); // ���� ī�޶� ȸ��
        CameraRotationX = _mouseRotationX * lookSensitivity; // ���콺 �������� �� ī�޶� õõ�� �����̰� ����

        _mouseRotationY = Input.GetAxisRaw("Mouse X"); // �¿� ���콺 ȸ��
        CameraRotationY = _mouseRotationY * lookSensitivity;
    }
    //protected void CameraRotation() // ���� ī�޶� ȸ��
    //{
    //    currentCameraRotationX -= CameraRotationX;
    //    currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // ī�޶� ���� �ִ밪 ����

    //    theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f); //localEulerAngles : Rotation x,y,z

    //}

    //protected void CharacterRotation() // �¿� ī�޶� ȸ��. (ĳ���͵� ���� ȸ����)
    //{
    //    Vector3 _characterRotationY = new Vector3(0f, CameraRotationY, 0f);
    //    myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    //    // ���� ����Ƽ ���ο� ȸ���� quaternion �����. �츮�� ���� vector(euler)�� quaternion���� �ٲ��ִ� ����
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

