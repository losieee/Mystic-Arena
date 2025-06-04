using System;
using UnityEngine;

public enum PatternType
{
    LineAoE,
    MapSweep,
    AreaDrop
}

[Serializable]
public class BossAttackPatternData : MonoBehaviour
{
    public int id;
    public string patternName;
    public string patternType;
    [NonSerialized]
    public PatternType parsedPatternType;
    public int patternDamage;
    public int purifyEssence;
    [TextArea]
    public string description;

    public void InitializeEnums()
    {
        if (Enum.TryParse(patternType.Replace(" ", ""), out PatternType parsed))
        {
            parsedPatternType = parsed;
        }
        else
        {
            Debug.LogWarning($"[BossAttackPattern] 잘못된 패턴 타입: {patternType}");
            parsedPatternType = PatternType.LineAoE;
        }
    }
}
