using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPosition : MonoBehaviour
{
    // 要同步的目标物体
    public Transform targetObject; 

    // 是否实时同步（开启后会在每帧更新位置）
    public bool syncInRealTime = true;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("目标物体未设置！请指定一个目标物体。");
            return;
        }

        // 如果不需要实时同步，可以在 Start 中直接设置一次
        if (!syncInRealTime)
        {
            SyncPositionWithTarget();
        }
    }

    void Update()
    {
        // 如果开启实时同步，每帧更新位置
        if (syncInRealTime && targetObject != null)
        {
            SyncPositionWithTarget();
        }
    }

    // 同步位置的方法
    private void SyncPositionWithTarget()
    {
        // 将当前物体的位置设置为目标物体的位置
        transform.position = targetObject.position;
    }
}
