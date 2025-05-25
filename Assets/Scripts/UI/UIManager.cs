using System;

/// <summary>
/// Handles events that involve the user interface
/// </summary>
public static class UIManager {
    public static event Action<bool> OnMenuToggled;
    public static event Action<bool> OnPauseMenuToggleRequest;
    
    public static void InvokeOnMenuToggled(bool active) {
        OnMenuToggled?.Invoke(active);
    }

    public static void InvokeOnPauseMenuToggleRequest(bool active) {
        OnPauseMenuToggleRequest?.Invoke(active);
    }
}
