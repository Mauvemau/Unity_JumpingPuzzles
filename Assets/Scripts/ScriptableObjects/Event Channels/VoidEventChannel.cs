using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Void event channel that can broadcast events to listeners
/// </summary>
[CreateAssetMenu(menuName = "Events/Void Channel")]
public class VoidEventChannel : ScriptableObject {
    public UnityAction OnEventRaised;

    public void RaiseEvent() {
        OnEventRaised?.Invoke();
    }
}
