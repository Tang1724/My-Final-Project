using UnityEngine;
using UnityEngine.UI;

public class SniperScope : MonoBehaviour
{
    public Camera mainCamera;      // 主相机

    public RawImage scopeUI;       // UI 瞄准镜
    public float normalFOV = 60f;  // 默认 FOV
    public float sniperFOV = 15f;  // 瞄准 FOV
    public float zoomSpeed = 8f;   // 变焦速度
    private PlayerFlashlight playerFlashlight;
    public bool isScoping = false; // 是否开镜
    
    void Start()
    {
        playerFlashlight = FindObjectOfType<PlayerFlashlight>();
        scopeUI.enabled = false; // 初始隐藏瞄准镜 UI
    }

    void Update()
    {
        if(playerFlashlight.currentCamera == true)
        {

            if (Input.GetMouseButtonDown(1)) // 右键开镜
            {
                isScoping = !isScoping;
                scopeUI.enabled = isScoping;
            }

            // 平滑调整 FOV
            float targetFOV = isScoping ? sniperFOV : normalFOV;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        }
    }
}