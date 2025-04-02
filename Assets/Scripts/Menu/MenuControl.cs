using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public GameObject pauseMenuUI;

    void Update()
    {
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
    }

    public void Pause()
    {
        // 打开暂停菜单
        pauseMenuUI.SetActive(true);
        
        // 暂停时间
        Time.timeScale = 0f;
        
        // 暂停背景音乐
        AudioManager.instance.PauseMusic();
    }

    public void LoadMainMenu()
    {
        // 停止背景音乐
        AudioManager.instance.StopMusic();
        
        // 加载主菜单场景
        SceneManager.LoadScene("Menu");
        
        // 确保时间恢复正常
        Resume();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
