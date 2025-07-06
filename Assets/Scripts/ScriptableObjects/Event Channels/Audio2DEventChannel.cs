using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event channel that can broadcast Audio Clips to listeners
/// </summary>
[CreateAssetMenu(menuName = "Events/Audio 2D Event Channel")]
public class Audio2DEventChannel : ScriptableObject {
    public UnityAction<AudioClip> OnEventRaised;

    public void RaiseEvent(AudioClip clip) {
        OnEventRaised?.Invoke(clip);
    }
}
