using UnityEngine;

public class LockMouse : MonoBehaviour
{
    void Start()
    {
        // 隐藏并锁定鼠标光标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 按 Esc 键解锁鼠标光标
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
