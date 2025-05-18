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
    [Tooltip("Multiplies the amount of force applied to the character when airborne")]
    private float airControlFactor = 1.5f;
    [Header("Jump")]
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    [Tooltip("Defines the time window in which a jump input will be accepted if it's pressed before the character has landed")]
    private float earlyJumpWindow = .2f;

    public void OnMove(Vector2 horizontalInput) {
        ForceRequest request = new ForceRequest();
        request.Direction = new Vector3(horizontalInput.x, 0, horizontalInput.y);
        request.Speed = speed;

        // Different level of control if airborne or grounded.
        request.Acceleration = _character.feet.IsGrounded() ? force : force * airControlFactor;

        _character.RequestContinuousForce(request);
    }

    public void OnJump() {
        if (_character.feet.IsGrounded()) {
            ForceRequest request = new ForceRequest();
            request.Direction = Vector3.up;
            request.Acceleration = jumpForce;
            request.Speed = speed;
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
