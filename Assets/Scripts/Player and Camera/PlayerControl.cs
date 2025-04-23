using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
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

    [Header("地面检测")]
    public Transform[] groundChecks;     // 多个检测点
    public float checkRadius = 0.3f;

    [Tooltip("可被识别为地面的图层，支持多选")] 
    public LayerMask groundLayerMask;    // ✅ 多选 LayerMask

    public bool isGrounded;

    private Vector3 moveDirection;
    private bool isMoving;

    [Header("控制器引用")]
    public MinecraftFlightController flyController; // 拖拽赋值

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        jumpCount = maxJumpCount;

        if (groundChecks == null || groundChecks.Length == 0)
        {
            Debug.LogError("未设置地面检测点！");
        }
    }

    private void Update()
    {
        if (!enabled) return; 
        CheckGroundStatus();
        CacheJumpInput();
        ProcessMovementInput();
        HandleJump();
        ApplyGravity();
        Move();
        HandleFootstepSound();

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCurrentScene();
        }
    }

    private void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;

        foreach (Transform check in groundChecks)
        {
            if (Physics.CheckSphere(check.position, checkRadius, groundLayerMask))
            {
                isGrounded = true;
                break;
            }
        }

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
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

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

    public void ResetCurrentScene()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"🔄 重新加载当前场景：{currentScene}");
        SceneManager.LoadScene(currentScene);
    }

    // ✅ 可视化地面检测 Gizmos
    private void OnDrawGizmosSelected()
    {
        if (groundChecks == null) return;

        Gizmos.color = Color.green;

        foreach (Transform check in groundChecks)
        {
            if (check != null)
            {
                Gizmos.DrawWireSphere(check.position, checkRadius);
            }
        }
    }
}