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
            DontDestroyOnLoad(gameObject); // (����) �� ��ȯ���� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
