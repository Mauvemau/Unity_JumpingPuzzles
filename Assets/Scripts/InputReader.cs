using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class ActionBuffer {
    private static readonly Dictionary<float, string> Buffer = new();

    public static void Add(string action) {
        Buffer.TryAdd(Time.time, action);
    }

    public static bool HasActionBeenExecuted(string targetAction, float timeWindow) {
        float currentTime = Time.time;
        foreach (var (actionTime, action) in Buffer) {
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

    private void HandleJumpInput(InputAction.CallbackContext ctx) {
        if(playerControllerReference) {
            ActionBuffer.Add(ctx.action.name);
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
        else {
            Debug.LogError($"{nameof(moveAction)} is null!");
        }
        if (jumpAction) {
            jumpAction.action.started += HandleJumpInput;
            //jumpAction.action.canceled += HandleJumpInput;
        }
        else {
            Debug.LogError($"{nameof(jumpAction)} is null!");
        }
    }

    private void Awake() {
        if(playerControllerReference == null) {
            Debug.LogError("Player Controller reference is null!");
        }
    }
}
