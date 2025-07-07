using UnityEngine;

/// <summary>
/// Enables or disables a button container.
/// </summary>
public class UIButtonContainerToggle : MonoBehaviour {
    [Header("Config")]
    [Tooltip("Defines if the container is meant to be enabled or disabled by default")]
    [SerializeField] private bool activeOnAwake = true;

    [Header("EventListeners")] 
    [SerializeField] private BoolEventChannel toggleContainerChannel;

    private GameObject[] _children;
    
    private void ToggleContainer(bool toggleActive) {
        foreach (var child in _children) {
            if (child) {
                child.SetActive(toggleActive);
            }
        }
    }

    private void Awake() {
        // Acquire all direct children
        var childCount = transform.childCount;
        _children = new GameObject[childCount];
        for (var i = 0; i < childCount; i++) {
            _children[i] = transform.GetChild(i).gameObject;
        }

        // Disable all children if not active on awake
        if (!activeOnAwake) {
            ToggleContainer(false);
        }
    }

    private void OnEnable() {
        if (toggleContainerChannel)
            toggleContainerChannel.OnEventRaised += ToggleContainer;
    }

    private void OnDisable() {
        if (toggleContainerChannel)
            toggleContainerChannel.OnEventRaised -= ToggleContainer;
    }
}
