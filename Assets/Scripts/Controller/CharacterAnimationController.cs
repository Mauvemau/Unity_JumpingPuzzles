using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour {
    [Header("Config")]
    [SerializeField] private float rotationSmoothness = 20f;
    [SerializeField] private float rotationStrength = 0.01f;
    [SerializeField] private float decaySpeed = 3f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip3DRandomizer footstepSfx;
    [SerializeField] private AudioClip3DRandomizer jumpingSfx;
    [SerializeField] private AudioClip3DRandomizer landingSfx;
    [Tooltip("Defines at which velocity to start and stop playing footstep sound effects")]
    [SerializeField] private float footstepSfxVelocityOffset = .5f;
    
    
    private Animator _animator;
    private Vector2 _currentVelocity;
    private Vector2 _targetVelocity;
    private bool _isJumping = false;

    public void HandleLandingFeedback() {
        if (!landingSfx) return;
        landingSfx.PlayAudioClip(transform.position);
    }
    
    public void HandleJumpingFeedback() {
        if (!jumpingSfx) return; 
        landingSfx.PlayAudioClip(transform.position);
    }
    
    public void HandleFootstepFeedback() {
        if (!footstepSfx) return;
        if (_currentVelocity.magnitude < footstepSfxVelocityOffset) return;
        footstepSfx.PlayAudioClip(transform.position);
    }
    
    public void UpdateMeshDirection(Vector2 velocity, bool hasInput) {
        if (hasInput && velocity.sqrMagnitude > 0.01f) {
            _targetVelocity = velocity;
        } else {
            _targetVelocity = Vector2.zero;
        }
    }

    public void UpdateIsJumping(bool isJumping) => _isJumping = isJumping;

    private void Update() {
        _currentVelocity = Vector2.Lerp(_currentVelocity, _targetVelocity, Time.deltaTime * decaySpeed);

        if (_currentVelocity.sqrMagnitude > rotationStrength) {
            var moveDirection = new Vector3(_currentVelocity.x, 0f, _currentVelocity.y);
            var targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothness);
        }

        _animator.SetFloat("horizontalSpeed", _currentVelocity.magnitude);
        _animator.SetBool("isJumping", _isJumping);
    }

    private void Awake() {
        _animator = GetComponent<Animator>();
    }
}
