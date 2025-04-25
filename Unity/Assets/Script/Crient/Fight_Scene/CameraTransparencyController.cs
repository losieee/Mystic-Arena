using UnityEngine;

public class CameraTransparencyController : MonoBehaviour
{
    [SerializeField] private Transform targetObject; // 투명하게 만들 오브젝트
    [SerializeField] private Transform centerObject; // 원기둥의 끝 (캐릭터)
    [SerializeField] private float cylinderRadius = 2f; // 원기둥 반지름
    [SerializeField][Range(0f, 1f)] private float transparencyAmount = 0.3f; // 투명도

    private Material objectMaterial;
    private Camera mainCamera;
    private static readonly int CylinderStartId = Shader.PropertyToID("_CylinderStart");
    private static readonly int CylinderEndId = Shader.PropertyToID("_CylinderEnd");
    private static readonly int RadiusId = Shader.PropertyToID("_Radius");
    private static readonly int AlphaMultiplierId = Shader.PropertyToID("_AlphaMultiplier");

    void Start()
    {
        mainCamera = Camera.main;
        if (targetObject != null && targetObject.GetComponent<Renderer>() != null)
        {
            objectMaterial = targetObject.GetComponent<Renderer>().material;
        }
        else
        {
            Debug.LogError("Target Object 또는 Renderer가 없습니다.");
            enabled = false;
        }

        if (centerObject == null)
        {
            Debug.LogError("Center Object가 할당되지 않았습니다.");
            enabled = false;
        }

        if (objectMaterial.HasProperty(AlphaMultiplierId))
        {
            Color color = objectMaterial.color;
            color.a = 1f;
            objectMaterial.color = color;
        }
    }

    void Update()
    {
        if (objectMaterial != null && centerObject != null && mainCamera != null)
        {
            // 셰이더에 원기둥 시작점 (카메라 월드 위치) 전달
            objectMaterial.SetVector(CylinderStartId, mainCamera.transform.position);
            // 셰이더에 원기둥 끝점 (캐릭터 월드 위치) 전달
            objectMaterial.SetVector(CylinderEndId, centerObject.position);
            // 셰이더에 반지름 전달
            objectMaterial.SetFloat(RadiusId, cylinderRadius);

            // 카메라와 캐릭터 사이에 있는지 Raycast로 확인 (최적화를 위해 필요하다면)
            RaycastHit hit;
            if (Physics.Linecast(mainCamera.transform.position, centerObject.position, out hit))
            {
                if (hit.transform == targetObject)
                {
                    if (objectMaterial.HasProperty(AlphaMultiplierId))
                    {
                        Color color = objectMaterial.color;
                        color.a = transparencyAmount;
                        objectMaterial.color = color;
                    }
                    else
                    {
                        SetMaterialTransparent(objectMaterial, transparencyAmount);
                    }
                }
                else
                {
                    if (objectMaterial.HasProperty(AlphaMultiplierId))
                    {
                        Color color = objectMaterial.color;
                        color.a = 1f;
                        objectMaterial.color = color;
                    }
                    else
                    {
                        SetMaterialOpaque(objectMaterial);
                    }
                }
            }
            else
            {
                if (objectMaterial.HasProperty(AlphaMultiplierId))
                {
                    Color color = objectMaterial.color;
                    color.a = 1f;
                    objectMaterial.color = color;
                }
                else
                {
                    SetMaterialOpaque(objectMaterial);
                }
            }
        }
    }
    void SetMaterialTransparent(Material material, float alpha)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        Color color = material.color;
        color.a = alpha;
        material.color = color;
        material.renderQueue = 3000;
    }

    void SetMaterialOpaque(Material material)
    {
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        Color color = material.color;
        color.a = 1f;
        material.color = color;
        material.renderQueue = -1;
    }
}