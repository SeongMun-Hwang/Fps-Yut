using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // Ȱ��ȭ ����
    public static bool isActivate = true;

    [SerializeField]
    private Gun currentGun; // ���� ��

    private float currentFireRate; // ���� �ӵ� ���

    private AudioSource audioSource; // ȿ����

    public Transform bulletPos; // �Ѿ��� ������ ��ġ
    public GameObject bullet;
    public int bulletSpeed;

    //�浹 ���� �޾ƿ�
    private RaycastHit hitInfo;

    // �ʿ��� ������Ʈ
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

    // ����ӵ� ����
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 1�ʿ� 1 ����
    }

    // �߻� �õ�
    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <=0)
        {
            Fire();
        }
    }

    // �߻� ��
    private void Fire()
    {
        currentFireRate = currentGun.fireRate;
        Shoot();
    }

    // �߻� ��
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

    // ���� ����Ʈ
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public void GunChange(Gun _gun)
    {
        if(WeaponManager.currentWeapon != null) // ������ ����ִ� ���
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
