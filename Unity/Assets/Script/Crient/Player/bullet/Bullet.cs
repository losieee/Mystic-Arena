//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//public enum AttackType
//{
//    Normal_Attack,
//    Qkey_Attack,
//    Ekey_Attack,
//}

//public class Bullet : MonoBehaviour
//{
//    public AttackType attackType;
//    //public NormalEnemy NormalEnemy;              // 추후 몬스터 로직 오브젝트 가져오기
//    public PlayerSO playerSO;
//    public EnemySO enemySO;


//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Enemy"))
//        {
//            switch (attackType)
//            {
//                case AttackType.Normal_Attack:
//                    NormalAttack();
//                    break;
//                case AttackType.Qkey_Attack:
//                    QkeyAttack();
//                    break;
//                case AttackType.Ekey_Attack:
//                    EkeyAttack();
//                    break;
//            }
//        }

//        if (collision.gameObject.CompareTag("Player"))
//        {
//            playerSO.player_CurrHp -= enemySO.enemy_Damage;
//            return;
//        }

//    }



//    public void NormalAttack()
//    {
//        //일반 공격 처리
//        Debug.Log("일반공격 데미지");
//    }

//    public void QkeyAttack()
//    {
//        //Q 공격 처리
//        Debug.Log("Q스킬 공격 데미지");
//    }

//    public void EkeyAttack()
//    {
//        //E 공격 처리
//        Debug.Log("E스킬 공격 데미지");
//    }
//}
