using UnityEngine;

public class CharacterFoot : MonoBehaviour {
    [Header("Ground Settings")]
    [Tooltip("Everything outside of this layer will be ignored by the feet's logic")]
    [SerializeField] private LayerMask groundLayer;
    [Min(0)]
    [Tooltip("Defines the distance in which the foot detects the ground")]
    [SerializeField] private float groundCheckDistance = 1f;
    [Tooltip("Defines distance of the front foot towards the pivot")]
    [SerializeField] private float frontFootOffset = .2f;
    [Tooltip("Amount of extra raycasts casted for a more accurate ground reading")]
    [SerializeField] private int amountExtraRaycast = 10;
    [Tooltip("Distance around the player where extra raycasts will be cast")]
    [SerializeField] private float extraRaycastRadius = .5f;
    
    [Header("Jump Settings")] 
    [Min(0)]
    [Tooltip("Defines how long until the character is allowed to jump again after jumping")]
    [SerializeField] private float jumpCooldown = .2f;
    private float _jumpTimestamp;
    private bool _jumping = false;

    [Header("Coyote Time Settings")] 
    [Min(0)]
    [Tooltip("Defines a time window in which the foot will still consider itself as grounded after falling off a platform")]
    [SerializeField] private float coyoteTime = .5f;
    private float _lastGroundedTime;
    
    [Header("Feedback Config")]
    [SerializeField] protected CharacterAnimationController animationController;

    /// <summary>
    /// Casts rays in a circular pattern around the player to check for ground contact.
    /// </summary>
    private bool CheckSurroundingArea()
    {
        var angleStep = 360f / amountExtraRaycast;

        for (var i = 0; i < amountExtraRaycast; i++)
        {
            var angleRad = angleStep * i * Mathf.Deg2Rad;
            var offset = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad)) * extraRaycastRadius;
            var origin = transform.position + offset;

            if (Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns whether the feet are touching the ground.
    /// Only considers the ground layer and ground check distance.
    /// </summary>
    private bool IsTouchingGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer)
               || CheckSurroundingArea();
    }

    /// <summary>
    /// Returns whether the foot are on the ground.
    /// </summary>
    public bool IsGrounded() {
        if (_jumping) return false;
        return (Time.time <= _lastGroundedTime + coyoteTime);
    }

    /// <summary>
    /// Returns the normal of the current surface the foot are standing on
    /// </summary>
    public Vector3 GetGroundNormal() {
        return Physics.Raycast(transform.position, Vector3.down, out var hit, groundCheckDistance, groundLayer) ? hit.normal : Vector3.up;
    }

    /// <summary>
    /// Overload of GetGroundNormal. It receives a direction and simulates a "front foot" returning the normal of an upcoming slope
    /// </summary>
    public Vector3 GetGroundNormal(Vector3 moveDirection) {
        var rayOrigin = transform.position + moveDirection.normalized * frontFootOffset;
        return Physics.Raycast(rayOrigin, Vector3.down, out var hit, groundCheckDistance, groundLayer) ? 
            hit.normal : // Normal of the surface the front foot is touching
            GetGroundNormal(); // If front foot fails, try center foot
    }

    public float GetLastJumpTimestamp() {
        return _jumpTimestamp;
    }
    
    /// <summary>
    /// Setting "jumping" to true will make "grounded" return false until the jumping cooldown is refreshed
    /// </summary>
    public void SetJumping() {
        _jumping = true;
        _jumpTimestamp = Time.time;
    }
    
    /// <summary>
    /// Used for handling feedback triggers like animations, sound, or effects
    /// </summary>
    private void HandleFeedback() {
        if (!animationController) return;
        animationController.UpdateIsJumping(!IsTouchingGround() || _jumping);
    }
    
    private void FixedUpdate() {
        var groundedNow = IsTouchingGround(); // If foot are grounded on this update tick
        
        if (groundedNow) {
            _lastGroundedTime = Time.time; // tracking last grounded time
        }
        
        if (_jumping && Time.time > _jumpTimestamp + jumpCooldown && groundedNow) { // ! Keep _jumping as first check
            _jumping = false;
        }
        HandleFeedback();
    }

    private void Awake() {
        if (gameObject.layer == 0) {
            Debug.LogWarning($"{name}: Layer for foot's game-object is set to default!");
        }
        if (groundLayer.value == 0) {
            Debug.LogWarning($"{name}: {groundLayer}'s value is not configured.");
        }
        if (groundCheckDistance <= 0) {
            Debug.LogWarning($"{name}: {groundCheckDistance} raycast size is set to zero.");
        }
        if (!animationController) {
            Debug.Log($"{name}: Animation controller not configured, verify if intended.");
        }
    }
}
