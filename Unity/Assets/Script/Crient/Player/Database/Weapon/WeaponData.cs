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
            Debug.Log($"���� '{weapon_type}'�� ��ȿ���� ���� ������ Ÿ�� : {weaponTypeString}");
            // �⺻�� ����
            weapon_type = WeaponType.Consumable;
        }
    }
}

