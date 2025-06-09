using UnityEngine;
using TMPro; // TextMeshPro를 사용하는 경우 추가
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    private const int MaxAI = 3;
    public List<GameObject> aiTextObjects; // 미리 배치된 AI Text 오브젝트들을 연결할 리스트
    private int currentTextIndex = 0;

    private void Start()
    {
        // 로비에 들어오자마자 AI 추가 수 초기화 및 Text 오브젝트 초기 상태 설정
        PlayerPrefs.SetInt("AICount", 0);
        PlayerPrefs.Save();
        InitializeTextObjects();
    }

    public void AddAI()
    {
        int currentCount = PlayerPrefs.GetInt("AICount", 0);

        if (currentCount >= MaxAI)
        {
            Debug.Log("AI는 최대 3명까지만 추가할 수 있습니다.");
            return;
        }

        currentCount++;
        PlayerPrefs.SetInt("AICount", currentCount);
        PlayerPrefs.Save();

        Debug.Log($"AI 한 명 추가됨 (총 AI 수: {currentCount})");

        // 다음 Text 오브젝트 활성화
        ActivateNextText();
    }

    private void InitializeTextObjects()
    {
        if (aiTextObjects != null)
        {
            foreach (GameObject textObject in aiTextObjects)
            {
                if (textObject != null)
                {
                    textObject.SetActive(false); // 초기에는 모든 Text 오브젝트 비활성화
                }
            }
            currentTextIndex = 0; // 인덱스 초기화
        }
        else
        {
            Debug.LogError("AI Text 오브젝트 리스트가 연결되지 않았습니다.");
        }
    }

    private void ActivateNextText()
    {
        if (aiTextObjects != null && currentTextIndex < aiTextObjects.Count)
        {
            if (aiTextObjects[currentTextIndex] != null)
            {
                aiTextObjects[currentTextIndex].SetActive(true);
                currentTextIndex++;
            }
        }
    }
}