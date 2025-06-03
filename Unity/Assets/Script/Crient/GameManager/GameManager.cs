using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

  

    public int WabeCount = 3;
    public int EnemyCount = 5;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // (선택) 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
