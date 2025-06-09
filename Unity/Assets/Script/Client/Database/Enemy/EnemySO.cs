using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Enmey", menuName = "Database/Enmey")]
public class EnemySO : ScriptableObject
{
    // ���� GameDB Excel ���� ������ ������ ���� �Ѵ�.
    public int id;
    public string monsterName;
    public string spawnStage;
    public string monsterAttackType;
    public EnemyAttackType enemyAttackType;
    public int monsterHp;
    public int monsterAttack;
    public int monsterAttackInterval;


    public override string ToString()
    {
        return $"[{id}] :  {monsterName}���ʹ� {enemyAttackType} Ÿ�� �Դϴ�.";
    }
}
