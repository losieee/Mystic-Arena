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
    public GameObject weaponPrefab;  //손에 들 프리팹 연결

    public override string ToString()
    {
        return $"[{id}] :  {weaponName}무기는 {WeapnType} 타입 입니다.";
    }
}
