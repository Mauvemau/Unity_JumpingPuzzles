using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Decentralized canvas container handles activation of the canvas with scriptable objects and defines how the UI manager should act on activation
/// </summary>
[Serializable]
public class ToggleableCanvas {
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button firstSelectedButton;
    [Tooltip("If true, all other canvases will be automatically toggled off when this one is enabled")]
    [SerializeField] private bool disableOtherCanvases = true;
    public BoolEventChannel toggleListenerChannel;

    public static event Action<Button> OnRequestSelectButton = delegate {};
    public static event Action OnRequestCloseAllCanvases = delegate {};
    public static event Action OnDisabledCanvas = delegate {};
    
    public Canvas Canvas => canvas;
    
    public void Toggle(bool toggle) {
        this.canvas.gameObject.SetActive(true); // If dev disabled the object in editor want to make use it's always enabled during runtime
        this.canvas.enabled = toggle;
        if (!toggle) {
            OnDisabledCanvas?.Invoke();
            return;
        }
        if (disableOtherCanvases) {
            OnRequestCloseAllCanvases?.Invoke();
        }
        if (firstSelectedButton) {
            OnRequestSelectButton?.Invoke(firstSelectedButton);
        }
    }
}

/// <summary>
/// Centralized class that handles the event system and configures external behaviour for canvases
/// </summary>
public class NavigationManager : MonoBehaviour {
    [SerializeField] private EventSystem eventSystem;
    [Tooltip("The main canvas, the root node of the navigation system. This is the first canvas that will be enabled")]
    [SerializeField] private Canvas mainCanvas;
    [Tooltip("If the UI should be enabled by default")]
    [SerializeField] private bool activeOnAwake = false;
    [Tooltip("Contains the references of all the canvases in the navigation system")]
    [SerializeField] private List<ToggleableCanvas> canvases;
    private int _mainCanvasIndex = -1;
    
    /// <summary>
    /// Lets the Input manager know when to lock/unlock mouse.
    /// </summary>
    public static event Action<bool> OnToggleUI = delegate {};

    private void HandleCanvasDisabled() {
        ToggleEventSystemInput(false);
        OnToggleUI?.Invoke(false);
    }
    
    private void ToggleEventSystemInput(bool active) {
        if (!eventSystem) return;
        if (eventSystem.TryGetComponent<BaseInputModule>(out var inputModule)) {
            inputModule.enabled = active;
        }
    }
    
    private void SelectButton(Button button) {
        if (!eventSystem) return;
        if (eventSystem.TryGetComponent<BaseInputModule>(out var inputModule)) {
            inputModule.enabled = true; // If the canvas contains a button it means it has input
        }
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(button.gameObject);
        OnToggleUI?.Invoke(true);
    }

    private void CloseAllCanvases() {
        foreach (var canvas in canvases) {
            canvas.Toggle(false);
        }
    }

    private void Awake() {
        if (!eventSystem) {
            Debug.LogError($"{name}: There is no Event System assigned!!");
            return;
        }
        if (canvases.Count < 1) {
            Debug.LogWarning($"{name}: There are no canvases in the navigation system!");
            return;
        }
        CloseAllCanvases();
        if (activeOnAwake) {
            canvases[_mainCanvasIndex].Toggle(true);
        }
    }
    
    private void OnValidate() {
        if (!mainCanvas && canvases.Count > 0) {
            mainCanvas = canvases[0].Canvas;
            _mainCanvasIndex = 0;
            return;
        }

        for (var i = 0; i < canvases.Count; i++) {
            if (canvases[i].Canvas != mainCanvas) continue;
            _mainCanvasIndex = i;
            return;
        }

        mainCanvas = null;
        _mainCanvasIndex = -1;
        Debug.LogWarning($"{name}: Main canvas not found in the canvas list!");
    }


    private void OnEnable() {
        foreach (var canvas in canvases) {
            canvas.toggleListenerChannel.OnEventRaised += canvas.Toggle;
        }
        ToggleableCanvas.OnRequestSelectButton += SelectButton;
        ToggleableCanvas.OnRequestCloseAllCanvases += CloseAllCanvases;
        ToggleableCanvas.OnDisabledCanvas += HandleCanvasDisabled;
    }

    private void OnDisable() {
        foreach (var canvas in canvases) {
            canvas.toggleListenerChannel.OnEventRaised -= canvas.Toggle;
        }
        ToggleableCanvas.OnRequestSelectButton -= SelectButton;
        ToggleableCanvas.OnRequestCloseAllCanvases -= CloseAllCanvases;
        ToggleableCanvas.OnDisabledCanvas -= HandleCanvasDisabled;
    }
}
