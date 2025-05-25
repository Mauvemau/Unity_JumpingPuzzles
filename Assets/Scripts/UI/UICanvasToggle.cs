using UnityEngine;

/// <summary>
/// Enables or disables a canvas with an event listener
/// </summary>
[RequireComponent(typeof(Canvas))]
public class UICanvasToggle : MonoBehaviour {
    [Header("Config")]
    [SerializeField]
    [Tooltip("Defines if the canvas is meant to be enabled or disabled by default")]
    private bool activeOnAwake = true;

    [Header("EventListeners")] 
    [SerializeField]
    private BoolEventChannel toggleCanvasChannel;
    
    private Canvas _canvas;

    private void ToggleCanvas(bool toggleActive) {
        UIManager.InvokeOnMenuToggled(toggleActive);
        _canvas.enabled = toggleActive;
    }
    
    private void Awake() {
        _canvas = GetComponent<Canvas>();
        if (!activeOnAwake) {
            _canvas.enabled = false;
        }
    }

    private void OnEnable() {
        toggleCanvasChannel.OnEventRaised += ToggleCanvas;
    }

    private void OnDisable() {
        toggleCanvasChannel.OnEventRaised -= ToggleCanvas;
    }
}
