using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Main Actors")] 
    [SerializeField]
    private GameObject playerPrefab;

    [Header("Game Configuration")]
    [SerializeField]
    private Vector3 defaultPlayerSpawnPosition;
    
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
    private Vector3 _currentPlayerSpawnPosition;
    private bool _gameInitialized = false;
    
    public void RespawnPlayer() {
        var player = _playerInstance.GetComponent<PlayerCharacter>();
        player.RequestSetPosition(_currentPlayerSpawnPosition);
    }
    
    /// <summary>
    /// Changes the position of current player spawn position
    /// </summary>
    public void SetPlayerRespawnPosition(Vector3 position) {
        _currentPlayerSpawnPosition = position;
    }
    
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
        var controllerPlayerComponent = _playerInstance.GetComponent<PlayerCharacterController>();
        ServiceLocator.SetService(characterPlayerComponent);
        ServiceLocator.SetService(controllerPlayerComponent);
        OnPlayerSpawned?.Invoke();
    }

    private void ResetInstances() {
        _gameInitialized = false;
        _currentPlayerSpawnPosition = defaultPlayerSpawnPosition;
        var player = _playerInstance.GetComponent<PlayerCharacter>();
        player.RequestSetPosition(defaultPlayerSpawnPosition);
        _playerInstance.SetActive(false);
    }
    
    [ContextMenu("Force End Game")]
    private void EndGame() {
        ResetInstances();
    }
    
    private void LoadInstances() {
        _currentPlayerSpawnPosition = defaultPlayerSpawnPosition;
        _playerInstance = Instantiate(playerPrefab, _currentPlayerSpawnPosition, Quaternion.identity);
        _playerInstance.SetActive(false);
    }
    
    private void Awake() {
        if (!playerPrefab) {
            Debug.LogError($"{name}: There is no player player prefab assigned!");
            return;
        }
        ServiceLocator.SetService(this);
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
