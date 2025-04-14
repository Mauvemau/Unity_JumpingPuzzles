using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private Vector3 velocity = new Vector3();
    [SerializeField]
    private float movementSpeed = 25.0f;
    [SerializeField]
    private Rigidbody rb;
    [Header("Jumping")]
    [SerializeField] 
    private float groundCheckDistance = 1.5f;
    [SerializeField] 
    private float initialJumpForce = 5f;
    [SerializeField]
    private float maxJumpHoldTime = .5f;
    [SerializeField] 
    private float holdJumpForce = 10f;
    [SerializeField] 
    private LayerMask groundlayer;
    private bool jumping;
    private float jumpHoldTimer = 0f;

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundlayer);
    }

    public void SetJumping(float value) {
        jumping = value > .5f;
        if(IsGrounded() && jumping) {
            rb.AddForce(Vector3.up * initialJumpForce, ForceMode.Impulse);
            jumpHoldTimer = maxJumpHoldTime;
        }
        if(!jumping){
            jumpHoldTimer = 0; // We simply reset the timer because the player released the button early.
        }
    }

    public void UpdateVelocity(Vector2 velocity) {
        this.velocity.x = velocity.x * (movementSpeed + Time.deltaTime);
        this.velocity.z = velocity.y * (movementSpeed + Time.deltaTime);
    }

    private void HandlePlayerPhysics() {
        rb.AddForce(new Vector3(velocity.x, 0, velocity.z), ForceMode.Force);
        if(jumping && jumpHoldTimer > 0f) {
            rb.AddForce(Vector3.up * holdJumpForce, ForceMode.Force);
            jumpHoldTimer -= Time.fixedDeltaTime;
        }
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        HandlePlayerPhysics();
    }
}
