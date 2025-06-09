using UnityEngine;
using TMPro; // TextMeshPro�� ����ϴ� ��� �߰�
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    private const int MaxAI = 3;
    public List<GameObject> aiTextObjects; // �̸� ��ġ�� AI Text ������Ʈ���� ������ ����Ʈ
    private int currentTextIndex = 0;

    private void Start()
    {
        // �κ� �����ڸ��� AI �߰� �� �ʱ�ȭ �� Text ������Ʈ �ʱ� ���� ����
        PlayerPrefs.SetInt("AICount", 0);
        PlayerPrefs.Save();
        InitializeTextObjects();
    }

    public void AddAI()
    {
        int currentCount = PlayerPrefs.GetInt("AICount", 0);

        if (currentCount >= MaxAI)
        {
            Debug.Log("AI�� �ִ� 3������� �߰��� �� �ֽ��ϴ�.");
            return;
        }

        currentCount++;
        PlayerPrefs.SetInt("AICount", currentCount);
        PlayerPrefs.Save();

        Debug.Log($"AI �� �� �߰��� (�� AI ��: {currentCount})");

        // ���� Text ������Ʈ Ȱ��ȭ
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
                    textObject.SetActive(false); // �ʱ⿡�� ��� Text ������Ʈ ��Ȱ��ȭ
                }
            }
            currentTextIndex = 0; // �ε��� �ʱ�ȭ
        }
        else
        {
            Debug.LogError("AI Text ������Ʈ ����Ʈ�� ������� �ʾҽ��ϴ�.");
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