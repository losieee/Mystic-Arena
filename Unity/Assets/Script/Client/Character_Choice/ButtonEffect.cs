using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image hoverImage; // ���콺 �ø��� ��Ÿ���� �̹���
    public Sprite flippedSprite; // ��ư�� �������� ���� �̹���
    private Button button;
    private bool isFlipped = false;
    private bool isSelected = false; // ��ư�� ���õǾ����� ����
    private bool isDisabled = false; // ��Ȱ��ȭ ����
    public GameObject prefabToSpawn;


    void Start()
    {
        button = GetComponent<Button>();
        hoverImage.gameObject.SetActive(false); // ó������ ����
    }

    // ���콺�� �ø��� hoverImage Ȱ��ȭ (��, ���õ��� ���� ��� & ��Ȱ��ȭ���� ���� ��츸)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && !isDisabled)
        {
            hoverImage.gameObject.SetActive(true);
        }
    }

    // ���콺�� ����� hoverImage ��Ȱ��ȭ (��, ���õ��� ���� ��츸)
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            hoverImage.gameObject.SetActive(false);
        }
    }

    // ��ư Ŭ�� �� �ִϸ��̼� ���� �� hoverImage ����
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFlipped && ButtonManager.Instance.CanSelect())
        {
            isSelected = true;
            OnButtonClick(); // �� ���⿡ ������ ���� �� ���� ó��

            hoverImage.gameObject.SetActive(true);
            StartCoroutine(FlipButton());
        }
    }

    IEnumerator FlipButton()
    {
        float duration = 0.3f; // �ִϸ��̼� ���� �ð�
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
        button.image.sprite = flippedSprite; // ��ư �̹��� ����
        isFlipped = true;
    }

    // �ٸ� ��ư�� ���õǾ��� �� ��Ȱ��ȭ (���콺 �÷��� hoverImage �� ���̰� ����)
    public void DisableButton()
    {
        button.interactable = false;
        isDisabled = true;  // ��ư ��Ȱ��ȭ ���·� ����
        hoverImage.gameObject.SetActive(false); // hoverImage ����
    }
    public void OnButtonClick()
    {
        if (ButtonManager.Instance.CanSelect())
        {
            ButtonManager.Instance.selectedPrefab = prefabToSpawn; // ������ ����
            ButtonManager.Instance.SetSelectedButton(this);
        }
    }
}
