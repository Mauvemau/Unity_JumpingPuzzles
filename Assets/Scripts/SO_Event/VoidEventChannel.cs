using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Void channel for SO Events
/// </summary>
[CreateAssetMenu(menuName = "Events/Void Channel")]
public class VoidEventChannel : ScriptableObject
{
    private readonly UnityAction _onEventRaised;

    public VoidEventChannel(UnityAction onEventRaised) {
        _onEventRaised = onEventRaised;
    }

    public void RaiseEvent()
    {
        _onEventRaised?.Invoke();
    }
}
