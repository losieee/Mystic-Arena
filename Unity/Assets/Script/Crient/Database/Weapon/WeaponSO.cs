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
    public GameObject weaponPrefab;  //�տ� �� ������ ����

    public override string ToString()
    {
        return $"[{id}] :  {weaponName}����� {WeapnType} Ÿ�� �Դϴ�.";
    }
}
