using UnityEngine;

public class AIHitBox : MonoBehaviour
{
    public float damageAmount = 10f; // ������ �� (���ϴ� ��� ���� ����)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            Fight_Demo player = other.GetComponentInParent<Fight_Demo>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }
}
