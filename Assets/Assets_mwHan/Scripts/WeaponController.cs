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

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && !PlayerController.Instance.isdead)
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
}
