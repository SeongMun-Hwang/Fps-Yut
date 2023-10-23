using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 필요한 네임스페이스 추가
using UnityEngine.EventSystems;

public class YutScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static Vector3[] YutVelocity;
    public Rigidbody[] rb;
    public Button throwButton; // 던지기 버튼 참조
    public Image gaugeImage;  // 게이지 이미지 참조
    private float currentForce; // 누적된 힘
    private const float maxForce = 100f; // 최대 힘
    private bool isOverThrowButton = false;
    private bool isIncreasing = true;

    void Start()
    {
        UpdateGaugeFill();
        YutVelocity = new Vector3[4];
        throwButton.onClick.AddListener(ThrowYuts); // 버튼 클릭 리스너 추가
    }

    void Update()
    {
        if (isOverThrowButton)
        {
            HandleForceAccumulation();
            UpdateGaugeFill();
        }
    }
    public void PublicOnPointerDown()
    {
        OnPointerDown(null);
    }

    public void PublicOnPointerUp()
    {
        OnPointerUp(null);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isOverThrowButton = true;
    }

    // 마우스가 버튼에서 나갔을 때
    public void OnPointerUp(PointerEventData eventData)
    {
        isOverThrowButton = false;
        currentForce = 0; // 버튼에서 손을 떼면 게이지 리셋
        YutText.result = 0;
        UpdateGaugeFill();
    }

    void HandleForceAccumulation()
    {
        if (isIncreasing)
        {
            currentForce += Time.deltaTime * 100;
            if (currentForce >= maxForce)
            {
                currentForce = maxForce;
                isIncreasing = false;
            }
        }
        else
        {
            currentForce -= Time.deltaTime * 100;
            if (currentForce <= 0)
            {
                currentForce = 0;
                isIncreasing = true;
            }
        }
    }

    void UpdateGaugeFill()
    {
        float normalizedForce = currentForce / maxForce;
        gaugeImage.fillAmount = normalizedForce; // 게이지의 채워진 정도를 업데이트
    }

    void ThrowYuts()
    {
        YutText.result = 0;
        for (int i = 0; i < 4; i++)
        {
            YutCheck.isAdd[i] = false;
        }
        for (int i = 0; i < 4; i++)
        {
            rb[i].position = new Vector3(8, 2+i, -8);
            rb[i].rotation = Quaternion.identity;
            rb[i].AddForce(transform.forward * currentForce); // 플레이어가 바라보는 방향으로 힘을 가합니다.
            float dirX = Random.Range(0, 500);
            float dirY = Random.Range(0, 500);
            float dirZ = Random.Range(0, 500);
            rb[i].AddTorque(dirX, dirY, dirZ);
        }
    }
}
