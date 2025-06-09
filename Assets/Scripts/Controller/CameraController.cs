using UnityEngine;

/// <summary>
/// Decides actions performed by the camera based on input received
/// </summary>
[RequireComponent(typeof(MainCamera))]
public class CameraController : MonoBehaviour {
    private MainCamera _camera;
    [SerializeField]
    [Min(0)]
    private float mouseSensitivity = .5f;
    [SerializeField]
    [Min(0)]
    private float analogStickSensitivity = 100.0f;
    
    public void OnLook(Vector2 directionInput, bool isMouseInput) {
        if (!_camera) return;
        if (isMouseInput) {
            _camera.RequestMouseRotation(directionInput * mouseSensitivity);
        }
        else {
            _camera.RequestAnalogRotation(directionInput * analogStickSensitivity);
        }
    }

    private void Awake() {
        _camera = GetComponent<MainCamera>();
        if(!_camera) {
            Debug.LogError($"{name}: {nameof(_camera)} is null!");
        }
        if (mouseSensitivity <= 0) {
            Debug.LogWarning($"{name}: {nameof(mouseSensitivity)}'s value is below 1, Camera might not move as intended with a mouse!");
        }
        if (analogStickSensitivity <= 0) {
            Debug.LogWarning($"{name}: {nameof(analogStickSensitivity)}'s value is below 1, Camera might not move as intended with a controller!");
        }
    }
}
