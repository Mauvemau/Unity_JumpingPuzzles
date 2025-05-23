using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Main Actors")] 
    [SerializeField]
    private GameObject playerPrefab;

    [Header("Game Configuration")]
    [SerializeField]
    private Vector3 playerSpawnPosition;
    
    private GameObject _playerInstance;

    public static event Action OnPlayerSpawned;

    [ContextMenu("Force Init Game")]
    public void InitGame() {
        _playerInstance.SetActive(true);
        var characterPlayerComponent = _playerInstance.GetComponent<PlayerCharacter>();
        var controllerPlayerComponent = _playerInstance.GetComponent<PlyController>();
        ServiceLocator.SetService(characterPlayerComponent);
        ServiceLocator.SetService(controllerPlayerComponent);
        OnPlayerSpawned?.Invoke();
    }

    private void LoadInstances() {
        _playerInstance = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        _playerInstance.SetActive(false);
    }
    
    private void Awake() {
        if (!playerPrefab) {
            Debug.LogError($"{name}: There is no player player prefab assigned!");
            return;
        }
        LoadInstances();
    }
}
