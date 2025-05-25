using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Main Actors")] 
    [SerializeField]
    private GameObject playerPrefab;

    [Header("Game Configuration")]
    [SerializeField]
    private Vector3 playerSpawnPosition;
    
    [Header("Event Listeners")]
    [SerializeField]
    private VoidEventChannel initGameChannel;
    [SerializeField] 
    private VoidEventChannel endGameChannel;
    [SerializeField] 
    private VoidEventChannel exitApplicationChannel;

    [Header("Event Invokers")]
    [SerializeField]
    private BoolEventChannel toggleMainMenuChannel;
    [SerializeField] 
    private BoolEventChannel togglePauseMenuChannel;
    
    // Event Actions
    public static event Action OnPlayerSpawned;
    
    private GameObject _playerInstance;
    private bool _gameInitialized = false;
    
    /// <summary>
    /// Makes pause menu visible or not
    /// </summary> 
    private void TogglePauseMenuVisibility(bool visible) {
        if (!_gameInitialized) return;
        togglePauseMenuChannel.RaiseEvent(visible);
    }
    
    /// <summary>
    /// Makes main menu visible or not
    /// </summary>
    private void ToggleMainMenuVisibility(bool visible) {
        toggleMainMenuChannel.RaiseEvent(visible);
    }

    [ContextMenu("Force Close Game")]
    private void ExitApplication() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    [ContextMenu("Force Init Game")]
    private void InitGame() {
        if (_gameInitialized) return;
        ToggleMainMenuVisibility(false);
        _gameInitialized = true;
        
        _playerInstance.SetActive(true);
        var characterPlayerComponent = _playerInstance.GetComponent<PlayerCharacter>();
        var controllerPlayerComponent = _playerInstance.GetComponent<PlyController>();
        ServiceLocator.SetService(characterPlayerComponent);
        ServiceLocator.SetService(controllerPlayerComponent);
        OnPlayerSpawned?.Invoke();
    }

    private void ResetInstances() {
        _gameInitialized = false;
        var player = _playerInstance.GetComponent<PlayerCharacter>();
        player.RequestSetPosition(playerSpawnPosition);
        _playerInstance.SetActive(false);
    }
    
    [ContextMenu("Force End Game")]
    private void EndGame() {
        ResetInstances();
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

    private void OnEnable() {
        // SO
        initGameChannel.OnEventRaised += InitGame;
        endGameChannel.OnEventRaised += EndGame;
        exitApplicationChannel.OnEventRaised += ExitApplication;
        //
        UIManager.OnPauseMenuToggleRequest += TogglePauseMenuVisibility;
    }

    private void OnDisable() {
        // SO
        initGameChannel.OnEventRaised -= InitGame;
        endGameChannel.OnEventRaised -= EndGame;
        exitApplicationChannel.OnEventRaised -= ExitApplication;
        //
        UIManager.OnPauseMenuToggleRequest += TogglePauseMenuVisibility;
    }
}
