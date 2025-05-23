using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Stores actions performed by the player in a buffer
/// self;TODO: Would be nice if there was an abstract buffer class instead of this static.
/// </summary>
public static class ActionBuffer {
    private static readonly Dictionary<float, string> Buffer = new();

    public static void Add(string action) {
        Buffer.TryAdd(Time.time, action);
    }

    public static bool HasActionBeenExecuted(string targetAction, float timeWindow) {
        var currentTime = Time.time;
        foreach (var (actionTime, action) in Buffer) {
            if (currentTime - actionTime <= timeWindow && action == targetAction) {
                return true;
            }
        }
        return false;
    }
}

/// <summary>
/// Handles input for entities controlled by the player
/// </summary>
public class InputReader : MonoBehaviour {
    [Header("Controller references")]
    [SerializeField]
    private PlyController playerControllerReference;
    [SerializeField]
    private CameraController cameraControllerReference;
    [Header("Player Actions")]
    [SerializeField]
    private InputActionReference moveAction;
    [SerializeField]
    private InputActionReference lookAction;
    [SerializeField]
    private InputActionReference jumpAction;
    [Header("UI Actions")]
    [SerializeField]
    [Tooltip("Toggles between locked and unlocked camera")]
    private InputActionReference toggleMouseLockAction;
    [SerializeField]
    [Tooltip("Toggles between locked and unlocked camera only when the player is holding the button")]
    private InputActionReference holdToggleMouseLockAction;
    
    private bool _mouseLocked = false; // ? IDE says it's redundant because bool are set to false by default, but I prefer doing it anyway

    private void OnPlayerSpawned() {
        if (!ServiceLocator.TryGetService<PlyController>(out var playerController)) return;
        playerControllerReference = playerController;
    }
    
    private void SetMouseLocked(bool locked) {
        _mouseLocked = locked;
        if (_mouseLocked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } 
        else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleToggleMouseLockInput(InputAction.CallbackContext ctx) {
        SetMouseLocked(!_mouseLocked);
    }
    
    private void HandleJumpInput(InputAction.CallbackContext ctx) {
        if (!playerControllerReference) return;
        ActionBuffer.Add(ctx.action.name);
        playerControllerReference.OnJump();
    }

    private void HandleLookInput(InputAction.CallbackContext ctx) {
        if (!cameraControllerReference) return;
        // We don't add camera input to the action buffer
        var isMouseInput = ctx.control.device.name.Contains("Mouse");
        if (isMouseInput && !_mouseLocked) {
            // We don't move the camera with the mouse if it isn't locked
            // We also cancel the movement in case it's toggled while the mouse is moving
            cameraControllerReference.OnLook(Vector2.zero, true);
            return;
        } 
        cameraControllerReference.OnLook(ctx.ReadValue<Vector2>(), isMouseInput);
    }
    
    private void HandleMoveInput(InputAction.CallbackContext ctx) {
        if (!playerControllerReference) return;
        // We don't add movement to the action buffer
        playerControllerReference.OnMove(ctx.ReadValue<Vector2>());
    }

    private void Update() {
        //if (!Application.isFocused) return; // We don't want to capture input when the game is unfocused
        if(moveAction) {
            moveAction.action.started += HandleMoveInput; // When the value stops being 0
            moveAction.action.performed += HandleMoveInput; // Is only executed when values change
            moveAction.action.canceled += HandleMoveInput; // When the value returns to 0
        }
        if (lookAction) {
            lookAction.action.started += HandleLookInput;
            lookAction.action.performed += HandleLookInput;
            lookAction.action.canceled += HandleLookInput;
        }
        if (jumpAction) {
            jumpAction.action.started += HandleJumpInput;
            //jumpAction.action.canceled += HandleJumpInput;
        }
        if (holdToggleMouseLockAction) {
            holdToggleMouseLockAction.action.started += HandleToggleMouseLockInput;
            holdToggleMouseLockAction.action.canceled += HandleToggleMouseLockInput; // We toggle it back on cancel
        }
        if (toggleMouseLockAction) {
            toggleMouseLockAction.action.started += HandleToggleMouseLockInput;
        }
    }

    private void Awake() {
        if(!playerControllerReference) {
            Debug.Log($"{name} doesn't currently have a player controller reference."); // We simply log it because it could be intended
        }
        if (!cameraControllerReference) {
            Debug.Log($"{name} doesn't currently have a camera controller reference.");
        }
        if (!moveAction) {
            Debug.LogWarning($"{name}: {nameof(moveAction)} is null!");
        }
        if (!lookAction) {
            Debug.LogWarning($"{name}: {nameof(lookAction)} is null!");
        }
        if (!jumpAction) {
            Debug.LogWarning($"{name}: {nameof(jumpAction)} is null!");
        }
        if (!toggleMouseLockAction) {
            Debug.LogWarning($"{name}: {nameof(toggleMouseLockAction)} is null!");
        }
        if (!holdToggleMouseLockAction) {
            Debug.LogWarning($"{name}: {nameof(holdToggleMouseLockAction)} is null!");
        }
    }

    private void OnEnable() {
        GameManager.OnPlayerSpawned += OnPlayerSpawned;
    }
    
    private void OnDisable() {
        GameManager.OnPlayerSpawned -= OnPlayerSpawned;
    }
}
