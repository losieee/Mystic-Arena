using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Database/Skill")]
public class PlayerSkillSO : ScriptableObject
{
    public int id;
    public string playerSkillName;
    public string playerSkillKey;
    public string playerSkillTypeString;
    public PlayerSkillType playerSkillType;
    public string playerWeaponTypeString;
    public string playerDescription;

    public float playerSkillCooldown;
    public float playerSkillDamage;
    public bool playerUseWeaponDamage;
    public string playerTotalDamageFormula;

    public string playerBuffType;
    public string playerBuffValue;
    public float playerDuration;

    [Header("Prefab")]
    public GameObject skillPrefab;

    public override string ToString()
    {
        return $"[{id}] : {playerSkillName} 스킬은 {playerSkillType} 타입 입니다.";
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerSkillSO other && id == other.id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
