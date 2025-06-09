using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnmeyDatabase", menuName ="Database/EnmeyDatabase")]
public class EnemyDatabaseSO : ScriptableObject
{
    public List<EnemySO> enemys = new List<EnemySO>();          

    // ĳ���� ���� ����
    private Dictionary<int, EnemySO> enemyByid;                       // ID�� ������ ã�� ���� ĳ��
    private Dictionary<string, EnemySO> enemyByName;                   // �̸����� ������ ã��

    private void Initialize()
    {
        enemyByid = new Dictionary<int, EnemySO>();                   // ���� ���� �߱� ������ Dictionary Ȱ��
        enemyByName = new Dictionary<string, EnemySO>();

        foreach (var enemy in enemys)                                   // enemys ����Ʈ�� ���� �Ǿ� �ִ°��� ������  Dictionary�� �Է��Ѵ�.                         
        {
            enemyByid[enemy.id] = enemy;
            enemyByName[enemy.monsterName] = enemy;
        }
    }

    // ID�� ������ ã��
    public EnemySO GetEnemyByld(int id)
    {
        if (enemyByid == null)                                         //weaponByld  �� ĳ���� �Ǿ� ���� �ʴٸ� �ʱ�ȭ �Ѵ�.
        {
            Initialize();
        }

        if (enemyByid.TryGetValue(id, out EnemySO enemy))              // id ���� ã�Ƽ� WeaponSO �� ���� �Ѵ�.
            return enemy;

        return null;                                                 // ���� ��� NULL
    }

    public EnemySO GetItemByName(string name)
    {
        if (enemyByName == null)                                         //enemyByName  �� ĳ���� �Ǿ� ���� �ʴٸ� �ʱ�ȭ �Ѵ�.
        {
            Initialize();
        }

        if (enemyByName.TryGetValue(name, out EnemySO enemy))              // name ���� ã�Ƽ� WeaponSO �� ���� �Ѵ�.
            return enemy;

        return null;                                                 // ���� ��� NULL
    }

    // Ÿ������ ������ ���͸�
    public List<EnemySO> GetitemByType(EnemyAttackType type)
    {
        return enemys.FindAll(enemy => enemy.enemyAttackType == type);
    }
}
