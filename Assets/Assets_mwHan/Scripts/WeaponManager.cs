using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // 무기 중복 교체 실행 방지
    public static bool isChangeWeapon = false; // true 되면 무기 교체 안되게 막을거임

    // 현재 무기와 현재 무기 애니메이션
    public static Transform currentWeapon;
    // public static Animator currentWeaponAnim;

    // 현재 무기 타입
    [SerializeField]
    private string currentWeaponType;

    // 무기 교체 딜레이 무기 교체 완전 끝난 시점
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    // 무기 종류들 전부 관리
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private Hand[] hands;

    // 관리 차원에서 무기 쉽게 접근 가능하도록
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, Hand> handDictionary = new Dictionary<string, Hand>();

    // 필요한 컴포넌트.
    [SerializeField]
    private GunController theGunController;
    [SerializeField]
    private HandController theHandController;


    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
        
        for (int i = 0; i < hands.Length; i++)
        {
            handDictionary.Add(hands[i].handName, hands[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isChangeWeapon);
        if (!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartCoroutine(ChangeWeaponCoroutine("HAND", "맨손"));
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(ChangeWeaponCoroutine("GUN", "Yut"));
        }
    }

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CanclePreWeaponAction();
        WeaponChange(_type, _name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = _type;
        isChangeWeapon = false;
    }

    private void CanclePreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                GunController.isActivate = false;
                break;
            case "HAND":
                HandController.isActivate = false;
                break;
        }
    }

    private void WeaponChange(string _type, string _name)
    {
        if (_type == "GUN")
            theGunController.GunChange(gunDictionary[_name]);

        else if (_type == "HAND")
            theHandController.HandChange(handDictionary[_name]);

    }
}
