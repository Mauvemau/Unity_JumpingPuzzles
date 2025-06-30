using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioCue {
    [SerializeField] private AudioClip audioClip;
    [SerializeField] public AudioSourceEventChannel audioTriggerChannel;

    public void HandlePlayAudio(AudioSource source) { 
    
    }
}

public class AudioManager : MonoBehaviour {
    [SerializeField] private List<AudioCue> cues;

    private void OnEnable() {
        foreach (var cue in cues) {
            cue.audioTriggerChannel.OnEventRaised += cue.HandlePlayAudio;
        }
    }

    private void OnDisable() {
        foreach (var cue in cues) {
            cue.audioTriggerChannel.OnEventRaised -= cue.HandlePlayAudio;
        }
    }
}
