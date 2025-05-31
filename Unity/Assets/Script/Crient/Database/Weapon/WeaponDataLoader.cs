using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

public class WeaponDataLoader : MonoBehaviour
{
    // �̱��� ó��
    private static WeaponDataLoader instance;

    [SerializeField]
    private string jsonFileName = "Weapons";                        //Resources �������� ������ JSON ���� �̸�

    private List<WeaponData> weaponList;

    private void Awake()
    {
       if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadWeaponData();
    }

    void LoadWeaponData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);           // TextAsset ���·� Josn ������ �ε��Ѵ�.

        if (jsonFile != null)
        {
            // ���� �ؽ�Ʈ���� UTF-8�� ��ȯó��
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string correntText = Encoding.UTF8.GetString(bytes);

            // ��ȯ�� �ؽ�Ʈ ���
            weaponList = JsonConvert.DeserializeObject<List<WeaponData>>(correntText);

            Debug.Log($"�ε�� ������ �� : {weaponList.Count}");

            foreach (var weapon in weaponList)
            {
                Debug.Log($"����: {EncodeKorean(weapon.weaponName)}, ���� : {EncodeKorean(weapon.description)}");
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
