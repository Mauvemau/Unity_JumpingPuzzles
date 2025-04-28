using UnityEngine;

public class PlayerFoot : MonoBehaviour {
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

    private void Update() {
        if (_jumping) {
            if (Time.time > _jumpTimestamp + jumpCooldown && IsTouchingGround()) {
                _jumping = false;
            }
        }
    }
}
