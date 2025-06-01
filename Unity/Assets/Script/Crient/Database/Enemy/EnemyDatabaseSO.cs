using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnmeyDatabase", menuName ="Database/EnmeyDatabase")]
public class EnemyDatabaseSO : ScriptableObject
{
    public List<EnemySO> enemys = new List<EnemySO>();          

    // 캐싱을 위한 사전
    private Dictionary<int, EnemySO> enemyByid;                       // ID로 아이템 찾기 위한 캐싱
    private Dictionary<string, EnemySO> enemyByName;                   // 이름으로 아이템 찾기

    private void Initialize()
    {
        enemyByid = new Dictionary<int, EnemySO>();                   // 위에 선언만 했기 때문에 Dictionary 활당
        enemyByName = new Dictionary<string, EnemySO>();

        foreach (var enemy in enemys)                                   // enemys 리스트에 선어 되어 있는것을 가지고  Dictionary에 입력한다.                         
        {
            enemyByid[enemy.id] = enemy;
            enemyByName[enemy.monsterName] = enemy;
        }
    }

    // ID로 아이템 찾기
    public EnemySO GetEnemyByld(int id)
    {
        if (enemyByid == null)                                         //weaponByld  가 캐싱이 되어 있지 않다면 초기화 한다.
        {
            Initialize();
        }

        if (enemyByid.TryGetValue(id, out EnemySO enemy))              // id 값을 찾아서 WeaponSO 를 리턴 한다.
            return enemy;

        return null;                                                 // 없을 경루 NULL
    }

    public EnemySO GetItemByName(string name)
    {
        if (enemyByName == null)                                         //enemyByName  가 캐싱이 되어 있지 않다면 초기화 한다.
        {
            Initialize();
        }

        if (enemyByName.TryGetValue(name, out EnemySO enemy))              // name 값을 찾아서 WeaponSO 를 리턴 한다.
            return enemy;

        return null;                                                 // 없을 경루 NULL
    }

    // 타입으로 아이템 필터링
    public List<EnemySO> GetitemByType(EnemyAttackType type)
    {
        return enemys.FindAll(enemy => enemy.enemyAttackType == type);
    }
}
