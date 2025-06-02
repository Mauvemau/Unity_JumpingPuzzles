using UnityEngine;

/// <summary>
/// Controls the behaviour of the camera and it's position
/// </summary>
public class MainCamera : MonoBehaviour {
    [SerializeField] 
    private Transform target;
    [Header("Position")]
    [SerializeField]
    private Vector3 offset;
    [Header("Sensitivity")]
    [SerializeField]
    [Min(0)]
    private float horizontalSpeed = 1f;
    [SerializeField]
    [Min(0)]
    private float verticalSpeed = 1f;
    [SerializeField]
    [Min(0)]
    private float rotationSmoothness = 0.1f;
    [Header("Configuration")]
    [SerializeField]
    private float minClampAngle = -70f;
    [SerializeField]
    private float maxClampAngle = 80f;
    [SerializeField]
    public LayerMask collisionLayer;
    [SerializeField]
    [Min(0)]
    private float minDistance = 1f;
    [SerializeField]
    [Min(0)]
    private float maxDistance = 7f;

    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private Vector2 _requestedRotationVelocity;
    private Vector2 _smoothedRotation; // We declare these here so that we don't do it in every late update loop
    private float _pitch = 0;
    private float _yaw = 0;
    
    public Vector3 GetCameraForward() {
        return transform.forward;
    }

    public Vector3 GetCameraRight() {
        return transform.right;
    }
    
    public void RequestRotation(Vector2 rotationDirection) {
        _requestedRotationVelocity += rotationDirection;
    }

    private void OnPlayerSpawned() {
        if (!ServiceLocator.TryGetService<PlayerCharacter>(out var player)) return;
        target = player.transform;
        player.AssignCameraReference(this);
    }

    private Vector3 HandleCameraCollision(Vector3 desiredPosition) {
        var direction = (desiredPosition - target.position).normalized;

        if (Physics.Raycast(target.position, direction, out var hit, maxDistance, collisionLayer)) {
            var distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            return target.position + direction * distance;
        }
        
        return desiredPosition; // If no collision is detected we just return it as it is.
    }

    private Vector3 HandleCameraMovement() {
        //_smoothedRotation = Vector2.Lerp(_smoothedRotation, _requestedRotationVelocity, rotationSmoothness); // Get rid of this

        _yaw += _requestedRotationVelocity.x * horizontalSpeed * Time.deltaTime;
        _pitch -= _requestedRotationVelocity.y * verticalSpeed * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, minClampAngle, maxClampAngle);

        Vector3 rotationVector = new(_pitch, _yaw, 0);
        Debug.DrawRay(transform.position, rotationVector, Color.blue);
        var desiredRotation = Quaternion.Euler(rotationVector);
        _requestedRotationVelocity = Vector2.zero;                      // Reset
        return target.position + desiredRotation * offset;
    }

    private void HandleIdleLogic() {
        transform.position = _defaultPosition;
        transform.rotation = _defaultRotation;
    }
    
    private void LateUpdate() {
        if (!target) return;
        if (!target.gameObject.activeInHierarchy) {
            HandleIdleLogic();
            return;
        }

        var desiredPosition = HandleCameraMovement();

        var finalPosition = HandleCameraCollision(desiredPosition);

        transform.position = finalPosition;
        //transform.RotateAround(target.position, target.up, _yaw);
        transform.LookAt(target);
    }

    private void Awake() {
        _defaultPosition = transform.position;
        _defaultRotation = transform.rotation;
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
