using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "Skills"; // Resources 폴더에 있는 JSON 파일 이름

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

            Debug.Log($"로드된 스킬 수 : {playerSkillList.Count}");

            foreach (var skill in playerSkillList)
            {
                skill.InitializeEnums();
                Debug.Log($"스킬: {EncodeKorean(skill.playerSkillName)}, 설명 : {EncodeKorean(skill.playerDescription)}");
            }

        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다 : {jsonFileName}");
        }
    }

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        byte[] bytes = Encoding.Default.GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    }
}
