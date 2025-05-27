using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextPromptPanel : MonoBehaviour {
    [Header("Config")] 
    [SerializeField]
    private float promptDuration = 8f;
    [Header("Event Listeners")]
    [SerializeField]
    private StringEventChannel textPromptPanelEventChannel;
    
    private Coroutine _activePromptCoroutine;
    private TextMeshProUGUI _textDisplay;
    private Image _background;

    private IEnumerator HandlePrompt(string text) {
        if (_background) {
            _background.enabled = true;
        }
        _textDisplay.text = text;
        
        yield return new WaitForSeconds(promptDuration);

        if (_background) {
            _background.enabled = false;
        }
        _textDisplay.text = "";
    }

    private void ShowPrompt(string text) {
        if (_activePromptCoroutine != null) {
            StopCoroutine(_activePromptCoroutine);
        }
        _activePromptCoroutine = StartCoroutine(HandlePrompt(text));
    }
    
    private void Awake() {
        if (!textPromptPanelEventChannel) {
            Debug.LogWarning($"{name} event listener not configured!");
        }
        _textDisplay = GetComponentInChildren<TextMeshProUGUI>();
        if (!_textDisplay) {
            Debug.LogError($"{name}: TextMeshProUGUI component not found in children!");
        }
        else {
            _textDisplay.text = "";
        }
        _background = GetComponent<Image>(); // We don't really mind that much if it has one or not
        if (_background) {
            _background.enabled = false;
        }
    }

    private void OnEnable() {
        if (textPromptPanelEventChannel)
            textPromptPanelEventChannel.OnEventRaised += ShowPrompt;
    }

    private void OnDisable() {
        if (textPromptPanelEventChannel)
            textPromptPanelEventChannel.OnEventRaised -= ShowPrompt;
    }
}
