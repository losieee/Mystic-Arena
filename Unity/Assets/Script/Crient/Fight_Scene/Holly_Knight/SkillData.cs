using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public float cooldownTime;
    public KeyCode activationKey;
    public Sprite skillIcon;
}
