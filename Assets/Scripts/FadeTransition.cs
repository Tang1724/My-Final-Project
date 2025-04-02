using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    public Image fadeImage; // 过渡图片
    public float fadeDuration = 1f; // 淡入淡出时间
    public float delayBeforeLoad = 0.5f; // 场景加载前的延迟时间
    public string nextLevelName; // 下一关的场景名称
    public string previousLevelName; // 上一关的场景名称

    private bool isFading = false; // 防止重复触发

    // 进入下一关的接口方法
    public void GoToNextLevel()
    {
        if (!isFading && !string.IsNullOrEmpty(nextLevelName))
        {
            StartCoroutine(FadeOutAndLoadLevel(nextLevelName));
        }
        else
        {
            Debug.LogError("⚠ 下一关的场景名称未设置，请在 Inspector 中指定！");
        }
    }

    // 返回上一关的接口方法
    public void GoToPreviousLevel()
    {
        if (!isFading && !string.IsNullOrEmpty(previousLevelName))
        {
            StartCoroutine(FadeOutAndLoadLevel(previousLevelName));
        }
        else
        {
            Debug.LogError("⚠ 上一关的场景名称未设置，请在 Inspector 中指定！");
        }
    }

    // 淡出并加载场景
    private IEnumerator FadeOutAndLoadLevel(string levelName)
    {
        isFading = true;

        // 淡出效果
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration); // 逐渐增加透明度
            fadeImage.color = color;
            yield return null;
        }

        // 延迟加载场景
        Debug.Log($"⏳ 延迟 {delayBeforeLoad} 秒后加载场景：{levelName}");
        yield return new WaitForSeconds(delayBeforeLoad);

        // 加载场景
        Debug.Log($"🚪 正在加载场景：{levelName}");
        SceneManager.LoadScene(levelName);

        // 淡入效果（在场景加载完成后执行）
        yield return new WaitForEndOfFrame(); // 确保场景加载完成
        StartCoroutine(FadeIn());
    }

    // 淡入效果
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsedTime / fadeDuration)); // 逐渐减少透明度
            fadeImage.color = color;
            yield return null;
        }

        isFading = false;
    }
}
