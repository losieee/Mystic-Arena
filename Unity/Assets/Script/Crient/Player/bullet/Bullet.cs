using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AttackType
{
    Normal_Attack,
    Qkey_Attack,
    Wkey_Attack,
    Ekey_Attack,
}

public class Bullet : MonoBehaviour
{
    public AttackType attackType;
    // public NormalEnemy NormalEnemy;              // 추후 몬스터 로직 오브젝트 가져오기
    public PlayerSO playerSO;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            switch (attackType)
            {
                case AttackType.Normal_Attack:
                    NormalAttack();

                    break;
                case AttackType.Qkey_Attack:
                    QkeyAttack();
                    break;
                case AttackType.Wkey_Attack:
                    WkeyAttack();
                    break;
                case AttackType.Ekey_Attack:
                    EkeyAttack();
                    break;
            }
        }

    }

    public void NormalAttack()
    {
        //일반 공격 처리
        Debug.Log("일반공격 데미지");
    }

    public void QkeyAttack()
    {
        //Q 공격 처리
        Debug.Log("Q스킬 공격 데미지");
    }

    public void WkeyAttack()
    {
        //W 공격 처리
        Debug.Log("W스킬 공격 데미지");
    }

    public void EkeyAttack()
    {
        //E 공격 처리
        Debug.Log("E스킬 공격 데미지");
    }
}
