using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EnemyDataLoader : MonoBehaviour
{

    [SerializeField]
    private string jsonFileName = "Monsters";                        //Resources �������� ������ JSON ���� �̸�

    private List<EnemyData> EnemyList;

    // Start is called before the first frame update
    void Start()
    {
        LoadEnemyData();
    }

    void LoadEnemyData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);           // TextAsset ���·� Josn ������ �ε��Ѵ�.

        if (jsonFile != null)
        {
            // ���� �ؽ�Ʈ���� UTF-8�� ��ȯó��
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correntText = Encoding.UTF8.GetString(bytes);

            // ��ȯ�� �ؽ�Ʈ ���
            EnemyList = JsonConvert.DeserializeObject<List<EnemyData>>(correntText);

            Debug.Log($"�ε�� ������ �� : {EnemyList.Count}");

            foreach (var enemy in EnemyList)
            {
                Debug.Log($"����: {EncodeKorean(enemy.monsterName)}");
            }
        }
        else
        {
            Debug.LogError($"JSON ������ ã�� �� �����ϴ�. : {jsonFileName}");
        }
    }

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";                          // �ؽ�Ʈ�� NULL ���̸� �Լ��� ������.
        byte[] bytes = Encoding.Default.GetBytes(text);                     // String �� Byte �迭�� ��ȯ�� ��
        return Encoding.UTF8.GetString(bytes);                              // ���ڵ��� UTF8�� �ٲ۴�.
    }
}