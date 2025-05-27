using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CollectibleManager : MonoBehaviour {
    [Header("Event Invokers")] 
    [SerializeField]
    private StringEventChannel collectiblesTextUpdateChannel;
    [SerializeField] 
    private StringEventChannel victoryAnnouncementPromptChannel;
    
    private readonly Dictionary<CollectibleType, int> _maxCollectibles = new Dictionary<CollectibleType, int>();
    private readonly Dictionary<CollectibleType, int> _collectedCollectibles = new Dictionary<CollectibleType, int>();
    
    private bool _victory = false;
    
    public void OnCollectibleCollected(CollectibleType type) {
        AddCollectedCollectible(type);
    }

    public void OnCollectibleSpawned(CollectibleType type) {
        AddMaxCollectible(type);
    }

    public void Clear() {
        _maxCollectibles.Clear();
        _collectedCollectibles.Clear();
        RefreshDictionaries();
    }

    private void HandleVictory() {
        if (_victory) return;
        _victory = true;
        const string victoryText = "You win!\nYou have collected all the main collectibles!";
        victoryAnnouncementPromptChannel.RaiseEvent(victoryText);
    }
    
    private void HandleCollectibleProgress() {
        var progressOutput = "\n";
        foreach (var type in _maxCollectibles.Keys)
        {
            var maxCount = _maxCollectibles[type];
            var collectedCount = _collectedCollectibles.GetValueOrDefault(type, 0);
            var progress = maxCount > 0 ? (float)collectedCount / maxCount : 0f;
            if (type == CollectibleType.MainObjective && progress >= 1f) { // Scuffed, will fix later
                HandleVictory();
            }
            progressOutput += $"{type}: {collectedCount}/{maxCount} ({progress * 100:F2}%)\n";
            
        }
        collectiblesTextUpdateChannel.RaiseEvent(progressOutput);
    }
    
    private void AddMaxCollectible(CollectibleType type) {
        if (!_maxCollectibles.ContainsKey(type)) {
            Debug.LogError($"{name}: Invalid collectible type {type}");
            return;
        }

        _maxCollectibles[type]++;
        HandleCollectibleProgress();
    }

    private void AddCollectedCollectible(CollectibleType type) {
        if (!_collectedCollectibles.ContainsKey(type)) {
            Debug.LogError($"{name}: Invalid collectible type {type}");
            return;
        }

        _collectedCollectibles[type]++;
        HandleCollectibleProgress();
    }

    private void RefreshDictionaries() {
        foreach (CollectibleType type in Enum.GetValues(typeof(CollectibleType))) {
            _maxCollectibles.TryAdd(type, 0);
            _collectedCollectibles.TryAdd(type, 0);
        }
    }
    
    private void Awake() {
        ServiceLocator.SetService(this);
        RefreshDictionaries();
    }
}
