using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] // 쓰면 인스펙터 창에 뜬대. 근데 권장은 안한대. 예외도 있음
    private float walkSpeed; // 걷기 속도
    [SerializeField]
    private float runSpeed; // 달리기 속도
    private float applySpeed; // 지금 속도

    [SerializeField]
    private float jumpForce; // 점프 뛰는 힘

    // 상태 변수
    private bool isRun = false; // 지금 달리고 있나?
    private bool isGround = true; // 지금 땅에 붙어있나?

    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private float lookSensitivity; // 카메라 민감도

    // 카메라 한계
    [SerializeField]
    private float cameraRotationLimit; // 마우스 올리면 360도 돌아가면 이상하니까. 돌아갈 수 있는 각도 제한
    private float currentCameraRotationX = 0; // 초기엔 정면

    //필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid; // 플레이어 실제 몸. 물리학 입히는거

    // Start is called before the first frame update
    void Start()
    {
        // theCamera = FindObjectOfType<Camera>(); // 카메라는 플레이어에 들어있는게 아니라 자식개체인 카메라에 들어있는거. 그래서 Camera 가져옴
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();

        // 초기화
        applySpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update() // 프레임마다 실행
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
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f); // 땅에 붙어있는지 확인. 대각선, 계단 고려 약간의 여유
        // 무조건 아래로 레이져 쏴야해서 Vector3.down, 가운대부터 캡슐 크기 반만큼 아래로 레이져,
    }
    private void Jump()
    {
        myRigid.velocity = transform.up * jumpForce; // velocity는 내가 달리는 속도. 순간적으로 위로 가는 힘 가해서 공중으로
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

    private void Running() // 달리기
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancle() // 달리기 끝
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // 좌우 오른쪽 방향키 1, 왼쪽 -1, 안누르면 0
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // 정면, 뒤

        Vector3 _moveHorizontal = transform.right * _moveDirX; // transform은 유니티 창 기본 컴퍼넌트.  기본 (1,0,0)
        Vector3 _moveVertical = transform.forward * _moveDirZ; // 기본 (0,0,1) 위, 아래 구분

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; // normalized는 방향 유지되면서 합 1 나오도록 정규화

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        // Time.deltaTime : 그냥 움직이게 하면 텔포하는것처럼 보이니까 시간 쪼개기?
    }

    private void CameraRotation() // 상하 카메라 회전
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity; // 마우스 움직였을 때 카메라 천천히 움직이게 해줌
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // 카메라 각도 최대값 고정

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f); //localEulerAngles : Rotation x,y,z

    }

    private void CharactorRotation() // 좌우 카메라 회전. (캐릭터도 같이 회전됨)
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        // 실제 유니티 내부에 회전은 quaternion 사용함. 우리가 구한 vector(euler)를 quaternion으로 바꿔주는 과정
    }
}
