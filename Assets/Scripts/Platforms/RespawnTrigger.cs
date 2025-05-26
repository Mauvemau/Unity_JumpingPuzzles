using UnityEngine;

/// <summary>
/// Respawns the player immediately after entering the trigger zone
/// </summary>
[RequireComponent(typeof(Collider))]
public class RespawnTrigger : MonoBehaviour {
    [Header("Config")]
    [SerializeField]
    private LayerMask playerLayer;

    private GameManager _gameManagerReference;
    private Collider _colliderReference;

    private void HandlePlayerTrigger(PlayerCharacter player) {
        if (!player) return;
        if (!ServiceLocator.TryGetService<GameManager>(out var gameManager)) return;
        gameManager.RespawnPlayer();
    }

    private void OnTriggerEnter(Collider collision) {
        var other = collision.gameObject;
        if (((1 << other.layer) & playerLayer) == 0) return;
        var player = other.GetComponent<PlayerCharacter>();
        HandlePlayerTrigger(player);
    }

    private void Awake() {
        _colliderReference = GetComponent<Collider>();
        _colliderReference.isTrigger = true;
    }

    private void OnValidate() {
        if (playerLayer != 0) return;
        var playerLayerIndex = LayerMask.NameToLayer("Player");
        if (playerLayerIndex != -1)
        {
            playerLayer = 1 << playerLayerIndex;
        }
    }
}
