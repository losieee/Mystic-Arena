using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FillController : MonoBehaviour
{
    public MeshRenderer fillRenderer;
    public float fillDuration = 2f;
    public UnityEngine.Events.UnityEvent onFillComplete;

    private Material fillMaterial;
    private float elapsed = 0f;
    private bool completed = false;

    private void Start()
    {
        if (fillRenderer == null)
        {
            enabled = false;
            return;
        }

        fillMaterial = fillRenderer.material;
        fillMaterial.SetFloat("_FillAmount", 0f);
    }

    private void Update()
    {
        if (completed) return;

        elapsed += Time.deltaTime;
        float fill = Mathf.Clamp01(elapsed / fillDuration);
        fillMaterial.SetFloat("_FillAmount", fill);

        if (fill >= 1f)
        {
            completed = true;
            onFillComplete?.Invoke(); // 한 번만 호출됨
            Destroy(transform.root.gameObject);
        }
    }
}
