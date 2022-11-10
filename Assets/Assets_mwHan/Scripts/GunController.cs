using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // 활성화 여부
    public static bool isActivate = true;

    [SerializeField]
    private Gun currentGun; // 현재 총

    private float currentFireRate; // 연사 속도 계산

    private AudioSource audioSource; // 효과음

    public Transform bulletPos; // 총알이 생성될 위치
    public GameObject bullet;
    public int bulletSpeed;

    //충돌 정보 받아옴
    private RaycastHit hitInfo;

    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCam;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        // WeaponManager.currentWeaponAnim = currentGun.anim;
    }

    private void Update()
    {
        if (isActivate)
        {
            GunFireRateCalc();
            TryFire();
        }
    }

    // 연사속도 재계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 1초에 1 감소
    }

    // 발사 시도
    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <=0)
        {
            Fire();
        }
    }

    // 발사 전
    private void Fire()
    {
        currentFireRate = currentGun.fireRate;
        Shoot();
    }

    // 발사 후
    private void Shoot()
    {
        PlaySE(currentGun.fire_Sound);
        //Hit();
        StartCoroutine(ShootBullet());


    }

    IEnumerator ShootBullet()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * bulletSpeed;
        yield return null;
    }

    private void Hit()
    {
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo, currentGun.range))
        {
            Debug.Log(hitInfo.transform.name);
        }
    }

    // 사운드 이펙트
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public void GunChange(Gun _gun)
    {
        if(WeaponManager.currentWeapon != null) // 뭔가를 들고있는 경우
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        // WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;
    }
}
