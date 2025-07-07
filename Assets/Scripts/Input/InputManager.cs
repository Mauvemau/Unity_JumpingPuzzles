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
public class InputManager : MonoBehaviour {
    //[Header("References")]
    private ICharacterController _playerControllerReference; 
    private ICameraController _cameraControllerReference;
    private IGameManager _gameManagerReference;
    
    [Header("Player Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference respawnAction;

    [Header("Debug Actions")] 
    [SerializeField] private InputActionReference cheatLevelAction;
    [SerializeField] private InputActionReference cheatJumpAction;
    [SerializeField] private InputActionReference cheatSpeedAction;

    [Header("UI Actions")] 
    [SerializeField] private InputActionReference requestPauseGameAction;
    [Tooltip("Toggles between locked and unlocked camera")]
    [SerializeField] private InputActionReference toggleMouseLockAction;
    [Tooltip("Toggles between locked and unlocked camera only when the player is holding the button")]
    [SerializeField] private InputActionReference holdToggleMouseLockAction;
    
    public static event Action OnCheatLevelInputPerformed = delegate {};
    public static event Action OnCheatJumpInputPerformed = delegate {};
    public static event Action OnCheatSpeedInputPerformed = delegate {};
    
    private bool _mouseLocked = false;

    /// <summary>
    /// Verifies we're in a situation where it's appropriate to read player input
    /// </summary>
    private bool ShouldReadGameplayRelatedInput() {
        return (_gameManagerReference != null && _gameManagerReference.GetIsGameReady());
    }
    
    private void OnPlayerSpawned() {
        if (!ServiceLocator.TryGetService<ICharacterController>(out var playerController)) {
            Debug.LogError($"{name}: There is no PlayerCharacterController entry in the Service Locator!!");
            return;
        }
        _playerControllerReference = playerController;
        if (!ServiceLocator.TryGetService<ICameraController>(out var cameraController)) {
            Debug.LogError($"{name}: There is no CameraController entry in the Service Locator!!");
            return;
        }
        _cameraControllerReference = cameraController;
        if (!ServiceLocator.TryGetService<IGameManager>(out var gameManager)) {
            Debug.LogError($"{name}: There is no GameManager entry in the Service Locator!!");
            return;
        }
        _gameManagerReference = gameManager;
    }

    private void HandleUIToggled(bool active) {
        SetMouseLocked(!active); // Active = Unlocked / Unactive = Locked
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
#if UNITY_EDITOR
        SetMouseLocked(!_mouseLocked);
#endif
    }

    private void HandleRequestPauseGameInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadGameplayRelatedInput()) return;
        _gameManagerReference?.SetGamePaused(true);
    }
    
