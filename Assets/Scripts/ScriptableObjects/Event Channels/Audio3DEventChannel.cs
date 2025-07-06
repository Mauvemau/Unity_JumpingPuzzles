using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event channel that can broadcast Audio Clips at a position to listeners
/// </summary>
[CreateAssetMenu(menuName = "Events/Audio 3D Event Channel")]
public class Audio3DEventChannel : ScriptableObject {
    public UnityAction<AudioClip, Vector3> OnEventRaised;

    public void RaiseEvent(AudioClip clip, Vector3 position) {
        OnEventRaised?.Invoke(clip, position);
    }
}
