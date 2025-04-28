using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour {

    [SerializeField]
    private string sceneName;

    [ContextMenu("Load New Scene")]
    private void LoadNewScene()
    {
        SceneManager.LoadScene(sceneName); // Carga una nueva esceana y destruye la anterior
    }

    [ContextMenu("Load Scene Additive")]
    private void LoadSceneAdditive()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive); // Carga una nueva escena y no destruye la anterior
    }


    [ContextMenu("Unload Scene")]
    private void UnloadMyScene()
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
