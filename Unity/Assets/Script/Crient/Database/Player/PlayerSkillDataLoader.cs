using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerSkillDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "PlayerSkills";  // Resources ���� �� JSON ���ϸ�

    private List<PlayerSkillData> playerSkillList;

    void Start()
    {
        LoadPlayerSkillData();
    }

    void LoadPlayerSkillData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile != null)
        {
            // ���� �ؽ�Ʈ�� UTF-8�� ��ȯ
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correctText = Encoding.UTF8.GetString(bytes);

            // JSON -> List<PlayerSkillData> ��ȯ
            playerSkillList = JsonConvert.DeserializeObject<List<PlayerSkillData>>(correctText);
            Debug.Log($"�ε�� �÷��̾� ��ų �� : {playerSkillList.Count}");

            foreach (var skill in playerSkillList)
            {
                Debug.Log($"��ų��: {EncodeKorean(skill.skillName)}");

                // Enum �ʱ�ȭ (�ʿ��)
                skill.InitializeEnums();
            }
        }
        else
        {
            Debug.LogError($"JSON ������ ã�� �� �����ϴ�. : {jsonFileName}");
        }
    }

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        byte[] bytes = Encoding.Default.GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    }
}

