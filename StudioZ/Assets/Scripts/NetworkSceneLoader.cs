using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneLoader : NetworkBehaviour
{
    public void LoadSceneForAll(string sceneName)
    {
        if (!IsServer)
        {
            return;
        }

        NetworkManager.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void UnloadSceneForAll(string sceneName)
    {
        if (!IsServer)
            return;

        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.IsValid())
        {
            NetworkManager.SceneManager.UnloadScene(scene);
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' is not loaded, cannot unload.");
        }
    }
}
