using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkillDatabase", menuName = "Database/PlayerSkillDatabase")]
public class PlayerSkillDatabaseSO : ScriptableObject
{
    public List<PlayerSkillSO> skills = new List<PlayerSkillSO>();

    private Dictionary<int, PlayerSkillSO> skillById;
    private Dictionary<string, PlayerSkillSO> skillByName;

    private void Initialize()
    {
        skillById = new Dictionary<int, PlayerSkillSO>();
        skillByName = new Dictionary<string, PlayerSkillSO>();

        foreach (var skill in skills)
        {
            skillById[skill.id] = skill;
            skillByName[skill.skillName] = skill;
        }
    }

    public PlayerSkillSO GetSkillById(int id)
    {
        if (skillById == null) Initialize();
        return skillById.TryGetValue(id, out var skill) ? skill : null;
    }

    public PlayerSkillSO GetSkillByName(string name)
    {
        if (skillByName == null) Initialize();
        return skillByName.TryGetValue(name, out var skill) ? skill : null;
    }

    public List<PlayerSkillSO> GetSkillsByType(SkillType type)
    {
        return skills.FindAll(skill => skill.skillType == type);
    }
}

