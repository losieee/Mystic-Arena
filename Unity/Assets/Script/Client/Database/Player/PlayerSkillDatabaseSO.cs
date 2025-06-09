using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Database/SkillDatabase")]
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
            skillByName[skill.playerSkillName] = skill;
        }
    }

    public PlayerSkillSO GetSkillById(int id)
    {
        if (skillById == null)
        {
            Initialize();
        }

        if (skillById.TryGetValue(id, out PlayerSkillSO skill))
            return skill;

        return null;
    }

    public PlayerSkillSO GetSkillByName(string name)
    {
        if (skillByName == null)
        {
            Initialize();
        }

        if (skillByName.TryGetValue(name, out PlayerSkillSO skill))
            return skill;

        return null;
    }

    public List<PlayerSkillSO> GetSkillsByType(PlayerSkillType type)
    {
        return skills.FindAll(skill => skill.playerSkillType == type);
    }
}
