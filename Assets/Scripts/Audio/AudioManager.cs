using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfx2DSource;
    [Tooltip("A prefab game-object with an audio source attached to play sounds at a specific location.")]
    [SerializeField] private AudioSource sfx3DPrefab;

    [Header("Music")] 
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private List<AudioClip> musicPlaylist;
    
    [Header("Event Channels")]
    [SerializeField] private Audio2DEventChannel[] audio2DChannels;
    [SerializeField] private Audio3DEventChannel[] audio3DChannels;

    private int _currentMusicIndex = 0;
    
    private void PlayMusicAtIndex(int index) {
        if (!musicSource) return;
        if (musicPlaylist == null || musicPlaylist.Count < 1) return;
        if (index < 0 || index >= musicPlaylist.Count) return;

        musicSource.clip = musicPlaylist[index];
        musicSource.Play();
    }
    
    private void Play3DSound(AudioClip clip, Vector3 position) {
        if (!sfx3DPrefab) return;
        if (!clip) return;
        var source = Instantiate(sfx3DPrefab, position, Quaternion.identity, transform);
        source.clip = clip;
        source.Play();
        Destroy(source.gameObject, clip.length);
    }
    
    private void Play2DSound(AudioClip clip) {
        if (!sfx2DSource) return;
        if (!clip) return;
        sfx2DSource.PlayOneShot(clip);
    }

    private void Update() {
        if (!musicSource || musicSource.isPlaying || musicPlaylist.Count <= 0) return;
        _currentMusicIndex = (_currentMusicIndex + 1) % musicPlaylist.Count;
        PlayMusicAtIndex(_currentMusicIndex);
    }
    
    private void Start() {
        PlayMusicAtIndex(_currentMusicIndex);
    }
    
    private void Awake() {
        if (!sfx3DPrefab) {
            Debug.LogWarning($"{name}: Prefab for 3D adio not configured!");
        }
        if (!musicSource) {
            Debug.LogWarning($"{name}: Music source is not assigned!");
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

