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
            Debug.Log($"���� '{weapon_type}'�� ��ȿ���� ���� ������ Ÿ�� : {WeaponType_String}");
            // �⺻�� ����
            weapon_type = WeaponType.Consumable;
        }
    }
}

