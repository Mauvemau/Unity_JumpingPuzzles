using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Contains the forward and right of a camera's transform. Used for controlling the behaviour of other objects based on where the camera is pointing
/// </summary>
public struct CameraTransformOffset {
    public Vector3 Forward;
    public Vector3 Right;
}

/// <summary>
/// Decides actions performed by the camera based on input received
/// </summary>
[RequireComponent(typeof(MyCamera))]
public class CameraController : MonoBehaviour {
    private MyCamera _camera;
    [SerializeField]
    private float mouseSensitivity = 1.0f;
    [SerializeField] 
    private float analogStickSensitivity = 80.0f;
    
    /// <summary>
    /// Returns the normalized direction and right vectors of the camera
    /// </summary>
    public CameraTransformOffset GetCameraTransformOffset() {
        CameraTransformOffset cameraOffset;
        cameraOffset.Forward = _camera.GetCameraForward().normalized;
        cameraOffset.Right = _camera.GetCameraRight().normalized;
        return cameraOffset;
    }
    
    public void OnLook(Vector2 directionInput, bool isMouseInput) {
        var sensitivityMultiplier = isMouseInput ? mouseSensitivity : analogStickSensitivity;
        _camera.RequestRotation(directionInput * sensitivityMultiplier);
    }

    private void Awake() {
        _camera = GetComponent<MyCamera>();
    }
}
