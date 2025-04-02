using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public FadeTransition fadeTransition; // 淡入淡出脚本

    private void Update()
    {
        // 按下数字键 1，返回上一关
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            fadeTransition.GoToPreviousLevel();
        }

        // 按下数字键 2，进入下一关
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            fadeTransition.GoToNextLevel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否触碰到的是玩家
        if (other.CompareTag("Player"))
        {
            Debug.Log("🎮 玩家触发了下一关触发器！");
            fadeTransition.GoToNextLevel();
        }
    }
}
