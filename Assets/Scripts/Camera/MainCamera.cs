using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

/// <summary>
/// Controls the behaviour of the camera and it's position
/// </summary>
public class MainCamera : MonoBehaviour {
    [SerializeField] 
    private Transform target;
    [Header("Rotation")]
    [SerializeField]
    [Min(0)]
    private float horizontalSpeed = 1f;
    [SerializeField]
    [Min(0)]
    private float verticalSpeed = 1f;
    [SerializeField]
    private float minClampAngle = -80f;
    [SerializeField]
    private float maxClampAngle = 80f;
    [Header("Collision")]
    [SerializeField]
    private LayerMask collisionLayer;
    [SerializeField]
    private float collisionRadius = .3f;
    [SerializeField]
    [Min(0)]
    private float minDistance = 1f;
    [SerializeField]
    [Min(0)]
    private float maxDistance = 7f;

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

    public void RequestMouseRotation(Vector2 delta)
    {
        _mouseInput += delta;
    }

    public void RequestAnalogRotation(Vector2 input)
    {
        _analogInput = input;
    }

    private void OnPlayerSpawned() {
        if (!ServiceLocator.TryGetService<PlayerCharacter>(out var player)) return;
        target = player.transform;
        player.AssignCameraReference(this);
    }

    private Vector3 HandleCameraCollision(Vector3 desiredPosition)
    {
        Vector3 direction = (desiredPosition - target.position).normalized;
        float maxCheckDistance = Vector3.Distance(target.position, desiredPosition);

        if (Physics.SphereCast(target.position, collisionRadius, direction, out var hit, maxCheckDistance, collisionLayer)) {
            float clampedDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            return target.position + direction * clampedDistance;
        }

        return desiredPosition;
    }

    private void HandleCameraRotation() {
        Vector2 analogDelta = _analogInput * Time.deltaTime;

        _yaw += (_mouseInput.x * horizontalSpeed) + (analogDelta.x * horizontalSpeed);
        _pitch -= (_mouseInput.y * verticalSpeed) + (analogDelta.y * verticalSpeed);
        _pitch = Mathf.Clamp(_pitch, minClampAngle, maxClampAngle);

        _mouseInput = Vector2.zero;

        float distance = Mathf.Clamp(_currentDistance, minDistance, maxDistance);
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPosition = target.position + offset;

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

        if (!target) {
            Debug.Log($"{name}: The camera doesn't currently have a {nameof(target)}, verify if intended.");
        }
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
        GameManager.OnPlayerSpawned += OnPlayerSpawned;
    }

    private void OnDisable() {
        GameManager.OnPlayerSpawned -= OnPlayerSpawned;
    }
}
