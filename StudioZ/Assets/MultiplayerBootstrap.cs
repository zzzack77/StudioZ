using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerBootstrap : MonoBehaviour
{
    

    public async void OnjoinButtonPressed()
    { 
        SceneManager.LoadScene("EasyLobbyScene");
        SimpleMatchmaking.Instance.CreateOrJoinLobby();
        
    }
    
    
    public void ChangePlayerName(string newName)
    {
        SimpleMatchmaking.Instance.ChangePlayerName(newName);
    }
}
