#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Windows;
using System;

public class JsonToScriptableConverter : EditorWindow
{
    private string jsonFilePath = "";                                           // JSON ���� ��� ���ڿ� ��
    private string outputFolder = "Assets/ScriptableObject";                    // ��� SO ������ ��ΰ�
    private bool createdatabase = true;                                         // ������ ���̽��� ����� ���̴����� ���� bool ��

    [MenuItem("Tools/Json to Scriptable Object")]

    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("Json to Scriptable Object");
    }

    private void ConvertJsonToScriptableObjects()                               // JSON ������ ScriptableObject ���Ϸ� ��ȯ �����ִ� �Լ�
    {
        // Ǯ�� ����
        if (!Directory.Exists(outputFolder))                                    // ���� ��ġ�� Ȯ���ϰ� ������ ���� �Ѵ�.
        {
            Directory.CreateDirectory(outputFolder);
        }
        
        //string jsonText = File.ReadAllText(jsonFiilePath);                   // JSON ������ �д´�.
    }
}

#endif