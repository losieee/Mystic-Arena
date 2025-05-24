using UnityEngine;
using TMPro;


public enum SkillType
{
    Q,
    E,
    Shift
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public float cooldownTime;
    public KeyCode activationKey;
    public Sprite skillIcon;
    public AudioClip skillSound;

    public SkillType skillType;
}
