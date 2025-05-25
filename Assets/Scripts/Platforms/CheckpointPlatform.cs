using System;
using UnityEngine;

/// <summary>
/// Turns an object with collision into a player character checkpoint
/// </summary>
[RequireComponent(typeof(Collider))]
public class CheckpointPlatform : MonoBehaviour {
    [Header("Config")] 
    [SerializeField]
    private Vector3 respawnOffset;
    [SerializeField] 
    private LayerMask playerLayer;
    
    private Collider _colliderReference;
    
    private void HandlePlayerCollision(PlayerCharacter player) {
        if (!player) return;
        if (!ServiceLocator.TryGetService<GameManager>(out var gameManager)) return;
        var spawnPosition = respawnOffset;
        if (spawnPosition == Vector3.zero) {
            spawnPosition = transform.position;
            spawnPosition.y = transform.position.y + 2;
        }
        gameManager.SetPlayerRespawnPosition(spawnPosition);
    }
    
    private void OnCollisionEnter(Collision collision) {
        var other = collision.gameObject;
        if (((1 << other.layer) & playerLayer) == 0) return;
        var player = other.GetComponent<PlayerCharacter>();
        HandlePlayerCollision(player);
    }
    
    private void Awake() {
        _colliderReference = GetComponent<Collider>();
    }

    private void OnValidate() {
        if (playerLayer != 0) return;
        var playerLayerIndex = LayerMask.NameToLayer("Player");
        if (playerLayerIndex != -1) {
            playerLayer = 1 << playerLayerIndex;
        }
    }
}
