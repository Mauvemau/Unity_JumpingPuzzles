using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Scriptable Object that plays a random audio clip from a list of audio-clips at a position
/// </summary>
[CreateAssetMenu(menuName = "Containers/AudioClip3D Randomizer")]
public class AudioClip3DRandomizer : ScriptableObject {
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Audio3DEventChannel audioBroadcastChannel;
    
    private void Awake() {
        Random.InitState(DateTime.Now.Millisecond);
    }
    
    public void PlayAudioClip(Vector3 position) {
        if (audioClips == null || audioClips.Length == 0 || !audioBroadcastChannel) return;

        var index = Random.Range(0, audioClips.Length);
        audioBroadcastChannel.RaiseEvent(audioClips[index], position);
    }
}
