using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("钥匙设置")]
    public int keyID = 0;            // 钥匙唯一标识
    public GameObject collectEffect; // 拾取特效（可选）

    private void OnTriggerEnter(Collider other)
    {
        // 检测玩家触碰（玩家需有"Player"标签）
        if (other.CompareTag("Player")) 
        {
            // 通知游戏管理器
            GameManager.Instance.PlayerCollectedKey(keyID);
            
            // 播放拾取特效
            if(collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }
            
            // 销毁钥匙
            Destroy(gameObject);
        }
    }
}
