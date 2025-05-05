using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Decides actions taken by the player based on input received.
/// </summary>
[RequireComponent(typeof(Character))]
public class PlyController : MonoBehaviour {
    private Character _character;
    [Header("Movement")]
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float force = 10f;
    [SerializeField]
    private float airControlFactor = 1.5f;
    [Header("Jump")]
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField] 
    private float earlyJumpWindow = .2f;

    public void OnMove(Vector2 horizontalInput) {
        ForceRequest request = new ForceRequest();
        request.direction = new Vector3(horizontalInput.x, 0, horizontalInput.y);
        request.speed = speed;

        // Different level of control if airborne or grounded.
        request.acceleration = _character.feet.IsGrounded() ? force : force * airControlFactor;

        _character.RequestContinuousForce(request);
    }

    public void OnJump() {
        if (_character.feet.IsGrounded()) {
            ForceRequest request = new ForceRequest();
            request.direction = Vector3.up;
            request.acceleration = jumpForce;
            request.speed = speed;
            _character.feet.SetJumping();
            _character.RequestInstantForce(request);
        }
    }

    private void Awake() {
        _character = GetComponent<Character>();
    }

    private void FixedUpdate() {
        if (_character.feet.IsGrounded() && ActionBuffer.HasActionBeenExecuted("Jump", earlyJumpWindow)) {
            OnJump();
        }
    }
}
