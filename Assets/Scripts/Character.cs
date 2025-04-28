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
    [SerializeField]
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

    private void Awake() {
        _rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate() {
        if (_continuousForceRequest != null) {
            var speedPercentage = _rb.linearVelocity.magnitude / _continuousForceRequest.speed;
            var remainingSpeedPercentage = Mathf.Clamp01(1f - speedPercentage);
            _rb.AddForce(_continuousForceRequest.direction * (_continuousForceRequest.acceleration * remainingSpeedPercentage), ForceMode.Force);
        }
        if (_instantForceRequest != null) {
            // Resetting vertical velocity before jumping so that the jump always has the same impulse.
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            _rb.AddForce(_instantForceRequest.direction * _instantForceRequest.acceleration, ForceMode.Impulse);
            _instantForceRequest = null;
        }
    }
}
