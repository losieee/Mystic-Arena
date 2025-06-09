#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Unity.VisualScripting;

public enum ConversionType
{
    Weapon,
    Enemy,
    BossAttackPatternData,
    PlayerSkill,
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
                    case ConversionType.BossAttackPatternData:
                        ConvertBossPatternJson();
                        break;
                    case ConversionType.PlayerSkill:
                        ConvertPlayerSkillJson();
                        break;
                }
            }
            catch (Exception e)
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

            if (Enum.TryParse(weaponData.weaponTypeString, out WeaponType parsedType))
            {
                weaponSO.WeapnType = parsedType;
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

            enemySO.id = enemyData.id;
            enemySO.monsterName = enemyData.monsterName;
            enemySO.spawnStage = enemyData.spawnStage;
            enemySO.monsterAttackType = enemyData.monsterAttackType;
            enemySO.monsterHp = enemyData.monsterHp;
            enemySO.monsterAttack = enemyData.monsterAttack;
            enemySO.monsterAttackInterval = enemyData.monsterAttackInterval;

            if (Enum.TryParse(enemyData.monsterAttackType, out EnemyAttackType parsedType))
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

    private void ConvertBossPatternJson()
    {
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        string jsonText = File.ReadAllText(jsonFilePath);
        List<BossAttackPatternData> bossPatternDataList = JsonConvert.DeserializeObject<List<BossAttackPatternData>>(jsonText);

        List<BossPatternSO> createdPatterns = new List<BossPatternSO>();

        foreach (var data in bossPatternDataList)
        {
            BossPatternSO patternSO = ScriptableObject.CreateInstance<BossPatternSO>();

            patternSO.id = data.id;
            patternSO.patternName = data.patternName;
            patternSO.patternType = data.patternType;
            patternSO.patternDamage = data.patternDamage;
            patternSO.purifyEssence = data.purifyEssence;
            patternSO.description = data.description;

            patternSO.InitializeEnum();

            createdPatterns.Add(patternSO);

            AssetDatabase.CreateAsset(patternSO, $"{outputFolder}/{patternSO.patternName}.asset");
            EditorUtility.SetDirty(patternSO);
        }

        if (createDatabase && createdPatterns.Count > 0)
        {
            BossPatternDatabaseSO database = ScriptableObject.CreateInstance<BossPatternDatabaseSO>();
            database.patterns = createdPatterns;

            AssetDatabase.CreateAsset(database, $"{outputFolder}/bossPatternDatabase.asset");
            EditorUtility.SetDirty(database);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", $"Created {createdPatterns.Count} Boss Pattern ScriptableObjects!", "OK");
    }

    private void ConvertPlayerSkillJson()
    {
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        string jsonText = File.ReadAllText(jsonFilePath);
        List<PlayerSkillData> skillDataList = JsonConvert.DeserializeObject<List<PlayerSkillData>>(jsonText);

        List<PlayerSkillSO> createdSkills = new List<PlayerSkillSO>();

        foreach (var skillData in skillDataList)
        {
            PlayerSkillSO playerskillSO = ScriptableObject.CreateInstance<PlayerSkillSO>();

            playerskillSO.id = skillData.id;
            playerskillSO.playerSkillName = skillData.playerSkillName;
            playerskillSO.playerSkillKey = skillData.playerSkillKey;
            playerskillSO.playerSkillTypeString = skillData.playerSkillTypeString;

            if (Enum.TryParse(skillData.playerSkillTypeString, out PlayerSkillType skillTypeParsed))
            {
                playerskillSO.playerSkillType = skillTypeParsed;
            }
            else
            {
                Debug.LogWarning($"'{skillData.playerSkillName}'의 잘못된 스킬 타입: {skillData.playerSkillTypeString}");
            }

            playerskillSO.playerWeaponTypeString = skillData.playerWeaponTypeString;
            playerskillSO.playerDescription = skillData.playerDescription;
            playerskillSO.playerSkillCooldown = skillData.playerSkillCooldown;
            playerskillSO.playerSkillDamage = skillData.playerSkillDamage;
            playerskillSO.playerUseWeaponDamage = skillData.playerUseWeaponDamage;
            playerskillSO.playerTotalDamageFormula = skillData.playerTotalDamageFormula;
            playerskillSO.playerBuffType = skillData.playerBuffType;
            playerskillSO.playerBuffValue = skillData.playerBuffValue;
            playerskillSO.playerDuration = skillData.playerDuration ?? 0f;

            createdSkills.Add(playerskillSO);

            AssetDatabase.CreateAsset(playerskillSO, $"{outputFolder}/{playerskillSO.playerSkillName}.asset");
            EditorUtility.SetDirty(playerskillSO);
        }

        if (createDatabase && createdSkills.Count > 0)
        {
            PlayerSkillDatabaseSO database = ScriptableObject.CreateInstance<PlayerSkillDatabaseSO>();
            database.skills = createdSkills;

            AssetDatabase.CreateAsset(database, $"{outputFolder}/playerSkillDatabase.asset");
            EditorUtility.SetDirty(database);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", $"Created {createdSkills.Count} Player Skill ScriptableObjects!", "OK");
    }
}
#endif
