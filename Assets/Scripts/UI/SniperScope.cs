using UnityEngine;
using UnityEngine.UI;

public class SniperScope : MonoBehaviour
{
    [Header("摄像机")]
    public Camera mainCamera;             // 主相机（比如 MainCamera）
    public Camera firstPersonCamera;      // 第一人称前景摄像机（如 FirstPersonCamera）

    [Header("UI")]
    public RawImage scopeUI;              // UI 瞄准镜界面

    [Header("参数")]
    public float normalFOV = 60f;         // 默认视角
    public float sniperFOV = 15f;         // 开镜视角
    public float zoomSpeed = 8f;          // 变焦速度

    private PlayerFlashlight playerFlashlight;
    public bool isScoping = false;        // 是否处于开镜状态

    void Start()
    {
        playerFlashlight = FindObjectOfType<PlayerFlashlight>();

        if (scopeUI) scopeUI.enabled = false;

        // 默认确保第一人称摄像机是启用的
        if (firstPersonCamera) firstPersonCamera.enabled = true;
    }

    void Update()
    {
        // 确保是在玩家持有摄像机状态下才允许操作
        if (playerFlashlight != null && playerFlashlight.currentCamera == true)
        {
            if (Input.GetMouseButtonDown(1)) // 右键切换开镜状态
            {
                isScoping = !isScoping;

                // 显示/隐藏 瞄准 UI
                if (scopeUI) scopeUI.enabled = isScoping;

                // 显示/隐藏 第一人称摄像机
                if (firstPersonCamera)
                    firstPersonCamera.enabled = !isScoping;
            }

            // 平滑调整主摄像机的 FOV
            float targetFOV = isScoping ? sniperFOV : normalFOV;
            if (mainCamera)
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        }
    }
}