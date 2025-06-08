using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "Skills"; // Resources ������ �ִ� JSON ���� �̸�

    private List<PlayerSkillData> playerSkillList;

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

            playerSkillList = JsonConvert.DeserializeObject<List<PlayerSkillData>>(correctText);

            Debug.Log($"�ε�� ��ų �� : {playerSkillList.Count}");

            foreach (var skill in playerSkillList)
            {
                skill.InitializeEnums();
                Debug.Log($"��ų: {EncodeKorean(skill.playerSkillName)}, ���� : {EncodeKorean(skill.playerDescription)}");
            }

        }
        else
        {
            Debug.LogError($"JSON ������ ã�� �� �����ϴ� : {jsonFileName}");
        }
    }

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        byte[] bytes = Encoding.Default.GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    }
}
