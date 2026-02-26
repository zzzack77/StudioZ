using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerBootstrap : MonoBehaviour
{
    private string privateCode;

    public async void OnJoinButtonPressed()
    { 
        await SimpleMatchmaking.Instance.CreateOrJoinLobby();
        
        NetworkManager.Singleton.SceneManager.LoadScene("easyLobbyScene",  LoadSceneMode.Single);
    }
    
    
    public void ChangePlayerName(string newName)
    {
        Debug.Log(privateCode);
        SimpleMatchmaking.Instance.ChangePlayerName(newName);
    }
    
    
    public async void OnPrivateJoinButtonPressed()
    { 
        
        if (privateCode.Length > 0)
        {
            if ( await SimpleMatchmaking.Instance.JoinPrivateLobbyWithCode(privateCode))
            {
                NetworkManager.Singleton.SceneManager.LoadScene("easyLobbyScene",  LoadSceneMode.Single);
            }
            else
            {
                // Incorrect lobby code logic
                Debug.Log("No game with code: " +privateCode);
            }
            
        }
        
        
    }
    
    public async void OnPrivateHostButtonPressed()
    { 
        
        
       await SimpleMatchmaking.Instance.CreatePrivateLobby();
        
       NetworkManager.Singleton.SceneManager.LoadScene("easyLobbyScene",  LoadSceneMode.Single);
    }

    public void SetPrivateCode(string code)
    {
        privateCode = code;
        Debug.Log(privateCode);
    }
}
