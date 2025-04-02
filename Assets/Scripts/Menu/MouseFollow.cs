using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    private Camera mainCamera;
    public float fixedZ = 0f; // 固定的 Z 轴值
    public Vector2 rangeX = new Vector2(-5f, 5f); // X 轴范围
    public Vector2 rangeY = new Vector2(-5f, 5f); // Y 轴范围

    private void Start()
    {
        // 获取主相机
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 获取鼠标在屏幕上的位置
        Vector3 mousePosition = Input.mousePosition;

        // 将鼠标的屏幕坐标转换为世界坐标
        // 使用固定的 Z 轴值
        mousePosition.z = mainCamera.nearClipPlane + Mathf.Abs(mainCamera.transform.position.z - fixedZ);

        // 将屏幕坐标转换为世界坐标
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // 限制 X 和 Y 轴的范围
        float clampedX = Mathf.Clamp(worldPosition.x, rangeX.x, rangeX.y);
        float clampedY = Mathf.Clamp(worldPosition.y, rangeY.x, rangeY.y);

        // 固定 Z 轴，只更新 X 和 Y 轴，并应用范围限制
        transform.position = new Vector3(clampedX, clampedY, fixedZ);
    }
}
