using UnityEngine;
using LibCSG;

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
    private CSGBrush resultBrush;
    private CSGBrush brushA;

    private float lastOperationTime = -0.5f;
    private const float operationCooldown = 0.5f;

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
        if (playerFlashlight.currentCamera == true)
        {
            if(flashlight.IsSpotlightOn == true){
                if (Input.GetKeyDown(KeyCode.F) && Time.time - lastOperationTime >= operationCooldown)
                {
                    if (ValidateInputObjects())
                    {
                        if (operationType == Operation.OPERATION_SUBTRACTION && playerInsideDetector.isPlayerInside)
                        {
                            Debug.Log("⚠️ 玩家已在区域内，且当前为 Subtraction 模式，跳过布尔运算。");
                            return;
                        }

                        AudioManager.instance.PlaySound("Camera", false);
                        PerformBooleanOperation();
                        lastOperationTime = Time.time;
                    }
                }
            }
        }
    }

    private void ValidateResultObject()
    {
        if (resultObject == null)
        {
            Debug.LogError("⚠ 结果物体 (Result Object) 未设置！");
        }
        else
        {
            resultBrush = new CSGBrush(resultObject);
        }
    }

    private bool ValidateInputObjects()
    {
        if (objectA == null || (objectB == null && objectC == null))
        {
            Debug.LogWarning("⚠ 缺少 objectA、objectB 或 objectC！");
            return false;
        }
        return true;
    }

    private void PerformBooleanOperation()
    {
        if (flashlight.spot1.activeSelf)
        {
            Debug.Log("🔦 Spot1 开启，执行 objectB 的布尔运算...");
            PerformBooleanOperationWith(objectB);
        }
        else if (flashlight.spot2.activeSelf)
        {
            Debug.Log("🔦 Spot2 开启，执行 objectC 的布尔运算...");
            PerformBooleanOperationWith(objectC);
        }

        CameraFlashEffect cameraFlash = FindObjectOfType<CameraFlashEffect>();
        if (cameraFlash != null)
        {
            cameraFlash.TakePhoto();
        }
    }

    private void PerformBooleanOperationWith(GameObject objectX)
    {
        if (objectX == null) return;

        Debug.Log($"🔄 处理 `{objectX.name}` 的布尔运算...");

        // 初始化 objectA
        brushA = new CSGBrush(objectA);
        brushA.build_from_mesh(objectA.GetComponent<MeshFilter>().mesh);

        // 初始化 objectX
        CSGBrush brushX = new CSGBrush(objectX);
        brushX.build_from_mesh(objectX.GetComponent<MeshFilter>().mesh);

        // 执行布尔运算
        ExecuteBooleanOperation(brushA, brushX);
    }

    private void ExecuteBooleanOperation(CSGBrush brushA, CSGBrush brushX)
    {
        if (brushA == null || brushX == null)
        {
            Debug.LogWarning("⚠ brushA 或 brushX 为空！");
            return;
        }

        CSGBrush tempResult = new CSGBrush(resultObject);
        csgOperation.merge_brushes(operationType, brushA, brushX, ref tempResult);
        brushA = tempResult;

        UpdateResultObject(brushA);
    }

    private void UpdateResultObject(CSGBrush brush)
    {
        Mesh resultMesh = brush.getMesh();
        if (IsMeshValid(resultMesh))
        {
            UpdateMesh(resultMesh);
            UpdateMeshCollider(resultMesh);
            Debug.Log("✅ 布尔运算结果已更新！");
        }
        else
        {
            Debug.LogWarning("⚠ 布尔运算结果无效，跳过更新！");
        }
    }

    private bool IsMeshValid(Mesh mesh)
    {
        return mesh != null && mesh.vertices.Length > 0;
    }

    private void UpdateMesh(Mesh resultMesh)
    {
        Mesh resultObjectMesh = resultObject.GetComponent<MeshFilter>().mesh;
        resultObjectMesh.Clear();
        resultObjectMesh.vertices = resultMesh.vertices;
        resultObjectMesh.triangles = resultMesh.triangles;
        resultObjectMesh.uv = resultMesh.uv;
        resultObjectMesh.RecalculateNormals();
        resultObjectMesh.RecalculateBounds();
    }

    private void UpdateMeshCollider(Mesh resultMesh)
    {
        MeshCollider meshCollider = resultObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = resultObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = resultMesh;
    }

    private void InitializeResultObjectMesh()
    {
        if (resultObject == null || objectA == null)
        {
            Debug.LogError("⚠ resultObject 或 objectA 未设置！");
            return;
        }

        MeshFilter objectAMeshFilter = objectA.GetComponent<MeshFilter>();
        if (objectAMeshFilter == null || objectAMeshFilter.sharedMesh == null)
        {
            Debug.LogError("⚠ objectA 缺少有效的 MeshFilter！");
            return;
        }

        MeshFilter resultMeshFilter = resultObject.GetComponent<MeshFilter>();
        if (resultMeshFilter == null)
        {
            resultMeshFilter = resultObject.AddComponent<MeshFilter>();
        }

        if (resultObject.GetComponent<MeshRenderer>() == null)
        {
            resultObject.AddComponent<MeshRenderer>();
        }

        resultMeshFilter.sharedMesh = Instantiate(objectAMeshFilter.sharedMesh);

        MeshCollider meshCollider = resultObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = resultObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = resultMeshFilter.sharedMesh;

        Debug.Log("✅ resultObject 已初始化！");
    }
}