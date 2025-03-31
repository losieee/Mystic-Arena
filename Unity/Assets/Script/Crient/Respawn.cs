using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // 카운트다운 UI 텍스트
    private float countdownTime = 10f; // 10초 카운트다운
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Countdown());
    }
    IEnumerator Countdown()
    {
        while (countdownTime > 0)
        {
            countdownText.text =countdownTime.ToString("F0"); // 소수점 제거
            yield return new WaitForSeconds(1.0f);
            countdownTime--;
        }

        countdownText.text = "Start!"; // 카운트다운 끝나면 표시
        yield return new WaitForSeconds(1.0f); // 1초 대기 후 씬 이동
        SceneManager.LoadScene("SampleScene");
    }
}
