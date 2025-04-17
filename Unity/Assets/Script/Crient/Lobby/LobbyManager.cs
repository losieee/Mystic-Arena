using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public AudioClip startSound; // Start 버튼 효과음
    private AudioSource audioSource;

    private bool isClicked = false; // 중복 클릭 방지

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    // "게임 시작" 버튼을 누르면 효과음 재생 후 Character_Choice 씬으로 이동
    public void StartGame()
    {
        if (isClicked) return; // 중복 클릭 방지
        isClicked = true;

        if (startSound != null)
            audioSource.PlayOneShot(startSound);

        // 효과음 길이만큼 기다렸다가 씬 전환
        float delay = startSound != null ? startSound.length : 0f;
        Invoke(nameof(LoadNextScene), delay);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Character_Choice");
    }
}
