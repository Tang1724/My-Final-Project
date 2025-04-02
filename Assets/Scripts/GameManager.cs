using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayerCollectedKey(int keyID)
    {
        // 遍历场景中所有门
        foreach (DoorController door in FindObjectsOfType<DoorController>())
        {
            door.TryOpenDoor(keyID);
        }
    }
}
