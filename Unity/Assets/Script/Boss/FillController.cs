using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillController : MonoBehaviour
{
    public MeshRenderer fillRenderer;
    public float fillDuration = 2f;

    private Material fillMaterial;
    private float elapsed = 0f;

    private void Start()
    {
        if (fillRenderer == null)
        {
            enabled = false;
            return;
        }

        // 머티리얼 인스턴스화 (다른 오브젝트와 공유되지 않게)
        fillMaterial = fillRenderer.material;
        fillMaterial.SetFloat("_FillAmount", 0f);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float fill = Mathf.Clamp01(elapsed / fillDuration);
        fillMaterial.SetFloat("_FillAmount", fill);

        if (fill >= 1f)
        {
            // 오브젝트 삭제 (BasePlane 포함 WarningArea 전체)
            Destroy(transform.root.gameObject);
        }
    }
}
