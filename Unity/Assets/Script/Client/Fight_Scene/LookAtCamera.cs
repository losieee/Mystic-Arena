using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCameraTransform;
    void Start()
    {
        // 씬에 MainCamera 태그가 붙은 카메라를 찾습니다.
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCameraTransform = mainCamera.transform;
        }
        else
        {
            Debug.LogError("No MainCamera found in the scene. Please tag your main camera as 'MainCamera'.");
            enabled = false; // Main Camera가 없으면 스크립트를 비활성화합니다.
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // UI 요소가 카메라를 바라보도록 회전시킵니다.
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                             mainCameraTransform.rotation * Vector3.up);
        }
    }
}