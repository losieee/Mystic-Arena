using UnityEngine;

public enum SkillType
{
    Q,
    E,
    Shift
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;
    public SkillType skillType;
    public KeyCode activationKey;
    public float cooldownTime;

    [Header("아이콘 / 사운드")]
    public Sprite skillIcon;
    public AudioClip skillSound;
    [Range(0f, 1f)]
    public float skillSoundVolume = 1f;
    [Range(0.1f, 3f)]
    public float skillSoundPitch = 1f;

    [Header("스킬 이펙트")]
    public GameObject skillEffectPrefab;   // 스킬 이펙트
    public GameObject trailEffectPrefab;   // 대시 이펙트

    [Tooltip("이펙트 지속 시간")]
    public float effectDuration = 1.0f;

    [Tooltip("대시 거리")]
    public float effectSpawnDistance = 5.0f; // Shift라면 대시 거리
}
