using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    // ���� ����
    private bool isGround = true; // ���� ���� �پ��ֳ�?
    private CapsuleCollider capsuleCollider;
    private Vector3 _velocity;

    [SerializeField] // ���� �ν����� â�� ���. �ٵ� ������ ���Ѵ�. ���ܵ� ����
    private float walkSpeed; // �ȱ� �ӵ�
    [SerializeField]
    private float runSpeed; // �޸��� �ӵ�
    private float applySpeed; // ���� �ӵ�

    [SerializeField]
    private float jumpForce; // ���� �ٴ� ��

    [SerializeField]
    private float lookSensitivity; // ī�޶� �ΰ���

    // ī�޶� �Ѱ�
    [SerializeField]
    private float cameraRotationLimit; // ���콺 �ø��� 360�� ���ư��� �̻��ϴϱ�. ���ư� �� �ִ� ���� ����
    private float currentCameraRotationX = 0; // �ʱ⿣ ����

    //�ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCamera;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        capsuleCollider = GetComponent<CapsuleCollider>();
        // �ʱ�ȭ
        applySpeed = walkSpeed;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (!isdead)
        {
            if (!isKnockedBack)
            {
                GetKey();
                Move();
                TryRun();
                IsGround();
                TryJump();
            }
            CameraRotation();
            CharacterRotation();
        }
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f); // ���� �پ��ִ��� Ȯ��. �밢��, ��� ��� �ణ�� ����
        // ������ �Ʒ��� ������ �����ؼ� Vector3.down, �������� ĸ�� ũ�� �ݸ�ŭ �Ʒ��� ������,
    }
    private void Jump()
    {
        myRigid.velocity = transform.up * jumpForce; // velocity�� ���� �޸��� �ӵ�. ���������� ���� ���� �� ���ؼ� ��������
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

    private void Running() // �޸���
    {
        applySpeed = runSpeed;
    }

    private void RunningCancle() // �޸��� ��
    {
        applySpeed = walkSpeed;
    }

    private void GetKey()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // �¿� ������ ����Ű 1, ���� -1, �ȴ����� 0
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // ����, ��

        Vector3 _moveHorizontal = transform.right * _moveDirX; // transform�� ����Ƽ â �⺻ ���۳�Ʈ.  �⺻ (1,0,0)
        Vector3 _moveVertical = transform.forward * _moveDirZ; // �⺻ (0,0,1) ��, �Ʒ� ����

        _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; // normalized�� ���� �����Ǹ鼭 �� 1 �������� ����ȭ
    }
    private void Move()
    {
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        // Time.deltaTime : �׳� �����̰� �ϸ� �����ϴ°�ó�� ���̴ϱ� �ð� �ɰ���?
    }

    private void CameraRotation() // ���� ī�޶� ȸ��
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity; // ���콺 �������� �� ī�޶� õõ�� �����̰� ����
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // ī�޶� ���� �ִ밪 ����

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f); //localEulerAngles : Rotation x,y,z

    }

    private void CharacterRotation() // �¿� ī�޶� ȸ��. (ĳ���͵� ���� ȸ����)
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        // ���� ����Ƽ ���ο� ȸ���� quaternion �����. �츮�� ���� vector(euler)�� quaternion���� �ٲ��ִ� ����
    }
}
