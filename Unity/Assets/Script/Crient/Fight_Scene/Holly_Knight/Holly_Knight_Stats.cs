using UnityEngine;

[CreateAssetMenu(fileName = "Holly_Knight_Stats", menuName = "ScriptableObjects/Holly_Knight_Stats", order = 1)]
public class Holly_Knight_Stats : ScriptableObject
{
    [Header("Health")]
    public float maxHealth = 150f;
    public float HealAmount => maxHealth * 0.5f;

    [Header("Movement")]
    public float dashDistance = 3f;
    public float dashSpeed = 10f;
    public float interactionRange = 3f;

    [Header("Skills")]
    public float skillCooldownQ = 5f;
    public float skillCooldownW = 5f;
    public float skillCooldownE = 5f;
    public float skillCooldownG = 8f;
}
