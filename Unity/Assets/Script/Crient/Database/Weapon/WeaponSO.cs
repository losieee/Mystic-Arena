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
    public WeaponType WeapnType;
    public float baseDamage;
    public int dropStage;
    public string description;
    public Sprite icon;


    public override string ToString()
    {
        return $"[{id}] :  {weaponName}����� {WeapnType} Ÿ�� �Դϴ�.";
    }

    //public string DisplayName
    //{
    //    get { return string.IsNullOrEmpty(nameEng)? }
    //}
}
