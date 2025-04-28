using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NetworkInputData 구조체 정의
public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 0x01;
    public const byte MOUSEBUTTON1 = 0x02;
    public NetworkButtons buttons;
    public Vector3 direction;
    public Vector3 lookDirection;
}

// NetworkInputHandler 클래스
public class NetworkInputHandler : MonoBehaviour
{
    public NetworkInputData GetNetworkInput()
    {
        var data = new NetworkInputData();

        float horizontal = Input.GetAxis("Horizontal");                 // 좌 우
        float vertical = Input.GetAxis("Vertical");                     // 위 아레

        data.direction = new Vector3(horizontal, 0, vertical);

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            data.direction = cameraRight * horizontal + cameraForward * vertical;
        }

        if (data.direction.magnitude > 1)
        {
            data.direction.Normalize();
        }

        data.lookDirection = mainCamera != null ? mainCamera.transform.forward : Vector3.forward;

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, Input.GetMouseButton(0));
        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, Input.GetMouseButton(1));

        return data;
    }
}