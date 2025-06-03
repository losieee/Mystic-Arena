using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Enmey", menuName = "Database/Enmey")]
public class EnemySO : ScriptableObject
{
    // 기존 GameDB Excel 에서 선언한 변수를 나열 한다.
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
        return $"[{id}] :  {monsterName}몬스터는 {enemyAttackType} 타입 입니다.";
    }
}
