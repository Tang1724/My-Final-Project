using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EdgeDetectNormalsAndDepth : MonoBehaviour {

	public Shader edgeDetectShader;
	public Material edgeDetectMaterial;

	[Range(0.0f, 1.0f)]
	public float edgesOnly = 0.0f;

    [ColorUsage(true, true)]
    public Color edgeColor = Color.black;

    [ColorUsage(true,true)]
    public Color backgroundColor = Color.white;

	public float sampleDistance = 1.0f;

	public float sensitivityDepth = 1.0f;

	public float sensitivityNormals = 1.0f;
	
	void OnEnable() {
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
	}

	//仅对不透明物体进行后处理
	[ImageEffectOpaque]
	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		if (edgeDetectMaterial != null) {
			edgeDetectMaterial.SetFloat("_EdgeOnly", edgesOnly);
			edgeDetectMaterial.SetColor("_EdgeColor", edgeColor);
			edgeDetectMaterial.SetColor("_BackgroundColor", backgroundColor);
			edgeDetectMaterial.SetFloat("_SampleDistance", sampleDistance);
			edgeDetectMaterial.SetVector("_Sensitivity", new Vector4(sensitivityNormals, sensitivityDepth, 0.0f, 0.0f));

			Graphics.Blit(src, dest, edgeDetectMaterial);
		} else {
			Graphics.Blit(src, dest);
		}
	}
}
