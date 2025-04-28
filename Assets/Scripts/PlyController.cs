using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Decides actions taken by the player based on input received.
/// </summary>
[RequireComponent(typeof(Character))]
public class PlyController : MonoBehaviour {
    private Character _character;
    [Header("Look")]
    [SerializeField]
    private InputActionReference lookAction;
    [Header("Movement")]
    [SerializeField]
    private InputActionReference moveAction;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float force = 10f;
    [Header("Jump")]
    [SerializeField]
    private InputActionReference jumpAction;
    [SerializeField]
    private float jumpForce = 5f;

    private void OnLook(InputAction.CallbackContext ctx) {
        var value = ctx.ReadValue<Vector2>();
    }

    public void OnMove(Vector2 horizontalInput) {
        ForceRequest request = new ForceRequest();
        request.direction = new Vector3(horizontalInput.x, 0, horizontalInput.y);
        request.speed = speed;
        request.acceleration = force;
        _character.RequestContinuousForce(request);
    }

    public void OnJump() {
        if (_character.feet.IsGrounded()) {
            ForceRequest request = new ForceRequest();
            request.direction = Vector3.up;
            request.acceleration = jumpForce;
            request.speed = speed;
            _character.feet.setJumping();
            _character.RequestInstantForce(request);
        }
    }

    private void Awake() {
        _character = GetComponent<Character>();
    }
}
