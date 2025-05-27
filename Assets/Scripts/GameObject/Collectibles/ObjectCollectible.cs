using System;
using UnityEngine;

public enum CollectibleType {
    MainObjective, 
    Bonus
}

[RequireComponent(typeof(Collider))]
public class ObjectCollectible : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private CollectibleType type = CollectibleType.MainObjective;
    [SerializeField] private LayerMask playerLayer;

    private CollectibleManager _collectibleManagerReference;
    private Collider _colliderReference;
    private bool _triggered = false; // Prevent from triggering more than once.

    private void HandlePlayerTrigger() {
        if (!_collectibleManagerReference) return;
        
        _collectibleManagerReference.OnCollectibleCollected(type);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision) {
        var other = collision.gameObject;
        if (((1 << other.layer) & playerLayer) == 0) return;
        if (_triggered) return;
        _triggered = true;
        HandlePlayerTrigger();
    }

    private void OnGameStarted() {
        if (!ServiceLocator.TryGetService<CollectibleManager>(out var collectibleManager)) {
            Debug.LogWarning($"{name}: Unable to find collectible manager in Service Locator!");
            return;
        }
        _collectibleManagerReference = collectibleManager;
        _collectibleManagerReference.OnCollectibleSpawned(type);
    }
    
    private void Awake() {
        _colliderReference = GetComponent<Collider>();
        _colliderReference.isTrigger = true;
    }

    private void OnValidate() {
        if (playerLayer != 0) return;

        var playerLayerIndex = LayerMask.NameToLayer("Player");
        if (playerLayerIndex != -1) {
            playerLayer = 1 << playerLayerIndex;
        }
    }

    private void OnEnable() {
        GameManager.OnPlayerSpawned += OnGameStarted;
    }

    private void OnDisable() {
        GameManager.OnPlayerSpawned -= OnGameStarted;
    }
}
