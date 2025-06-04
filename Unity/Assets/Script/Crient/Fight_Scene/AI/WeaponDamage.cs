using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float currentDamage = 50f;       // �⺻ ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(currentDamage);
            }
        }
    }
}
