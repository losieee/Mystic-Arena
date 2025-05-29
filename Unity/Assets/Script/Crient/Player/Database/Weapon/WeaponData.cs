using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Consumable,
    Gun,
    Sword
}

[Serializable]
public class WeaponData : MonoBehaviour
{

    public int id;
    public string weaponName;
    public string weaponTypeString;
    [NonSerialized]
    public WeaponType weapon_type;
    public float baseDamage;
    public int dropStage;
    public string description;

    public void InitalizeEnums()
    {
        if (Enum.TryParse(weaponTypeString, out WeaponType parsedType))
        {
            weapon_type = parsedType;
        }
        else
        {
            Debug.Log($"무기 '{weapon_type}'에 유효하지 않은 아이템 타입 : {weaponTypeString}");
            // 기본값 설정
            weapon_type = WeaponType.Consumable;
        }
    }
}

