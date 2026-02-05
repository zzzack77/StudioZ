using System;
using System.Collections;
using System.Collections.Generic;
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

/// <summary>
/// Handles all integration with Unity Services (Authentication, Lobby, Relay)
/// and manages the core logic for Netcode for GameObjects (NGO) start/stop,
/// including Host Migration.
/// </summary>
public class SimpleMatchmaking : MonoBehaviour
{
    // --- Editor Configuration & Static Instance ---
    
    [SerializeField] private GameObject buttons; // UI elements to hide after joining/creating a lobby

    public static SimpleMatchmaking Instance; // Singleton instance
    
    public event Action<List<Player>> OnLobbyPlayersUpdated;
    public Lobby ConnectedLobby => connectedLobby;
    // --- Private Fields ---
    
    private bool isGameInProgress = false;
    private Lobby connectedLobby;
    private UnityTransport transport;
    // Key used to store the Relay Join Code within the Lobby data.
    private const string JoinCodeKey = "j"; 
    private string playerId; // Unique ID for the current player (used for Auth, Lobby, and Host check)
    public string playerName;  // player controlled name with default
    
   
    
    // Track the heartbeat coroutine so we can stop/restart it (critical for Host Migration)
    private Coroutine heartbeatCoroutine;
    // We need to keep a reference to lobby events to unsubscribe later or handle migration
    private LobbyEventCallbacks lobbyEventCallbacks;


    

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Get the UTP transport component attached in the scene
        transport = FindFirstObjectByType<UnityTransport>();
        playerName = "Player" + UnityEngine.Random.Range(0, 999999);


    }

    void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    // --- Primary Lobby/Game Setup Methods ---

    /// <summary>
    /// Authenticates the player, then attempts to Quick Join a lobby. 
    /// If Quick Join fails, it creates a new public lobby.
    /// </summary>
    public async void CreateOrJoinLobby()
    {
        await Authenticate();
        // Null-coalescing operator: try QuickJoin, if null, run CreateLobby
        connectedLobby = await QuickJoinLobby() ?? await CreateLobby();
        //if (connectedLobby != null) buttons.SetActive(false);
    }

    /// <summary>
    /// Creates a private lobby and sets the local player as the Host.
    /// </summary>
    public async void CreatePrivateLobby()
    {
        await Authenticate();
        const int maxPlayers = 4;

        try
        {
            // 1. Create a Relay Allocation for networking
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // 2. Setup Lobby options, making it private and storing the Relay code
            var options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Data = new Dictionary<string, DataObject>
                {
                    // Use VisibilityOptions.Member for private lobbies
                    { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            };

            // 3. Create the Lobby
            connectedLobby = await LobbyService.Instance.CreateLobbyAsync("PrivateLobby", maxPlayers, options);

            // 4. Start listening for lobby changes (required for host migration)
            await SubscribeToLobbyEvents();

            // 5. Start the Heartbeat to keep the lobby alive on the server
            StartHeartbeat();

            // 6. Configure the NGO Transport to use the Relay Allocation data for hosting
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // 7. Start NGO as Host
            NetworkManager.Singleton.StartHost();
            Debug.Log($"Private lobby created. Code: {connectedLobby.LobbyCode}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create private lobby {e}");
        }
    }

    /// <summary>
    /// Joins a private lobby using a Lobby Code provided by the user.
    /// </summary>
    /// <param name="joinCode">The 6-character Lobby Code.</param>
    public async void JoinPrivateLobbyWithCode(string joinCode)
    {
        await Authenticate();

        try
        {
            var options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer() 
            };
            
            // 1. Join the lobby using the code
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode);
            // 2. Get the Relay join code from the lobby data
            var relayCode = lobby.Data[JoinCodeKey].Value;
            // 3. Join the Relay Allocation
            var allocation = await RelayService.Instance.JoinAllocationAsync(relayCode);

            // 4. Configure NGO Transport as a Client
            SetTransportAsClient(allocation);
            // 5. Start NGO as Client
            NetworkManager.Singleton.StartClient();

            connectedLobby = lobby;
            
            // 6. Start listening for migration events
            await SubscribeToLobbyEvents();
            
            OnLobbyPlayersUpdated?.Invoke(connectedLobby.Players);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join private lobby: {e}");
        }
    }

    // --- Utility Methods ---

    /// <summary>
    /// Initializes Unity Services and signs the player in anonymously.
    /// </summary>
    private async Task Authenticate()
    {
        var options = new InitializationOptions();

        // This creates a unique profile name based on the process so Editor and Build don't clash.
#if UNITY_EDITOR
        // If using ParrelSync, use the clone argument. Otherwise use "Editor".
        string profile = ClonesManager.IsClone() ? ClonesManager.GetArgument() : "EditorProfile";
        options.SetProfile(profile);
#else
    // If running a Build, assume it's a Client and give it a generic "Build" profile
    // OR better yet, use a random one for testing so you can run multiple builds.
    options.SetProfile("BuildProfile_" + UnityEngine.Random.Range(0, 1000));
#endif
           
        await UnityServices.InitializeAsync(options);
        
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        
        playerId = AuthenticationService.Instance.PlayerId;
    }

    /// <summary>
    /// Attempts to quickly join an existing public lobby.
    /// </summary>
    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            
            var options = new QuickJoinLobbyOptions
            {
                Player = GetPlayer() 
            };
            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            var allocation = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            SetTransportAsClient(allocation);
            NetworkManager.Singleton.StartClient();
            
            connectedLobby = lobby;
            await SubscribeToLobbyEvents(); // Listen for migration
            
            OnLobbyPlayersUpdated?.Invoke(connectedLobby.Players);
            
            return lobby;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Quick Join Failed at step: {e.Message}");
            // No lobbies available to quick join
            return null;
        }
    }

    /// <summary>
    /// Creates a new public lobby and sets the local player as the Host.
    /// </summary>
    private async Task<Lobby> CreateLobby()
    {
        try
        {
            const int maxPlayers = 4;
            // 1. Create Relay Allocation
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // 2. Setup Lobby options (public visibility)
            var options = new CreateLobbyOptions()
            {
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                    // Use VisibilityOptions.Public for quick-joinable lobbies
                    { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };

            // 3. Create the Lobby
            var lobby = await LobbyService.Instance.CreateLobbyAsync("PublicLobby", maxPlayers, options);

            connectedLobby = lobby;
            await SubscribeToLobbyEvents(); // IMPORTANT: Hook up events for migration
            StartHeartbeat();

            OnLobbyPlayersUpdated?.Invoke(connectedLobby.Players);
            
            // 4. Configure NGO Transport for hosting
            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            // 5. Start NGO as Host
            NetworkManager.Singleton.StartHost();



            // ---- Ive added this extra call so it updates the ui after setting the host.
            // If this needs to go or is causing problems let me know
            // - Zack Clarke
            OnLobbyPlayersUpdated?.Invoke(connectedLobby.Players);

            return lobby;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create lobby: {e}");
            return null;
        }
    }

    /// <summary>
    /// Configures the UTP Transport with Relay information for a client connection.
    /// </summary>
    /// <param name="a">The JoinAllocation object obtained from Relay Service.</param>
    private void SetTransportAsClient(JoinAllocation a)
    {
        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }
    /// <summary>
    /// Helper function to create Player Data object
    /// </summary>
    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }
    
    
    /// <summary>
    /// Locks the lobby to prevent new players from joining and marks the game as started.
    /// </summary>
    public async void LockLobby()
    {
        if (!IsHost) return;

        isGameInProgress = true;

        try
        {
            // 1. Lock the Lobby so it doesn't appear in QuickJoins or Queries
            var updateOptions = new UpdateLobbyOptions
            {
                IsLocked = true
            };

            await LobbyService.Instance.UpdateLobbyAsync(connectedLobby.Id, updateOptions);
            Debug.Log("Game Started. Lobby is now Locked.");

            
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to lock lobby: {e}");
        }
    }
    
    
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // 1. Default approval settings
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null; 

        // 2. If the game has started, reject them
        if (isGameInProgress)
        {
            response.Approved = false;
            response.Reason = "Game has already started.";
            Debug.Log("Connection denied: Game in progress.");
            return;
        }
    
        // 3. Optional: Cap player count strictly using NetworkManager (Backup to Lobby)
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= 4) // Hard cap example
        {
            response.Approved = false;
            response.Reason = "Lobby is full.";
        }

        // 4. Pending Approval Logic (Optional)
        // If you need to validate specific data from the client, you can check request.Payload here
    }

    // -------------------------------------------------------------------------
    // --- LOBBY EVENTS & HOST MIGRATION LOGIC ---
    // -------------------------------------------------------------------------

    /// <summary>
    /// Subscribes the current player to receive real-time updates from the connected lobby.
    /// </summary>
    private async Task SubscribeToLobbyEvents()
    {
        lobbyEventCallbacks = new LobbyEventCallbacks();
        // Assign the handler for any changes detected in the lobby
        lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
        
        try 
        { 
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(connectedLobby.Id, lobbyEventCallbacks); 
        }
        catch (Exception e) 
        { 
            Debug.LogError($"Failed to subscribe to lobby events: {e}"); 
        }
    }

    /// <summary>
    /// Event handler triggered when the lobby state changes (e.g., Host leaves, data updates).
    /// This is the core of the Host Migration logic.
    /// </summary>
    private void OnLobbyChanged(ILobbyChanges changes)
    {
        // 1. Apply changes to the local lobby object
        changes.ApplyToLobby(connectedLobby);

        // 2. Fire the event so the UI knows to redraw
        if (changes.PlayerJoined.Changed || changes.PlayerLeft.Changed)
        {
            OnLobbyPlayersUpdated?.Invoke(connectedLobby.Players);
        }
        
        
        
        // 1. Check if the Host has changed
        if (changes.HostId.Changed)
        {
            // Update local lobby data to ensure we have the latest HostId
            changes.ApplyToLobby(connectedLobby);

            // Check if *this* player has been promoted to Host
            if (connectedLobby.HostId == playerId)
            {
                Debug.Log("Host left. I have been promoted to Host! Starting Migration...");
                MigrateToHost();
            }
        }

        // 2. Check if the Lobby Data (specifically the Relay Join Code) has changed
        if (changes.Data.Changed)
        {
            changes.ApplyToLobby(connectedLobby);
            
            // If I am NOT the host (i.e., I'm a client), I need to reconnect
            if (connectedLobby.HostId != playerId)
            {
                if (changes.Data.Value.ContainsKey(JoinCodeKey))
                {
                    // The new host has updated the lobby with a new Relay code
                    string newCode = changes.Data.Value[JoinCodeKey].Value.Value;
                    Debug.Log($"Relay Code changed to {newCode}. Reconnecting...");
                    MigrateToClient(newCode);
                }
            }
        }
    }

    /// <summary>
    /// Executes the steps required for a promoted client to become the new Host.
    /// This involves creating a new Relay allocation and updating the Lobby.
    /// </summary>
    private async void MigrateToHost()
    {
        // 1. Shutdown the old network connection (required before starting new Host)
        NetworkManager.Singleton.Shutdown();

        try 
        {
            // 2. Create NEW Relay Allocation for the new Host
            var allocation = await RelayService.Instance.CreateAllocationAsync(connectedLobby.MaxPlayers);
            var newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

           
            // IMPORTANT FIX: Get the original visibility (Public/Member) before updating.
            // Visibility cannot be changed, or the request will fail.
            var visibility = DataObject.VisibilityOptions.Member; // Default fallback
            if (connectedLobby.Data != null && connectedLobby.Data.ContainsKey(JoinCodeKey))
            {
                visibility = connectedLobby.Data[JoinCodeKey].Visibility;
            }
            

            // 3. Update Lobby with the new Relay Code. 
            // This triggers the 'Data.Changed' event on all other clients.
            await LobbyService.Instance.UpdateLobbyAsync(connectedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { JoinCodeKey, new DataObject(visibility, newJoinCode) }
                }
            });

            // 4. Start Heartbeat (since we are now the Host)
            StartHeartbeat();

            // 5. Configure Transport and start NGO as Host
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4, 
                (ushort)allocation.RelayServer.Port, 
                allocation.AllocationIdBytes, 
                allocation.Key, 
                allocation.ConnectionData
            );
        
            NetworkManager.Singleton.StartHost();
            Debug.Log("Migration to Host Complete.");
        }
        catch(Exception e)
        {
            Debug.LogError($"Host Migration failed: {e}");
        }
    }

    /// <summary>
    /// Executes the steps required for a client to reconnect to the new Relay after migration.
    /// </summary>
    /// <param name="newRelayCode">The Relay code published by the new Host.</param>
    private async void MigrateToClient(string newRelayCode)
    {
        // 1. Stop the currently failed/disconnected client connection
        NetworkManager.Singleton.Shutdown();

        // 2. Wait a small moment to ensure clean shutdown (optional but good practice)
        await Task.Delay(500);

        try
        {
            // 3. Join the new Relay Allocation
            var allocation = await RelayService.Instance.JoinAllocationAsync(newRelayCode);
            SetTransportAsClient(allocation);

            // 4. Start NGO as Client, connecting to the new Host
            NetworkManager.Singleton.StartClient();
            Debug.Log("Reconnected to migrated game.");
        }
        catch(Exception e)
        {
            Debug.LogError($"Client Migration failed: {e}");
        }
    }

    // --- HEARTBEAT HELPERS ---

    /// <summary>
    /// Starts the lobby heartbeat coroutine, ensuring the lobby stays active.
    /// </summary>
    private void StartHeartbeat()
    {
        StopHeartbeat(); // Ensure no duplicates are running
        heartbeatCoroutine = StartCoroutine(HeartBeatLobbyCoroutine(connectedLobby.Id, 15));
    }

    /// <summary>
    /// Stops the currently running lobby heartbeat coroutine.
    /// </summary>
    private void StopHeartbeat()
    {
        if (heartbeatCoroutine != null)
        {
            StopCoroutine(heartbeatCoroutine);
            heartbeatCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine that sends a ping to the Lobby Service every 15 seconds.
    /// If the server doesn't receive a ping, it deletes the lobby after 30 seconds.
    /// </summary>
    private static IEnumerator HeartBeatLobbyCoroutine(string lobbyId, int waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    // --- CLEANUP ---

    /// <summary>
    /// Called when the GameObject is destroyed (e.g., when the game closes).
    /// Cleans up the lobby connection.
    /// </summary>
    private void OnDestroy()
    {
        try
        {
            StopAllCoroutines();

            if (connectedLobby != null)
            {
                
                // Host Migration Cleanup Logic:
                if (connectedLobby.HostId == playerId)
                {
                    // The Host is gracefully closing. Instead of deleting the lobby, 
                    // we remove ourselves, allowing the Lobby Service to promote the next player.
                    LobbyService.Instance.RemovePlayerAsync(connectedLobby.Id, playerId);
                }
                else
                {
                    // A client is leaving. Simply remove the player from the lobby.
                    LobbyService.Instance.RemovePlayerAsync(connectedLobby.Id, playerId);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error shutting down lobby: {e}");
        }
    }

    public void ChangePlayerName(string newName)
    {
        if (newName.Length > 1)
        {
            playerName = newName;
        }
        
    }
    
    public bool IsHost =>
        connectedLobby != null && connectedLobby.HostId == playerId;
}