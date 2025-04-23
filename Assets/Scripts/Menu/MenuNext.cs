using UnityEngine;

public class MenuNext : MonoBehaviour
{
    public FadeTransition fadeTransition; // 淡入淡出脚本

    public GameObject[] nextLevelObjects; // 触发下一关的物体数组
    public GameObject[] exitGameObjects;  // 触发退出游戏的物体数组

    private BooleanSimple[] nextLevelBoolean;  // 缓存下一关物体的 BooleanSimple 引用
    private BooleanSimple[] exitGameBoolean;   // 缓存退出游戏物体的 BooleanSimple 引用

    void Start()
    {
        // 初始化下一关物体的 BooleanSimple 引用
        if (nextLevelObjects != null && nextLevelObjects.Length > 0)
        {
            nextLevelBoolean = new BooleanSimple[nextLevelObjects.Length];
            for (int i = 0; i < nextLevelObjects.Length; i++)
            {
                if (nextLevelObjects[i] != null)
                {
                    nextLevelBoolean[i] = nextLevelObjects[i].GetComponent<BooleanSimple>();
                    if (nextLevelBoolean[i] == null)
                    {
                        Debug.LogError($"未在 {nextLevelObjects[i].name} 上找到 BooleanSimple 脚本！");
                    }
                }
            }
        }

        // 初始化退出游戏物体的 BooleanSimple 引用
        if (exitGameObjects != null && exitGameObjects.Length > 0)
        {
            exitGameBoolean = new BooleanSimple[exitGameObjects.Length];
            for (int i = 0; i < exitGameObjects.Length; i++)
            {
                if (exitGameObjects[i] != null)
                {
                    exitGameBoolean[i] = exitGameObjects[i].GetComponent<BooleanSimple>();
                    if (exitGameBoolean[i] == null)
                    {
                        Debug.LogError($"未在 {exitGameObjects[i].name} 上找到 BooleanSimple 脚本！");
                    }
                }
            }
        }
    }

    void Update()
    {
        if (fadeTransition == null) return;

        Debug.Log($"[DEBUG] 当前 FadeTransition 对象: {fadeTransition.gameObject.name}, nextLevelName: {fadeTransition.nextLevelName}");

        // 检查下一关物体
        if (nextLevelBoolean != null && nextLevelBoolean.Length > 0)
        {
            foreach (BooleanSimple booleanSimple in nextLevelBoolean)
            {
                if (booleanSimple != null && booleanSimple.isMeshChanged)
                {
                    Debug.Log("🎮 下一关物体触发条件满足，进入下一关！");
                    fadeTransition.GoToNextLevel();
                    return; // 只触发一次
                }
            }
        }

        // 检查退出游戏物体
        if (exitGameBoolean != null && exitGameBoolean.Length > 0)
        {
            foreach (BooleanSimple booleanSimple in exitGameBoolean)
            {
                if (booleanSimple != null && booleanSimple.isMeshChanged)
                {
                    Debug.Log("🎮 退出游戏物体触发条件满足，退出游戏！");
                    Application.Quit();
                    return; // 只触发一次
                }
            }
        }
    }
}
