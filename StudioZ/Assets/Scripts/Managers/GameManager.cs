using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    private LevelManager levelManager;

    public List<GameObject> playerGameObjects = new List<GameObject>();
    // A list for the tracked targets in the Cinemachine Target Group
    public List<GameObject> trackedTargets = new List<GameObject>();
    [Header("Debug Level Loading (Editor Only)")]
    [SerializeField] private bool setLevel;
    [SerializeField] private int level;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        levelManager = GetComponent<LevelManager>();
    }

    private void Update()
    {
        // Debug manual level change from Inspector
        if (setLevel)
        {
            setLevel = false;
            if (NetworkManager != null) RequestLoadLevel(level); 
            else LoadLevel(level);
        }
    }

    // --------------------------
    //  PUBLIC API (Clients call this)
    // --------------------------
    public void RequestLoadLevel(int index)
    {
        if (NetworkManager != null) LoadLevelServerRpc(index);
        else LoadLevel(index);
    }

    // --------------------------
    //  SERVER loads level
    // --------------------------
    [ServerRpc(RequireOwnership = false)]

    private void LoadLevelServerRpc(int index)
    {
        LoadLevel(index);             // Server loads the level
        LoadLevelClientRpc(index);    // Tell all clients to load it
    }

    // --------------------------
    //  CLIENTS load level
    // --------------------------
    [ClientRpc]
    private void LoadLevelClientRpc(int index)
    {
        // Prevent the host from loading twice
        if (!IsServer)
        {
            LoadLevel(index);
        }
    }

    // --------------------------
    //  ACTUAL LEVEL LOAD FUNCTION
    // --------------------------
    private void LoadLevel(int index)
    {
        // Reset all player positions and respawn
        foreach (GameObject gameObject in playerGameObjects)
        {
            NetworkPlayerMovement networkPlayerMovement = gameObject.GetComponent<NetworkPlayerMovement>();
            if (networkPlayerMovement != null)
            {
                networkPlayerMovement.currentCheckpoint = Vector2.zero;
                networkPlayerMovement.SpawnPlayer();
            }
        }

        // Load the actual level
        levelManager.LoadLevel(index);
    }

    // --------------------------
    //  MISC FUNCTIONS
    // --------------------------
    public void Reloadlevel()
    {
        levelManager.ReloadLevel();
    }

    public void setCurrentLevelTime(float time)
    {
        if (PlayerDataManager.Instance.GetSingleLevelTime(levelManager.CurrentLevelIndex) > time ||
            PlayerDataManager.Instance.GetSingleLevelTime(levelManager.CurrentLevelIndex) == 0)
        {
            PlayerDataManager.Instance.SetSingleLevelTime(levelManager.CurrentLevelIndex, time);
        }
    }

    public float GetCurrentLevelBestTime()
    {
        return PlayerDataManager.Instance.GetSingleLevelTime(levelManager.CurrentLevelIndex);
    }

    // --------------------------
    //  PLAYER REGISTRATION
    // --------------------------
    public void RegisterPlayer(GameObject player)
    {
        if (!playerGameObjects.Contains(player))
            playerGameObjects.Add(player);
    }

    public void UnregisterPlayer(GameObject player)
    {
        if (playerGameObjects.Contains(player))
            playerGameObjects.Remove(player);
    }
}
