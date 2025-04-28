using UnityEngine;

public class CharacterFoot : MonoBehaviour {
    [Header("Ground Settings")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float groundCheckDistance = 1f;
    [Header("Jump Settings")] 
    [SerializeField]
    private float jumpCooldown = .1f;
    private float _jumpTimestamp = 0f;
    private bool _jumping = false;

    private bool IsTouchingGround() {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    public void SetJumping() {
        _jumping = true;
        _jumpTimestamp = Time.time;
    }
    
    public bool IsGrounded() {
        return (IsTouchingGround() && !_jumping);
    }

    private void FixedUpdate() {
        if (_jumping) {
            if (Time.time > _jumpTimestamp + jumpCooldown && IsTouchingGround()) {
                _jumping = false;
            }
        }
    }

    private void Awake() {
        if (gameObject.layer == 0) {
            Debug.LogWarning("Layer for character foot is set to default");
        }
        if (groundLayer.value == 0) {
            Debug.LogWarning("Ground layer for character foot raycast is not configured.");
        }
    }
}
