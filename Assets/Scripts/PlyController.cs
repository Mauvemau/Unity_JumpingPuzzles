using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads input and decides actions taken by the player
/// </summary>
public class PlyController : MonoBehaviour
{
    [SerializeField]
    private Character character;
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

    private void OnLook(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        ForceRequest request = new ForceRequest();
        Vector2 horizontalInput = ctx.ReadValue<Vector2>();
        request.direction = new Vector3(horizontalInput.x, 0, horizontalInput.y);
        request.speed = speed;
        request.acceleration = force;
        character.RequestContinuousForce(request);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (character.IsGrounded())
        {
            ForceRequest request = new ForceRequest();
            request.direction = Vector3.up;
            request.acceleration = jumpForce;
            request.speed = speed;
            character.RequestInstantForce(request);
        }
    }

    private void OnEnable()
    {
        if (moveAction == null) return;
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove;
        if (jumpAction == null) return;
        jumpAction.action.performed += OnJump;
        if (lookAction == null) return;
        lookAction.action.performed += OnLook;
    }
}
