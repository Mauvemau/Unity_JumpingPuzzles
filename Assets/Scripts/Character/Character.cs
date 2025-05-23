using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;

/// <summary>
/// Represents a force that can be applied to a rigidBody
/// </summary>
public class ForceRequest
{
    public Vector3 Direction;
    public float Acceleration;
    public float Speed;
}

/// <summary>
/// Moves the character, controls everything related to the world and the position
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour {
    [Header("Foot Config")]
    [Tooltip("Used for logic that requires knowing if the character is grounded")]
    public CharacterFoot feet;
    [SerializeField]
    [Tooltip("Defines the maximum angle in which the player can walk up a slope")]
    private float maxSlopeAngle = 45f; 
    [SerializeField]
    [Tooltip("Modifies how it begins to get hard to walk up a slope depending on the maximum angle")]
    private float slopeEffortCurveMultiplier = 3f;

    protected Rigidbody Rb;
    
    protected ForceRequest InstantForceRequest;
    protected ForceRequest ContinuousForceRequest;

    public void RequestInstantForce(ForceRequest forceRequest) {
        InstantForceRequest = forceRequest;
    }

    public void RequestContinuousForce(ForceRequest forceRequest) {
        ContinuousForceRequest = forceRequest;
    }

    /// <summary>
    /// Cancels momentum on every axis except the direction the player is moving towards
    /// </summary>
    protected void ResetMomentum(Vector3 currentDirection) {
        if (InstantForceRequest == null) { 
            Rb.linearVelocity = Vector3.zero;
            return;
        }

        var projectedSpeed = Vector3.Dot(Rb.linearVelocity, currentDirection);
        var correctedVelocity = currentDirection * projectedSpeed;
        Rb.linearVelocity = new Vector3(correctedVelocity.x, 0, correctedVelocity.z);
    }
    
    /// <summary>
    /// Calculates the speed in which the player should move up a slope depending on the inclination of the surface
    /// </summary>
    protected float GetSlopeEffortMultiplier(float slopeAngle) {
        if (slopeAngle <= 0f) return 1f;
        if (slopeAngle >= maxSlopeAngle) return 0f;
        var t = slopeAngle / maxSlopeAngle;
        var effortCurve = 1f - Mathf.Pow(t, slopeEffortCurveMultiplier);
        return effortCurve;
    }
    
    /// <summary>
    /// Returns the angle of the current slope based on the current direction (positive if moving up the slope, negative if moving down)
    /// </summary>
    protected static float GetSlopeAngle(Vector3 direction, Vector3 groundNormal) {
        var normalizedDirection = direction.normalized;
        var slopeDirection = Vector3.Cross(Vector3.Cross(Vector3.up, groundNormal), groundNormal).normalized;
        var dot = Vector3.Dot(normalizedDirection, slopeDirection);
        var angle = Vector3.Angle(groundNormal, Vector3.up);
        return dot > 0 ? -angle : angle; // Downhill = negative
    }
    
    /// <summary>
    /// Projects a movement vector into a slope (affecting the Y axis)
    /// </summary>
    protected static Vector3 ProjectSlope(Vector3 currentDirection, Vector3 groundNormal) {
        return Vector3.ProjectOnPlane(currentDirection, groundNormal).normalized;
    }

    private void HandleMovement() {
        if (ContinuousForceRequest == null) return;
        var currentDirection = ContinuousForceRequest.Direction;
        var groundNormal = feet.GetGroundNormal(currentDirection);
            
        var projectedDirection = ProjectSlope(currentDirection, groundNormal);
            
        var slopeAngle = GetSlopeAngle(currentDirection, groundNormal);
        var slopeMultiplier = GetSlopeEffortMultiplier(slopeAngle);

        projectedDirection.y *= slopeMultiplier; // We reduce "Y" force based on slope steepness.
            
        var speedPercentage = Rb.linearVelocity.magnitude / ContinuousForceRequest.Speed;
        var remainingSpeedPercentage = Mathf.Clamp01(1f - speedPercentage);
        Rb.AddForce(projectedDirection * (ContinuousForceRequest.Acceleration * remainingSpeedPercentage), ForceMode.Force);
    }

    private void HandleJumping() {
        if (InstantForceRequest == null) return;
        var currentDirection = ContinuousForceRequest.Direction;
        // Resetting vertical velocity before jumping so that the jump always has the same impulse.
        ResetMomentum(currentDirection);
        Rb.AddForce(InstantForceRequest.Direction * InstantForceRequest.Acceleration, ForceMode.Impulse);
        InstantForceRequest = null;
    }
    
    private void FixedUpdate() {
        HandleMovement();
        HandleJumping();
    }

    private void Awake() {
        Rb = GetComponent<Rigidbody>();
        if (!feet) {
            Debug.LogWarning($"{name}: Feet are not configured!");
        }
    }
}
