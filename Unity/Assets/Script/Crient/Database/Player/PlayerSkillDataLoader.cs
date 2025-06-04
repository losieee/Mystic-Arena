using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerSkillDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "PlayerSkills";  // Resources 폴더 내 JSON 파일명

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
            // 원본 텍스트를 UTF-8로 변환
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correctText = Encoding.UTF8.GetString(bytes);

            // JSON -> List<PlayerSkillData> 변환
            playerSkillList = JsonConvert.DeserializeObject<List<PlayerSkillData>>(correctText);
            Debug.Log($"로드된 플레이어 스킬 수 : {playerSkillList.Count}");

            foreach (var skill in playerSkillList)
            {
                Debug.Log($"스킬명: {EncodeKorean(skill.skillName)}");

                // Enum 초기화 (필요시)
                skill.InitializeEnums();
            }
        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다. : {jsonFileName}");
        }
    }

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        byte[] bytes = Encoding.Default.GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    }
}

