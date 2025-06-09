using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BossPatternDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "BossPatterns";  // Resources 폴더에 있는 JSON 파일명 (확장자 제외)

    public List<BossAttackPatternData> patternList { get; private set; }

    void Start()
    {
        LoadBossPatternData();
    }

    void LoadBossPatternData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile != null)
        {
            // Encoding 처리 (한글 깨짐 방지)
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correctText = Encoding.UTF8.GetString(bytes);

            // JSON 파싱
            patternList = JsonConvert.DeserializeObject<List<BossAttackPatternData>>(correctText);

            // Enum 초기화
            foreach (var pattern in patternList)
            {
                pattern.InitializeEnums();
            }

            Debug.Log($"로드된 보스 패턴 수: {patternList.Count}");

            foreach (var pattern in patternList)
            {
                Debug.Log($"패턴 이름: {EncodeKorean(pattern.patternName)} | 타입: {pattern.parsedPatternType}");
            }
        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {jsonFileName}");
        }
    }

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        byte[] bytes = Encoding.Default.GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    }
}

