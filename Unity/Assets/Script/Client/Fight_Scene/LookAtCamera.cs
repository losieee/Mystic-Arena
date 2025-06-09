using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCameraTransform;
    void Start()
    {
        // ���� MainCamera �±װ� ���� ī�޶� ã���ϴ�.
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCameraTransform = mainCamera.transform;
        }
        else
        {
            Debug.LogError("No MainCamera found in the scene. Please tag your main camera as 'MainCamera'.");
            enabled = false; // Main Camera�� ������ ��ũ��Ʈ�� ��Ȱ��ȭ�մϴ�.
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // UI ��Ұ� ī�޶� �ٶ󺸵��� ȸ����ŵ�ϴ�.
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                             mainCameraTransform.rotation * Vector3.up);
        }
    }
}