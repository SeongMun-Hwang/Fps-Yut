using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 플레이어 체력에 관한 부분
    public delegate void OnHealthChangedDelegate();
    public OnHealthChangedDelegate onHealthChangedCallback;

    public int Id { get; set; }

    protected bool _updated = false;

    Vector3 PlayerPos
    {
        get { return new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ); }

        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y && PosInfo.PosZ == value.z)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            PosInfo.PosZ = value.z;
            _updated = true;
        }
    }

    Vector3 PlayerRot
    {
        get { return new Vector3(RotInfo.RotX, RotInfo.RotY, RotInfo.RotZ); }

        set
        {
            if (RotInfo.RotX == value.x && RotInfo.RotY == value.y && RotInfo.RotZ == value.z)
                return;

            RotInfo.RotX = value.x;
            RotInfo.RotY = value.y;
            RotInfo.RotZ = value.z;
            _updated = true;
        }
    }

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

    PositionInfo _positionInfo = new PositionInfo();

    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            PlayerPos = new Vector3(value.PosX, value.PosY, value.PosZ);
        }
    }

    RotationInfo _rotationInfo = new RotationInfo();

    public RotationInfo RotInfo
    {
        get { return _rotationInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            PlayerRot = new Vector3(value.RotX, value.RotY, value.RotZ);
        }
    }

    [SerializeField]
    protected float health;
    [SerializeField]
    protected float maxHealth;
    public float Health { get { return health; } }
    public float MaxHealth { get { return maxHealth; } }

    public GameManager manager;

    // 넉백중인지 여부
    public bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    bool isDamage = false; // 데미지를 받고 있는 상태인가?

    public bool isdead = false; // 죽었나 확인

    protected Rigidbody myRigid; // 플레이어 실제 몸. 물리학 입히는거
    MeshRenderer[] meshs;

    protected void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();

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
    }

    protected void Update() // 프레임마다 실행
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
        }
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

                if (Health <= 0) Dodie();
                StartCoroutine(OnDamage());
                Debug.Log("피격됨");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("OutBottom"))
        {
            TakeDamage(health);
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


    public void Heal(float health) // 피 회복
    {
        this.health += health;
        ClampHealth();
    }

    public void TakeDamage(float dmg) // 피격
    {
        health -= dmg;
        ClampHealth();
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        if (onHealthChangedCallback != null)
            onHealthChangedCallback.Invoke();
    }



    private void Dodie() // 피가 다 닳아서 죽음
    {
        if (isdead)
            return;

        isdead = true;
        manager.loseplayer = this.gameObject.name;
        manager.GameOver();

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }
        myRigid.constraints = ~RigidbodyConstraints.FreezeAll;
        myRigid.AddForce(10, 0, 0);

    }
}
