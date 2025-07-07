using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A buffer that stores a string defining an action and a timestamp.
/// </summary>
public class TimedActionBuffer : BufferContainer<float, string> {
    private float _maxLifeTime = 20f;
    public float MaxLifetime {
        get => _maxLifeTime;
        set {
            _maxLifeTime = value;
            if (_maxLifeTime <= 0f)
                Debug.LogWarning("Buffer life time should be greater than 0");
        }
    }

    /// <summary>
    /// Removes entries older than maxLifeTime
    /// </summary>
    private void CleanupOldEntries() {
        var currentTime = Time.time;
        List<float> keysToRemove = new();
        foreach (var (timestamp, _) in buffer) {
            if (currentTime - timestamp > MaxLifetime) {
                keysToRemove.Add(timestamp);
            }
        }

        foreach (var key in keysToRemove) {
            buffer.Remove(key);
        }
    }
    
    public void AddAction(string actionName) {
        buffer.TryAdd(Time.time, actionName);
        CleanupOldEntries();
    }
    
    /// <summary>
    /// Returns true if the action occurred within a given time window
    /// </summary>
    public bool HasActionBeenExecuted(string targetAction, float timeWindow) {
        var currentTime = Time.time;
        foreach (var (timestamp, action) in buffer) {
            if (currentTime - timestamp <= timeWindow && action == targetAction) {
                return true;
            }
        }

        return false;
    }
}

public interface ICharacterController {
    /// <summary>
    /// Debug function, toggles between infinite jump mode and normal mode.
    /// </summary>
    public void ToggleInfiniteJump();
    /// <summary>
    /// Moves a character
    /// </summary>
    public void OnMove(Vector2 horizontalInput);
    /// <summary>
    /// Makes a character jump
    /// </summary>
    public void OnJump();
    /// <summary>
    /// Cancels a character's jump
    /// </summary>
    public void OnCancelJump();
}

/// <summary>
/// Decides actions taken by the player based on input received.
/// </summary>
[RequireComponent(typeof(Character))]
public class PlayerCharacterController : MonoBehaviour, ICharacterController {
    private Character _character;
    [Header("Movement")]
    [Min(0)]
    [SerializeField] private float speed = 25f;
    [Min(0)]
    [SerializeField] private float force = 30f;
    [Min(0)]
    [Tooltip("Multiplies the amount of force applied to the character when airborne")]
    [SerializeField] private float airControlFactor = .8f;
    [Header("Jump")]
    [Min(0)]
    [SerializeField] private float jumpForce = 5f;
    [Min(0)]
    [Tooltip("Vertical continuous force applied to the character when holding jump")]
    [SerializeField] private float holdJumpForce = 10f;
    [Min(0)]
    [Tooltip("Amount of time the player is allowed to holdJump")]
    [SerializeField] private float holdJumpTime = .35f;
    [Min(0)]
    [Tooltip("Defines the time window in which a jump input will be accepted if it's pressed before the character has landed")]
    [SerializeField] private float earlyJumpWindow = .2f;

    [Header("Debug")]
    [Tooltip("Allows the player to fly... sort of")]
    [SerializeField] private bool infiniteJump = false;

    private readonly TimedActionBuffer _actionBuffer = new TimedActionBuffer();
    
    public void ToggleInfiniteJump() {
        infiniteJump = !infiniteJump;
    }
    
    public void OnMove(Vector2 horizontalInput) {
        if (!_character) return;
        var request = new ForceRequest();
        request.Direction = new Vector3(horizontalInput.x, 0, horizontalInput.y);
        request.Speed = speed;

        // Different level of control if airborne or grounded
        request.Acceleration = _character.feet.IsGrounded() ? force : force * airControlFactor;

        _character.RequestContinuousForce(request);
    }

    /// <summary>
    /// For separating the buffering from player input and jump buffer logic.
    /// </summary>
    private void HandleJump() {
        if (!_character) return;
        if (!_character.feet.IsGrounded() && !infiniteJump) return;
        var request = new ForceRequest();
        request.Direction = Vector3.up;
        request.Acceleration = jumpForce;
        request.Speed = speed;
        _character.feet.SetJumping();
        _character.RequestInstantForce(request);
        _character.RequestStartVerticalImpulse(holdJumpForce);
    }
    
    /// <summary>
    /// Called only from outside
    /// </summary>
    public void OnJump() {
        _actionBuffer.AddAction("Jump");
        HandleJump();
    }

    public void OnCancelJump() {
        _character.RequestStopVerticalImpulse();
    }

    private void FixedUpdate() {
        if (!_character) return;
        if (_character.feet.IsGrounded() && _actionBuffer.HasActionBeenExecuted("Jump", earlyJumpWindow)) {
            HandleJump();
            OnCancelJump(); // Cancel hold jump immediately
        }

        if (!infiniteJump && (_character.feet.IsGrounded() || _character.feet.GetLastJumpTimestamp() + holdJumpTime < Time.time)) {
            _character.RequestStopVerticalImpulse();
        }
    }
    
    private void Awake() {
        _character = GetComponent<Character>();
        if (!_character) {
            Debug.LogError($"{name}: {nameof(_character)} is null!");
        }
        if (speed <= 0) {
            Debug.LogWarning($"{name}: {nameof(speed)}'s value is below 1, Character might not move as intended!");
        }
        if (force <= 0) {
            Debug.LogWarning($"{name}: {nameof(force)}'s value is below 1, Character might not move as intended!");
        }
        if (jumpForce <= 0) {
            Debug.LogWarning($"{name}: {nameof(jumpForce)}'s value is below 1, Character might not jump as intended!");
        }
        if (airControlFactor <= 0) {
            Debug.LogWarning($"{name}: {nameof(airControlFactor)}'s value is below 1, Character might not move as intended while airborne!");
        }
    }
}
