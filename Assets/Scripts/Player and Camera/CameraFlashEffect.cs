using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFlashEffect : MonoBehaviour
{
    public Image flashImage; // 在 Inspector 里拖入 UI Image
    public float flashDuration = 0.2f; // 闪光持续时间

    public void TakePhoto()
    {
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        float elapsedTime = 0f;
        Color color = flashImage.color;

        // **快速变亮**
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / (flashDuration / 2));
            flashImage.color = color;
            yield return null;
        }

        elapsedTime = 0f;

        // **快速变暗**
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / (flashDuration / 2));
            flashImage.color = color;
            yield return null;
        }
    }
}