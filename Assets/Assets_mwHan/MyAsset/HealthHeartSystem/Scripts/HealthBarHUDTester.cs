/*
 *  Author: ariel oliveira [o.arielg@gmail.com]
 */

using UnityEngine;

public class HealthBarHUDTester : MonoBehaviour
{
    public void Heal(float health)
    {
        PlayerController.Instance.Heal(health);
    }

    public void Hurt(float dmg)
    {
        PlayerController.Instance.TakeDamage(dmg);
    }
}
