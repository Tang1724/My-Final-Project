using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class EdgeGlowEffect : MonoBehaviour
{
    public Shader edgeGlowShader; // 边缘发光 Shader
    public Color edgeColor = new Color(0, 0.5f, 1, 1); // 边缘发光颜色 (默认亮蓝色)
    public Color fillColor = new Color(0, 0, 0.2f, 0.5f); // 填充颜色 (默认半透明暗蓝色)
    [Range(0, 1)]
    public float edgeThreshold = 0.1f; // 边缘检测阈值
    [Range(0, 5)]
    public float glowIntensity = 2; // 发光强度

    private Material material;

    void Start()
    {
        // 检查 Shader 是否支持
        if (edgeGlowShader == null || !edgeGlowShader.isSupported)
        {
            enabled = false;
            Debug.LogWarning("Shader 不支持或未设置！");
            return;
        }

        // 创建材质
        material = new Material(edgeGlowShader);
        material.hideFlags = HideFlags.HideAndDontSave;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            // 设置 Shader 参数
            material.SetColor("_EdgeColor", edgeColor);
            material.SetColor("_FillColor", fillColor);
            material.SetFloat("_EdgeThreshold", edgeThreshold);
            material.SetFloat("_GlowIntensity", glowIntensity);

            // 应用后处理
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    void OnDisable()
    {
        if (material != null)
        {
            DestroyImmediate(material);
        }
    }
}
