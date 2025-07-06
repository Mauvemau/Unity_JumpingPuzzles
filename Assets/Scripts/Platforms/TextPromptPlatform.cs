using UnityEngine;

/// <summary>
/// A platform that invokes a text event channel when touched by the player
/// </summary>
public class TextPromptPlatform : CollisionInteractable {
    [Header("Text")] 
    [SerializeField] private TextContainer textToDisplay;
    
    [Header("Event Invokers")] 
    [SerializeField] private StringEventChannel textPromptPanelEventChannel;

    protected override void HandleCollision(GameObject other) {
        if (!textPromptPanelEventChannel) return;
        textPromptPanelEventChannel.RaiseEvent(textToDisplay.Text);
    }

    protected override void HandleTrigger(GameObject other) { }
}
