using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EdgeDetectSpecifyObjectManager : MonoBehaviour
{
    public Vector3 drawPositionOffset;
    public Material mat;
    private Camera _camera;
    private EdgeDetectOutlineInfo[] edgeDetectOutlineInfos;
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.DepthNormals;
        
        //find all EdgeDetectOutlineInfo in scene
        edgeDetectOutlineInfos = FindObjectsOfType<EdgeDetectOutlineInfo>();
    }

    private void Update()
    {
        //init
        if (!Application.isPlaying)
        {
            edgeDetectOutlineInfos = FindObjectsOfType<EdgeDetectOutlineInfo>();
        }
        
        
        //execute all draw
        foreach (var edgeDetectOutlineInfo in edgeDetectOutlineInfos)
        {
            // print(edgeDetectOutlineInfo.name); 
            if(edgeDetectOutlineInfo != null && edgeDetectOutlineInfo.enableOutline)
                DrawObject(edgeDetectOutlineInfo.meshFilter, edgeDetectOutlineInfo.materialPropertyBlock);
        }
    }

    public void DrawObject(MeshFilter target, MaterialPropertyBlock materialPropertyBlock)
    {
        Matrix4x4 drawMatrix = target.transform.localToWorldMatrix;

        drawMatrix.m03 += drawPositionOffset.x;
        drawMatrix.m13 += drawPositionOffset.y;
        drawMatrix.m23 += drawPositionOffset.z;

        Graphics.DrawMesh(target.sharedMesh, drawMatrix, mat, 0, null, 0, materialPropertyBlock);
    }
}
