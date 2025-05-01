using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;               // 따라갈 대상
    public float height = 10.0f;           // 타겟으로부터의 높이
    public float distance = 6.0f;          // 타겟으로부터의 거리 (뒤쪽)
    public float angle = 45.0f;            // 아래로 기울어질 각도
    public float followSpeed = 10f;        // 따라가는 속도

    void LateUpdate()
    {
        if (target == null) return;

        // 카메라가 위치할 방향을 계산 (뒤쪽에서 위로 이동한 위치)
        Vector3 offset = Quaternion.Euler(angle, 0, 0) * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = target.position + offset + Vector3.up * height;

        // 부드러운 이동
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // 항상 타겟을 바라보게
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}