using System;
using TMPro;
using UnityEngine;


/// <summary>
/// Updates a TMP text with a ScriptableObject event
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class HudTextUpdate : MonoBehaviour {
    [Header("Event Listeners")]
    [SerializeField] private StringEventChannel textUpdateChannel;

    private TextMeshProUGUI _textDisplay;
    
    private void UpdateText(string text) {
        _textDisplay.text = text;
    }
    
    private void Awake() {
        if (!textUpdateChannel) {
            Debug.LogWarning($"{name} event listener not configured!");
        }
        _textDisplay = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        if (textUpdateChannel) {
            textUpdateChannel.OnEventRaised += UpdateText;
        }
    }

    private void OnDisable() {
        if (textUpdateChannel) {
            textUpdateChannel.OnEventRaised -= UpdateText;
        }
    }
}
