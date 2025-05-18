using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [Header("Anchoring Config")]
    [SerializeField]
    [Tooltip("Used for camera-based movement and animations")]
    private MyCamera cameraReference;
    [Header("Foot Config")]
    [Tooltip("Used for logic that requires knowing if the character is grounded")]
    public CharacterFoot feet;
    [SerializeField]
    [Tooltip("Defines the maximum angle in which the player can walk up a slope")]
    private float maxSlopeAngle = 45f; 
    [SerializeField]
    [Tooltip("Modifies how it begins to get hard to walk up a slope depending on the maximum angle")]
    private float slopeEffortCurveMultiplier = 3f;

    private Rigidbody _rb;
    private ForceRequest _instantForceRequest;
    private ForceRequest _continuousForceRequest;

    public void RequestInstantForce(ForceRequest forceRequest) {
        _instantForceRequest = forceRequest;
    }

    public void RequestContinuousForce(ForceRequest forceRequest) {
        _continuousForceRequest = forceRequest;
    }

    /// <summary>
    /// Cancels momentum on every axis except the direction the player is moving towards
    /// </summary>
    private void ResetMomentum(Vector3 currentDirection) {
        if (_instantForceRequest == null) { 
            _rb.linearVelocity = Vector3.zero;
            return;
        }

        var preservedDirection = currentDirection;
        var projectedSpeed = Vector3.Dot(_rb.linearVelocity, preservedDirection);
        var correctedVelocity = preservedDirection * projectedSpeed;
        _rb.linearVelocity = new Vector3(correctedVelocity.x, 0, correctedVelocity.z);
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
        return (camRight * _continuousForceRequest.Direction.x) +
               (camForward * _continuousForceRequest.Direction.z);
    }
    
    /// <summary>
    /// Calculates the speed in which the player should move up a slope depending on the inclination of the surface
    /// </summary>
    private float GetSlopeEffortMultiplier(float slopeAngle) {
        if (slopeAngle <= 0f) return 1f;
        if (slopeAngle >= maxSlopeAngle) return 0f;
        var t = slopeAngle / maxSlopeAngle;
        var effortCurve = 1f - Mathf.Pow(t, slopeEffortCurveMultiplier);
        return effortCurve;
    }
    
    /// <summary>
    /// Returns the angle of the current slope based on the current direction (positive if moving up the slope, negative if moving down)
    /// </summary>
    private float GetSlopeAngle(Vector3 direction, Vector3 groundNormal) {
        var normalizedDirection = direction.normalized;
        var slopeDirection = Vector3.Cross(Vector3.Cross(Vector3.up, groundNormal), groundNormal).normalized;
        var dot = Vector3.Dot(normalizedDirection, slopeDirection);
        var angle = Vector3.Angle(groundNormal, Vector3.up);
        return dot > 0 ? -angle : angle; // Downhill = negative
    }
    
    /// <summary>
    /// Projects a movement vector into a slope (affecting the Y axis)
    /// </summary>
    private Vector3 ProjectSlope(Vector3 currentDirection, Vector3 groundNormal) {
        return Vector3.ProjectOnPlane(currentDirection, groundNormal).normalized;
    }
    
    private void FixedUpdate() {
        if (_continuousForceRequest != null) {
            var cameraDirection = CalculateCameraAnchoring();
            var groundNormal = feet.GetGroundNormal(cameraDirection);
            
            var projectedDirection = ProjectSlope(cameraDirection, groundNormal);
            
            float slopeAngle = GetSlopeAngle(cameraDirection, groundNormal);
            float slopeMultiplier = GetSlopeEffortMultiplier(slopeAngle);

            projectedDirection.y *= slopeMultiplier; // Reduce Y force based on slope steepness.
            Debug.Log(projectedDirection);
            
            var speedPercentage = _rb.linearVelocity.magnitude / _continuousForceRequest.Speed;
            var remainingSpeedPercentage = Mathf.Clamp01(1f - speedPercentage);
            _rb.AddForce(projectedDirection * (_continuousForceRequest.Acceleration * remainingSpeedPercentage), ForceMode.Force);
        }
        if (_instantForceRequest != null) {
            var cameraDirection = CalculateCameraAnchoring(); // IDK if; Better to calculate it twice in one frame than once every frame?
            // Resetting vertical velocity before jumping so that the jump always has the same impulse.
            ResetMomentum(cameraDirection);
            _rb.AddForce(_instantForceRequest.Direction * _instantForceRequest.Acceleration, ForceMode.Impulse);
            _instantForceRequest = null;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (feet == null) {
            Debug.LogWarning($"Foot are not configured for {gameObject.name}!");
        }
        if (cameraReference == null) {
            Debug.Log($"Camera-based anchoring is not configured for {gameObject.name}, is this intentional?");
        }
    }
}
