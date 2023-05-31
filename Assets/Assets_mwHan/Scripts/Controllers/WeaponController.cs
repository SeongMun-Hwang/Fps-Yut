using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // PlayerController 스크립트 참조
    public PlayerController playerController;

    public GameObject owner;
    public GameObject Owner { get { return owner; } }

    public GameObject Hammer;
    public bool canAttack = true;
    public float AttackCooldown = 1f;


    public bool IsAttacking = false;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public float KnockbackForce = 15f;
    public float KnockbackForceY = 5f;
    public float KnockbackTime = 2f;

    // 플레이어 맞은 후 움직일 수 없도록 하는 변수, 타이머
    public float playerHitDuration = 3f;

    protected void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    protected void Update()
    {
        if (!playerController.isdead)  // 예시: 플레이어가 제어 가능한 상태일 때만 입력 받음
        {
            if (Input.GetMouseButtonDown(0))
            {
                //if (canAttack && IsOwner())
                if (canAttack)
                {
                    Attack();
                }
            }
        }
    }

    public void Attack()
    {
        canAttack = false;
        Animator anim = Hammer.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        StartCoroutine(ResetAttackCooldown());
    }

    protected IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(AttackCooldown);
        canAttack = true;
    }

    protected IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(0.1f);
        trailEffect.enabled = true;
        IsAttacking = true;
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.25f);
        IsAttacking = false;
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.25f);
        trailEffect.enabled = false;
    }

    //protected bool IsOwner()
    //{
    //    // 현재 무기의 소유자인지 확인하는 로직을 구현합니다.
    //    // 예시로 owner 변수를 사용하여 비교하도록 작성하였습니다.
    //    // 실제로는 무기와 소유자 사이의 관계를 어떻게 정의했는지에 따라 구현 방식이 달라질 수 있습니다.
    //    GameObject currentPlayer = GameManager.instance.GetPlayer(); // 현재 플레이어를 가져옵니다.

    //    if (currentPlayer != null && currentPlayer == owner)
    //    {
    //        return true; // 현재 플레이어가 무기의 소유자인 경우
    //    }
    //    else
    //    {
    //        return false; // 현재 플레이어가 무기의 소유자가 아닌 경우
    //    }
    //}
}