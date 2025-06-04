using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextReveal : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    [TextArea(3, 10)]
    public List<string> dialogueList;

    public float delay = 0.05f;

    private Coroutine revealCoroutine;

    public void StartReveal(int dialogueIndex)
    {
        if (dialogueIndex < 0 || dialogueIndex >= dialogueList.Count)
        {
            Debug.LogWarning($"Dialogue index {dialogueIndex} is out of range!");
            return;
        }

        if (revealCoroutine != null)
            StopCoroutine(revealCoroutine);

        revealCoroutine = StartCoroutine(RevealText(dialogueList[dialogueIndex]));
    }

    private IEnumerator RevealText(string fullText)
    {
        textComponent.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            textComponent.text += fullText[i];
            yield return new WaitForSeconds(delay);
        }
    }

    public void ResetText()
    {
        if (revealCoroutine != null)
            StopCoroutine(revealCoroutine);

        textComponent.text = "";
    }
}