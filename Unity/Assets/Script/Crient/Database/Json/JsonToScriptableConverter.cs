#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using static UnityEditor.Progress;

public class JsonToScriptableConverter : EditorWindow
{
    private string jsonFilePath = "";                                           // JSON 파일 경로 문자열 값
    private string outputFolder = "Assets/ScriptableObject/Weapons";            // 출력 SO 파일을 경로값
    private bool createdatabase = true;                                         // 데이텅 베이스를 사용할 것이니지에 대한 bool 값

    [MenuItem("Tools/Json to Scriptable Object")]

    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("Json to Scriptable Object");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON to Scriptable Object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();                                                            // GUI상에 빈공간

        if (GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
        }

        EditorGUILayout.LabelField("Selected File : ", jsonFilePath);
        EditorGUILayout.Space();
        outputFolder = EditorGUILayout.TextField("Output Folder : ", outputFolder);
        createdatabase = EditorGUILayout.Toggle("Create Database Asset", createdatabase);
        EditorGUILayout.Space();

        if (GUILayout.Button("Convert to Scriptable objects"))
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a JSON file firest!", "OK");
            }
            ConvertJsonToScriptableObjects();
        }
    }

    private void ConvertJsonToScriptableObjects()                               // JSON 파일을 ScriptableObject 파일로 변환 시켜주는 함수
    {
        // 풀더 생성
        if (!Directory.Exists(outputFolder))                                    // 폴더 위치를 확인하고 없으면 생성 한다.
        {
            Directory.CreateDirectory(outputFolder);
        }

        string jsonText = File.ReadAllText(jsonFilePath);                   // JSON 파일을 읽는다.

        try
        {
            // JSON 파싱
            List<WeaponData> weaponDataList = JsonConvert.DeserializeObject<List<WeaponData>>(jsonText);

            List<WeaponSO> createWeapons = new List<WeaponSO>();

            //각 무기 데이터를 스크립블 오브젝트로 변환
            foreach (var weaponData in weaponDataList)
            {

                WeaponSO weaponSO = ScriptableObject.CreateInstance<WeaponSO>();                    //weaponSO 파일 생성

                // 데이터 복사
                weaponSO.id = weaponData.id;
                weaponSO.weaponName = weaponData.weaponName;
                weaponSO.weaponTypeString = weaponData.weaponTypeString;
                weaponSO.baseDamage = weaponData.baseDamage;
                weaponSO.dropStage = weaponData.dropStage;
                weaponSO.description = weaponData.description;

                if (System.Enum.TryParse(weaponData.weaponTypeString, out WeaponType parsedType))
                {
                    weaponData.weaponType = parsedType;
                }
                else
                {
                    Debug.LogWarning($"아이템 '{weaponData.weaponName}'의 유혀하지 않은 타입 : {weaponData.weaponTypeString}");
                }


                // 누락된 부분: 리스트에 추가
                createWeapons.Add(weaponSO);

                //ScriptableObject를 애셋으로 저장
                AssetDatabase.CreateAsset(weaponSO, $"{outputFolder}/{weaponSO.weaponName}.asset");
                EditorUtility.SetDirty(weaponSO);
            }
            // 데이터 베이스 생성
            if (createdatabase && createWeapons.Count > 0)
            {
                WeaponDatabaseSO database_Weapon = ScriptableObject.CreateInstance<WeaponDatabaseSO>();         // WeaponDatabaseSO 생성
                database_Weapon.weapons = createWeapons;

                AssetDatabase.CreateAsset(database_Weapon, $"{outputFolder}/weaponDatabase.asset");
                EditorUtility.SetDirty(database_Weapon);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createWeapons.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 : {e}");
        }
    }
}

#endif