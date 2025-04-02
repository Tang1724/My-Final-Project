using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("门设置")]
    public int requiredKeyID = 0;    // 匹配钥匙ID
    public float destroyDelay = 0f;  // 立即销毁（可设置为>0添加延迟）

    public void TryOpenDoor(int keyID)
    {
        // ID匹配时销毁门
        if (keyID == requiredKeyID)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
