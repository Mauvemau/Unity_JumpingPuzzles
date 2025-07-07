using System;
using UnityEngine;

public interface IGameManager {
    public void SetGamePaused(bool paused);
    /// <summary>
    /// Returns if the game is initialized and not paused
    /// </summary>
    public bool GetIsGameReady();
    /// <summary>
    /// Changes the position of current player spawn position
    /// </summary>
    public void SetPlayerRespawnPosition(Vector3 position);
    public void RespawnPlayer();
}

public class GameManager : MonoBehaviour, IGameManager {
    [Header("Main Actors")] 
    [SerializeField] private GameObject playerPrefab;

    [Header("Game Configuration")]
    [SerializeField] private Transform defaultPlayerSpawnPosition;
    
    [Header("Event Listeners")]
    [SerializeField] private VoidEventChannel requestInitGameChannel;
    [SerializeField] private VoidEventChannel requestEndGameChannel;
    [SerializeField] private VoidEventChannel requestExitApplicationChannel;
    [SerializeField] private BoolEventChannel requestPauseGame;

    [Header("Event Invokers")] 
    [SerializeField] private BoolEventChannel requestToggleHud;
    [SerializeField] private BoolEventChannel requestTogglePauseMenuChannel;
    
    // Event Actions
    public static event Action OnGameStarted = delegate {};
    /// <summary>
    /// Some classes might need to init before others
    /// </summary>
    public static event Action OnPreGameStarted = delegate {};
    public static event Action<bool> OnGamePaused = delegate {};
    
    private GameObject _playerInstance;
    private Vector3 _currentPlayerSpawnPosition;
    private bool _gameInitialized = false;
    private bool _gamePaused = false;

    public void SetGamePaused(bool paused) {
        _gamePaused = paused;
        _playerInstance.SetActive(!paused);
        requestToggleHud.RaiseEvent(!paused);
        if(requestTogglePauseMenuChannel)
            requestTogglePauseMenuChannel.RaiseEvent(paused);
        OnGamePaused?.Invoke(paused);
    }
    
    public bool GetIsGameReady() {
        return (_gameInitialized && !_gamePaused);
    }
    
    public void SetPlayerRespawnPosition(Vector3 position) {
        _currentPlayerSpawnPosition = position;
    }

    public void RespawnPlayer() {
        var player = _playerInstance.GetComponent<PlayerCharacter>();
        player.RequestSetPosition(_currentPlayerSpawnPosition);
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
        if (_gameInitialized) {
            Debug.LogError($"{name}: Tried to start a game that has already been initialized!");
            return;
        }
        _gamePaused = false;
        
        _playerInstance.SetActive(true);
        OnPreGameStarted?.Invoke();
        OnGameStarted?.Invoke();
        
        _gameInitialized = true;
        requestToggleHud.RaiseEvent(true);
    }

    [ContextMenu("Force End Game")]
    private void EndGame() {
        requestToggleHud.RaiseEvent(false);
        _gameInitialized = false;
        _gamePaused = false;
        _currentPlayerSpawnPosition = !defaultPlayerSpawnPosition ? Vector3.zero : defaultPlayerSpawnPosition.position;
        RespawnPlayer();
        _playerInstance.SetActive(false);
    }
    
    private void PerformInitialInit() {
        ServiceLocator.SetService<IGameManager>(this);
        _currentPlayerSpawnPosition = !defaultPlayerSpawnPosition ? Vector3.zero : defaultPlayerSpawnPosition.position;
        _playerInstance = Instantiate(playerPrefab, _currentPlayerSpawnPosition, Quaternion.identity);
        _playerInstance.SetActive(false);
        
        var characterPlayerComponent = _playerInstance.GetComponent<PlayerCharacter>();
        var controllerPlayerComponent = _playerInstance.GetComponent<PlayerCharacterController>();
        ServiceLocator.SetService<IPlayableCharacter>(characterPlayerComponent);
        ServiceLocator.SetService<ICharacterController>(controllerPlayerComponent);
    }
    
    private void Awake() {
        if (!playerPrefab) {
            Debug.LogError($"{name}: There is no player player prefab assigned!");
            return;
        }
        if (!defaultPlayerSpawnPosition) {
            Debug.LogError($"{name}: There is no default spawn position assigned! Defaulting to 0");
        }
        PerformInitialInit();
    }

    private void OnEnable() {
        if(requestInitGameChannel)
            requestInitGameChannel.OnEventRaised += InitGame;
        if(requestEndGameChannel)
            requestEndGameChannel.OnEventRaised += EndGame;
        if (requestPauseGame)
            requestPauseGame.OnEventRaised += SetGamePaused;
        
        if(requestExitApplicationChannel)
            requestExitApplicationChannel.OnEventRaised += ExitApplication;
    }

    private void OnDisable() {
        if(requestInitGameChannel)
            requestInitGameChannel.OnEventRaised -= InitGame;
        if(requestEndGameChannel)
            requestEndGameChannel.OnEventRaised -= EndGame;
        if (requestPauseGame)
            requestPauseGame.OnEventRaised -= SetGamePaused;
        
        if(requestExitApplicationChannel)
            requestExitApplicationChannel.OnEventRaised -= ExitApplication;
    }
}