    private void HandleCheatLevelInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadGameplayRelatedInput()) return;
        OnCheatLevelInputPerformed?.Invoke();
    }
    
    private void HandleCheatJumpInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadGameplayRelatedInput()) return;
        OnCheatJumpInputPerformed?.Invoke();
    }
    
    private void HandleCheatSpeedInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadGameplayRelatedInput()) return;
        OnCheatSpeedInputPerformed?.Invoke();
    }
    
    private void HandleJumpInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadGameplayRelatedInput()) return;
        if (ctx.started) {
            ActionBuffer.Add(ctx.action.name); // [!] We buffer exclusively the start inputs
            _playerControllerReference?.OnJump();
        }
        else {
            _playerControllerReference?.OnCancelJump();
        }
    }

    private void HandleRespawnInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadGameplayRelatedInput()) return;
        _gameManagerReference?.RespawnPlayer();
    }
    
    private void HandleLookInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadGameplayRelatedInput()) return;
        // We don't add camera input to the action buffer
        var isMouseInput = ctx.control.device.name.Contains("Mouse");
        if (isMouseInput && !_mouseLocked) {
            // We don't move the camera with the mouse if it isn't locked
            // We also cancel the movement in case it's toggled while the mouse is moving
            _cameraControllerReference?.OnLook(Vector2.zero, true);
            return;
        } 
        _cameraControllerReference?.OnLook(ctx.ReadValue<Vector2>(), isMouseInput);
    }
    
    private void HandleMoveInput(InputAction.CallbackContext ctx) {
        if (!ShouldReadGameplayRelatedInput()) return;
        // We don't add movement to the action buffer
        _playerControllerReference?.OnMove(ctx.ReadValue<Vector2>());
    }

    private void Awake() {
        if (!moveAction) {
            Debug.LogWarning($"{name}: {nameof(moveAction)} is null!");
        }
        if (!lookAction) {
            Debug.LogWarning($"{name}: {nameof(lookAction)} is null!");
        }
        if (!jumpAction) {
            Debug.LogWarning($"{name}: {nameof(jumpAction)} is null!");
        }
        if (!respawnAction) {
            Debug.LogWarning($"{name}: {nameof(respawnAction)} is null!");
        }
        
        if (!cheatLevelAction) {
            Debug.LogWarning($"{name}: {nameof(cheatLevelAction)} is null!");
        }
        if (!cheatJumpAction) {
            Debug.LogWarning($"{name}: {nameof(cheatJumpAction)} is null!");
        }
        if (!cheatSpeedAction) {
            Debug.LogWarning($"{name}: {nameof(cheatSpeedAction)} is null!");
        }
        
        if (!requestPauseGameAction) {
            Debug.LogWarning($"{name}: {nameof(requestPauseGameAction)} is null!");
        }
        if (!toggleMouseLockAction) {
            Debug.LogWarning($"{name}: {nameof(toggleMouseLockAction)} is null!");
        }
        if (!holdToggleMouseLockAction) {
            Debug.LogWarning($"{name}: {nameof(holdToggleMouseLockAction)} is null!");
        }
    }

    private void OnEnable() {
        GameManager.OnGameStarted += OnPlayerSpawned;
        NavigationManager.OnToggleUI += HandleUIToggled;
        
        //
        
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
            jumpAction.action.canceled += HandleJumpInput;
        }
        if (respawnAction) {
            respawnAction.action.started += HandleRespawnInput;
        }
        
        if (cheatLevelAction) {
            cheatLevelAction.action.started += HandleCheatLevelInput;
        }
        if (cheatJumpAction) {
            cheatJumpAction.action.started += HandleCheatJumpInput;
        }
        if (cheatSpeedAction) {
            cheatSpeedAction.action.started += HandleCheatSpeedInput;
        }
        
        if (requestPauseGameAction) {
            requestPauseGameAction.action.started += HandleRequestPauseGameInput;
        }
        if (holdToggleMouseLockAction) {
            holdToggleMouseLockAction.action.started += HandleToggleMouseLockInput;
            holdToggleMouseLockAction.action.canceled += HandleToggleMouseLockInput; // We toggle it back on cancel
        }
        if (toggleMouseLockAction) {
            toggleMouseLockAction.action.started += HandleToggleMouseLockInput;
        }
    }
    
    private void OnDisable() {
        GameManager.OnGameStarted -= OnPlayerSpawned;
        NavigationManager.OnToggleUI -= HandleUIToggled;
        
        //
        
        if(moveAction) {
            moveAction.action.started -= HandleMoveInput;
            moveAction.action.performed -= HandleMoveInput;
            moveAction.action.canceled -= HandleMoveInput;
        }
        if (lookAction) {
            lookAction.action.started -= HandleLookInput;
            lookAction.action.performed -= HandleLookInput;
            lookAction.action.canceled -= HandleLookInput;
        }
        if (jumpAction) {
            jumpAction.action.started -= HandleJumpInput;
            jumpAction.action.canceled -= HandleJumpInput;
        }
        if (respawnAction) {
            respawnAction.action.started -= HandleRespawnInput;
        }
        
        if (cheatLevelAction) {
            cheatLevelAction.action.started -= HandleCheatLevelInput;
        }
        if (cheatJumpAction) {
            cheatJumpAction.action.started -= HandleCheatJumpInput;
        }
        if (cheatSpeedAction) {
            cheatSpeedAction.action.started -= HandleCheatSpeedInput;
        }

        if (requestPauseGameAction) {
            requestPauseGameAction.action.started += HandleRequestPauseGameInput;
        }
        if (holdToggleMouseLockAction) {
            holdToggleMouseLockAction.action.started -= HandleToggleMouseLockInput;
            holdToggleMouseLockAction.action.canceled -= HandleToggleMouseLockInput;
        }
        if (toggleMouseLockAction) {
            toggleMouseLockAction.action.started -= HandleToggleMouseLockInput;
        }
    }
}
