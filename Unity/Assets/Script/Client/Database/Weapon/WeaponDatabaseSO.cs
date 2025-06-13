using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Database/WeaponDatabase")]
public class WeaponDatabaseSO : ScriptableObject
{
    public List<WeaponSO> weapons = new List<WeaponSO>();                // WezponSO 리스트 관리한다.

    // 캐싱을 위한 사전
    private Dictionary<int, WeaponSO> weaponByld;                       // ID로 아이템 찾기 위한 캐싱
    private Dictionary<string, WeaponSO> weaponByName;                   // 이름으로 아이템 찾기

    private void Initialize()
    {
        weaponByld = new Dictionary<int, WeaponSO>();                   // 위에 선언만 했기 때문에 Dictionary 활당
        weaponByName = new Dictionary<string, WeaponSO>();

        foreach(var weapon in weapons)                                   // weapons 리스트에 선어 되어 있는것을 가지고  Dictionary에 입력한다.                         
        {
            weaponByld[weapon.id] = weapon;
            weaponByName[weapon.weaponName] = weapon;
        }
    }

    // ID로 아이템 찾기
    public WeaponSO GetItemByld(int id)
    {
        if (weaponByld == null)                                         //weaponByld  가 캐싱이 되어 있지 않다면 초기화 한다.
        {
            Initialize();
        }

        if (weaponByld.TryGetValue(id, out WeaponSO weapon))              // id 값을 찾아서 WeaponSO 를 리턴 한다.
            return weapon;

        return null;                                                 // 없을 경루 NULL
    }

    public WeaponSO GetItemByName(string name)
    {
        if (weaponByName == null)                                         //weaponByName  가 캐싱이 되어 있지 않다면 초기화 한다.
        {
            Initialize();
        }

        if (weaponByName.TryGetValue(name, out WeaponSO weapon))              // name 값을 찾아서 WeaponSO 를 리턴 한다.
            return weapon;

        return null;                                                 // 없을 경루 NULL
    }

    // 타입으로 아이템 필터링
    public List<WeaponSO> GetitemByType(WeaponType type)
    {
        return weapons.FindAll(weapon => weapon.WeapnType == type);
    }
}
