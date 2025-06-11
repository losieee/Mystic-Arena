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
                boss.TakeDamage(weaponSO.baseDamage);

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

            Monster1 enemy1 = other.GetComponentInParent<Monster1>();
            Monster2 enemy2 = other.GetComponentInParent<Monster2>();
            Monster3 enemy3 = other.GetComponentInParent<Monster3>();
            Monster4 enemy4 = other.GetComponentInParent<Monster4>();

            if (enemy1 != null)
            {
                enemy1.TakeDamage(weaponSO.baseDamage);

                if (player != null && player.attackSound != null)
                {
                    AudioSource.PlayClipAtPoint(player.attackSound, enemy1.transform.position, 1f);
                }

                if (hit_Particle != null)
                {
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    GameObject hitEffect = Instantiate(hit_Particle, hitPoint, Quaternion.identity);
                    Destroy(hitEffect, 1f);
                }
            }
            else if (enemy3 != null)
            {
                enemy3.TakeDamage(playerSO.playerAttack);

                if (player != null && player.attackSound != null)
                {
                    AudioSource.PlayClipAtPoint(player.attackSound, enemy3.transform.position, 1f);
                }

                if (hit_Particle != null)
                {
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    GameObject hitEffect = Instantiate(hit_Particle, hitPoint, Quaternion.identity);
                    Destroy(hitEffect, 1f);
                }
            }
            else if (enemy2 != null)
            {
                enemy2.TakeDamage(playerSO.playerAttack);

                if (player != null && player.attackSound != null)
                {
                    AudioSource.PlayClipAtPoint(player.attackSound, enemy2.transform.position, 1f);
                }

                if (hit_Particle != null)
                {
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    GameObject hitEffect = Instantiate(hit_Particle, hitPoint, Quaternion.identity);
                    Destroy(hitEffect, 1f);
                }
            }
            else if (enemy4 != null)
            {
                enemy4.TakeDamage(playerSO.playerAttack);

                if (player != null && player.attackSound != null)
                {
                    AudioSource.PlayClipAtPoint(player.attackSound, enemy4.transform.position, 1f);
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
