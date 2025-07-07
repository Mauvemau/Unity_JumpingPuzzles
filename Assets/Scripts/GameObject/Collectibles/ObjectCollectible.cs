using UnityEngine;

public enum CollectibleType {
    MainObjective, 
    Bonus
}

public class ObjectCollectible : CollisionInteractable {
    [Header("Config")]
    [SerializeField] private CollectibleType type = CollectibleType.MainObjective;

    [Header("Sound Effects")] 
    [SerializeField] private AudioClip2DContainer pickupSfx;
    
    private ICollectibleManager _collectibleManagerReference;
    private Collider _colliderReference;
    private MeshRenderer _meshRendererReference;
    private bool _triggered = false; // Prevent from triggering more than once.

    private void HandleTriggerLogic() {
        _collectibleManagerReference?.OnCollectibleCollected(type);
        
        if (pickupSfx) {
            pickupSfx.PlayAudioClip();
        }
        
        if (_meshRendererReference) {
            _meshRendererReference.enabled = false;
        }
        else {
            Debug.LogWarning($"{name}: Doesn't have any mesh renderer!");
        }
    }
    
    protected override void HandleCollision(GameObject other) {}

    protected override void HandleTrigger(GameObject other) {
        if (_triggered) return;
        _triggered = true;
        HandleTriggerLogic();
    }

    private void OnGameStarted() {
        _meshRendererReference.enabled = true;
        _triggered = false;
        if (!ServiceLocator.TryGetService<ICollectibleManager>(out var collectibleManager)) {
            Debug.LogWarning($"{name}: Unable to find collectible manager in Service Locator!");
            return;
        }
        _collectibleManagerReference = collectibleManager;
        _collectibleManagerReference?.OnCollectibleSpawned(type);
    }
    
    private void Awake() {
        _colliderReference = GetComponent<Collider>();
        _colliderReference.isTrigger = true;
        TryGetComponent(out _meshRendererReference);
    }

    private void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
    }

    private void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
    }
}
