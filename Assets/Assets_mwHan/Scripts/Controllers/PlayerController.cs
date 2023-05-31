using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int Id { get; set; }

    StatInfo _stat = new StatInfo();

    public StatInfo Stat
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.Hp = value.Hp;
            _stat.MaxHp = value.MaxHp;
            _stat.Speed = value.Speed;
            _stat.RunSpeed = value.RunSpeed;
            _stat.JumpForce = value.JumpForce;
        }
    }

    public float Speed
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }

    public float RunSpeed
    {
        get { return Stat.RunSpeed; }
        set { Stat.RunSpeed = value; }
    }

    public int Hp
    {
        get { return Stat.Hp; }
        set { Stat.Hp = value; }
    }

    public int MaxHp
    {
        get { return Stat.MaxHp; }
        set { MaxHp = value; }
    }

    public float JumpForce
    {
        get { return Stat.JumpForce; }
        set { Stat.JumpForce = value; }
    }

    public PositionInfo _positionInfo = new PositionInfo();

    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            PlayerPos = new Vector3(value.PosX, value.PosY, value.PosZ);
            //MoveDir = new Vector2(value.MoveDirX, value.MoveDirZ);
            MoveDirX = value.MoveDirX;
            MoveDirZ = value.MoveDirZ;
        }
    }

    public Vector3 PlayerPos
    {
        get { return new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ); }

        set
        {
            if (PosInfo.Equals(value))
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            PosInfo.PosZ = value.z;
        }
    }

    public float MoveDirX
    {
        get { return PosInfo.MoveDirX; }
        set
        {
            if (PosInfo.MoveDirX == value)
                return;

            PosInfo.MoveDirX = value;
        }
    }

    public float MoveDirZ
    {
        get { return PosInfo.MoveDirZ; }
        set
        {
            if (PosInfo.MoveDirZ == value)
                return;

            PosInfo.MoveDirZ = value;
        }
    }

    public RotationInfo _rotationInfo = new RotationInfo();

    public RotationInfo RotInfo
    {
        get { return _rotationInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            PlayerRot = new Quaternion(value.RotX, value.RotY, value.RotZ, value.RotW);
            CameraRotationY = value.CameraRotY;
            CameraRotationX = value.CameraRotX;
        }
    }

    public Quaternion PlayerRot
    {
        get { return new Quaternion(RotInfo.RotX, RotInfo.RotY, RotInfo.RotZ, RotInfo.RotW); }

        set
        {
            if (RotInfo.Equals(value))
                return;

            RotInfo.RotX = value.x;
            RotInfo.RotY = value.y;
            RotInfo.RotZ = value.z;
            RotInfo.RotW = value.w;
        }
    }

    public float CameraRotationY
    {
        get { return RotInfo.CameraRotY; }
        set
        {
            if (RotInfo.CameraRotY == value)
                return;

            RotInfo.CameraRotY = value;
        }
    }

    public float CameraRotationX
    {
        get { return RotInfo.CameraRotX; }
        set
        {
            if (RotInfo.CameraRotX == value)
                return;

            RotInfo.CameraRotX = value;
        }
    }

    // 상태 변수
    protected bool isGround = true; // 지금 땅에 붙어있나?
    protected CapsuleCollider capsuleCollider;
    protected Vector3 _velocity;

    protected float applySpeed; // 지금 속도

    //필요한 컴포넌트
    [SerializeField]
    protected GameObject theCamera;

    // 카메라 한계
    [SerializeField]
    protected float cameraRotationLimit; // 마우스 올리면 360도 돌아가면 이상하니까. 돌아갈 수 있는 각도 제한
    protected float currentCameraRotationX = 0; // 초기엔 정면

    // 플레이어 체력에 관한 부분
    public delegate void OnHealthChangedDelegate();
    public OnHealthChangedDelegate onHealthChangedCallback;

    // 이전 위치와 회전값을 저장할 변수
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    #region Sigleton
    private static PlayerController instance;
    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerController>();
            return instance;
        }
    }
    #endregion


    public GameManager manager;

    // 넉백중인지 여부
    public bool isKnockedBack = false;
    protected float knockbackTimer = 0f;

    bool isDamage = false; // 데미지를 받고 있는 상태인가?

    public bool isdead = false; // 죽었나 확인

    protected Rigidbody myRigid; // 플레이어 실제 몸. 물리학 입히는거
    protected MeshRenderer[] meshs;

    protected void Start()
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

        InvokeRepeating("ForceMoveAndRot", 0.2f, 0.2f);
    }

    protected void Update() // 프레임마다 실행
    {
        if (!isdead)
        {
            if (!isKnockedBack)
            {
                //Move();

                // 서버로부터 전송받은 position과 rotation을 보간하여 자연스럽게 이동 및 회전
                if (!PosInfo.Equals(previousPosition))
                {
                    StartCoroutine(MoveToPosition(PosInfo));
                }

                if (!RotInfo.Equals(previousRotation))
                {
                    StartCoroutine(RotateToRotation(RotInfo));
                }
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
            //CameraRotation();
            //CharacterRotation();

            // 이전 위치와 회전값 업데이트
            previousPosition = PlayerPos;
            previousRotation = PlayerRot;
        }
    }

    IEnumerator MoveToPosition(PositionInfo targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.2f; // 0.2초 동안 보간

        Vector3 startPosition = transform.position;
        Vector3 target = new Vector3(targetPosition.PosX, targetPosition.PosY, targetPosition.PosZ);

        while (elapsedTime < duration)
        {
            // 보간된 위치로 이동
            transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치로 이동
        transform.position = target;
    }

    IEnumerator RotateToRotation(RotationInfo targetRotation)
    {
        float elapsedTime = 0f;
        float duration = 0.2f; // 0.2초 동안 보간

        Quaternion startRotation = transform.rotation;
        Quaternion target = new Quaternion(targetRotation.RotX, targetRotation.RotY, targetRotation.RotZ, targetRotation.RotW);

        while (elapsedTime < duration)
        {
            // 보간된 회전값으로 회전
            transform.rotation = Quaternion.Slerp(startRotation, target, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 회전값으로 회전
        transform.rotation = target;
    }



    protected void Move()
    {
        Vector3 _moveHorizontal = transform.right * MoveDirX; // transform은 유니티 창 기본 컴퍼넌트.  기본 (1,0,0)
        Vector3 _moveVertical = transform.forward * MoveDirZ; // 기본 (0,0,1) 위, 아래 구분

        _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; // normalized는 방향 유지되면서 합 1 나오도록 정규화

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        // Time.deltaTime : 그냥 움직이게 하면 텔포하는것처럼 보이니까 시간 쪼개기?
    }

    protected void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f); // 땅에 붙어있는지 확인. 대각선, 계단 고려 약간의 여유
        // 무조건 아래로 레이져 쏴야해서 Vector3.down, 가운대부터 캡슐 크기 반만큼 아래로 레이져,
    }

    protected void Jump()
    {
        myRigid.velocity = transform.up * JumpForce; // velocity는 내가 달리는 속도. 순간적으로 위로 가는 힘 가해서 공중으로
    }

    protected void Running() // 달리기
    {
        applySpeed = RunSpeed;
    }

    protected void RunningCancle() // 달리기 끝
    {
        applySpeed = Speed;
    }

    protected void CameraRotation() // 상하 카메라 회전
    {
        currentCameraRotationX -= CameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // 카메라 각도 최대값 고정

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f); //localEulerAngles : Rotation x,y,z

    }

    protected void CharacterRotation() // 좌우 카메라 회전. (캐릭터도 같이 회전됨)
    {
        Vector3 _characterRotationY = new Vector3(0f, CameraRotationY, 0f);
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        // 실제 유니티 내부에 회전은 quaternion 사용함. 우리가 구한 vector(euler)를 quaternion으로 바꿔주는 과정
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponController weaponController = other.GetComponent<WeaponController>();
        if (other.CompareTag("Weapon") && weaponController.Owner != gameObject) // 무기에 맞으면
        {
            if (!isDamage) // 무적시간이 아니면
            {
                float knockbackForce = weaponController.KnockbackForce;

                isKnockedBack = true; // 맞은 상태
                knockbackTimer = weaponController.KnockbackTime;

                // 망치 방향으로 밀어내기
                Vector3 pushDirection = transform.position - other.transform.position;
                pushDirection.Normalize();
                pushDirection *= knockbackForce;
                pushDirection.y = weaponController.KnockbackForceY;
                myRigid.AddForce(pushDirection, ForceMode.Impulse);

                Debug.Log($"KnockbackForce = {knockbackForce}");
                TakeDamage(1); // 데미지 입음

                if (Hp <= 0) Dodie();
                StartCoroutine(OnDamage());
                Debug.Log("피격됨");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("OutBottom"))
        {
            TakeDamage(Hp);
            Dodie();
        }
    }
    IEnumerator OnDamage()
    {
        isDamage = true;
        foreach (MeshRenderer mesh in meshs)
        {
            if (mesh.CompareTag("Weapon")) continue;
            mesh.material.color = Color.gray;
        }
        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            if (mesh.CompareTag("Weapon")) continue;
            mesh.material.color = Color.green;
        }
    }


    public void Heal(int health) // 피 회복
    {
        this.Hp += health;
        ClampHealth();
    }

    public void TakeDamage(int dmg) // 피격
    {
        this.Hp -= dmg;
        ClampHealth();
    }

    void ClampHealth()
    {
        Hp = Mathf.Clamp(Hp, 0, MaxHp);

        if (onHealthChangedCallback != null)
            onHealthChangedCallback.Invoke();
    }



    private void Dodie() // 피가 다 닳아서 죽음
    {
        if (isdead)
            return;

        isdead = true;
        manager.loseplayer = gameObject.name;
        manager.GameOver();

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }
        myRigid.constraints = ~RigidbodyConstraints.FreezeAll;
        myRigid.AddForce(10, 0, 0);
    }

    void ForceMoveAndRot()
    {
        myRigid.MovePosition(PlayerPos);
        myRigid.MoveRotation(PlayerRot);
        previousPosition = PlayerPos;
        previousRotation = PlayerRot;
    }
}
