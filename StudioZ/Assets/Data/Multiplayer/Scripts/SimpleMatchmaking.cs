using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;
using ParrelSync;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class SimpleMatchmaking : MonoBehaviour
{
    [SerializeField] private GameObject buttons;

    private Lobby connectedLoby;
    private QueryResponse lobbies;
    private UnityTransport transport;
    private const string JoinCodeKey = "j";
    private string playerId;
    
   
    void Awake() => transport = FindObjectOfType<UnityTransport>();

    public async void CreateOrJoinLobby()
    {
        await Authenticate();

        connectedLoby = await QuickJoinLobby() ?? await CreateLobby();
        
        if(connectedLoby != null) buttons.SetActive(false);
    }

    private async Task Authenticate()
    {
        var options = new InitializationOptions();
#if UNITY_EDITOR
        options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif        
        await UnityServices.InitializeAsync(options);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        playerId = AuthenticationService.Instance.PlayerId;
    }

    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {

            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            SetTransfromAsClient(a);

            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch (Exception e)
        {
            Debug.Log("no lobbies available via quick join");
            return null;
        }
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            const int maxPlayers = 10;

            var a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            var options = new CreateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>
                    { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };

            var lobby = await LobbyService.Instance.CreateLobbyAsync("localhost", maxPlayers, options);

            StartCoroutine(HeartBeatLobbyCoroutine(lobby.Id, 15));
            transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key,
                a.ConnectionData);

            NetworkManager.Singleton.StartHost();
            return lobby;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to create lobby");
            return null;
        }
    }

    private void SetTransfromAsClient(JoinAllocation a)
    {
        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key,a.ConnectionData,a.HostConnectionData);
    }

    private static IEnumerator HeartBeatLobbyCoroutine(string lobbyId, int waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private void OnDestroy()
    {
        try
        {
            StopAllCoroutines();

            if (connectedLoby != null)
            {
                if (connectedLoby.HostId == playerId) LobbyService.Instance.DeleteLobbyAsync(connectedLoby.Id);
                else LobbyService.Instance.RemovePlayerAsync(connectedLoby.Id, playerId);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"error shutting down lobby:  {e}");
        }
        
    }
}
