using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
using ParrelSync;
using TMPro;

#endif

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
    
   
    void Awake() => transport = FindFirstObjectByType<UnityTransport>();

    public async void CreateOrJoinLobby()
    {
        await Authenticate();

        connectedLoby = await QuickJoinLobby() ?? await CreateLobby();
        
        if(connectedLoby != null) buttons.SetActive(false);
    }

    public async void CreatePrivateLobby()
    {
        await Authenticate();

        const int maxPlayers = 4;

        try
        {
            //Create Realay allocation
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Create Private lobby options
            var options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Data = new Dictionary<string, DataObject>
                {
                    { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            };

            // Create lobby
            connectedLoby = await LobbyService.Instance.CreateLobbyAsync(
                "PrivateLobby",
                maxPlayers,
                options
            );

            // Heartbeat to keep the loby alive
            StartCoroutine(HeartBeatLobbyCoroutine(connectedLoby.Id, 15));

            // Configure NGO Host Transport
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            
            Debug.Log($"Private lobby created. Join code: {relayJoinCode}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create private lobby {e}");
        }
    }

    public async void JoinPrivateLobbyWithCode(string joinCode)
    {
        await Authenticate();

        try
        {
            // Look up the lobby using the join code
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode);

            // get Relay join code from lobby data
            string relayCode = lobby.Data[JoinCodeKey].Value;

            //join Relay
            var allocation = await RelayService.Instance.JoinAllocationAsync(relayCode);

            SetTransportAsClient(allocation);

            //Start NGO client
            NetworkManager.Singleton.StartClient();

            connectedLoby = lobby;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join private lobby: {e}");
        }
    }

    public void JoinPrivateLobbyFromInput(TMP_InputField input)
    {
        JoinPrivateLobbyWithCode(input.text);
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

            SetTransportAsClient(a);

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
            const int maxPlayers = 4;

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

    private void SetTransportAsClient(JoinAllocation a)
    {
        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key,a.ConnectionData,a.HostConnectionData);
    }

    // Tells Unity Services that the lobby is alive (deletes itself after 30sec without this)
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
