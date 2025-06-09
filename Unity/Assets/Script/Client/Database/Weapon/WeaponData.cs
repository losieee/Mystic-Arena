using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Consumable,
    Gun01,
    Gun02,
    Sword01,
    Sword02
}

[Serializable]
public class WeaponData : MonoBehaviour
{

    public int id;
    public string weaponName;
    public string weaponTypeString;
    [NonSerialized]
    public WeaponType weaponType;
    public float baseDamage;
    public int dropStage;
    public string description;

    public void InitalizeEnums()
    {
        if (Enum.TryParse(weaponTypeString, out WeaponType parsedType))
        {
            weaponType = parsedType;
        }
        else
        {
            Debug.Log($"���� '{weaponType}'�� ��ȿ���� ���� ������ Ÿ�� : {weaponTypeString}");
            // �⺻�� ����
            weaponType = WeaponType.Consumable;
        }
    }
}

