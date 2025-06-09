using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIButtonImageSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Optional Scene Load")]
    public string sceneToLoad = "";

    public Sprite defaultSprite;
    public Sprite hoverSprite;
    public Sprite selectedSprite;
    public bool isInitiallySelected = false;

    private Image buttonImage;
    private static UIButtonImageSwap currentlySelected = null;
    private static bool hasInitialized = false; // �ߺ� �ʱ�ȭ ������

    void Awake()
    {
        buttonImage = GetComponent<Image>();

        // static �ʱ�ȭ�� �� ����!
        if (!hasInitialized)
        {
            currentlySelected = null;
            hasInitialized = true;
        }
    }

    void Start()
    {
        // ���� ���۵� �� �̺�Ʈ �ý����� ���������� �۵��ϴ��� Ȯ�� (EventSystem üũ)
        if (!EventSystem.current)
        {
            Debug.LogWarning("EventSystem not found in scene!");
        }

        // PlayerPrefs�� ����� ���õ� ��ư Ȯ��
        string lastSelected = PlayerPrefs.GetString("LastSelectedButton", "");

        if (isInitiallySelected || gameObject.name == lastSelected)
        {
            SelectThisButton();
        }
        else
        {
            buttonImage.sprite = defaultSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this != currentlySelected)
            buttonImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this != currentlySelected)
            buttonImage.sprite = defaultSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sceneToLoad != "")
        {
            PlayerPrefs.SetString("LastSelectedButton", gameObject.name);
            PlayerPrefs.Save();
            hasInitialized = false; // �� ��ȯ �� �ʱ�ȭ�� �ǵ��� ����
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            SelectThisButton();
        }
    }

    private void SelectThisButton()
    {
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.buttonImage.sprite = currentlySelected.defaultSprite;
        }

        currentlySelected = this;
        buttonImage.sprite = selectedSprite;
    }

    // �� ��ȯ �� UI ����
    public static void ResetUI()
    {
        currentlySelected = null; // �� ��ȯ �� �ʱ�ȭ
    }
}