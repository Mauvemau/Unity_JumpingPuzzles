using System;
using UnityEngine;

/// <summary>
/// Represents a force that can be applied to a rigidBody
/// </summary>
public class ForceRequest {
    public Vector3 Direction = Vector3.zero;
    public float Acceleration = 0.0f;
    public float Speed = 0.0f;
}

/// <summary>
/// Moves the character, controls everything related to the world and the position
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour {
    [Header("Rigidbody Config")]
    [SerializeField]
    [Tooltip("Defines a maximum speed for the rigidbody")]
    private float maxRbSpeed = 0f;
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
    protected float VerticalForceRequest = 0f;

    public void RequestInstantForce(ForceRequest forceRequest) {
        InstantForceRequest = forceRequest;
    }

    public void RequestContinuousForce(ForceRequest forceRequest) {
        ContinuousForceRequest = forceRequest;
    }

    public void RequestStartVerticalImpulse(float verticalForce) {
        VerticalForceRequest = verticalForce;
    }

    public void RequestStopVerticalImpulse() {
        VerticalForceRequest = 0;
    }
    
    public void RequestSetPosition(Vector3 position) {
        Rb.linearVelocity = Vector3.zero;
        transform.position = position;
    }
    
    /// <summary>
    /// Clamps the maximum speed of the rigidbody
    /// </summary>
    protected void EvilSpeedLimiter() {
        if(maxRbSpeed > 0) {
            Vector3 clampedVelocity = Rb.linearVelocity;
            clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxRbSpeed, maxRbSpeed);
            clampedVelocity.z = Mathf.Clamp(clampedVelocity.z, -maxRbSpeed, maxRbSpeed);

            Rb.linearVelocity = clampedVelocity; // For your safety!
        }
    }

    /// <summary>
    /// Cancels momentum on every axis except the direction the player is moving towards
    /// </summary>
    protected void ResetVerticalMomentum(Vector3 currentDirection) {
        if (InstantForceRequest == null) { 
            Rb.linearVelocity = Vector3.zero;
            return;
        }
        Rb.linearVelocity = new Vector3(Rb.linearVelocity.x, 0, Rb.linearVelocity.z);
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
        EvilSpeedLimiter();
    }

    private void HandleJumping() {
        if (ContinuousForceRequest == null) return;
        Rb.AddForce(Vector3.up * VerticalForceRequest, ForceMode.Force);
        if (InstantForceRequest == null) return;
        var currentDirection = ContinuousForceRequest.Direction;
        // Resetting vertical velocity before jumping so that the jump always has the same impulse.
        ResetVerticalMomentum(currentDirection);
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
