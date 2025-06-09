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
    [Header("�⺻ ����")]
    public string skillName;
    public SkillType skillType;
    public KeyCode activationKey;
    public float cooldownTime;

    [Header("������ / ����")]
    public Sprite skillIcon;
    public AudioClip skillSound;
    [Range(0f, 1f)]
    public float skillSoundVolume = 1f;
    [Range(0.1f, 3f)]
    public float skillSoundPitch = 1f;

    [Header("��ų ����Ʈ")]
    public GameObject skillEffectPrefab;   // ��ų ����Ʈ
    public GameObject trailEffectPrefab;   // ��� ����Ʈ

    [Tooltip("����Ʈ ���� �ð�")]
    public float effectDuration = 1.0f;

    [Tooltip("��� �Ÿ�")]
    public float effectSpawnDistance = 5.0f; // Shift��� ��� �Ÿ�
}
