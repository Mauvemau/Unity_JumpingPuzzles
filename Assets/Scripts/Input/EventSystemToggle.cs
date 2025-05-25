using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Toggles on and off an event system. Used to enable and disable input from an interface.
/// </summary>
[RequireComponent(typeof(EventSystem))]
public class EventSystemToggle : MonoBehaviour {
    [Header("Config")]
    [SerializeField]
    [Tooltip("Defines if the Event System is meant to be enabled or disabled by default")]
    private bool activeOnAwake = true;

    [Header("EventListeners")] 
    [SerializeField]
    private BoolEventChannel toggleEventSystemChannel;
    
    private EventSystem _eventSystem;
    
    private void ToggleEventSystem(bool toggleActive) {
        _eventSystem.enabled = toggleActive;
        if (!toggleActive) return;
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_eventSystem.firstSelectedGameObject); // Selecting the first object again
    }
    
    private void Awake() {
        _eventSystem = GetComponent<EventSystem>();
        if (!activeOnAwake) {
            _eventSystem.enabled = false;
        }
    }

    private void OnEnable() {
        if(toggleEventSystemChannel)
            toggleEventSystemChannel.OnEventRaised += ToggleEventSystem;
    }

    private void OnDisable() {
        if(toggleEventSystemChannel)
            toggleEventSystemChannel.OnEventRaised -= ToggleEventSystem;
    }
}
