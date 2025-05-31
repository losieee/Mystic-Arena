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
    private string jsonFilePath = "";                                           // JSON ���� ��� ���ڿ� ��
    private string outputFolder = "Assets/ScriptableObject/Weapons";            // ��� SO ������ ��ΰ�
    private bool createdatabase = true;                                         // ������ ���̽��� ����� ���̴����� ���� bool ��

    [MenuItem("Tools/Json to Scriptable Object")]

    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("Json to Scriptable Object");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON to Scriptable Object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();                                                            // GUI�� �����

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

    private void ConvertJsonToScriptableObjects()                               // JSON ������ ScriptableObject ���Ϸ� ��ȯ �����ִ� �Լ�
    {
        // Ǯ�� ����
        if (!Directory.Exists(outputFolder))                                    // ���� ��ġ�� Ȯ���ϰ� ������ ���� �Ѵ�.
        {
            Directory.CreateDirectory(outputFolder);
        }

        string jsonText = File.ReadAllText(jsonFilePath);                   // JSON ������ �д´�.

        try
        {
            // JSON �Ľ�
            List<WeaponData> weaponDataList = JsonConvert.DeserializeObject<List<WeaponData>>(jsonText);

            List<WeaponSO> createWeapons = new List<WeaponSO>();

            //�� ���� �����͸� ��ũ���� ������Ʈ�� ��ȯ
            foreach (var weaponData in weaponDataList)
            {

                WeaponSO weaponSO = ScriptableObject.CreateInstance<WeaponSO>();                    //weaponSO ���� ����

                // ������ ����
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
                    Debug.LogWarning($"������ '{weaponData.weaponName}'�� �������� ���� Ÿ�� : {weaponData.weaponTypeString}");
                }


                // ������ �κ�: ����Ʈ�� �߰�
                createWeapons.Add(weaponSO);

                //ScriptableObject�� �ּ����� ����
                AssetDatabase.CreateAsset(weaponSO, $"{outputFolder}/{weaponSO.weaponName}.asset");
                EditorUtility.SetDirty(weaponSO);
            }
            // ������ ���̽� ����
            if (createdatabase && createWeapons.Count > 0)
            {
                WeaponDatabaseSO database_Weapon = ScriptableObject.CreateInstance<WeaponDatabaseSO>();         // WeaponDatabaseSO ����
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
            Debug.LogError($"JSON ��ȯ ���� : {e}");
        }
    }
}

#endif