using System;
using UnityEngine;
using Newtonsoft.Json;

public enum PlayerSkillType
{
    Active,
    Buff,
    Movement
}

public enum WeaponTypeString
{
    Sword,
    Gun,
    All
}

public enum BuffType
{
    None,
    Movespeed,
    Invincible
}


public class PlayerSkillData : MonoBehaviour
{
    public int id;
    public string skillName;
    public string skillKey;
    public string skillType;
    public string weaponTypeString;
    public string description;
    public int skillCooldown;
    public int skillDamage;
    public string useWeaponDamage;
    public string totalDamageFormula;
    public string buffType;
    public string buffValue;
    public float? duration;

    [NonSerialized] public SkillType skillTypeEnum;
    [NonSerialized] public WeaponTypeString weaponTypeEnum;
    [NonSerialized] public BuffType buffTypeEnum;
    [NonSerialized] public bool useWeaponDamageBool;

    public void InitializeEnums()
    {
        Enum.TryParse(skillType, out skillTypeEnum);
        Enum.TryParse(weaponTypeString, out weaponTypeEnum);
        Enum.TryParse(buffType, out buffTypeEnum);
        useWeaponDamageBool = useWeaponDamage == "TRUE";
    }
}