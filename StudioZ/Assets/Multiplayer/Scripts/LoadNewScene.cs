using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewScene : MonoBehaviour
{
    [SerializeField] private string[] scenes;
  
    
    
    public void SwitchScene(string sceneName)
    {
        // Only the host can change scenes in NGO
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(scenes[0], LoadSceneMode.Single);
        }
    }
}
