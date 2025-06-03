using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossSkillType
{
    nomar,
    LineAoe,
    MapSweep,
    AreaDrop
}
public class BossData : MonoBehaviour
{
    public int id;
    public string bossName;
    public int bossHealth;
    public string patternName;
    public string patternType;
    [NonSerialized]
    public BossSkillType pattern_Type;
    public int patternDemage;
    public int purifyEssenceCount;              // 정화 에너지 카운트
    public string description;

    public void InitalizeEnums()
    {
        if (Enum.TryParse(patternType, out BossSkillType parsedType))
        {
            pattern_Type = parsedType;
        }
        else
        {
            parsedType = BossSkillType.nomar;
        }
    }
}
