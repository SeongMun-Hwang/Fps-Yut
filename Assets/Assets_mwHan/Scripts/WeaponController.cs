using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject Hammer;
    public bool canAttack = true;
    public float AttackCooldown = 1f;
    //public AudioClip HammerAttackSound;
    public bool IsAttacking = false;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public float KnockbackForce = 15f;
    public float KnockbackForceY = 3f;
    public float KnockbackTime = 2f;

    // PlayerController 스크립트 참조
    public PlayerController playerController;

    // 플레이어 맞은 후 움직일 수 없도록 하는 변수, 타이머
    private bool isPlayerHit = false;
    private float playerHitTimer = 0f;
    public float playerHitDuration = 3f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canAttack)
            {
                HammerAttack();
            }
        }
    }

    public void HammerAttack()
    {
        canAttack = false;
        Animator anim = Hammer.GetComponent<Animator>();
        //AudioSource ac = GetComponent<AudioSource>();
        anim.SetTrigger("Attack");
        //ac.PlayOneShot(HammerAttackSound);
        StartCoroutine(ResetAttackCooldown());

    }

    IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(AttackCooldown);
        canAttack = true;
    }

    IEnumerator ResetAttackBool()
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

    public void OnPlayerHit()
    {
        isPlayerHit = true;
        playerHitTimer = playerHitDuration;
        playerController.isKnockedBack = true;
    }
}
