using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float currentDamage = 50f;       // 기본 공격
    public GameObject hit_Particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            var player = GetComponentInParent<Fight_Demo>();
            if (player != null)
            {
                player.SetCurrentAttackHit(true);
            }

            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(currentDamage);

                if (player != null && player.attackSound != null)
                {
                    AudioSource.PlayClipAtPoint(player.attackSound, boss.transform.position, 1f);
                }

                if (hit_Particle != null)
                {
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    GameObject hitEffect = Instantiate(hit_Particle, hitPoint, Quaternion.identity);
                    Destroy(hitEffect, 1f);
                }
            }
        }
        else if (other.CompareTag("vicinityEnemy"))
        {
            var player = GetComponentInParent<Fight_Demo>();
            if (player != null)
            {
                player.SetCurrentAttackHit(true);
            }

            AIAttack enemy = other.GetComponentInParent<AIAttack>();

            if (enemy != null)
            {
                enemy.TakeDamage(10f);

                if (player != null && player.attackSound != null)
                {
                    AudioSource.PlayClipAtPoint(player.attackSound, enemy.transform.position, 1f);
                }

                if (hit_Particle != null)
                {
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    GameObject hitEffect = Instantiate(hit_Particle, hitPoint, Quaternion.identity);
                    Destroy(hitEffect, 1f);
                }
            }
        }
    }
}
