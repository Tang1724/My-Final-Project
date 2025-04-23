using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FollowFreezeByWall : MonoBehaviour
{
    [Header("挂点设置")]
    public Transform followPoint;         // 📌 手电筒挂点
    public Transform referencePoint;      // 🎯 恢复位置的参考点
    public Transform playerUnstickPoint;  // 🚶 玩家身上的坐标点用于判断是否离开墙体

    [Header("墙体检测设置")]
    public LayerMask wallLayer;           // 墙体Layer
    public float skinWidth = 0.01f;       // 射线偏移
    public float unstickDistance = 0.5f;  // 玩家离开障碍的最小距离

    private Collider itemCollider;        // 当前手电本体的Collider
    private bool isFrozen = false;        // 是否处于冻结状态
    private Vector3 frozenFollowPos;      // 冻结时的挂点位置
    private Quaternion frozenFollowRot;   // 冻结时的挂点旋转
    private Vector3 recordedPlayerPos;    // 玩家撞墙时的位置

    private PlayerFlashlight playerFlashlight; // 玩家脚本引用

    void Start()
    {
        itemCollider = GetComponent<Collider>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            playerFlashlight = player.GetComponent<PlayerFlashlight>();
        }

        if (!followPoint || !referencePoint || !playerUnstickPoint)
        {
            Debug.LogError("❌ followPoint / referencePoint / playerUnstickPoint 未设置！");
        }
    }

    void LateUpdate()
    {
        // ✅ 仅在玩家持有手电筒时运行
        if (playerFlashlight == null || !playerFlashlight.currentCamera) return;
        if (!itemCollider || !followPoint || !referencePoint || !playerUnstickPoint) return;

        bool hitWall = IsBlockedByWall();

        // ✅ 第一次撞墙时冻结挂点
        if (hitWall && !isFrozen)
        {
            frozenFollowPos = followPoint.position;
            frozenFollowRot = followPoint.rotation;
            recordedPlayerPos = playerUnstickPoint.position;
            isFrozen = true;

            Debug.Log($"✅ {gameObject.name} 被墙体阻挡 —— 冻结 followPoint");
        }

        // ✅ 一旦被冻结，就持续判断玩家是否离开
        if (isFrozen)
        {
            float distance = Vector3.Distance(playerUnstickPoint.position, recordedPlayerPos);
            Debug.Log($"🟡 玩家离开距离：{distance:F3} / 阈值：{unstickDistance}");

            if (distance > unstickDistance)
            {
                // ✅ 玩家远离障碍，恢复挂点位置
                followPoint.position = referencePoint.position;
                followPoint.rotation = referencePoint.rotation;
                isFrozen = false;

                Debug.Log($"✅ 玩家已离开障碍 —— 恢复 followPoint");
            }
            else
            {
                // 继续冻结
                followPoint.position = frozenFollowPos;
                followPoint.rotation = frozenFollowRot;
            }
        }
    }

    /// <summary>
    /// 从 Collider 中心往 6 个方向发射射线检测是否被墙阻挡
    /// </summary>
    private bool IsBlockedByWall()
    {
        Vector3 origin = itemCollider.bounds.center;

        Vector3[] directions = new Vector3[]
        {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right,
            transform.up,
            -transform.up
        };

        bool hit = false;

        foreach (Vector3 dir in directions)
        {
            float extent = GetExtentInDirection(itemCollider, dir) + skinWidth;

            if (Physics.Raycast(origin, dir, out RaycastHit hitInfo, extent, wallLayer))
            {
                Debug.DrawRay(origin, dir * extent, Color.red);
                hit = true;
                Debug.Log($"[射线命中] → {hitInfo.collider.name} | 方向: {dir}");
            }
            else
            {
                Debug.DrawRay(origin, dir * extent, Color.green);
            }
        }

        return hit;
    }

    /// <summary>
    /// 获取 Collider 在指定方向的“半尺寸”，用于决定射线长度
    /// </summary>
    private float GetExtentInDirection(Collider col, Vector3 worldDir)
    {
        Vector3 localDir = transform.InverseTransformDirection(worldDir).normalized;

        if (col is BoxCollider box)
        {
            Vector3 halfSize = box.size * 0.5f;
            return Mathf.Abs(halfSize.x * localDir.x) +
                   Mathf.Abs(halfSize.y * localDir.y) +
                   Mathf.Abs(halfSize.z * localDir.z);
        }

        if (col is SphereCollider sphere)
            return sphere.radius;

        if (col is CapsuleCollider capsule)
            return capsule.radius;

        return 0.5f;
    }

    /// <summary>
    /// 可视化冻结点和恢复点（调试用）
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (isFrozen)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(frozenFollowPos, 0.03f);
        }

        if (referencePoint)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(referencePoint.position, 0.03f);
        }

        if (playerUnstickPoint)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(recordedPlayerPos, playerUnstickPoint.position);
        }
    }
}