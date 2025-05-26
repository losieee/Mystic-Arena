//using UnityEngine;

//public class WeaponDamage : MonoBehaviour
//{
//    private bool canDealDamage = false;
//    private float damageCooldown = 0.5f; // 0.5초 동안 데미지 무시
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
//                Debug.Log("무기 충돌로 플레이어에게 데미지!");
//                player.Damage(10f);
//                lastDamageTime = Time.time; // 마지막 데미지 시간 갱신
//            }
//        }
//    }
//}
