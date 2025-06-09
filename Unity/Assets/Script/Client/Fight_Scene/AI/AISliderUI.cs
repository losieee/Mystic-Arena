using UnityEngine;
using UnityEngine.UI;

public class AISliderUI : MonoBehaviour
{
    private Slider slider;
    private Transform target; // AI Ʈ������
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

    // AI ���� ü�� �ٸ� ǥ���ϵ��� ������Ʈ
    void Update()
    {
        if (target != null && slider != null)
        {
            // ���� ��ǥ�� ĵ���� ��ǥ�� ��ȯ
            Vector3 worldPosition = target.position + offset; // ������ ����
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            // ĵ���� RectTransform�� ������ ���� ��ǥ�� ��ȯ
            if (slider.transform.parent is RectTransform canvasRect)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out Vector2 localPosition);
                slider.transform.localPosition = localPosition;
            }
        }
    }
}