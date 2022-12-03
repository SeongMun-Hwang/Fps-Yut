using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    // Ȱ��ȭ ����.
    public static bool isActivate = false;

    // ���� �������� Hand�� Ÿ�� ����
    [SerializeField]
    private Hand currentHand;

    // ������?
    private bool isAttack = false; // ���� ������?
    private bool isSwing = false; // �ֵθ��� �ֳ�?

    private RaycastHit hitInfo; // �� �������� ��ҳ�

    // Update is called once per frame
    void Update()
    {
        if (isActivate)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!isAttack)
            {
                // �ڷ�ƾ ����
                StartCoroutine(AttackCoroutine());

            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttack = true;

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        // ���� Ȱ��ȭ ����
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }

    IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                // �浹��
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    private bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range)) { // ������ ��� �� ��ġ�� �� ������ �¾�
            return true;
        }
        return false;
    }

    public void HandChange(Hand _hand)
    {
        if (WeaponManager.currentWeapon != null) // ������ ����ִ� ���
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentHand = _hand;
        WeaponManager.currentWeapon = currentHand.GetComponent<Transform>();
        // WeaponManager.currentWeaponAnim = currentGun.anim;

        currentHand.transform.localPosition = Vector3.zero;
        currentHand.gameObject.SetActive(true);
        isActivate = true;
    }
}