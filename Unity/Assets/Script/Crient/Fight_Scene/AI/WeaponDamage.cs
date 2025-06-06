using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float currentDamage = 50f;       // 기본 공격
    public GameObject hit_Particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(currentDamage);

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
