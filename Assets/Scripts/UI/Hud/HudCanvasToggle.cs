using UnityEngine;

/// <summary>
/// Enables or disables a canvas. [!] This only will disable it visually.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class HudCanvasToggle : MonoBehaviour {
    [Header("Config")]
    [SerializeField]
    [Tooltip("Defines if the canvas is meant to be enabled or disabled by default")]
    private bool activeOnAwake = true;

    [Header("EventListeners")] 
    [SerializeField]
    private BoolEventChannel toggleCanvasChannel;
    
    private Canvas _canvas;

    private void ToggleCanvas(bool toggleActive) {
        _canvas.enabled = toggleActive;
    }
    
    private void Awake() {
        _canvas = GetComponent<Canvas>();
        if (!activeOnAwake) {
            _canvas.enabled = false;
        }
    }

    private void OnEnable() {
        if(toggleCanvasChannel)
            toggleCanvasChannel.OnEventRaised += ToggleCanvas;
    }

    private void OnDisable() {
        if(toggleCanvasChannel)
            toggleCanvasChannel.OnEventRaised -= ToggleCanvas;
    }
}
