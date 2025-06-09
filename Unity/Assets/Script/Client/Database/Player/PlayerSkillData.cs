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
    public string playerBuffValue; // ���� OK
    public float? playerDuration;  // ������ �� nullable float

    public void InitializeEnums()
    {
        // Skill Type �Ľ�
        if (Enum.TryParse(playerSkillTypeString, true, out PlayerSkillType parsedSkillType))
        {
            playerSkillType = parsedSkillType;
        }
        else
        {
            Debug.LogWarning($"[PlayerSkillData] '{playerSkillName}' �� ��ȿ���� ���� PlayerSkillType: '{playerSkillTypeString}', �⺻�� 'Active' ���");
            playerSkillType = PlayerSkillType.Active;
        }

        // Weapon Type �Ľ�
        if (Enum.TryParse(playerWeaponTypeString, true, out PlayerWeaponType parsedWeaponType))
        {
            playerWeaponType = parsedWeaponType;
        }
        else
        {
            Debug.LogWarning($"[PlayerSkillData] '{playerSkillName}' �� ��ȿ���� ���� PlayerWeaponType: '{playerWeaponTypeString}', �⺻�� 'All' ���");
            playerWeaponType = PlayerWeaponType.All;
        }
    }
}
