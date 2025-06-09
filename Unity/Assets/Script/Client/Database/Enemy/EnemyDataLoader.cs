using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EnemyDataLoader : MonoBehaviour
{

    [SerializeField]
    private string jsonFileName = "Monsters";                        //Resources 폴더에서 가져올 JSON 파일 이름

    private List<EnemyData> EnemyList;

    // Start is called before the first frame update
    void Start()
    {
        LoadEnemyData();
    }

    void LoadEnemyData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);           // TextAsset 형태로 Josn 파일을 로딩한다.

        if (jsonFile != null)
        {
            // 원본 텍스트에서 UTF-8로 반환처리
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correntText = Encoding.UTF8.GetString(bytes);

            // 변환된 텍스트 사용
            EnemyList = JsonConvert.DeserializeObject<List<EnemyData>>(correntText);

            Debug.Log($"로드된 아이템 수 : {EnemyList.Count}");

            foreach (var enemy in EnemyList)
            {
                Debug.Log($"몬스터: {EncodeKorean(enemy.monsterName)}");
            }
        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다. : {jsonFileName}");
        }
    }

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";                          // 텍스트가 NULL 값이면 함수를 끝낸다.
        byte[] bytes = Encoding.Default.GetBytes(text);                     // String 을 Byte 배열로 변환한 후
        return Encoding.UTF8.GetString(bytes);                              // 인코딩을 UTF8로 바꾼다.
    }
}