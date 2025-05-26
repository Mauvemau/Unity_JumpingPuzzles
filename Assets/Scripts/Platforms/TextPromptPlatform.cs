using UnityEngine;

/// <summary>
/// A platform that invokes a text event channel when touched by the player
/// </summary>
[RequireComponent(typeof(Collider))]
public class TextPromptPlatform : MonoBehaviour {
    [Header("Text")] 
    [SerializeField] 
    private TextContainer textToDisplay;
    
    [Header("Config")]
    [SerializeField] 
    private LayerMask playerLayer;
    
    [Header("Event Invokers")] 
    [SerializeField]
    private StringEventChannel textPromptPanelEventChannel;

    private void HandlePlayerCollision() {
        if (!textPromptPanelEventChannel) return;
        textPromptPanelEventChannel.RaiseEvent(textToDisplay.Text);
    }
    
    private void OnCollisionEnter(Collision collision) {
        var other = collision.gameObject;
        if (((1 << other.layer) & playerLayer) == 0) return;
        HandlePlayerCollision();
    }
    
    private void Awake() {
        if (!textToDisplay) {
            Debug.LogWarning($"{name}: Text is not configured!");
        }
        if (!textPromptPanelEventChannel) {
            Debug.LogWarning($"{name}: Event channel not configured!");
        }
    }

    private void OnValidate() {
        if (playerLayer != 0) return;
        var playerLayerIndex = LayerMask.NameToLayer("Player");
        if (playerLayerIndex != -1) {
            playerLayer = 1 << playerLayerIndex;
        }
    }
}
