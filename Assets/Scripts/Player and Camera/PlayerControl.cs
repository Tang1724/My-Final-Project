using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController cc;

    [Header("移动设置")]
    public float moveSpeed = 6f;

    [Header("跳跃设置")]
    public float jumpSpeed = 8f;
    public int maxJumpCount = 1;
    private int jumpCount;
    private bool jumpPressed;

    [Header("重力设置")]
    public float gravityMultiplier = 2f;
    public float maxFallSpeed = -50f;
    private Vector3 velocity;

    [Header("多地面检测点")]
    public Transform[] groundChecks; // 改为数组形式
    public float checkRadius = 0.3f;
    public LayerMask groundLayer;
    public bool isGrounded;

    private Vector3 moveDirection;
    private bool isMoving;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        jumpCount = maxJumpCount;
        
        // 安全检测
        if (groundChecks == null || groundChecks.Length == 0)
        {
            Debug.LogError("未设置地面检测点！");
        }
    }
    private void Update()
    {
        CheckGroundStatus();
        CacheJumpInput();
        ProcessMovementInput();
        HandleJump();
        ApplyGravity();
        Move();
        HandleFootstepSound();
    }
    // 修改后的多检测点地面检测
    private void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;
        
        // 检查所有检测点
        foreach (Transform check in groundChecks)
        {
            if (Physics.CheckSphere(check.position, checkRadius, groundLayer))
            {
                isGrounded = true;
                break; // 只要一个检测点命中就退出循环
            }
        }
        // 重置跳跃次数
        if (isGrounded && !wasGrounded)
        {
            jumpCount = maxJumpCount;
        }
    }

    private void CacheJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
    }

    private void ProcessMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
        float vertical = Input.GetAxis("Vertical") * moveSpeed;
        moveDirection = transform.forward * vertical + transform.right * horizontal;
    }

    private void HandleJump()
    {
        if (jumpPressed)
        {
            if (isGrounded || jumpCount > 0)
            {
                velocity.y = jumpSpeed;
                jumpCount--;

                AudioManager.instance.PlaySound("Jump", false);
            }

            jumpPressed = false;
        }
    }

    private void ApplyGravity()
    {
        // 使用 Unity 自带重力乘以倍数
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        // 限制最大下落速度
        if (velocity.y < maxFallSpeed)
        {
            velocity.y = maxFallSpeed;
        }
    }

    private void Move()
    {
        cc.Move(moveDirection * Time.deltaTime); // 水平移动
        cc.Move(velocity * Time.deltaTime);      // 垂直移动
    }

    private void HandleFootstepSound()
    {
        bool wasMoving = isMoving;
        isMoving = moveDirection.magnitude > 0.1f && isGrounded;

        if (isMoving && !wasMoving)
        {
            AudioManager.instance.PlaySound("Walk", true);
        }
        else if (!isMoving && wasMoving)
        {
            AudioManager.instance.StopSound("Walk");
        }
    }
}