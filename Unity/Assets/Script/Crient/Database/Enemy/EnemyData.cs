using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttackType
{
    Consumable,                     // �⺻
    Melee,                          // ���� ����
    Ranged                          // ���Ÿ� ����
}

public class EnemyData : MonoBehaviour
{
    public int id;
    public string monsterName;
    public string spawnStage;
    public string monsterAttackType;
    [NonSerialized]
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
            Debug.Log($"���� ���� '{enemyAttackType}'�� ��ȿ���� ���� ���� ���� Ÿ�� : {monsterAttackType}");
            enemyAttackType = EnemyAttackType.Consumable;
        }
    }
}
