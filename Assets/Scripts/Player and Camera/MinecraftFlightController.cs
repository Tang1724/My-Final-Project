using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MinecraftFlightController : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float flySpeed = 10f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float rotationSpeed = 2f;

    [Header("飞行控制")]
    [SerializeField] private float ascendDescendSpeed = 5f;
    [SerializeField] private float maxVerticalAngle = 89f;

    [Header("碰撞检测")]
    [SerializeField] private float skinWidth = 0.08f;
    [SerializeField] private LayerMask obstacleMask;

    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 currentVelocity;
    private float verticalRotation = 0f;

    private bool isFlying = false;

    [Header("控制器引用")]
    public PlayerControl walkController; // 拖拽赋值

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // 切换飞行状态
            isFlying = !isFlying;

            if (walkController != null)
                walkController.enabled = !isFlying;

            // 当前脚本启用
            this.enabled = true;

            // 重置物理状态
            controller.enabled = false;
            controller.enabled = true;

            Debug.Log(isFlying ? "🛫 飞行模式激活" : "🚶 行走模式激活");
        }

        if (!isFlying) return;

        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(0, mouseX, 0);

        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);
        cameraTransform.localEulerAngles = new Vector3(verticalRotation, 0, 0);
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float vertical = isFlying ? (Input.GetKey(KeyCode.Space) ? 1 : Input.GetKey(KeyCode.LeftShift) ? -1 : 0) : 0;

        Vector3 targetVelocity;
        if (isFlying)
        {
            targetVelocity = (transform.right * h + transform.forward * v + transform.up * vertical).normalized * flySpeed;
        }
        else
        {
            targetVelocity = (transform.right * h + transform.forward * v).normalized * walkSpeed;
            targetVelocity.y = -9.8f;
        }

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

        Vector3 finalMove = currentVelocity * Time.deltaTime;
        if (CheckCollision(finalMove, out Vector3 adjustedMove))
        {
            finalMove = adjustedMove;
        }

        controller.Move(finalMove);
    }

    private bool CheckCollision(Vector3 move, out Vector3 adjustedMove)
    {
        adjustedMove = move;

        float rayDistance = controller.radius + skinWidth;
        Vector3 rayOrigin = transform.position + controller.center;

        bool hitDetected = Physics.SphereCast(
            rayOrigin,
            controller.radius,
            move.normalized,
            out RaycastHit hit,
            rayDistance + move.magnitude,
            obstacleMask
        );

        if (hitDetected)
        {
            float safeDistance = hit.distance - controller.radius - skinWidth;
            adjustedMove = move.normalized * Mathf.Max(safeDistance, 0);
            return true;
        }

        return false;
    }
}