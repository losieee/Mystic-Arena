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
    private string jsonFilePath = "";                                           // JSON 파일 경로 문자열 값
    private string outputFolder = "Assets/ScriptableObject";                    // 출력 SO 파일을 경로값
    private bool createdatabase = true;                                         // 데이텅 베이스를 사용할 것이니지에 대한 bool 값

    [MenuItem("Tools/Json to Scriptable Object")]

    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("Json to Scriptable Object");
    }

    private void ConvertJsonToScriptableObjects()                               // JSON 파일을 ScriptableObject 파일로 변환 시켜주는 함수
    {
        // 풀더 생성
        if (!Directory.Exists(outputFolder))                                    // 폴더 위치를 확인하고 없으면 생성 한다.
        {
            Directory.CreateDirectory(outputFolder);
        }
        
        //string jsonText = File.ReadAllText(jsonFiilePath);                   // JSON 파일을 읽는다.
    }
}

#endif