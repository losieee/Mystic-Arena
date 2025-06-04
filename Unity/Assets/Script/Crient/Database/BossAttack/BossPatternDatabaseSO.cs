using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPatternDatabase", menuName = "Database/Boss Pattern Database")]
public class BossPatternDatabaseSO : ScriptableObject
{
    public List<BossPatternSO> patterns = new List<BossPatternSO>();

    // Ä³½Ì µñ¼Å³Ê¸®
    private Dictionary<int, BossPatternSO> patternById;
    private Dictionary<string, BossPatternSO> patternByName;

    private void Initialize()
    {
        patternById = new Dictionary<int, BossPatternSO>();
        patternByName = new Dictionary<string, BossPatternSO>();

        foreach (var pattern in patterns)
        {
            patternById[pattern.id] = pattern;
            patternByName[pattern.patternName] = pattern;
        }
    }

    public BossPatternSO GetPatternById(int id)
    {
        if (patternById == null)
        {
            Initialize();
        }

        if (patternById.TryGetValue(id, out BossPatternSO pattern))
            return pattern;

        return null;
    }

    public BossPatternSO GetPatternByName(string name)
    {
        if (patternByName == null)
        {
            Initialize();
        }

        if (patternByName.TryGetValue(name, out BossPatternSO pattern))
            return pattern;

        return null;
    }

    public List<BossPatternSO> GetPatternsByType(PatternType type)
    {
        return patterns.FindAll(p => p.parsedPatternType == type);
    }
}

