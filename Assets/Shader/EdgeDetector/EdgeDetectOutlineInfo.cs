using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(EdgeDetectOutlineInfo))]
public class EdgeDetectOutlineInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("选择相机对象"))
        {
            Camera camera = FindObjectOfType<Camera>();
            Selection.objects = new UnityEngine.Object[] {camera.gameObject};
        }
    }
}
#endif


[ExecuteInEditMode]
public class EdgeDetectOutlineInfo : MonoBehaviour
{
    public bool enableOutline = true;
    
    [ColorUsage(true, true)] 
    public Color _EdgeColor = Color.cyan;
    public float _SampleDistance = 60;
    public float _NormalSensitivity = 1;
    public float _DepthSensitivity = 1;

    [HideInInspector]
    public MeshFilter meshFilter;
    [HideInInspector]
    public MaterialPropertyBlock materialPropertyBlock;//脚本使用MPB使得在相同材质可以有不同属性
    
    private void Update()
    {
        //init
        if(meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();
        if(materialPropertyBlock == null)
            materialPropertyBlock = new MaterialPropertyBlock();

        materialPropertyBlock.SetColor("_EdgeColor", _EdgeColor);
        materialPropertyBlock.SetFloat("_SampleDistance", _SampleDistance);
        materialPropertyBlock.SetFloat("_NormalSensitivity", _NormalSensitivity);
        materialPropertyBlock.SetFloat("_DepthSensitivity", _DepthSensitivity);
    }
}
