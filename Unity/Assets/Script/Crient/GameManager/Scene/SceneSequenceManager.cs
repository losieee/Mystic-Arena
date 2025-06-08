using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneSequenceManager : MonoBehaviour
{
    public static SceneSequenceManager Instance { get; private set; }

    [SerializeField]
    private List<string> sceneNames = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            string nextScene = SceneSequenceManager.Instance?.GetNextScene();
        }
    }
    public string GetNextScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int currentIndex = sceneNames.IndexOf(currentScene);

        if (currentIndex >= 0 && currentIndex < sceneNames.Count - 1)
        {
            return sceneNames[currentIndex + 1];
        }

        Debug.LogWarning("���� ���� �������� �ʰų� ���� ���� ã�� �� �����ϴ�.");
        return null;
    }
}
