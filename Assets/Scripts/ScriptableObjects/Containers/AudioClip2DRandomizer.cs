using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Scriptable Object that plays a random audio clip from a list of audio-clips
/// </summary>
[CreateAssetMenu(menuName = "Containers/AudioClip2D Randomizer")]
public class AudioClip2DRandomizer : ScriptableObject {
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Audio2DEventChannel audioBroadcastChannel;

    private void Awake() {
        Random.InitState(DateTime.Now.Millisecond);
    }

    public void PlayAudioClip() {
        if (audioClips == null || audioClips.Length == 0 || !audioBroadcastChannel) return;

        var index = Random.Range(0, audioClips.Length);
        audioBroadcastChannel.RaiseEvent(audioClips[index]);
    }
}
