using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttackType
{
    Consumable,
    Melee,
    Ranged
}

public class EnemyData : MonoBehaviour
{
    public int id;
    public string monsterName;
    public string spawnStage;
    public string monsterAttackType;
    [SerializeField]
    public EnemyAttackType enemyAttackType;
    public int monsterHp;
    public int monsterAttack;
    public int monsterAttackInterval;

    public void InitalizeEnums()
    {
        if (Enum.TryParse(monsterAttackType, out EnemyAttackType parsedType))
        {
            enemyAttackType = parsedType;
        }
        else
        {
            Debug.Log($"몬스터 공격 '{enemyAttackType}'에 유효하지 않은 몬스터 공격 타입 : {monsterAttackType}");
            enemyAttackType = EnemyAttackType.Consumable;
        }
    }
}
