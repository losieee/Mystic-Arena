using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BossPatternDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "BossPatterns";  // Resources ������ �ִ� JSON ���ϸ� (Ȯ���� ����)

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
            // Encoding ó�� (�ѱ� ���� ����)
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correctText = Encoding.UTF8.GetString(bytes);

            // JSON �Ľ�
            patternList = JsonConvert.DeserializeObject<List<BossAttackPatternData>>(correctText);

            // Enum �ʱ�ȭ
            foreach (var pattern in patternList)
            {
                pattern.InitializeEnums();
            }

            Debug.Log($"�ε�� ���� ���� ��: {patternList.Count}");

            foreach (var pattern in patternList)
            {
                Debug.Log($"���� �̸�: {EncodeKorean(pattern.patternName)} | Ÿ��: {pattern.parsedPatternType}");
            }
        }
        else
        {
            Debug.LogError($"JSON ������ ã�� �� �����ϴ�: {jsonFileName}");
        }
    }

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        byte[] bytes = Encoding.Default.GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    }
}

