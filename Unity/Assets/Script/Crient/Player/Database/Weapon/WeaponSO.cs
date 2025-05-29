using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Database/Weapon")]
public class WeaponSO : ScriptableObject
{
    // ���� GameDB Excel ���� ������ ������ ���� �Ѵ�.
    public int id;
    public string weaponName;
    public string weaponTypeString;
    public WeaponType weapon_type;
    public float baseDamage;
    public int dropStage;
    public string description;


    public override string ToString()
    {
        return $"[{id}] :  {weaponName}����� {weapon_type} Ÿ�� �Դϴ�.";
    }

    //public string DisplayName
    //{
    //    get { return string.IsNullOrEmpty(nameEng)? }
    //}
}
