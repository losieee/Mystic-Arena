using UnityEngine;

public class AIHitBox : MonoBehaviour
{
    public float damageAmount = 10f; // 데미지 값 (원하는 대로 조절 가능)

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
