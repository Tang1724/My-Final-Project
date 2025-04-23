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

    private CharacterController characterController;

    void Start()
    {
        sniperScope = FindObjectOfType<SniperScope>();
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError("玩家未找到 CharacterController 组件！");
        }
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
            GameObject nearestFlashlight = GetNearestFlashlight();
            if (nearestFlashlight != null)
            {
                currentFlashlight = nearestFlashlight;
                currentFlashlight.transform.SetParent(holdPoint);
                currentFlashlight.transform.localPosition = Vector3.zero;
                currentFlashlight.transform.localRotation = Quaternion.identity;
                currentCamera = true;

                // 忽略与 CharacterController 的碰撞
                IgnoreCollisionWithCharacterController(currentFlashlight, true);

                Debug.Log($"捡起了手电筒：{currentFlashlight.name}");
            }
            else
            {
                Debug.Log("附近没有可捡起的手电筒！");
            }
        }
        else
        {
            if (sniperScope != null && sniperScope.isScoping == false)
            {
                Debug.Log($"放下了手电筒：{currentFlashlight.name}");

                // 恢复与 CharacterController 的碰撞
                IgnoreCollisionWithCharacterController(currentFlashlight, false);

                currentFlashlight.transform.SetParent(null);
                currentCamera = false;
                currentFlashlight = null;
            }
        }
    }

    GameObject GetNearestFlashlight()
    {
        GameObject nearestFlashlight = null;
        float nearestDistance = float.MaxValue;

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

    void IgnoreCollisionWithCharacterController(GameObject flashlight, bool ignore)
    {
        if (characterController == null) return;

        Collider[] flashlightColliders = flashlight.GetComponentsInChildren<Collider>();
        Collider playerCollider = characterController; // CharacterController 继承自 Collider

        foreach (Collider fc in flashlightColliders)
        {
            Physics.IgnoreCollision(fc, playerCollider, ignore);
        }
    }
}