using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ToggableCanvas {
    [SerializeField] private Canvas canvas;
    public BoolEventChannel toggleListenerChannel;
    
    public void Toggle(bool toggle) {
        this.canvas.enabled = toggle;
    }
}

public class NavigationManager : MonoBehaviour {
    [SerializeField] private List<ToggableCanvas> canvases;
    
    private void OnEnable() {
        foreach (var canvas in canvases) {
            canvas.toggleListenerChannel.OnEventRaised += canvas.Toggle;
        }
    }

    private void OnDisable() {
        foreach (var canvas in canvases) {
            canvas.toggleListenerChannel.OnEventRaised -= canvas.Toggle;
        }
    }
}
