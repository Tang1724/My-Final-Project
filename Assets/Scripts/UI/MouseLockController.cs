using UnityEngine;

public class MouseLockController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    private bool wasCursorVisible;
    private CursorLockMode previousLockState;
    
    void Start()
    {
        // 初始状态应与菜单状态一致
        UpdateCursorState(!pauseMenuUI.activeSelf);
    }
    
    void Update()
    {
        // ESC键处理已经在MenuControl中，这里不需要重复
    }
    
    public void OnMenuOpened()
    {
        // 保存当前光标状态（用于恢复）
        wasCursorVisible = Cursor.visible;
        previousLockState = Cursor.lockState;
        
        // 显示并解锁鼠标
        UpdateCursorState(false);
    }
    
    public void OnMenuClosed()
    {
        // 恢复之前的光标状态
        Cursor.visible = wasCursorVisible;
        Cursor.lockState = previousLockState;
        
        // 如果是游戏状态，锁定鼠标
        if (Time.timeScale > 0)
        {
            UpdateCursorState(true);
        }
    }
    
    private void UpdateCursorState(bool shouldLock)
    {
        Cursor.visible = !shouldLock;
        Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
