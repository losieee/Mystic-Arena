//using UnityEngine;

//public class WeaponDamage : MonoBehaviour
//{
//    private bool canDealDamage = false;
//    private float damageCooldown = 0.5f; // 0.5�� ���� ������ ����
//    private float lastDamageTime = -Mathf.Infinity;

//    public void EnableDamage()
//    {
//        canDealDamage = true;
//    }

//    public void DisableDamage()
//    {
//        canDealDamage = false;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (Time.time - lastDamageTime < damageCooldown) return;

//        if (other.CompareTag("Player"))
//        {
//            var player = other.GetComponentInParent<Tutorial_Knight_Move>();
//            if (player != null && !player.isDead)
//            {
//                Debug.Log("���� �浹�� �÷��̾�� ������!");
//                player.Damage(10f);
//                lastDamageTime = Time.time; // ������ ������ �ð� ����
//            }
//        }
//    }
//}
