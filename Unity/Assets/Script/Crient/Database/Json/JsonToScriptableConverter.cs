#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

public enum ConversionType
{
    Weapon,
    Enemy
}

public class JsonToScriptableConverter : EditorWindow
{
    private string jsonFilePath = "";
    private string outputFolder = "Assets/ScriptableObject";
    private bool createDatabase = true;
    private ConversionType conversionType = ConversionType.Weapon;

    [MenuItem("Tools/Json to Scriptable Object")]

    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("Json to Scriptable Object");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON to Scriptable Object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 변환 타입 선택
        conversionType = (ConversionType)EditorGUILayout.EnumPopup("Conversion Type", conversionType);

        if (GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
        }

        EditorGUILayout.LabelField("Selected File:", jsonFilePath);
        EditorGUILayout.Space();

        outputFolder = EditorGUILayout.TextField("Output Folder:", outputFolder);
        createDatabase = EditorGUILayout.Toggle("Create Database Asset", createDatabase);
        EditorGUILayout.Space();

        if (GUILayout.Button("Convert to Scriptable Objects"))
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a JSON file first!", "OK");
                return;
            }

            try
            {
                switch (conversionType)
                {
                    case ConversionType.Weapon:
                        ConvertWeaponJson();
                        break;
                    case ConversionType.Enemy:
                        ConvertEnemyJson();
                        break;
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Conversion failed: {e.Message}", "OK");
                Debug.LogError(e);
            }
        }
    }

    private void ConvertWeaponJson()
    {
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        string jsonText = File.ReadAllText(jsonFilePath);
        List<WeaponData> weaponDataList = JsonConvert.DeserializeObject<List<WeaponData>>(jsonText);

        List<WeaponSO> createdWeapons = new List<WeaponSO>();

        foreach (var weaponData in weaponDataList)
        {
            WeaponSO weaponSO = ScriptableObject.CreateInstance<WeaponSO>();

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
                Debug.LogWarning($"아이템 '{weaponData.weaponName}'의 유효하지 않은 타입: {weaponData.weaponTypeString}");
            }

            createdWeapons.Add(weaponSO);

            AssetDatabase.CreateAsset(weaponSO, $"{outputFolder}/{weaponSO.weaponName}.asset");
            EditorUtility.SetDirty(weaponSO);
        }

        if (createDatabase && createdWeapons.Count > 0)
        {
            WeaponDatabaseSO databaseWeapon = ScriptableObject.CreateInstance<WeaponDatabaseSO>();
            databaseWeapon.weapons = createdWeapons;

            AssetDatabase.CreateAsset(databaseWeapon, $"{outputFolder}/weaponDatabase.asset");
            EditorUtility.SetDirty(databaseWeapon);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", $"Created {createdWeapons.Count} Weapon ScriptableObjects!", "OK");
    }

    private void ConvertEnemyJson()
    {
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        string jsonText = File.ReadAllText(jsonFilePath);
        List<EnemyData> enemyDataList = JsonConvert.DeserializeObject<List<EnemyData>>(jsonText);

        List<EnemySO> createdEnemies = new List<EnemySO>();

        foreach (var enemyData in enemyDataList)
        {
            EnemySO enemySO = ScriptableObject.CreateInstance<EnemySO>();

            // 데이터 복사
            enemySO.id = enemyData.id;
            enemySO.monsterName = enemyData.monsterName;
            enemySO.spawnStage = enemyData.spawnStage;
            enemySO.monsterAttackType = enemyData.monsterAttackType;
            enemySO.monsterHp = enemyData.monsterHp;
            enemySO.monsterAttack = enemyData.monsterAttack;
            enemySO.monsterAttackInterval = enemyData.monsterAttackInterval;

            // 문자열을 Enum 으로 변환
            if (System.Enum.TryParse(enemyData.monsterAttackType, out EnemyAttackType parsedType))
            {
                enemySO.enemyAttackType = parsedType;
            }
            else
            {
                Debug.LogWarning($"'{enemyData.monsterName}'의 잘못된 공격 타입: {enemyData.monsterAttackType}");
            }

            createdEnemies.Add(enemySO);

            AssetDatabase.CreateAsset(enemySO, $"{outputFolder}/{enemySO.monsterName}.asset");
            EditorUtility.SetDirty(enemySO);
        }

        if (createDatabase && createdEnemies.Count > 0)
        {
            EnemyDatabaseSO databaseEnemy = ScriptableObject.CreateInstance<EnemyDatabaseSO>();
            databaseEnemy.enemys = createdEnemies;

            AssetDatabase.CreateAsset(databaseEnemy, $"{outputFolder}/enemyDatabase.asset");
            EditorUtility.SetDirty(databaseEnemy);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", $"Created {createdEnemies.Count} Enemy ScriptableObjects!", "OK");
    }
}
#endif
