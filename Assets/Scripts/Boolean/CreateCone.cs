using UnityEngine;
using UnityEngine.ProBuilder; // 引入ProBuilder命名空间

public class CreateCone : MonoBehaviour
{
    [Header("目标点")]
    public GameObject targetPoint; // 挂载的目标点

    [Header("圆锥参数")]
    public float height = 2f; // 圆锥高度
    public float radius = 1f; // 圆锥底部半径
    public int sides = 24; // 圆锥分段数

    void Start()
    {
        if (targetPoint == null)
        {
            Debug.LogError("未指定目标点！请挂载一个空物体点。");
            return;
        }

        // 在当前空物体上生成圆锥体
        GenerateConeOnCurrentObject();

        // 将当前空物体挂载到目标点下
        transform.SetParent(targetPoint.transform, false);

        Debug.Log($"圆锥已成功生成在当前空物体上，并挂载到目标点: {targetPoint.transform.position}");
    }

    void GenerateConeOnCurrentObject()
    {
        // 检查当前空物体是否已经有ProBuilderMesh组件
        ProBuilderMesh coneMesh = GetComponent<ProBuilderMesh>();
        if (coneMesh == null)
        {
            coneMesh = gameObject.AddComponent<ProBuilderMesh>();
        }

        // 生成圆锥体
        ProBuilderMesh newConeMesh = ShapeGenerator.GenerateCone(
            PivotLocation.Center, // 枢轴点位置
            radius, // 圆锥底部半径
            height, // 圆锥高度
            sides // 圆锥分段数
        );

        // 将圆锥体的网格数据复制到当前空物体上
        coneMesh.Clear();
        coneMesh.positions = newConeMesh.positions;
        coneMesh.faces = newConeMesh.faces;
        coneMesh.textures = newConeMesh.textures;
        coneMesh.sharedVertices = newConeMesh.sharedVertices;

        // 调整圆锥的旋转，使其与光锥方向一致
        coneMesh.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        // 更新网格
        coneMesh.ToMesh();
        coneMesh.Refresh();

        // 销毁生成的临时圆锥体对象
        Destroy(newConeMesh.gameObject);

        // 添加Trigger Collider
        AddTriggerCollider(gameObject);

        // 隐藏圆锥的渲染
        HideConeRenderer(gameObject);
    }

    void AddTriggerCollider(GameObject targetObject)
    {
        // 检查是否已经存在Collider
        if (targetObject.GetComponent<Collider>() != null)
        {
            Debug.LogWarning("目标物体已存在Collider，未添加新的Collider。");
            return;
        }

        // 添加MeshCollider并设置为Trigger
        MeshCollider collider = targetObject.AddComponent<MeshCollider>();
        collider.convex = true; // 设置为凸包，以便支持布尔运算
        collider.isTrigger = true; // 设置为Trigger
        collider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation; // 优化性能

        Debug.Log("Trigger MeshCollider已成功添加。");
    }

    void HideConeRenderer(GameObject targetObject)
    {
        // 获取MeshRenderer组件
        MeshRenderer renderer = targetObject.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            // 禁用MeshRenderer
            renderer.enabled = false;
            Debug.Log("圆锥的渲染已隐藏。");
        }
        else
        {
            Debug.LogWarning("未找到MeshRenderer组件，无法隐藏圆锥的渲染。");
        }
    }
}
