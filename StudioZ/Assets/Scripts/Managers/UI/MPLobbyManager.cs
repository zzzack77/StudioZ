using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

public class MPLobbyManager : NetworkBehaviour
{
    public static MPLobbyManager Instance;

    [SerializeField] GameObject lobbyUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
    public void StartPublicMatch()
    {
        if (!IsServer)
        {
            Debug.Log("Only the host can start the match!");
            return;
        }
        HideLobbyUIClientRpc();
    }
    [ClientRpc]
    private void HideLobbyUIClientRpc()
    {
        Debug.Log($"Hiding Lobby UI | ClientId: {NetworkManager.Singleton.LocalClientId}");
        lobbyUI.SetActive(false);
    }
}
