using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneData {
#if UNITY_EDITOR
    [SerializeField]
    private SceneAsset sceneAsset;
#endif
    [SerializeField]
    [HideInInspector]
    private int sceneIndex;

    public int Index => sceneIndex;
    
    public void OnValidate() { // Not called by Engine, needs to be called manually
#if UNITY_EDITOR
        sceneIndex = SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(sceneAsset));
#endif
    }
}

public class MySceneManager : MonoBehaviour {
#if UNITY_EDITOR
    [SerializeField]
    private List<SceneData> loadOnBoot = new List<SceneData>();
#endif

    private readonly List<AsyncOperation> _loadOperations = new List<AsyncOperation>();

    // In case we want to make a loading screen for the boot load later
    public static event Action OnStartLoadingScenes;
    public static event Action OnFinishedLoadingScenes;

    private IEnumerator LoadBootScenes() {
        OnStartLoadingScenes?.Invoke();

        _loadOperations.Clear();

#if UNITY_EDITOR
        foreach (var sceneData in loadOnBoot) {
            var index = sceneData.Index;
            if (index >= 0 && !SceneUtils.IsSceneLoaded(index)) {
                var op = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
                if (op != null) {
                    _loadOperations.Add(op);
                }
            }
        }
#endif

        foreach (var op in _loadOperations) {
            while (!op.isDone) {
                yield return null;
            }
        }

        OnFinishedLoadingScenes?.Invoke();
    }

    public void LoadNewScene(int sceneIndex) {
        if (sceneIndex >= 0) {
            SceneManager.LoadScene(sceneIndex);
        } else {
            Debug.LogError("Invalid scene index.");
        }
    }

    public void LoadSceneAdditive(int sceneIndex) {
        if (sceneIndex >= 0) {
            var op = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            if (op == null) return;
            _loadOperations.Add(op);
            op.allowSceneActivation = false;
        } else {
            Debug.LogError("Invalid scene index.");
        }
    }

    public void ActivateLoadedScene() {
        foreach (var op in _loadOperations) {
            if (op != null) {
                op.allowSceneActivation = true;
            }
        }
    }

    public void UnloadScene(int sceneIndex) {
        if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded) {
            SceneManager.UnloadSceneAsync(sceneIndex);
        } else {
            Debug.LogWarning($"Scene {sceneIndex} is not loaded.");
        }
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
#if UNITY_EDITOR
        if (loadOnBoot.Count > 0) {
            StartCoroutine(LoadBootScenes());
        }
        else {
            Debug.LogWarning($"{name}: There are no scenes in {nameof(loadOnBoot)}!");
        }
#endif
    }
    
    private void OnValidate() {
#if UNITY_EDITOR
        foreach (var scene in loadOnBoot) {
            scene?.OnValidate();
        }
#endif
    }
}

