using UnityEngine;
using UnityEngine.UI;

public class AISliderUI : MonoBehaviour
{
    private Slider slider;
    private Transform target; // AI 트랜스폼
    public Vector3 offset = new Vector3(0f, 2f, 0f);

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }
    }

    // AI 위에 체력 바를 표시하도록 업데이트
    void Update()
    {
        if (target != null && slider != null)
        {
            // 월드 좌표를 캔버스 좌표로 변환
            Vector3 worldPosition = target.position + offset; // 오프셋 적용
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            // 캔버스 RectTransform을 가져와 로컬 좌표로 변환
            if (slider.transform.parent is RectTransform canvasRect)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out Vector2 localPosition);
                slider.transform.localPosition = localPosition;
            }
        }
    }
}