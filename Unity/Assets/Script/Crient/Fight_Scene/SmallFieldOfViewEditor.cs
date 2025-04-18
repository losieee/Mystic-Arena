#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SmallFieldOfView))]

public class SmallFieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        // SmallFieldOfView 스크립트 가져오기
        SmallFieldOfView fow = (SmallFieldOfView)target;
        // 시야 반경 원으로 표시
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.smallViewRadius);
        // 시야각 표시
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.smallViewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.smallViewRadius);

        // 적 발견 시 표시
        Handles.color = Color.red;
        foreach (Transform visible in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visible.transform.position);
        }
    }
}
#endif