using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject spot1;  // 默认手电筒 Spotlight
    public GameObject spot2;  // 开镜时的 Spotlight
    public bool IsSpotlightOn = true;

    private SniperScope sniperScope;
    private PlayerFlashlight playerFlashlight;

    [Header("区域检测器列表")]
    public PlayerInsideDetector[] insideDetectors; // ✅ 多个区域

    [Header("需要控制的物品碰撞体")]
    public Collider[] targetColliders; // ✅ 拖拽需要启/禁的碰撞体

    void Start()
    {
        playerFlashlight = FindObjectOfType<PlayerFlashlight>();
        sniperScope = FindObjectOfType<SniperScope>();

        UpdateSpotlightState();
    }

    void Update()
    {
        // ✅ 控制 Collider 启用状态
        UpdateCollidersState();

        if (playerFlashlight != null && playerFlashlight.currentCamera == true)
        {
            // ✅ 检查是否有任意一个区域内为 true
            bool playerInsideAny = false;

            foreach (var detector in insideDetectors)
            {
                if (detector != null && detector.isPlayerInside)
                {
                    playerInsideAny = true;
                    break;
                }
            }

            IsSpotlightOn = !playerInsideAny;

            UpdateSpotlightState();
        }
    }

    void UpdateSpotlightState()
    {
        if (sniperScope == null) return;

        if (IsSpotlightOn)
        {
            spot1.SetActive(!sniperScope.isScoping);
            spot2.SetActive(sniperScope.isScoping);
        }
        else
        {
            spot1.SetActive(false);
            spot2.SetActive(false);
        }
    }

    void UpdateCollidersState()
    {
        if (targetColliders == null || targetColliders.Length == 0 || playerFlashlight == null)
            return;

        bool enableColliders = !playerFlashlight.currentCamera;

        foreach (var col in targetColliders)
        {
            if (col != null)
            {
                col.enabled = enableColliders;
            }
        }
    }
}