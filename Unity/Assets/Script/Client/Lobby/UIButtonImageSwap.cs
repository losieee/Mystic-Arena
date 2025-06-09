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
    private static bool hasInitialized = false; // 중복 초기화 방지용

    void Awake()
    {
        buttonImage = GetComponent<Image>();

        // static 초기화는 한 번만!
        if (!hasInitialized)
        {
            currentlySelected = null;
            hasInitialized = true;
        }
    }

    void Start()
    {
        // 씬이 시작될 때 이벤트 시스템이 정상적으로 작동하는지 확인 (EventSystem 체크)
        if (!EventSystem.current)
        {
            Debug.LogWarning("EventSystem not found in scene!");
        }

        // PlayerPrefs에 저장된 선택된 버튼 확인
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
            hasInitialized = false; // 씬 전환 후 초기화가 되도록 설정
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

    // 씬 전환 후 UI 리셋
    public static void ResetUI()
    {
        currentlySelected = null; // 씬 전환 후 초기화
    }
}