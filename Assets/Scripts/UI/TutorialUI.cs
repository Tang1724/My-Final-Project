using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public static bool IsTutorialActive { get; private set; } = false;
    [SerializeField] public GameObject tutorialPanel;
    [SerializeField] private KeyCode hideKey = KeyCode.F;
    
    private float originalTimeScale;
    private bool isPaused = false;

    private void Start()
    {
        originalTimeScale = Time.timeScale;
        if (tutorialPanel != null)
        {
            PauseGame();
            tutorialPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(hideKey))
        {
            if (tutorialPanel.activeSelf)
            {
                HideTutorial();
            }
        }
    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            // 完全暂停游戏
            Time.timeScale = 0f;
            AudioListener.pause = true; // 暂停所有声音
            isPaused = true;
            
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            // 恢复游戏
            Time.timeScale = originalTimeScale;
            AudioListener.pause = false; // 恢复所有声音
            isPaused = false;
            
            // 可选：恢复鼠标锁定
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
    }

    public void HideTutorial()
    {
        if (tutorialPanel != null && tutorialPanel.activeSelf)
        {
            tutorialPanel.SetActive(false);
            ResumeGame();

            // ✅ 设置为 false
            TutorialUI.IsTutorialActive = false;
        }
    }
}
