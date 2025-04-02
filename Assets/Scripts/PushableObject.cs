using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("可推动设置")]
    public string playerTag = "Player";
    public float pushForce = 5f;
    
    private bool isBeingPushed = false;
    private Transform playerTransform;
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            isBeingPushed = true;
            playerTransform = collision.transform;
        }
        else
        {
            isBeingPushed = false;
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            isBeingPushed = false;
            playerTransform = null;
        }
    }
    
    private void FixedUpdate()
    {
        if (isBeingPushed && playerTransform != null)
        {
            Vector3 pushDirection = transform.position - playerTransform.position;
            pushDirection.y = 0; // 只在水平方向推动
            pushDirection.Normalize();
            
            rb.AddForce(pushDirection * pushForce, ForceMode.Force);
        }
    }
}
