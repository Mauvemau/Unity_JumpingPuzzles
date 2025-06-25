using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour {
    [Header("Config")]
    [SerializeField] private float rotationSmoothness = 10f;
    [SerializeField] private float rotationStrength = 0.1f;
    
    private Animator _animator;
    private Vector2 _currentVelocity;
    private Vector2 _lastVelocity; // Snapshot of velocity when input stops
    private bool _isJumping = false;
    
    public void UpdateMeshDirection(Vector2 velocity, bool hasInput) {
        if (hasInput && velocity.sqrMagnitude > 0.01f) {
            _lastVelocity = velocity;
            _currentVelocity = velocity;
        } else {
            _currentVelocity = Vector2.zero;
        }
    }
    
    public void UpdateIsJumping(bool isJumping) => _isJumping = isJumping;
    
    private void Update() {
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
