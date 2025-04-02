using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;
    private float minYAngle = -45f;
    private float maxYAngle = 45f;

    void Update()
    {
        ProcessMouseMovement();
    }

    private void ProcessMouseMovement()
    {
        float mouseX = GetMouseAxis("Mouse X");
        float mouseY = GetMouseAxis("Mouse Y");

        ApplyRotation(mouseX, mouseY);
    }

    private float GetMouseAxis(string axisName)
    {
        return Input.GetAxis(axisName) * mouseSensitivity * Time.deltaTime;
    }

    private void ApplyRotation(float mouseX, float mouseY)
    {
        // Horizontal rotation
        player.Rotate(Vector3.up * mouseX);

        // Vertical rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minYAngle, maxYAngle);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
