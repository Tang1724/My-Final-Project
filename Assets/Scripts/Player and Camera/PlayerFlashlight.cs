using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    public List<GameObject> flashlights; // 手电筒列表，支持多个手电筒
    public Transform holdPoint; // 手电筒在玩家手中的位置
    public float pickupRange = 2.0f; // 捡起手电筒的最大范围

    private GameObject currentFlashlight = null; // 当前持有的手电筒

    private SniperScope sniperScope;
    public bool currentCamera = false;

    void Start()
    {
        sniperScope = FindObjectOfType<SniperScope>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        if (currentFlashlight == null)
        {
            // 尝试捡起一个手电筒
            GameObject nearestFlashlight = GetNearestFlashlight();
            if (nearestFlashlight != null)
            {
                // 捡起手电筒
                currentFlashlight = nearestFlashlight;
                currentFlashlight.transform.SetParent(holdPoint); // 设置父对象为持有点
                currentFlashlight.transform.localPosition = Vector3.zero; // 重置位置使其位于持有点
                currentFlashlight.transform.localRotation = Quaternion.identity; // 重置旋转使其与持有点对齐
                currentCamera = true;
                Debug.Log($"捡起了手电筒：{currentFlashlight.name}");
            }
            else
            {
                Debug.Log("附近没有可捡起的手电筒！");
            }
        }
        else
        {
            if (sniperScope.isScoping == false)
            {
                // 放下当前持有的手电筒
                Debug.Log($"放下了手电筒：{currentFlashlight.name}");
                currentFlashlight.transform.SetParent(null); // 移除父对象
                currentCamera = false;
                currentFlashlight = null;
            }
        }
    }

    GameObject GetNearestFlashlight()
    {
        GameObject nearestFlashlight = null;
        float nearestDistance = float.MaxValue;

        // 遍历手电筒列表，找到最近的手电筒
        foreach (GameObject flashlight in flashlights)
        {
            if (flashlight != null)
            {
                float distance = Vector3.Distance(transform.position, flashlight.transform.position);
                if (distance <= pickupRange && distance < nearestDistance)
                {
                    nearestFlashlight = flashlight;
                    nearestDistance = distance;
                }
            }
        }

        return nearestFlashlight;
    }
}