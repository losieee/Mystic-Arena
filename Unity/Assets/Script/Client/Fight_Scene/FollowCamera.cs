using UnityEngine;
using System.Collections.Generic;

public static class ObstructionManager
{
    public static HashSet<GameObject> transparentObjects = new HashSet<GameObject>();
}

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -7);
    public float smoothSpeed = 5f;
    public LayerMask obstructionMask;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private List<Renderer> currentObstructions = new List<Renderer>();

    public Material transparentMaterial;

    void LateUpdate()
    {
        if (target == null) return;

        // Follow
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(target);

        HandleObstructions();
    }

    void HandleObstructions()
    {
        // 원상 복구
        foreach (Renderer rend in currentObstructions)
        {
            if (rend != null)
            {
                if (originalMaterials.ContainsKey(rend))
                    rend.materials = originalMaterials[rend];

                ObstructionManager.transparentObjects.Remove(rend.gameObject);
            }
        }
        currentObstructions.Clear();
        originalMaterials.Clear();

        // 카메라 ↔ 타겟 사이 Ray 감지
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, obstructionMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null && !originalMaterials.ContainsKey(rend))
            {
                originalMaterials[rend] = rend.materials;

                Material[] newMats = new Material[rend.materials.Length];
                for (int i = 0; i < newMats.Length; i++)
                    newMats[i] = transparentMaterial;

                rend.materials = newMats;

                currentObstructions.Add(rend);
                ObstructionManager.transparentObjects.Add(rend.gameObject); // 여기서 등록
            }
        }
    }


}
