using UnityEngine;
using LibCSG;
using System.Collections;
using System;

public class BooleanOperations : MonoBehaviour
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
private bool isProcessing = false;

private const float operationCooldown = 1f;

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
            StartCoroutine(PerformBooleanWithCooldown());
                GC.Collect();
        }
    }
}

private IEnumerator PerformBooleanWithCooldown()
{
    isProcessing = true;

    if (!ValidateInputObjects())
    {
        isProcessing = false;
        yield break;
    }

    if (operationType == Operation.OPERATION_SUBTRACTION && playerInsideDetector.isPlayerInside)
    {
        Debug.Log("⚠️ 玩家已在区域内，且当前为 Subtraction 模式，跳过布尔运算。");
        isProcessing = false;
        yield break;
    }

    AudioManager.instance.PlaySound("Camera", false);
    yield return null;

    try
    {
        PerformBooleanOperation();
    }
    catch (System.Exception ex)
    {
        Debug.LogError("❌ 布尔运算发生异常：" + ex.Message);
    }

    isProcessing = false;
}

private void PerformBooleanOperation()
{
    GameObject target = null;

    if (flashlight.spot1.activeSelf)
    {
        target = objectB;
        Debug.Log("🔦 Spot1 开启，执行 objectB 的布尔运算...");
    }
    else if (flashlight.spot2.activeSelf)
    {
        target = objectC;
        Debug.Log("🔦 Spot2 开启，执行 objectC 的布尔运算...");
    }

    if (target != null)
    {
        TryPerformBoolean(objectA, target);
    }

    CameraFlashEffect cameraFlash = FindObjectOfType<CameraFlashEffect>();
    if (cameraFlash != null)
    {
        cameraFlash.TakePhoto();
    }
}

private void TryPerformBoolean(GameObject a, GameObject b)
{
    if (!IsMeshReadable(a) || !IsMeshReadable(b)) return;

    var brushA = new CSGBrush(a);
    brushA.build_from_mesh(a.GetComponent<MeshFilter>().sharedMesh);

    var brushB = new CSGBrush(b);
    brushB.build_from_mesh(b.GetComponent<MeshFilter>().sharedMesh);

    var result = new CSGBrush(resultObject);
    csgOperation.merge_brushes(operationType, brushA, brushB, ref result);

    Mesh resultMesh = result.getMesh();

    if (IsMeshValid(resultMesh))
    {
        Debug.Log($"✅ 布尔结果 Mesh 顶点数: {resultMesh.vertexCount}, 三角形数: {resultMesh.triangles.Length / 3}");
        ApplyMeshToResult(resultMesh);
    }
    else
    {
        Debug.LogWarning("⚠ 布尔结果无效，跳过更新！");
    }
}

private void ApplyMeshToResult(Mesh sourceMesh)
{
    if (resultObject == null) return;

    // 生成新的 mesh，避免破坏 sharedMesh
    Mesh newMesh = new Mesh
    {
        vertices = sourceMesh.vertices,
        triangles = sourceMesh.triangles,
        uv = sourceMesh.uv
    };
    newMesh.RecalculateNormals();
    newMesh.RecalculateBounds();

    MeshFilter mf = resultObject.GetComponent<MeshFilter>();
    if (mf == null) mf = resultObject.AddComponent<MeshFilter>();
    mf.mesh = newMesh; // ✅ 使用 mesh 而不是 sharedMesh

    MeshCollider mc = resultObject.GetComponent<MeshCollider>();
    if (mc == null) mc = resultObject.AddComponent<MeshCollider>();
    mc.sharedMesh = null;
    mc.sharedMesh = newMesh;
}

private bool IsMeshValid(Mesh mesh)
{
    return mesh != null && mesh.vertexCount > 0 && mesh.triangles.Length >= 3;
}

private bool IsMeshReadable(GameObject go)
{
    var mf = go.GetComponent<MeshFilter>();
    if (mf == null || mf.sharedMesh == null)
    {
        Debug.LogError($"❌ {go.name} 没有 MeshFilter 或 Mesh！");
        return false;
    }

    if (!mf.sharedMesh.isReadable)
    {
        Debug.LogError($"❌ {go.name} 的 Mesh 不可读！");
        return false;
    }

    return true;
}

private void ValidateResultObject()
{
    if (resultObject == null)
    {
        Debug.LogError("⚠️ 结果物体未设置！");
    }
}

private bool ValidateInputObjects()
{
    if (objectA == null || (objectB == null && objectC == null))
    {
        Debug.LogWarning("⚠️ 缺少 objectA 或布尔目标！");
        return false;
    }
    return true;
}

private void InitializeResultObjectMesh()
{
    if (resultObject == null || objectA == null)
    {
        Debug.LogError("⚠️ 初始化失败！resultObject 或 objectA 未设置！");
        return;
    }

    MeshFilter sourceMF = objectA.GetComponent<MeshFilter>();
    if (sourceMF == null || sourceMF.sharedMesh == null)
    {
        Debug.LogError("⚠️ objectA 缺少有效的 Mesh！");
        return;
    }

    MeshFilter resultMF = resultObject.GetComponent<MeshFilter>();
    if (resultMF == null) resultMF = resultObject.AddComponent<MeshFilter>();

    MeshRenderer renderer = resultObject.GetComponent<MeshRenderer>();
    if (renderer == null) renderer = resultObject.AddComponent<MeshRenderer>();

    resultMF.sharedMesh = Instantiate(sourceMF.sharedMesh);

    MeshCollider mc = resultObject.GetComponent<MeshCollider>();
    if (mc == null) mc = resultObject.AddComponent<MeshCollider>();
    mc.sharedMesh = resultMF.sharedMesh;

    Debug.Log("✅ resultObject Mesh 初始化完成！");
}
}