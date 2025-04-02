using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance { get; private set; }
    private ButtonEffect selectedButton = null;
    private float timeLimit = 10f; // 제한시간 10초
    private bool isTimeOver = false; // 시간 초과 여부
    public TMP_Text timerText; // UI에 표시할 타이머 텍스트

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 제한시간 카운트다운 시작
        Invoke("TimeOver", timeLimit);
    }

    void Update()
    {
        if (!isTimeOver)
        {
            timeLimit -= Time.deltaTime; // 남은 시간 감소
            if (timeLimit < 0) timeLimit = 0; // 0초 이하로 내려가지 않도록 방지
            UpdateTimerUI();
        }
    }

    // UI 타이머 업데이트
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = timeLimit.ToString("F0"); // 정수로 표시
        }
    }

    // 버튼을 눌러도 즉시 씬 이동하지 않고 선택만 유지
    public bool CanSelect()
    {
        return selectedButton == null && !isTimeOver;
    }

    // 버튼이 선택되었을 때 호출
    public void SetSelectedButton(ButtonEffect button)
    {
        selectedButton = button;
        DisableOtherButtons();
    }

    // 시간이 다 지나면, 버튼이 눌렸는지 여부에 따라 씬 이동
    private void TimeOver()
    {
        isTimeOver = true;
        if (selectedButton != null)
        {
            SceneManager.LoadScene("SampleScene"); // 버튼이 눌렸다면 SampleScene으로 이동
        }
        else
        {
            SceneManager.LoadScene("Lobby"); // 아무 버튼도 안 눌렸다면 Lobby로 이동
        }
    }

    // 다른 버튼을 비활성화
    private void DisableOtherButtons()
    {
        ButtonEffect[] allButtons = FindObjectsOfType<ButtonEffect>();
        foreach (ButtonEffect button in allButtons)
        {
            if (button != selectedButton)
            {
                button.DisableButton();
            }
        }
    }
}