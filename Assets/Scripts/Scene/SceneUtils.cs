using UnityEngine.SceneManagement;

public static class SceneUtils {
    /// <summary>
    /// Verifies if a scene is loaded
    /// </summary>
    public static bool IsSceneLoaded(string sceneName)
    {
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Verifies if a scene is loaded
    /// </summary>
    public static bool IsSceneLoaded(int sceneBuildIndex)
    {
        var scene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
        return scene.isLoaded;
    }
}
