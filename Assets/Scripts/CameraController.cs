using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    
    public void OnLook(Vector2 directionInput, bool isMouseInput) {
        var sensitivityMultiplier = isMouseInput ? mouseSensitivity : analogStickSensitivity;
        _camera.RequestRotation(directionInput * sensitivityMultiplier);
    }

    private void Awake() {
        _camera = GetComponent<MyCamera>();
    }
}
