using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class UIMultiplayerLobby : MonoBehaviour
{
   
    [SerializeField] private GameObject lobbyUIRoot;
    [Header("UI References")] 
    [SerializeField] private TextMeshProUGUI Player1Text;
    [SerializeField] private TextMeshProUGUI Player2Text;
    [SerializeField] private TextMeshProUGUI Player3Text;
    [SerializeField] private TextMeshProUGUI Player4Text;
    
    [SerializeField] private GameObject Player1;
    [SerializeField] private GameObject Player2;
    [SerializeField] private GameObject Player3;
    [SerializeField] private GameObject Player4;
    
    [SerializeField] private GameObject hostOnlyButton;

    [SerializeField] private GameObject JoinCodeUI;
    [SerializeField] private TextMeshProUGUI JoinCode;
    
    [Header("Scene Reference")] 
    [SerializeField] private string gameScene;
    
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
  
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            SimpleMatchmaking.Instance.ForceLobbyRefresh();
        }
        ClearUI();
        if (SimpleMatchmaking.Instance != null)
        {
            // Subscribe to the event
            SimpleMatchmaking.Instance.OnLobbyPlayersUpdated += UpdatePlayerList;
        }
        
        if (SimpleMatchmaking.Instance != null)
        {
            // Listen for the join event
            SimpleMatchmaking.Instance.OnLobbyJoined += HandleLobbyJoined;
        
            // Check if we are ALREADY in a lobby (returning from game)
            if (SimpleMatchmaking.Instance.ConnectedLobby != null)
            {
                HandleLobbyJoined();
            }
           
        }
        JoinCodeUI.SetActive(SimpleMatchmaking.Instance.ConnectedLobby.IsPrivate);
        JoinCode.text = SimpleMatchmaking.Instance.ConnectedLobby.LobbyCode;
        
    }
    
    private void OnDestroy()
    {
        // Clean up subscription to prevent errors
        if (SimpleMatchmaking.Instance != null)
        {
            SimpleMatchmaking.Instance.OnLobbyPlayersUpdated -= UpdatePlayerList;
        }
    }
  
    private void UpdatePlayerList(List<Player> players)
    {
        ClearUI();
        
  
        for (int i = 0; i < players.Count; i++)
        {

            switch (i)
            {
                case 0: 
                    Player1Text.text = players[i].Data["Name"].Value;
                    Player1.SetActive(true);
                    break;
                case 1: 
                    Player2Text.text = players[i].Data["Name"].Value;
                    Player2.SetActive(true);
                    break;
                case 2: 
                    Player3Text.text = players[i].Data["Name"].Value;
                    Player3.SetActive(true);
                    break; 
                case 3: 
                    Player4Text.text = players[i].Data["Name"].Value;
                    Player4.SetActive(true);
                    break;
            } }
    }

    private void ClearUI()
    {
        Player1Text.text = "";
        Player2Text.text = "";
        Player3Text.text = "";
        Player4Text.text = "";
        
        Player1.SetActive(false);
        Player2.SetActive(false);
        Player3.SetActive(false);
        Player4.SetActive(false);


        UpdateHostUI();
        
    }

    

    public void StartMatch()
    {
        if (SimpleMatchmaking.Instance != null && SimpleMatchmaking.Instance.IsHost)
        {
            
            SimpleMatchmaking.Instance.LockLobby();
            NetworkManager.Singleton.SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
        }
    } 
    
    
    private void UpdateHostUI()
    {
        if (!hostOnlyButton || NetworkManager.Singleton == null) return;

        hostOnlyButton.SetActive(NetworkManager.Singleton.IsHost);
        
    }
    
    private void HandleLobbyJoined()
    {
        StartCoroutine(WaitForNetworkStart());
    }
    
    
    
    public void HideLobbyUI()
    {
        lobbyUIRoot.SetActive(false);
    }
    
    private IEnumerator WaitForNetworkStart()
    {
        yield return new WaitUntil(() =>
            NetworkManager.Singleton != null &&
            (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost));

        
        lobbyUIRoot.SetActive(true);
        SimpleMatchmaking.Instance.ForceLobbyRefresh();
    }
}
