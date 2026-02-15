using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (UI)
        {
            UI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // for testing
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (UI)
            {
                UI.SetActive(true);
            }
        }
    }

    public void LeaveMultiplayerGame()
    {
        if (SimpleMatchmaking.Instance != null)
        {
            LeaveLobbyAndLoadScene();
        }
        else 
        {
            if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
        }
    }
    
    public async void LeaveLobbyAndLoadScene()
    {
        await SimpleMatchmaking.Instance.LeaveGame();

        
        
       
    }

    public void HostReturnToLobby()
    {
        if (NetworkManager.Singleton == null) return;
        // change scene to final multiplayer lobby scene
        NetworkManager.Singleton.SceneManager.LoadScene("EasyLobbyScene", LoadSceneMode.Single);
        SimpleMatchmaking.Instance.UnlockLobby();
        
        
    }

    public void Resume()
    {
        if (UI)
        {
            UI.SetActive(false);
        }
    }

    public void Settings()
    {
        //settings code
    }
}
