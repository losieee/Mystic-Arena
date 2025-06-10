using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public WeaponSO weaponSO;
    public PlayerSO playerSO;       
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
                boss.TakeDamage(playerSO.playerAttack += 70);

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

            Monster1 enemy = other.GetComponentInParent<Monster1>();

            if (enemy != null)
            {
                enemy.TakeDamage(playerSO.playerAttack);

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
