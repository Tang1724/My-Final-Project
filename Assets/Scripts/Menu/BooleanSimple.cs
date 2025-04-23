using UnityEngine;
using LibCSG;
using System.Collections.Generic;
using System.Linq;

public class BooleanSimple : MonoBehaviour
{
    public GameObject objectA; // 用户从 Inspector 选择的第一个物体
    public List<GameObject> objectsB; // 用户从 Inspector 选择的一系列物体 B
    public GameObject resultObject; // 用于显示布尔运算结果的物体
    public Operation operationType = Operation.OPERATION_SUBTRACTION; // 用户选择布尔运算类型 (默认差集)

    private CSGBrushOperation csgOperation; // 用于执行布尔运算
    private CSGBrush resultBrush;           // 存储布尔运算结果

    private CSGBrush brushA; // Brush 对象 A
    private List<CSGBrush> brushesB; // Brush 对象 B 的集合
    private float lastOperationTime = -0.5f; // 记录上次操作时间，初始化为 -0.5s 以便首次操作立即生效
    private const float operationCooldown = 0.5f; // 0.5 秒间隔

    private Mesh lastMesh; // 存储上一次的 Mesh 数据
    public bool isMeshChanged = false; // 标记 Mesh 是否发生变化

    void Start()
    {
        // 初始化布尔操作工具
        csgOperation = new CSGBrushOperation();

        // 检查结果对象是否存在
        if (resultObject == null)
        {
            Debug.LogError("结果物体 (Result Object) 未设置，请在 Inspector 中指定！");
            return;
        }

        

        // 创建用于存储结果的 CSGBrush
        resultBrush = new CSGBrush(resultObject);

        // 初始化 B 的 Brush 列表
        brushesB = new List<CSGBrush>();

        if (operationType == Operation.OPERATION_SUBTRACTION){
        PerformBooleanOperation();
        InitializeBrushA();
        }
        PerformBooleanOperation();


    }

    void Update()
    {
        // 检查输入物体是否设置
        if (objectA == null || objectsB == null || objectsB.Count == 0)
        {
            Debug.LogWarning("请在 Inspector 中指定物体 A 和至少一个物体 B！");
            return;
        }

        if (Input.GetKeyDown(KeyCode.F) && Time.time - lastOperationTime >= operationCooldown) // 按 F 且间隔大于 0.5s
        {
            MenuAudio.instance.PlayMenuSound("Camera");
            CameraFlashEffect cameraFlash = FindObjectOfType<CameraFlashEffect>();
            if (cameraFlash != null)
            {
                cameraFlash.TakePhoto();
            }
            PerformBooleanOperation();
        }
    }

    private void InitializeBrushA()
    {
        if (objectA == null)
        {
            Debug.LogWarning("⚠ objectA 未设置，无法初始化 brushA！");
            return;
        }

        // 确保 objectA 有 MeshFilter 和 MeshRenderer
        MeshFilter meshFilter = objectA.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("⚠ objectA 没有 MeshFilter，或者 Mesh 为空！");
            return;
        }

        if (objectA.GetComponent<MeshRenderer>() == null)
        {
            objectA.AddComponent<MeshRenderer>();
        }

        // 初始化 brushA 并设置 Mesh
        brushA = new CSGBrush(objectA);
        brushA.build_from_mesh(meshFilter.sharedMesh);

        // 初始复制 objectA 的 Mesh 到 resultObject
        Mesh resultMesh = new Mesh();
        resultMesh.vertices = meshFilter.sharedMesh.vertices;
        resultMesh.triangles = meshFilter.sharedMesh.triangles;
        resultMesh.uv = meshFilter.sharedMesh.uv;
        resultMesh.normals = meshFilter.sharedMesh.normals;
        resultMesh.RecalculateBounds();

        resultObject.GetComponent<MeshFilter>().mesh = resultMesh;

        // 确保 resultObject 有 MeshRenderer
        if (resultObject.GetComponent<MeshRenderer>() == null)
        {
            resultObject.AddComponent<MeshRenderer>();
        }

        // 确保 resultObject 有 MeshCollider 并更新碰撞体
        MeshCollider meshCollider = resultObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = resultObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = resultMesh;

