using UnityEngine;
using LibCSG1;
using System.Collections;

public class BooleanOperations1 : MonoBehaviour
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
    private float lastOperationTime = -999f;
    private const float operationCooldown = 1f;
    public Player playerMouseScript;

    // 缓存 Mesh，避免频繁 GC
    private Mesh reusableMesh;

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
                if (Time.time - lastOperationTime >= operationCooldown)
                {
                    lastOperationTime = Time.time;
                    StartCoroutine(PerformBooleanWithCooldown());
                }
                else
                {
                    Debug.Log("⏳ 冷却中...");
                }
            }
        }
    }

    private IEnumerator PerformBooleanWithCooldown()
    {
        isProcessing = true;

        // ⛔ 禁止鼠标控制
        if (playerMouseScript != null)
            playerMouseScript.allowMouseControl = false;

        try
        {
            if (!ValidateInputObjects())
                yield break;

            if (operationType == Operation.OPERATION_SUBTRACTION && playerInsideDetector.isPlayerInside)
            {
                Debug.Log("⚠️ 玩家在区域中，跳过布尔运算");
                yield break;
            }

            AudioManager.instance?.PlaySound("Camera", false);
            yield return StartCoroutine(PerformBooleanOperationAsync());
        }
        finally
        {
            // ✅ 恢复鼠标控制
            if (playerMouseScript != null)
                playerMouseScript.allowMouseControl = true;

            isProcessing = false;
        }
    }

    private IEnumerator PerformBooleanOperationAsync()
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
            yield return StartCoroutine(TryPerformBooleanAsync(objectA, target));
        }

        var cameraFlash = FindObjectOfType<CameraFlashEffect>();
        cameraFlash?.TakePhoto();
    }

    private IEnumerator TryPerformBooleanAsync(GameObject a, GameObject b)
    {
        if (!IsMeshReadable(a) || !IsMeshReadable(b)) yield break;

        var brushA = new CSGBrush(a);
        var brushB = new CSGBrush(b);
        var result = new CSGBrush(resultObject);

        yield return StartCoroutine(brushA.build_brush_from_mesh_async(
            a.GetComponent<MeshFilter>().sharedMesh,
            p => Debug.Log($"🧱 构建 A 中... {p:P0}")
        ));

        yield return StartCoroutine(brushB.build_brush_from_mesh_async(
            b.GetComponent<MeshFilter>().sharedMesh,
            p => Debug.Log($"🧱 构建 B 中... {p:P0}")
        ));

        yield return StartCoroutine(csgOperation.merge_brushes(
            operationType,
            brushA,
            brushB,
            result,
            0.00001f,
            p => Debug.Log($"🧩 布尔运算进度: {p:P0}")
        ));

        Mesh resultMesh = result.getMesh();

        if (IsMeshValid(resultMesh))
        {
            Debug.Log($"✅ 布尔结果：顶点 {resultMesh.vertexCount}, 三角形 {resultMesh.triangles.Length / 3}");
            ApplyMeshToResult(resultMesh);
        }
        else
        {
            Debug.LogWarning("⚠️ 无效布尔结果。");
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

        var mf = resultObject.GetComponent<MeshFilter>();
        if (mf == null) mf = resultObject.AddComponent<MeshFilter>();
        mf.sharedMesh = reusableMesh;

        var mc = resultObject.GetComponent<MeshCollider>();
        if (mc == null) mc = resultObject.AddComponent<MeshCollider>();
        mc.sharedMesh = null;
        mc.sharedMesh = reusableMesh;
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

        var sourceMF = objectA.GetComponent<MeshFilter>();
        if (sourceMF == null || sourceMF.sharedMesh == null)
        {
            Debug.LogError("⚠️ objectA 缺少有效的 Mesh！");
            return;
        }

        var resultMF = resultObject.GetComponent<MeshFilter>();
        if (resultMF == null) resultMF = resultObject.AddComponent<MeshFilter>();

        var renderer = resultObject.GetComponent<MeshRenderer>();
        if (renderer == null) renderer = resultObject.AddComponent<MeshRenderer>();

        resultMF.sharedMesh = Instantiate(sourceMF.sharedMesh);

        var mc = resultObject.GetComponent<MeshCollider>();
        if (mc == null) mc = resultObject.AddComponent<MeshCollider>();
        mc.sharedMesh = resultMF.sharedMesh;

        Debug.Log("✅ resultObject Mesh 初始化完成！");
    }
}