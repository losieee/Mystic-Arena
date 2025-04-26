using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; // 카메라가 따라갈 대상
    public float distance = 5.0f; // 타겟으로부터의 거리
    public float sensitivity = 3.0f; // 마우스 회전 감도
    public Vector2 rotationLimits = new Vector2(-40, 80); // 카메라 수직 회전 제한 (최소값, 최대값)

    private float _currentX = 0.0f;
    private float _currentY = 0.0f;

    void Update()
    {
        if (target == null) return;

        // 마우스 이동에 따른 회전 처리
        _currentX += Input.GetAxis("Mouse X") * sensitivity;
        _currentY -= Input.GetAxis("Mouse Y") * sensitivity;

        // 수직 회전 각도를 제한하여 카메라가 땅 밑으로 내려가는 것을 방지
        _currentY = Mathf.Clamp(_currentY, rotationLimits.x, rotationLimits.y);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 카메라의 위치와 회전 계산
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        Vector3 position = target.position + rotation * direction;

        // 카메라가 땅 밑으로 내려가는 것을 방지하기 위해 타겟의 높이보다 카메라가 아래로 내려가지 않게 설정
        position.y = Mathf.Max(position.y, target.position.y + 1.0f); // 카메라 높이를 타겟의 y축보다 항상 높게 설정

        // 카메라의 위치와 타겟을 바라보는 방향을 설정
        transform.position = position;
        transform.LookAt(target.position);
    }

}
