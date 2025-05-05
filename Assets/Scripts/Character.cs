using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Represents a force that can be applied to a rigidBody
/// </summary>
public class ForceRequest
{
    public Vector3 direction;
    public float acceleration;
    public float speed;
}

/// <summary>
/// Moves the character, controls everything related to the world and the position
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour {
    [Header("Anchoring Config")] // For camera-based movement and animations
    [SerializeField]
    private MyCamera cameraReference;
    [Header("Foot Config")] // For logic that requires knowing if the character is grounded
    public CharacterFoot feet;

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

        Vector3 preservedDirection = currentDirection;
        float projectedSpeed = Vector3.Dot(_rb.linearVelocity, preservedDirection);
        Vector3 correctedVelocity = preservedDirection * projectedSpeed;
        _rb.linearVelocity = new Vector3(correctedVelocity.x, 0, correctedVelocity.z);
    }   

    private Vector3 CalculateCameraAnchoring() {
        if (cameraReference != null) {
            var camForward = cameraReference.GetCameraForward().normalized;
            var camRight = cameraReference.GetCameraRight().normalized;
            camForward.y = 0;
            camRight.y = 0;
            // Convert input into world-space movement
            return (camRight * _continuousForceRequest.direction.x) +
                   (camForward * _continuousForceRequest.direction.z);
        }
        return Vector3.one; // No camera, no changes
    }

    private void FixedUpdate() {
        if (_continuousForceRequest != null) {
            Vector3 cameraDirection = CalculateCameraAnchoring();
            var speedPercentage = _rb.linearVelocity.magnitude / _continuousForceRequest.speed;
            var remainingSpeedPercentage = Mathf.Clamp01(1f - speedPercentage);
            _rb.AddForce(cameraDirection * (_continuousForceRequest.acceleration * remainingSpeedPercentage), ForceMode.Force);
        }
        if (_instantForceRequest != null) {
            Vector3 cameraDirection = CalculateCameraAnchoring(); // IDK if; Better to calculate it twice in one frame than once every frame?
            // Resetting vertical velocity before jumping so that the jump always has the same impulse.
            ResetMomentum(cameraDirection);
            _rb.AddForce(_instantForceRequest.direction * _instantForceRequest.acceleration, ForceMode.Impulse);
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
