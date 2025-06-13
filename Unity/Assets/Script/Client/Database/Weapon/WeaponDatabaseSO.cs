using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Database/WeaponDatabase")]
public class WeaponDatabaseSO : ScriptableObject
{
    public List<WeaponSO> weapons = new List<WeaponSO>();                // WezponSO ����Ʈ �����Ѵ�.

    // ĳ���� ���� ����
    private Dictionary<int, WeaponSO> weaponByld;                       // ID�� ������ ã�� ���� ĳ��
    private Dictionary<string, WeaponSO> weaponByName;                   // �̸����� ������ ã��

    private void Initialize()
    {
        weaponByld = new Dictionary<int, WeaponSO>();                   // ���� ���� �߱� ������ Dictionary Ȱ��
        weaponByName = new Dictionary<string, WeaponSO>();

        foreach(var weapon in weapons)                                   // weapons ����Ʈ�� ���� �Ǿ� �ִ°��� ������  Dictionary�� �Է��Ѵ�.                         
        {
            weaponByld[weapon.id] = weapon;
            weaponByName[weapon.weaponName] = weapon;
        }
    }

    // ID�� ������ ã��
    public WeaponSO GetItemByld(int id)
    {
        if (weaponByld == null)                                         //weaponByld  �� ĳ���� �Ǿ� ���� �ʴٸ� �ʱ�ȭ �Ѵ�.
        {
            Initialize();
        }

        if (weaponByld.TryGetValue(id, out WeaponSO weapon))              // id ���� ã�Ƽ� WeaponSO �� ���� �Ѵ�.
            return weapon;

        return null;                                                 // ���� ��� NULL
    }

    public WeaponSO GetItemByName(string name)
    {
        if (weaponByName == null)                                         //weaponByName  �� ĳ���� �Ǿ� ���� �ʴٸ� �ʱ�ȭ �Ѵ�.
        {
            Initialize();
        }

        if (weaponByName.TryGetValue(name, out WeaponSO weapon))              // name ���� ã�Ƽ� WeaponSO �� ���� �Ѵ�.
            return weapon;

        return null;                                                 // ���� ��� NULL
    }

    // Ÿ������ ������ ���͸�
    public List<WeaponSO> GetitemByType(WeaponType type)
    {
        return weapons.FindAll(weapon => weapon.WeapnType == type);
    }
}
