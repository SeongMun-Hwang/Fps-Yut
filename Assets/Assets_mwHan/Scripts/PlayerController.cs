using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] // ���� �ν����� â�� ���. �ٵ� ������ ���Ѵ�. ���ܵ� ����
    private float walkSpeed; // �ȱ� �ӵ�
    [SerializeField]
    private float runSpeed; // �޸��� �ӵ�
    private float applySpeed; // ���� �ӵ�

    [SerializeField]
    private float jumpForce; // ���� �ٴ� ��

    // ���� ����
    private bool isRun = false; // ���� �޸��� �ֳ�?
    private bool isGround = true; // ���� ���� �پ��ֳ�?

    // �� ���� ����
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private float lookSensitivity; // ī�޶� �ΰ���

    // ī�޶� �Ѱ�
    [SerializeField]
    private float cameraRotationLimit; // ���콺 �ø��� 360�� ���ư��� �̻��ϴϱ�. ���ư� �� �ִ� ���� ����
    private float currentCameraRotationX = 0; // �ʱ⿣ ����

    //�ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid; // �÷��̾� ���� ��. ������ �����°�

    // Start is called before the first frame update
    void Start()
    {
        // theCamera = FindObjectOfType<Camera>(); // ī�޶�� �÷��̾ ����ִ°� �ƴ϶� �ڽİ�ü�� ī�޶� ����ִ°�. �׷��� Camera ������
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();

        // �ʱ�ȭ
        applySpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update() // �����Ӹ��� ����
    {
        IsGround();
        TryJump();
        TryRun();
        Move();
        CameraRotation();
        CharactorRotation();
    }

    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
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
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancle() // �޸��� ��
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // �¿� ������ ����Ű 1, ���� -1, �ȴ����� 0
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // ����, ��

        Vector3 _moveHorizontal = transform.right * _moveDirX; // transform�� ����Ƽ â �⺻ ���۳�Ʈ.  �⺻ (1,0,0)
        Vector3 _moveVertical = transform.forward * _moveDirZ; // �⺻ (0,0,1) ��, �Ʒ� ����

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; // normalized�� ���� �����Ǹ鼭 �� 1 �������� ����ȭ

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

    private void CharactorRotation() // �¿� ī�޶� ȸ��. (ĳ���͵� ���� ȸ����)
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        // ���� ����Ƽ ���ο� ȸ���� quaternion �����. �츮�� ���� vector(euler)�� quaternion���� �ٲ��ִ� ����
    }
}
