using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Services.Lobbies.Models;


public class UIPublicLobby : MonoBehaviour
{
    private SimpleMatchmaking simpleMatchmaking;
    private VisualElement root;
    private bool controllerActive = false;
    private bool mouseActive = false;

    private VisualElement player1VE;
    private VisualElement player2VE;
    private VisualElement player3VE;
    private VisualElement player4VE;
    private Label player1Label;
    private Label player2Label;
    private Label player3Label;
    private Label player4Label;

    private Button backButton;
    private TextField textInput;
    private Button startButton;
    private void OnEnable()
    {
        
        SimpleMatchmaking.Instance.CreateOrJoinLobby();

        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        player1VE = root.Q<VisualElement>("1VE");
        player2VE = root.Q<VisualElement>("2VE");
        player3VE = root.Q<VisualElement>("3VE");
        player4VE = root.Q<VisualElement>("4VE");

        player1Label = root.Q<Label>("1Name");
        player2Label = root.Q<Label>("2Name");
        player3Label = root.Q<Label>("3Name");
        player4Label = root.Q<Label>("4Name");

        backButton = root.Q<Button>("BackButton");
        textInput = root.Q<TextField>("TextInput");
        startButton = root.Q<Button>("StartButton");

        startButton.clicked += StartMatch;

        //backButton.clicked += BackButtonPress;
        //enterButton.clicked += EnterButtonPress;


        //ClearUI();

        // Wait until the SimpleMatchmaking singleton is ready
        if (SimpleMatchmaking.Instance != null)
        {
            // Subscribe to the event
            SimpleMatchmaking.Instance.OnLobbyPlayersUpdated += UpdatePlayerList;
        }

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
        Debug.Log("Updating Player List in UI");
        ClearUI();

        for (int i = 0; i < players.Count; i++)
        {

            switch (i)
            {
                case 0:
                    player1Label.text = players[i].Data["Name"].Value;
                    player1VE.visible = true;
                    break;
                case 1:
                    player2Label.text = players[i].Data["Name"].Value;
                    player2VE.visible = true;
                    break;
                case 2:
                    player3Label.text = players[i].Data["Name"].Value;
                    player3VE.visible = true;
                    break;
                case 3:
                    player4Label.text = players[i].Data["Name"].Value;
                    player4VE.visible = true;
                    break;
            }
        }
    }

    private void ClearUI()
    {
        player1Label.text = "";
        player2Label.text = "";
        player3Label.text = "";
        player4Label.text = "";

        player1VE.visible = false;
        player2VE.visible = false;
        player3VE.visible = false;
        player4VE.visible = false;


        UpdateHostUI();
    }



    public void StartMatch()
    {
        //GameManager.Instance.StartMatch();
        
        GameManager.Instance.RequestLoadLevel(1);
        MPLobbyManager.Instance.StartPublicMatch();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartMatchServerRpc()
    {
        StartMatchInternal();
    }

    private void StartMatchInternal()
    {
        // This runs on server only
        Debug.Log("Server starting match");

        HideLobbyUIClientRpc();
    }

    [ClientRpc]
    private void HideLobbyUIClientRpc()
    {
        Debug.Log($"Hiding Lobby UI | ClientId: {NetworkManager.Singleton.LocalClientId}");

        HideLobbyUI();
    }

    private void HideLobbyUI()
    {
        Debug.Log("Hiding Lobby UI (Local)");
        // gameObject.SetActive(false);
    }

    private void UpdateHostUI()
    {
        if (NetworkManager.Singleton == null) return;

        startButton.visible = NetworkManager.Singleton.IsHost;
    }

}
