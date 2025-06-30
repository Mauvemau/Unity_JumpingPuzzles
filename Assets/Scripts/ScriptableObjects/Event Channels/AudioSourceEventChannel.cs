using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// AudioSource event channel that can broadcast events to listeners
/// </summary>
[CreateAssetMenu(menuName = "Events/AudioSource Channel")]
public class AudioSourceEventChannel : MonoBehaviour {
    public UnityAction<AudioSource> OnEventRaised;

    public void RaiseEvent(AudioSource value)
    {
        OnEventRaised?.Invoke(value);
    }
}
