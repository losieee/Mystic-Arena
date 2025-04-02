using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // "게임 시작" 버튼을 누르면 Character_Choice 씬으로 이동
    public void StartGame()
    {
        SceneManager.LoadScene("Character_Choice");
    }
}