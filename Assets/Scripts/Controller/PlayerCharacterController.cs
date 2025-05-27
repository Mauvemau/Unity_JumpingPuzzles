using UnityEngine;

/// <summary>
/// Decides actions taken by the player based on input received.
/// </summary>
[RequireComponent(typeof(Character))]
public class PlayerCharacterController : MonoBehaviour {
    private Character _character;
    [Header("Movement")]
    [SerializeField]
    [Min(0)]
    private float speed = 25f;
    [SerializeField]
    [Min(0)]
    private float force = 30f;
    [SerializeField]
    [Min(0)]
    [Tooltip("Multiplies the amount of force applied to the character when airborne")]
    private float airControlFactor = .8f;
    [Header("Jump")]
    [Min(0)]
    [SerializeField]
    private float jumpForce = 5f;
    [Min(0)]
    [SerializeField]
    [Tooltip("Vertical continuous force applied to the character when holding jump")]
    private float holdJumpForce = 10f;
    [Min(0)]
    [SerializeField]
    [Tooltip("Amount of time the player is allowed to holdJump")]
    private float holdJumpTime = .35f;
    [SerializeField]
    [Min(0)]
    [Tooltip("Defines the time window in which a jump input will be accepted if it's pressed before the character has landed")]
    private float earlyJumpWindow = .2f;

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Allows the player to fly... sort of")]
    private bool infiniteJump = false;

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

    public void OnJump() {
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

    public void OnCancelJump() {
        _character.RequestStopVerticalImpulse();
    }

    private void FixedUpdate() {
        if (!_character) return;
        if (_character.feet.IsGrounded() && ActionBuffer.HasActionBeenExecuted("Jump", earlyJumpWindow)) {
            OnJump();
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
