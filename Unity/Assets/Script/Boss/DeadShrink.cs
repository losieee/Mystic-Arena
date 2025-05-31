using System.Collections;
using UnityEngine;

public class DeadShrink : MonoBehaviour
{
    public float duration = 3f;


    private void Start()
    {
        
    }
    private void Update()
    {
        if (BossController.portalClosed)
        {
            StartCoroutine(ScaleAndDestroy());
        }
    }

    private IEnumerator ScaleAndDestroy()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
        Destroy(gameObject);
    }
}