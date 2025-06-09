using System;
using UnityEngine;

public enum PlayerSkillType
{
    Active,
    Buff,
    Movement
}

public enum PlayerWeaponType
{
    Sword,
    Gun,
    All
}

[Serializable]
public class PlayerSkillData
{
    public int id;
    public string playerSkillName;
    public string playerSkillKey;

    public string playerSkillTypeString;
    [NonSerialized]
    public PlayerSkillType playerSkillType;

    public string playerWeaponTypeString;
    [NonSerialized]
    public PlayerWeaponType playerWeaponType;

    public string playerDescription;

    public float playerSkillCooldown;
    public float playerSkillDamage;
    public bool playerUseWeaponDamage;
    public string playerTotalDamageFormula;

    public string playerBuffType;
    public string playerBuffValue; // 유지 OK
    public float? playerDuration;  // 수정됨 → nullable float

    public void InitializeEnums()
    {
        // Skill Type 파싱
        if (Enum.TryParse(playerSkillTypeString, true, out PlayerSkillType parsedSkillType))
        {
            playerSkillType = parsedSkillType;
        }
        else
        {
            Debug.LogWarning($"[PlayerSkillData] '{playerSkillName}' → 유효하지 않은 PlayerSkillType: '{playerSkillTypeString}', 기본값 'Active' 사용");
            playerSkillType = PlayerSkillType.Active;
        }

        // Weapon Type 파싱
        if (Enum.TryParse(playerWeaponTypeString, true, out PlayerWeaponType parsedWeaponType))
        {
            playerWeaponType = parsedWeaponType;
        }
        else
        {
            Debug.LogWarning($"[PlayerSkillData] '{playerSkillName}' → 유효하지 않은 PlayerWeaponType: '{playerWeaponTypeString}', 기본값 'All' 사용");
            playerWeaponType = PlayerWeaponType.All;
        }
    }
}
