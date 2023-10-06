using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // PlayerController 스크립트 참조
    public PlayerController playerController;

    public GameObject owner;
    public GameObject Owner 
    { 
        get { return owner; }
        set { owner = value; }
    }

    public GameObject Hammer;
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
        Owner = playerController.gameObject;
    }

    public void Attack()
    {
        playerController.canattack = false;
        Animator anim = Hammer.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        StartCoroutine(ResetAttackCooldown());
    }

    protected IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(AttackCooldown);
        playerController.canattack = true;
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
}