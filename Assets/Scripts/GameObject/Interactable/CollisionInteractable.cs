using UnityEngine;

/// <summary>
/// Provides functionality upon collision (or trigger) between this object and another object or character
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class CollisionInteractable : MonoBehaviour {
    [Header("Layering Config")]
    [SerializeField]
    private LayerMask collisionFilter;

    protected abstract void HandleCollision(GameObject other);
    protected abstract void HandleTrigger(GameObject other);

    private void OnCollisionEnter(Collision collision) {
        var other = collision.gameObject;
        if (((1 << other.layer) & collisionFilter) == 0) return;
        HandleCollision(other);
    }

    private void OnTriggerEnter(Collider collision) {
        var other = collision.gameObject;
        if (((1 << other.layer) & collisionFilter) == 0) return;
        HandleTrigger(other);
    }

    private void Awake() {
        if(collisionFilter < 1) {
            Debug.LogWarning($"{name}: Layer filter for platform is not configured!");
        }
    }

    private void OnValidate() {
        if (collisionFilter != 0) return; // Defaults to Player if unset
        var playerLayerIndex = LayerMask.NameToLayer("Player");
        if (playerLayerIndex != -1) {
            collisionFilter = 1 << playerLayerIndex;
        }
    }
}
