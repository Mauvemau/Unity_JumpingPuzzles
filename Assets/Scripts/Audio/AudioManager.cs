using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfx2DSource;
    [Tooltip("A prefab game-object with an audio source attached to play sounds at a specific location.")]
    [SerializeField] private AudioSource sfx3DPrefab;

    [Header("Event Channels")]
    [SerializeField] private Audio2DEventChannel[] audio2DChannels;
    [SerializeField] private Audio3DEventChannel[] audio3DChannels;

    private void Play3DSound(AudioClip clip, Vector3 position) {
        if (!sfx3DPrefab) return;
        if (!clip) return;
        var source = Instantiate(sfx3DPrefab, position, Quaternion.identity);
        source.clip = clip;
        source.Play();
        Destroy(source.gameObject, clip.length);
    }
    
    private void Play2DSound(AudioClip clip) {
        if (!sfx2DSource) return;
        if (!clip) return;
        sfx2DSource.PlayOneShot(clip);
    }

    private void Awake() {
        if (!sfx3DPrefab) {
            Debug.LogWarning($"{name}: Prefab for 3D adio not configured!");
        }
    }

    private void OnValidate() {
        if (TryGetComponent<AudioSource>(out var audioSourceComponent)) {
            sfx2DSource = audioSourceComponent;
        }
    }

    private void OnEnable() {
        foreach (var channel in audio2DChannels)
            channel.OnEventRaised += Play2DSound;

        foreach (var channel in audio3DChannels)
            channel.OnEventRaised += Play3DSound;
    }

    private void OnDisable() {
        foreach (var channel in audio2DChannels)
            channel.OnEventRaised -= Play2DSound;

        foreach (var channel in audio3DChannels)
            channel.OnEventRaised -= Play3DSound;
    }
}

