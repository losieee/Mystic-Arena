using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image hoverImage; // 마우스 올리면 나타나는 이미지
    public Sprite flippedSprite; // 버튼이 뒤집혔을 때의 이미지
    private Button button;
    private bool isFlipped = false;
    private bool isSelected = false; // 버튼이 선택되었는지 여부
    private bool isDisabled = false; // 비활성화 여부
    public GameObject prefabToSpawn;


    void Start()
    {
        button = GetComponent<Button>();
        hoverImage.gameObject.SetActive(false); // 처음에는 숨김
    }

    // 마우스를 올리면 hoverImage 활성화 (단, 선택되지 않은 경우 & 비활성화되지 않은 경우만)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && !isDisabled)
        {
            hoverImage.gameObject.SetActive(true);
        }
    }

    // 마우스를 벗어나면 hoverImage 비활성화 (단, 선택되지 않은 경우만)
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            hoverImage.gameObject.SetActive(false);
        }
    }

    // 버튼 클릭 시 애니메이션 실행 및 hoverImage 유지
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFlipped && ButtonManager.Instance.CanSelect())
        {
            isSelected = true;
            OnButtonClick(); // ← 여기에 프리팹 저장 및 선택 처리

            hoverImage.gameObject.SetActive(true);
            StartCoroutine(FlipButton());
        }
    }

    IEnumerator FlipButton()
    {
        float duration = 0.3f; // 애니메이션 지속 시간
        float elapsedTime = 0f;
        RectTransform rect = button.GetComponent<RectTransform>();

        while (elapsedTime < duration)
        {
            float angle = Mathf.Lerp(0, 180, elapsedTime / duration);
            rect.localRotation = Quaternion.Euler(0, angle, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rect.localRotation = Quaternion.Euler(0, 180, 0);
        button.image.sprite = flippedSprite; // 버튼 이미지 변경
        isFlipped = true;
    }

    // 다른 버튼이 선택되었을 때 비활성화 (마우스 올려도 hoverImage 안 보이게 설정)
    public void DisableButton()
    {
        button.interactable = false;
        isDisabled = true;  // 버튼 비활성화 상태로 변경
        hoverImage.gameObject.SetActive(false); // hoverImage 숨김
    }
    public void OnButtonClick()
    {
        if (ButtonManager.Instance.CanSelect())
        {
            ButtonManager.Instance.selectedPrefab = prefabToSpawn; // 프리팹 저장
            ButtonManager.Instance.SetSelectedButton(this);
        }
    }
}
