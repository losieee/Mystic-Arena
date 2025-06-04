using UnityEngine;

[CreateAssetMenu(fileName = "BossPattern", menuName = "Database/Boss Pattern")]
public class BossPatternSO : ScriptableObject
{
    public int id;
    public string patternName;
    public string patternType;
    public PatternType parsedPatternType;
    public int patternDamage;
    public int purifyEssence;
    [TextArea]
    public string description;

    public void InitializeEnum()
    {
        if (System.Enum.TryParse(patternType.Replace(" ", ""), out PatternType parsed))
        {
            parsedPatternType = parsed;
        }
        else
        {
            Debug.LogWarning($"[BossPatternSO] �߸��� ���� Ÿ��: {patternType}");
            parsedPatternType = PatternType.LineAoE;
        }
    }

    public override string ToString()
    {
        return $"[{id}] {patternName} - {parsedPatternType} | ������: {patternDamage}, ��ȭ����: {purifyEssence}";
    }
}

