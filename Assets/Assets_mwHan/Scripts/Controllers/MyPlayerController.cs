using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MyPlayerController : PlayerController
{
    float _mouseRotationY;
    float _mouseRotationX;

    Transform mycanvas;
    Transform gamePanel;
    GameObject timeimg;
    GameObject diedImage;
    TextMeshProUGUI _lefttimetxt;
    TextMeshProUGUI _gameendtxt;

    public float _playtime;
    private float _lefttime;
    int WaitTime = 3;

    [SerializeField]
    protected float lookSensitivity; // 카메라 민감도

    // Start is called before the first frame update
    new void Start()
    {
        _playtime = 60f;
        _lefttime = _playtime;
        myRigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        weaponController = GetComponentInChildren<WeaponController>();

        mycanvas = transform.Find("Canvas");
        gamePanel = mycanvas.Find("Game Panel");
        timeimg = gamePanel.Find("Time").gameObject;
        diedImage = gamePanel.Find("Died image").gameObject;
        _lefttimetxt = timeimg.GetComponentInChildren<TextMeshProUGUI>();
        _gameendtxt = diedImage.GetComponentInChildren<TextMeshProUGUI>();

        applySpeed = Speed;

        InvokeRepeating("UpdatePlayerInfo", 0f, 0.2f);
        InvokeRepeating("SendPlayerMoving", 0f, 0.2f);
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

            GetAttackKey();
            if (doAttack)
            {
                doAttack = false;
                C_DoAttack atkPacket = new C_DoAttack();
                Managers.Network.Send(atkPacket);
                weaponController.Attack();
            }
            //base.Update();
        }
    }

    private void LateUpdate()
    {
        _lefttime -= Time.deltaTime;

        int sec = (int)_lefttime % 60;
        _lefttimetxt.text = string.Format("{0:00}", sec);
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
            GameEnd();
        }
        //패배 테스트 용
        if (Input.GetKeyDown(KeyCode.Return) && isGround)
        {
            Jump();
            _gameendtxt = diedImage.GetComponentInChildren<TextMeshProUGUI>();
            _gameendtxt.text = string.Format("Game End\n Player Win");
            diedImage.gameObject.SetActive(true);
            stone.winner = stone.enemy;
            StartCoroutine(ChangeScene());
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

    private void GetAttackKey()
    {
        doAttack = Input.GetMouseButtonDown(0);
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

    void GameEnd()
    {
        _gameendtxt = diedImage.GetComponentInChildren<TextMeshProUGUI>();
        _gameendtxt.text = string.Format("Game End\n Player Win");
        diedImage.gameObject.SetActive(true);
        stone.winner = YutGameManager.Instance.GetTurn();
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        stone.isFight = false;
        yield return new WaitForSeconds(WaitTime);
        //SceneManager.LoadScene("YutPlay");
        YutGameManager.Instance.StartMainGame();
    }
}

