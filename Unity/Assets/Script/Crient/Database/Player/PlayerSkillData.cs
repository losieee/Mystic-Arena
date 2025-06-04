using System;
using UnityEngine;
using Newtonsoft.Json;

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

public enum PlayerBuffType
{
    None,
    Movespeed,
    Invincible
}

[Serializable]
public class PlayerSkillData : MonoBehaviour
{
    [JsonProperty("id")]
    public int id;

    [JsonProperty("skillName")]
    public string skillName;

    [JsonProperty("skillKey")]
    public string skillKey;

    [JsonProperty("skillType")]
    public string skillTypeString;

    [JsonProperty("weaponTypeString")]
    public string weaponTypeString;

    [TextArea]
    [JsonProperty("description")]
    public string description;

    [JsonProperty("skillColldown")]
    public int skillColldown;

    [JsonProperty("skillDamage")]
    public int skillDamage;

    [JsonProperty("useWaeponDamage")]
    public bool useWaeponDamage;

    [JsonProperty("totalDamageFormula")]
    public string totalDamageFormula;

    [JsonProperty("buffType")]
    public string buffTypeString;

    [JsonProperty("buffValue")]
    public string buffValueString;

    [JsonProperty("duration")]
    public float duration;

    [NonSerialized]
    public PlayerSkillType player_skillType;

    [NonSerialized]
    public PlayerWeaponType player_weaponType;

    [NonSerialized]
    public PlayerBuffType player_buffType;

    [NonSerialized]
    public object buffValue;

    public void InitializeEnums()
    {
        // skillType �Ľ�
        if (!Enum.TryParse(skillTypeString, out player_skillType))
        {
            Debug.LogWarning($"[SkillData] ��ȿ���� ���� SkillType: {skillTypeString}, �⺻�� Active �Ҵ�");
            player_skillType = PlayerSkillType.Active;
        }

        // weaponType �Ľ�
        if (!Enum.TryParse(weaponTypeString, out player_weaponType))
        {
            Debug.LogWarning($"[SkillData] ��ȿ���� ���� WeaponType: {weaponTypeString}, �⺻�� All �Ҵ�");
            player_weaponType = PlayerWeaponType.All;
        }

        // buffType �Ľ�
        if (string.IsNullOrEmpty(buffTypeString) || buffTypeString.ToLower() == "null")
        {
            player_buffType = PlayerBuffType.None;
            buffValue = null;
        }
        else if (!Enum.TryParse(buffTypeString, out player_buffType))
        {
            Debug.LogWarning($"[SkillData] ��ȿ���� ���� BuffType: {buffTypeString}, None �Ҵ�");
            player_buffType = PlayerBuffType.None;
            buffValue = null;
        }
        else
        {
            // buffValue ��ȯ
            if (bool.TryParse(buffValueString, out bool boolVal))
                buffValue = boolVal;
            else if (float.TryParse(buffValueString, out float floatVal))
                buffValue = floatVal;
            else if (int.TryParse(buffValueString, out int intVal))
                buffValue = intVal;
            else
                buffValue = buffValueString;
        }
    }
}
