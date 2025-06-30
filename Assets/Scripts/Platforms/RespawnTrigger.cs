using UnityEngine;

/// <summary>
/// Respawns the player immediately after entering the trigger zone
/// </summary>
public class RespawnTrigger : CollisionInteractable {

    protected override void HandleCollision(GameObject other) { }

    protected override void HandleTrigger(GameObject other) {
        if (!ServiceLocator.TryGetService<GameManager>(out var gameManager)) return;
        gameManager.RespawnPlayer();
    }
}
