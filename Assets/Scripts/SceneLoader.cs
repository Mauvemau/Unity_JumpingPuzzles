using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneData
{
#if UNITY_EDITOR
    [SerializeField]
    private UnityEditor.SceneAsset sceneAsset;
#endif
    [HideInInspector]
    [SerializeField] // Es necesario para que se guarde el valor para que funcione en build
    private int sceneIndex;

    public int Index => sceneIndex;

    /// <summary>
    /// OnValidate corre en editor y no en runtime y se ejecuta con cada cambio que se haga en editor
    /// </summary>
    public void OnValidate() // Not called by Engine, needs to be called manually
    {
#if UNITY_EDITOR
        sceneIndex = SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(sceneAsset));
#endif
    }
}

public class SceneLoader : MonoBehaviour {
#if UNITY_EDITOR
    [SerializeField]
    private UnityEditor.SceneAsset sceneAsset;
#endif
    [SerializeField] // Es necesario para que se guarde el valor para que funcione en build
    [HideInInspector]
    private int sceneIndex;

    AsyncOperation _loadOperation;

    [ContextMenu("Load New Scene")]
    private void LoadNewScene()
    {
        SceneManager.LoadScene(sceneIndex); // Carga una nueva esceana y destruye la anterior
    }

    [ContextMenu("Load Scene Additive")]
    private void LoadSceneAdditive()
    {
        _loadOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive); // Carga una nueva escena asincronicamente y no destruye la anterior
        if(_loadOperation != null)
        {
            _loadOperation.allowSceneActivation = false;
        }
    }

    [ContextMenu("Activate Loaded Scene")]
    private void ActivateLoadedScene()
    {
        if (_loadOperation != null)
        {
            _loadOperation.allowSceneActivation = true;
        }
    }

    [ContextMenu("Unload Scene")]
    private void UnloadMyScene()
    {
        SceneManager.UnloadSceneAsync(sceneIndex);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// OnValidate corre en editor y no en runtime y se ejecuta con cada cambio que se haga en editor
    /// </summary>
    private void OnValidate()
    {
#if UNITY_EDITOR
        sceneIndex = SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(sceneAsset));
#endif
    }
}
