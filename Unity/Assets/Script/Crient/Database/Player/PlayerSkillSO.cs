using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkill", menuName = "Database/PlayerSkill")]
public class PlayerSkillSO : ScriptableObject
{
    public int id;
    public string skillName;
    public string skillKey;
    public SkillType skillType;
    public WeaponTypeString weaponTypeString;
    public string description;
    public int skillCooldown;
    public int skillDamage;
    public bool useWeaponDamage;
    public string totalDamageFormula;
    public BuffType buffType;
    public string buffValue;
    public float duration;

    public override string ToString()
    {
        return $"[{id}] : {skillName} 스킬은 {skillType} 타입입니다.";
    }
}

