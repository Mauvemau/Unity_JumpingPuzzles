using UnityEngine;

/// <summary>
/// Scriptable Object that plays an audio clip at a position
/// </summary>
[CreateAssetMenu(menuName = "Containers/AudioClip3D Container")]
public class AudioClip3DContainer : ScriptableObject {
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private Audio3DEventChannel audioBroadcastChannel;

    public void PlayAudioClip(Vector3 position) {
        if (!audioBroadcastChannel) return;
        audioBroadcastChannel.RaiseEvent(audioClip, position);
    }
}
