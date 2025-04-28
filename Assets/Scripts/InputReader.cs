using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionBuffer {
    private Dictionary<float, InputAction> _buffer;

    public ActionBuffer() {
        _buffer = new Dictionary<float, InputAction>();
    }

    public void Add(InputAction action) {
        _buffer.TryAdd(Time.time, action);
    }

    public bool HasActionBeenExecuted(InputAction targetAction, float timeWindow) {
        float currentTime = Time.time;
        foreach (var (actionTime, action) in _buffer) {
            if (currentTime - actionTime <= timeWindow && action == targetAction) {
                return true;
            }
        }
        return false;
    }
}

public class InputReader : MonoBehaviour {
    [SerializeField]
    private PlyController playerControllerReference;
    [SerializeField]
    private InputActionReference moveAction;
    [SerializeField]
    private InputActionReference lookAction;
    [SerializeField]
    private InputActionReference jumpAction;

    private ActionBuffer _actionBuffer = new ActionBuffer();

    private void HandleJumpInput(InputAction.CallbackContext ctx) {
        if(playerControllerReference) {
            _actionBuffer.Add(ctx.action);
            playerControllerReference.OnJump();
        }
    } 

    private void HandleMoveInput(InputAction.CallbackContext ctx) {
        if (playerControllerReference) {
            // We don't add movement to the action buffer.
            playerControllerReference.OnMove(ctx.ReadValue<Vector2>());
        }
    }

    private void Update() {
        if(moveAction) {
            moveAction.action.started += HandleMoveInput; // Cuando deja de ser 0
            moveAction.action.performed += HandleMoveInput; // Solo corre cuando cambian los valores
            moveAction.action.canceled += HandleMoveInput; // Cuando vuelve a 0
        }
        if(jumpAction) {
            jumpAction.action.started += HandleJumpInput;
            //jumpAction.action.canceled += HandleJumpInput;
        }
    }
}
