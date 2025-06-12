using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSequenceManager : MonoBehaviour
{
    public static SceneSequenceManager Instance;

    public List<string> sceneSequence = new List<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9"
    };

    public int currentSceneIndex = 0;

    private readonly HashSet<string> allowedScenes = new HashSet<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9"
    };

    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (!allowedScenes.Contains(currentScene))
        {
            Debug.Log($"[SceneSequenceManager] ���� ��({currentScene})�� ������ �ʾ� ������Ʈ�� �ı��մϴ�.");
            Destroy(gameObject);
            return;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[SceneSequenceManager] �ʱ�ȭ �Ϸ�");
    }

    public void AdvanceToNextScene()
    {
        if (currentSceneIndex + 1 < sceneSequence.Count)
        {
            currentSceneIndex++;
            Debug.Log($"[SceneSequenceManager] currentSceneIndex ������ �� {currentSceneIndex}");
        }
        else
        {
            Debug.Log("[SceneSequenceManager] ��� �������� �Ϸ�� �� ������ Stage ����");
        }
    }

    public string GetCurrentScene()
    {
        return sceneSequence[currentSceneIndex];
    }

    public string PeekNextScene()
    {
        if (currentSceneIndex + 1 < sceneSequence.Count)
        {
            return sceneSequence[currentSceneIndex + 1];
        }
        else
        {
            return null; // ������ �������� ����
        }
    }
}
