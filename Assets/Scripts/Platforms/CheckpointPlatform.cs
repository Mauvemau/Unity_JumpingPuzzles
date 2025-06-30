using System;
using UnityEngine;

/// <summary>
/// Turns an object with collision into a player character checkpoint
/// </summary>
public class CheckpointPlatform : CollisionInteractable {
    [Header("Offset Config")] 
    [SerializeField]
    private Vector3 respawnOffset;
    
    protected override void HandleCollision(GameObject other) {
        if (!ServiceLocator.TryGetService<GameManager>(out var gameManager)) return;
        var spawnPosition = respawnOffset;
        if (spawnPosition == Vector3.zero) {
            spawnPosition = transform.position;
            spawnPosition.y = transform.position.y + 2;
        }
        gameManager.SetPlayerRespawnPosition(spawnPosition);
    }

    protected override void HandleTrigger(GameObject other) { }
}
