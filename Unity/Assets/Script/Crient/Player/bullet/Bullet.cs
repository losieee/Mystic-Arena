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
//    //public NormalEnemy NormalEnemy;              // ���� ���� ���� ������Ʈ ��������
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
//        //�Ϲ� ���� ó��
//        Debug.Log("�Ϲݰ��� ������");
//    }

//    public void QkeyAttack()
//    {
//        //Q ���� ó��
//        Debug.Log("Q��ų ���� ������");
//    }

//    public void EkeyAttack()
//    {
//        //E ���� ó��
//        Debug.Log("E��ų ���� ������");
//    }
//}
