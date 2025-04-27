using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public float cooldownTime;
    public KeyCode activationKey;
    public Sprite skillIcon;
    public AudioClip skillSound; // 스킬 효과음
}
