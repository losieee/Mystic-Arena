using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Database/Weapon")]
public class WeaponSO : ScriptableObject
{
    public int id;
    public string weaponName;
    public string weaponTypeString;
    public WeaponType WeapnType;
    public float baseDamage;
    public int dropStage;
    public string description;
    public Sprite icon;

    [Header("Prefab")]
    public GameObject weaponPrefab;

    public override string ToString()
    {
        return $"[{id}] : {weaponName}무기는 {WeapnType} 타입 입니다.";
    }

    public override bool Equals(object obj)
    {
        return obj is WeaponSO other && id == other.id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
