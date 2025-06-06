using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerSkillDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "PlayerSkills";

    private List<PlayerSkillData> skillList;

    void Start()
    {
        LoadSkillData();
    }

    void LoadSkillData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile != null)
        {
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correctText = Encoding.UTF8.GetString(bytes);

            skillList = JsonConvert.DeserializeObject<List<PlayerSkillData>>(correctText);

            Debug.Log($"로드된 스킬 수: {skillList.Count}");

            foreach (var skill in skillList)
            {
                string durationInfo = skill.duration.HasValue ? $"{skill.duration.Value}초" : "없음";
                Debug.Log($"스킬: {skill.skillName}, 지속시간: {durationInfo}");
            }
        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {jsonFileName}");
        }
    }
}
