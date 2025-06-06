using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneSequenceManager : MonoBehaviour
{
    public static SceneSequenceManager Instance { get; private set; }

    [SerializeField]
    private List<string> sceneNames = new List<string>
    {
        "GameSceneRoom01", "GameSceneRoom02", "GameSceneRoom03", "GameSceneRoom04", "GameSceneRoom05",
        "GameSceneRoom06", "GameSceneRoom07", "GameSceneRoom08", "GameSceneRoom09", "Boss"
    };

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

    public string GetNextScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int currentIndex = sceneNames.IndexOf(currentScene);

        if (currentIndex >= 0 && currentIndex < sceneNames.Count - 1)
        {
            return sceneNames[currentIndex + 1];
        }

        Debug.LogWarning("다음 씬이 존재하지 않거나 현재 씬을 찾을 수 없습니다.");
        return null;
    }
}
