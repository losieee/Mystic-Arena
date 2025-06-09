using UnityEngine;

public class CameraTransparencyController : MonoBehaviour
{
    [SerializeField] private Transform targetObject; // �����ϰ� ���� ������Ʈ
    [SerializeField] private Transform centerObject; // ������� �� (ĳ����)
    [SerializeField] private float cylinderRadius = 2f; // ����� ������
    [SerializeField][Range(0f, 1f)] private float transparencyAmount = 0.3f; // ����

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
            Debug.LogError("Target Object �Ǵ� Renderer�� �����ϴ�.");
            enabled = false;
        }

        if (centerObject == null)
        {
            Debug.LogError("Center Object�� �Ҵ���� �ʾҽ��ϴ�.");
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
            // ���̴��� ����� ������ (ī�޶� ���� ��ġ) ����
            objectMaterial.SetVector(CylinderStartId, mainCamera.transform.position);
            // ���̴��� ����� ���� (ĳ���� ���� ��ġ) ����
            objectMaterial.SetVector(CylinderEndId, centerObject.position);
            // ���̴��� ������ ����
            objectMaterial.SetFloat(RadiusId, cylinderRadius);

            // ī�޶�� ĳ���� ���̿� �ִ��� Raycast�� Ȯ�� (����ȭ�� ���� �ʿ��ϴٸ�)
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