using UnityEngine;
using LibCSG1;
using System.Collections;
using System;

public class BooleanOperationsSplit : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public GameObject objectC;
    public GameObject resultObject;
    public Operation operationType = Operation.OPERATION_SUBTRACTION;
    public Flashlight flashlight;
    private PlayerFlashlight playerFlashlight;
    public PlayerInsideDetector playerInsideDetector;

    private CSGBrushOperation csgOperation;
    private Mesh reusableMesh;
    private bool isProcessing = false;

    void Start()
    {
        playerFlashlight = FindObjectOfType<PlayerFlashlight>();
        csgOperation = new CSGBrushOperation();
        ValidateResultObject();

        if (operationType == Operation.OPERATION_SUBTRACTION)
        {
            InitializeResultObjectMesh();
        }
    }

    void Update()
    {
        if (playerFlashlight.currentCamera && flashlight.IsSpotlightOn)
        {
            if (Input.GetKeyDown(KeyCode.F) && !isProcessing)
            {
                StartCoroutine(PerformBooleanSplit());
            }
        }
    }

    private IEnumerator PerformBooleanSplit()
    {
        isProcessing = true;

        if (!ValidateInputObjects())
        {
            isProcessing = false;
            yield break;
        }

        if (operationType == Operation.OPERATION_SUBTRACTION && playerInsideDetector.isPlayerInside)
        {
            Debug.Log("⚠️ 玩家在区域内，跳过布尔运算");
            isProcessing = false;
            yield break;
        }

        AudioManager.instance.PlaySound("Camera", false);
        yield return null;

        GameObject target = flashlight.spot1.activeSelf ? objectB : flashlight.spot2.activeSelf ? objectC : null;
        if (target == null)
        {
            Debug.LogWarning("⚠️ 没有布尔目标");
            isProcessing = false;
            yield break;
        }

        // 分帧执行布尔逻辑
        yield return StartCoroutine(DoBooleanInFrames(objectA, target));

        CameraFlashEffect flash = FindObjectOfType<CameraFlashEffect>();
        if (flash != null) flash.TakePhoto();

        isProcessing = false;
    }

    private IEnumerator DoBooleanInFrames(GameObject a, GameObject b)
    {
        if (!IsMeshReadable(a) || !IsMeshReadable(b))
        {
            Debug.LogError("❌ Mesh 不可读，取消布尔运算");
            yield break;
        }

        // Step 1: 构建 BrushA
        var brushA = new CSGBrush(a);
        brushA.build_from_mesh(a.GetComponent<MeshFilter>().sharedMesh);
        yield return null;

        // Step 2: 构建 BrushB
        var brushB = new CSGBrush(b);
        brushB.build_from_mesh(b.GetComponent<MeshFilter>().sharedMesh);
        yield return null;

        // Step 3: 执行布尔运算
        var result = new CSGBrush(resultObject);
        csgOperation.merge_brushes(operationType, brushA, brushB,  result);
        yield return null;

        // Step 4: 获取结果 mesh
        Mesh resultMesh = result.getMesh();
        if (IsMeshValid(resultMesh))
        {
            Debug.Log($"✅ 布尔完成：顶点={resultMesh.vertexCount}, 三角形={resultMesh.triangles.Length / 3}");
            ApplyMeshToResult(resultMesh);
        }
        else
        {
            Debug.LogWarning("⚠️ 布尔结果无效");
        }
    }

    private void ApplyMeshToResult(Mesh sourceMesh)
    {
        if (resultObject == null) return;

        if (reusableMesh == null)
            reusableMesh = new Mesh();
        else
            reusableMesh.Clear();

        reusableMesh.vertices = sourceMesh.vertices;
        reusableMesh.triangles = sourceMesh.triangles;
        reusableMesh.uv = sourceMesh.uv;
        reusableMesh.RecalculateNormals();
        reusableMesh.RecalculateBounds();

        MeshFilter mf = resultObject.GetComponent<MeshFilter>() ?? resultObject.AddComponent<MeshFilter>();
        mf.mesh = reusableMesh;

        MeshCollider mc = resultObject.GetComponent<MeshCollider>() ?? resultObject.AddComponent<MeshCollider>();
        mc.sharedMesh = null;
        mc.sharedMesh = reusableMesh;
    }

    private bool IsMeshValid(Mesh mesh)
    {
        return mesh != null && mesh.vertexCount > 0 && mesh.triangles.Length >= 3;
    }

    private bool IsMeshReadable(GameObject obj)
    {
        var mf = obj.GetComponent<MeshFilter>();
        return mf != null && mf.sharedMesh != null && mf.sharedMesh.isReadable;
    }

    private void ValidateResultObject()
    {
        if (resultObject == null)
            Debug.LogError("⚠️ resultObject 未设置");
    }

    private bool ValidateInputObjects()
    {
        return objectA != null && (objectB != null || objectC != null);
    }

    private void InitializeResultObjectMesh()
    {
        if (resultObject == null || objectA == null)
        {
            Debug.LogError("⚠️ 初始化失败：未设置 objectA 或 resultObject");
            return;
        }

        var sourceMF = objectA.GetComponent<MeshFilter>();
        if (sourceMF == null || sourceMF.sharedMesh == null)
        {
            Debug.LogError("⚠️ objectA 没有有效 Mesh");
            return;
        }

        var resultMF = resultObject.GetComponent<MeshFilter>() ?? resultObject.AddComponent<MeshFilter>();
        resultMF.sharedMesh = Instantiate(sourceMF.sharedMesh);

        if (resultObject.GetComponent<MeshRenderer>() == null)
            resultObject.AddComponent<MeshRenderer>();

        var mc = resultObject.GetComponent<MeshCollider>() ?? resultObject.AddComponent<MeshCollider>();
        mc.sharedMesh = resultMF.sharedMesh;

        Debug.Log("✅ ResultObject 初始化完成");
    }
}