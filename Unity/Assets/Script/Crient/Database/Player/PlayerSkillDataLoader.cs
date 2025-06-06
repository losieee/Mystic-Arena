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

            Debug.Log($"�ε�� ��ų ��: {skillList.Count}");

            foreach (var skill in skillList)
            {
                string durationInfo = skill.duration.HasValue ? $"{skill.duration.Value}��" : "����";
                Debug.Log($"��ų: {skill.skillName}, ���ӽð�: {durationInfo}");
            }
        }
        else
        {
            Debug.LogError($"JSON ������ ã�� �� �����ϴ�: {jsonFileName}");
        }
    }
}