        Debug.Log("✅ brushA 已成功初始化并显示与 objectA 相同的 Mesh！");
    }

    void PerformBooleanOperation()
    {
        // 检查输入物体是否为 null
        if (objectA == null || objectsB == null || objectsB.Count == 0)
        {
            Debug.LogWarning("物体 A 或物体 B 未设置，请检查 Inspector 中的配置！");
            return;
        }

        // 初始化 Brush A
        if (brushA == null)
        {
            brushA = new CSGBrush(objectA);
            brushA.build_from_mesh(objectA.GetComponent<MeshFilter>().mesh);
        }

        // 初始化 Brushes B
        brushesB.Clear();
        foreach (GameObject objB in objectsB)
        {
            if (objB != null)
            {
                CSGBrush brushB = new CSGBrush(objB);
                brushB.build_from_mesh(objB.GetComponent<MeshFilter>().mesh);
                brushesB.Add(brushB);
            }
        }

        // 开始执行布尔运算：A 与所有 B
        CSGBrush currentResult = brushA;
        foreach (var brushB in brushesB)
        {
            // 创建一个临时结果 Brush
            CSGBrush tempResult = new CSGBrush(resultObject);
            csgOperation.merge_brushes(operationType, currentResult, brushB, ref tempResult);

            // 检查临时结果是否为空
            if (tempResult.is_empty())
            {
                Debug.LogWarning("布尔运算结果为空，请检查输入物体的 Mesh 是否存在或是否有效！");
                return; // 提前返回，避免处理空结果
            }

            currentResult = tempResult; // 更新当前结果
        }

        // 将最终结果赋值给 resultBrush
        resultBrush = currentResult;

        // 更新结果物体的 Mesh
        Mesh resultMesh = resultObject.GetComponent<MeshFilter>().mesh;
        resultMesh.Clear();
        resultBrush.getMesh(resultMesh);

        // 检查最终 Mesh 的顶点数量
        if (resultMesh.vertexCount == 0)
        {
            Debug.LogWarning("布尔运算结果为空，Mesh 没有顶点！");
            return; // 提前返回，避免将空 Mesh 分配给 MeshFilter 和 MeshCollider
        }

        // 确保法线正确
        resultMesh.RecalculateNormals();
        resultMesh.RecalculateBounds();

        // 添加或更新 MeshCollider
        UpdateMeshCollider(resultMesh);

        // 判断 Mesh 是否发生变化
        isMeshChanged = IsMeshDifferent(lastMesh, resultMesh);

        // 更新 lastMesh
        lastMesh = CopyMesh(resultMesh);

        // 输出布尔运算成功信息
        Debug.Log("布尔运算成功完成，结果 Mesh 包含 " + resultMesh.vertexCount + " 个顶点。");
        Debug.Log("Mesh 是否发生变化：" + isMeshChanged);
    }

    bool IsMeshDifferent(Mesh mesh1, Mesh mesh2)
    {
        // 如果 lastMesh 为 null，表示这是第一次运算，Mesh 必然发生变化
        if (mesh1 == null)
        {
            return true;
        }

        // 比较顶点数量
        if (mesh1.vertexCount != mesh2.vertexCount)
        {
            return true;
        }

        // 比较顶点数据
        if (!mesh1.vertices.SequenceEqual(mesh2.vertices))
        {
            return true;
        }

        // 比较三角形数据
        if (!mesh1.triangles.SequenceEqual(mesh2.triangles))
        {
            return true;
        }

        // 比较 UV 数据
        if (!mesh1.uv.SequenceEqual(mesh2.uv))
        {
            return true;
        }

        // 如果没有变化
        return false;
    }

    Mesh CopyMesh(Mesh source)
    {
        // 创建一个新的 Mesh 并复制数据
        Mesh copy = new Mesh();
        copy.vertices = source.vertices;
        copy.triangles = source.triangles;
        copy.uv = source.uv;
        copy.normals = source.normals;
        copy.tangents = source.tangents;
        return copy;
    }

    void UpdateMeshCollider(Mesh resultMesh)
    {
        // 检查是否已经有 MeshCollider
        MeshCollider meshCollider = resultObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            // 如果没有，添加一个
            meshCollider = resultObject.AddComponent<MeshCollider>();
        }

        // 更新 MeshCollider 的 Mesh
        meshCollider.sharedMesh = resultMesh;

        // 如果需要凸形碰撞体，可以启用以下代码（需根据需求修改）
    }
}