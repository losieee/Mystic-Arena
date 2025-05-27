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

    public int Weapon_Id;
    public string Weapon_Name;
    public string WeaponType_String;
    [NonSerialized]
    public WeaponType weapon_type;
    public float Weapon_nomarDamege;
    public int Weapon_Drop_Stage;
    public string Weapon_Description;

    public void InitalizeEnums()
    {
        if (Enum.TryParse(WeaponType_String, out WeaponType parsedType))
        {
            weapon_type = parsedType;
        }
        else
        {
            Debug.Log($"무기 '{weapon_type}'에 유효하지 않은 아이템 타입 : {WeaponType_String}");
            // 기본값 설정
            weapon_type = WeaponType.Consumable;
        }
    }
}

