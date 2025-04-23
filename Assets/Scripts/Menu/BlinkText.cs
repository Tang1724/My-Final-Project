using UnityEngine;
using UnityEngine.UI; // For UI.Text
using TMPro;          // For TextMeshPro

public class BlinkText : MonoBehaviour
{
    public float blinkInterval = 0.5f; // 闪烁间隔时间（秒）

    private float timer;
    private bool isVisible = true;

    private Text uiText;
    private TextMeshProUGUI tmpText;

    void Start()
    {
        // 获取 Text 或 TextMeshProUGUI 组件
        uiText = GetComponent<Text>();
        tmpText = GetComponent<TextMeshProUGUI>();

        if (uiText == null && tmpText == null)
        {
            Debug.LogError("⚠️ BlinkText 脚本需要挂在一个包含 Text 或 TextMeshProUGUI 的对象上！");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= blinkInterval)
        {
            timer = 0f;
            isVisible = !isVisible;

            if (uiText != null)
                uiText.enabled = isVisible;

            if (tmpText != null)
                tmpText.enabled = isVisible;
        }
    }
}