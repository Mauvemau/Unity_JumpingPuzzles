using UnityEngine;

/// <summary>
/// Scriptable Object that plays an audio clip
/// </summary>
[CreateAssetMenu(menuName = "Containers/AudioClip2D Container")]
public class AudioClip2DContainer : ScriptableObject {
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private Audio2DEventChannel audioBroadcastChannel;

    public void PlayAudioClip() {
        if (!audioBroadcastChannel) return;
        audioBroadcastChannel.RaiseEvent(audioClip);
    }
}
