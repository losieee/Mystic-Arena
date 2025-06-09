using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kino;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    //public GameObject NetworkManagerUI;
    public AudioClip clickSound;     // ���� �߰�: ��ư Ŭ�� ȿ����
    public AudioClip glitchSound;    // ���� �ִ� �۸�ġ ȿ����
    public AudioClip plusSound; // + Ŭ���� �� ȿ����
    private AudioSource audioSource;
    public TMP_InputField roomNameInputField;
    public GameObject playerSlotPanel;
    public GameObject backgroundBlocker;

    public DigitalGlitch glitchEffect;
    public AnalogGlitch analogglitch;
    public float clickSoundVolume = 0.5f;

 

    private bool isClicked = false;
    private bool isSlotPanelActive = false;

    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        if (glitchEffect != null)
        {
            glitchEffect.enabled = false;
            analogglitch.enabled = false;
        }

        if (playerSlotPanel != null)
            playerSlotPanel.SetActive(false);

        if (backgroundBlocker != null)
            backgroundBlocker.SetActive(false);
    }

    public void Update()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        if (isClicked) return;
        isClicked = true;

        glitchEffect.intensity = 0f;
        analogglitch.scanLineJitter = 0f;
        analogglitch.verticalJump = 0f;
        analogglitch.horizontalShake = 0f;
        analogglitch.colorDrift = 0f;

        StartCoroutine(DelayedGlitchAndSound());
    }

    public void Tutorial()
    {
        if (isClicked) return;
        isClicked = true;

        // 1. Ʃ�丮�� ��ư Ŭ�����ڸ��� ��ư ȿ���� ���
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, clickSoundVolume);
        }


        glitchEffect.intensity = 0f;
        analogglitch.scanLineJitter = 0f;
        analogglitch.verticalJump = 0f;
        analogglitch.horizontalShake = 0f;
        analogglitch.colorDrift = 0f;

        StartCoroutine(ToTutorialGlitchAndSound());
    }

    public void TogglePlayerSlotPanel()
    {
        if (isSlotPanelActive)
        {
            playerSlotPanel.SetActive(false);
            backgroundBlocker.SetActive(false);
        }
        else
        {
            audioSource.PlayOneShot(plusSound);
            playerSlotPanel.SetActive(true);
            backgroundBlocker.SetActive(true);
        }

        isSlotPanelActive = !isSlotPanelActive;
    }

    private IEnumerator ToTutorialGlitchAndSound()
    {
        yield return new WaitForSeconds(1f); // 1�� ���

        // 2. �۸�ġ ����Ʈ �ѱ�
        if (glitchEffect != null && analogglitch != null)
        {
            glitchEffect.enabled = true;
            analogglitch.enabled = true;
            StartCoroutine(IncreaseGlitch());
        }

        // 3. �۸�ġ ȿ���� ���
        if (glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
            yield return new WaitForSeconds(glitchSound.length);
        }

        SceneManager.LoadScene("Tutorial");
    }

    private IEnumerator DelayedGlitchAndSound()
    {
        yield return new WaitForSeconds(1f); // 1�� ���

        if (glitchEffect != null && analogglitch != null)
        {
            glitchEffect.enabled = true;
            analogglitch.enabled = true;
            StartCoroutine(IncreaseGlitch());
        }

        if (glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
            yield return new WaitForSeconds(glitchSound.length);
        }

        SceneManager.LoadScene("Character_Choice");
    }

    private IEnumerator IncreaseGlitch()
    {
        float duration = glitchSound != null ? glitchSound.length : 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            analogglitch.scanLineJitter = Mathf.Lerp(0.1f, 1.0f, t);
            analogglitch.verticalJump = Mathf.Lerp(0.05f, 0.5f, t);
            analogglitch.horizontalShake = Mathf.Lerp(0.1f, 0.8f, t);
            analogglitch.colorDrift = Mathf.Lerp(0.1f, 1.0f, t);
            glitchEffect.intensity = Mathf.Lerp(0.0f, 1.0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
 
    // �гθ��� �ٸ� �� Ŭ������ �� �г� ��Ȱ��ȭ
    private void ActivateBackgroundBlocker(bool active)
    {
        backgroundBlocker.SetActive(active);
    }
    public void OnBackgroundClicked()
    {
        if (isSlotPanelActive)
        {
            playerSlotPanel.SetActive(false);
            backgroundBlocker.SetActive(false);
            isSlotPanelActive = false;
        }
    }
}
