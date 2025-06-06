using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public GameObject pauseMenuUI;
    [SerializeField] private MouseLockController mouseLockController;
    
    void Update()
    {
        if (TutorialUI.IsTutorialActive)
        return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuUI.activeSelf)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        // 恢复背景音乐
        AudioManager.instance.ResumeMusic();
        
        // 关闭暂停菜单
        pauseMenuUI.SetActive(false);
        
        // 恢复正常时间流速
        Time.timeScale = 1f;
        
        // 通知鼠标锁定控制器
        if (mouseLockController != null)
            mouseLockController.OnMenuClosed();
    }

    public void Pause()
    {
        // 打开暂停菜单
        pauseMenuUI.SetActive(true);
        
        // 暂停时间
        Time.timeScale = 0f;
        
        // 暂停背景音乐
        AudioManager.instance.PauseMusic();
        
        // 通知鼠标锁定控制器
        if (mouseLockController != null)
            mouseLockController.OnMenuOpened();
    }

    public void LoadMainMenu()
    {
        Debug.Log("AudioManager.instance = " + AudioManager.instance);

        // 恢复时间
        Time.timeScale = 1f;

        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //  关闭暂停菜单（如果还开着）
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // 停止背景音乐（可选）
        AudioManager.instance.StopMusic();

        // 加载主菜单
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
