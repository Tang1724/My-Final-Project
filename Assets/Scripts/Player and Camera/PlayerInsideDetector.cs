using UnityEngine;

public class PlayerInsideDetector : MonoBehaviour
{
    // 设置一个 public bool，可以在其他脚本中访问
    public bool isPlayerInside = false;

    // 可选：设置要检测的标签，默认是 "Player"
    public string playerTag = "Player";

    // 当有物体持续处于触发器内时调用
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;
        }
    }

    // 当物体离开触发器时调用
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = false;
        }
    }

    // 可选：防止在某些情况下没有触发 TriggerStay 的情况
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;
        }
    }
}