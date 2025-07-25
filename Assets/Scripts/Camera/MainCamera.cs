﻿using UnityEngine;

/// <summary>
/// Controls the behaviour of the camera and it's position
/// </summary>
public class MainCamera : MonoBehaviour {
    [Tooltip("Target followed by the camera")]
    [SerializeField] private Transform target;
    [Header("Rotation")]
    [Min(0)]
    [SerializeField] private float horizontalSpeed = 1f;
    [Min(0)]
    [SerializeField] private float verticalSpeed = 1f;
    [Tooltip("Angle in which the camera stops rotating vertically")]
    [SerializeField] private float minClampAngle = -80f;
    [Tooltip("Angle in which the camera stops rotating vertically")]
    [SerializeField] private float maxClampAngle = 80f;
    [Header("Collision")]
    [Tooltip("The layer the camera will collide with")]
    [SerializeField] private LayerMask collisionLayer;
    [Tooltip("The size of the sphere collision around the camera")]
    [SerializeField] private float collisionRadius = .3f;
    [Min(0)]
    [Tooltip("How close to the target the camera can get")]
    [SerializeField] private float minDistance = 1f;
    [Min(0)]
    [Tooltip("How far from the target the camera can get")]
    [SerializeField] private float maxDistance = 7f;

    private Vector2 _mouseInput;
    private Vector2 _analogInput;
    private float _currentDistance;
    private float _pitch = 0;
    private float _yaw = 0;

    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;

    public Vector3 GetCameraForward() {
        return transform.forward;
    }

    public Vector3 GetCameraRight() {
        return transform.right;
    }

    public void RequestMouseRotation(Vector2 delta) {
        _mouseInput += delta;
    }

    public void RequestAnalogRotation(Vector2 input) {
        _analogInput = input;
    }

    private void OnPlayerSpawned() {
        if (!ServiceLocator.TryGetService<IPlayableCharacter>(out var player)) return;
        target = player.Transform;
        player?.AssignCameraReference(this);
    }

    private Vector3 HandleCameraCollision(Vector3 desiredPosition) {
        var direction = (desiredPosition - target.position).normalized;
        var maxCheckDistance = Vector3.Distance(target.position, desiredPosition);

        if (!Physics.SphereCast(target.position, collisionRadius, direction, out var hit, maxCheckDistance, collisionLayer)) 
            return desiredPosition;
        
        var clampedDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        return target.position + direction * clampedDistance;
    }

    private void HandleCameraRotation() {
        var analogDelta = _analogInput * Time.deltaTime;

        _yaw += (_mouseInput.x * horizontalSpeed) + (analogDelta.x * horizontalSpeed);
        _pitch -= (_mouseInput.y * verticalSpeed) + (analogDelta.y * verticalSpeed);
        _pitch = Mathf.Clamp(_pitch, minClampAngle, maxClampAngle);

        _mouseInput = Vector2.zero;

        var distance = Mathf.Clamp(_currentDistance, minDistance, maxDistance);
        var rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        var offset = rotation * new Vector3(0f, 0f, -distance);
        var desiredPosition = target.position + offset;

        transform.position = HandleCameraCollision(desiredPosition);
        transform.LookAt(target);
    }

    private void HandleIdleLogic() {
        transform.position = _defaultPosition;
        transform.rotation = _defaultRotation;
    }

    private void LateUpdate() {
        if (!target) return;
        if (!target.gameObject.activeInHierarchy)
        {
            HandleIdleLogic();
            return;
        }

        HandleCameraRotation();
    }

    private void Awake() {
        _defaultPosition = transform.position;
        _defaultRotation = transform.rotation;
        _currentDistance = maxDistance;
        
        if (collisionLayer.value == 0) {
            Debug.Log($"{name}: {nameof(collisionLayer)} is not configured, verify if intended.");
        }
        if (horizontalSpeed == 0) {
            Debug.LogWarning($"{name}: {nameof(horizontalSpeed)} is currently set to 0!");
        }
        if (verticalSpeed == 0) {
            Debug.LogWarning($"{name}: {nameof(verticalSpeed)} is currently set to 0!");
        }
    }

    private void OnEnable() {
        GameManager.OnGameStarted += OnPlayerSpawned;
    }

    private void OnDisable() {
        GameManager.OnGameStarted -= OnPlayerSpawned;
    }
}
