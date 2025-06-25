using UnityEngine;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerCharacter : Character {
    [Header("Anchoring Config")]
    [SerializeField]
    [Tooltip("Used for camera-based movement and animations")]
    private MainCamera cameraReference;

    /// <summary>
    /// Allows the camera to assign a reference to itself to the player
    /// </summary>
    public void AssignCameraReference(MainCamera receivedCameraReference) {
        cameraReference = receivedCameraReference;
    }
    
    /// <summary>
    /// Affects the direction in which the player moves based on the direction the camera is currently looking at horizontally 
    /// </summary>
    private Vector3 CalculateCameraAnchoring() {
        if (!cameraReference) return Vector3.one; // No camera, no changes
        var camForward = cameraReference.GetCameraForward().normalized;
        var camRight = cameraReference.GetCameraRight().normalized;
        camForward.y = 0;
        camRight.y = 0;
        // Convert input into world-space movement
        return (camRight * ContinuousForceRequest.Direction.x) +
               (camForward * ContinuousForceRequest.Direction.z);
    }
    
    private void HandleJumping() {
        if (ContinuousForceRequest == null) return;
        Rb.AddForce(Vector3.up * VerticalForceRequest, ForceMode.Force);
        if (InstantForceRequest == null) return;
        var cameraDirection = CalculateCameraAnchoring();
        // Resetting vertical velocity before jumping so that the jump always has the same impulse.
        ResetVerticalMomentum(cameraDirection);
        Rb.AddForce(InstantForceRequest.Direction * InstantForceRequest.Acceleration, ForceMode.Impulse);
        InstantForceRequest = null;
    }
    
    private void HandleMovement() {
        if (ContinuousForceRequest == null) return;
        var cameraDirection = CalculateCameraAnchoring();
        var groundNormal = feet.GetGroundNormal(cameraDirection);
            
        var projectedDirection = ProjectSlope(cameraDirection, groundNormal);
            
        var slopeAngle = GetSlopeAngle(cameraDirection, groundNormal);
        var slopeMultiplier = GetSlopeEffortMultiplier(slopeAngle);

        projectedDirection.y *= slopeMultiplier; // Reduce Y force based on slope steepness.
            
        var speedPercentage = Rb.linearVelocity.magnitude / ContinuousForceRequest.Speed;
        var remainingSpeedPercentage = Mathf.Clamp01(1f - speedPercentage);
        Rb.AddForce(projectedDirection * (ContinuousForceRequest.Acceleration * remainingSpeedPercentage), ForceMode.Force);
        EvilSpeedLimiter();
    }
    
    private void FixedUpdate() {
        HandleMovement();
        HandleJumping();
        HandleFeedback();
    }
    
}
