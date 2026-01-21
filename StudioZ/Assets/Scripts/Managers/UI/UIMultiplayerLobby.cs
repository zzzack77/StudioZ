using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class UIMultiplayerLobby : NetworkBehaviour
{
    [SerializeField] private SimpleMatchmaking simpleMatchmaking;
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
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Wait until the SimpleMatchmaking singleton is ready
        if (SimpleMatchmaking.Instance != null)
        {
            // Subscribe to the event
            SimpleMatchmaking.Instance.OnLobbyPlayersUpdated += UpdatePlayerList;
        }
        ClearUI();
        
        
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
        if (simpleMatchmaking&&simpleMatchmaking.IsHost)
        {
            HideLobbyUIServerRpc();
        }
    } 
    
    
    private void UpdateHostUI()
    {
        if (!hostOnlyButton  || !simpleMatchmaking) return;
        
        hostOnlyButton.SetActive(simpleMatchmaking.IsHost);
    }
    
    
    [ClientRpc]
    private void HideLobbyUIClientRpc()
    {
        HideLobbyUI();
    }

    [ServerRpc(RequireOwnership = false)]
    public void HideLobbyUIServerRpc()
    {
        HideLobbyUIClientRpc();
    }
    
    public void HideLobbyUI()
    {
        lobbyUIRoot.SetActive(false);
    }
    
   
}
