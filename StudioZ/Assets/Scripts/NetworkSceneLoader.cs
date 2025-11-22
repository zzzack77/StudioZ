using Unity.Netcode;
using UnityEngine;

public class NetworkSceneLoader : NetworkBehaviour
{
    public void LoadSceneForAll(string sceneName)
    {
        if (!IsServer)
        {
            return;
        }

        NetworkManager.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
