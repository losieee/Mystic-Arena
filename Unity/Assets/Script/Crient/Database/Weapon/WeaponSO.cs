using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Database/Weapon")]
public class WeaponSO : ScriptableObject
{
    // 기존 GameDB Excel 에서 선언한 변수를 나열 한다.
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
        return $"[{id}] :  {weaponName}무기는 {WeapnType} 타입 입니다.";
    }

    //public string DisplayName
    //{
    //    get { return string.IsNullOrEmpty(nameEng)? }
    //}
}
