using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    private LevelManager levelManager;

    [SerializeField] private GameObject LevelUI;

    public Dictionary<ulong, GameObject> playerGameObjects = new Dictionary<ulong, GameObject>();
    public List<GameObject> trackedTargets = new List<GameObject>();

    [Header("Debug Level Loading (Editor Only)")]
    [SerializeField] private bool isMultiplayer = true;
    [SerializeField] private bool setLevel;
    [SerializeField] private int level;

    private void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        levelManager = GetComponent<LevelManager>();

        GameMode.IsMultiplayer = isMultiplayer;
    }

    private void Update()
    {
        // Debug manual level change from inspector
        if (setLevel)
        {
            setLevel = false;
            RequestLoadLevel(level);
        }
    }

    // ============================================================
    //  PUBLIC CALL TO LOAD LEVEL (works for SP and MP)
    // ============================================================
    public void RequestLoadLevel(int index)
    {
        Debug.Log("passed 2");
        if (GameMode.IsMultiplayer)
        {
            Debug.Log("Load multiplayer level " + index);

            // Multiplayer -> server handles the load
            if (IsServer)
                LoadLevelServer(index);
            else
                LoadLevelServerRpc(index);
        }
        else
        {
            // Single Player -> load directly
            LoadLevel(index);
        }

        //SetUI(false);
    }

    // ============================================================
    //  MULTIPLAYER ONLY – SERVER LOGIC
    // ============================================================
    private void LoadLevelServer(int index)
    {
        LoadLevel(index);             // Server loads
        LoadLevelClientRpc(index);    // Tell clients
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadLevelServerRpc(int index)
    {
        LoadLevelServer(index);
    }

    // ============================================================
    //  MULTIPLAYER ONLY – CLIENT LOGIC
    // ============================================================
    [ClientRpc]
    private void LoadLevelClientRpc(int index)
    {
        // Don't load twice on host
        if (!IsServer)
        {
            LoadLevel(index);
        }
    }

    // ============================================================
    //  ACTUAL LEVEL LOAD (used by BOTH SP and MP)
    // ============================================================
    private void LoadLevel(int index)
    {
        foreach (GameObject go in playerGameObjects.Values)
        {
            var movement = go.GetComponent<NetworkPlayerMovement>();
            if (movement != null)
            {
                movement.currentCheckpoint = Vector2.zero;
                movement.SpawnPlayer();
            }
        }

        levelManager.LoadLevel(index);
    }

    // ============================================================
    //  MISC
    // ============================================================
    public void Reloadlevel()
    {
        levelManager.ReloadLevel();
    }

    public void setCurrentLevelTime(float time)
    {
        float best = PlayerDataManager.Instance.GetSingleLevelTime(levelManager.CurrentLevelIndex);

        if (best > time || best == 0)
            PlayerDataManager.Instance.SetSingleLevelTime(levelManager.CurrentLevelIndex, time);
    }

    public float GetCurrentLevelBestTime()
    {
        return PlayerDataManager.Instance.GetSingleLevelTime(levelManager.CurrentLevelIndex);
    }
    public int GetCurrentLevel()
    {
        return levelManager.CurrentLevelIndex;
    }

    public void SetUI(bool UIEnabled)
    {
        LevelUI.SetActive(UIEnabled);
    }

    // ============================================================
    //  PLAYER REGISTRATION
    // ============================================================
    public void RegisterPlayer(ulong playerID, GameObject player)
    {
        if (!playerGameObjects.ContainsKey(playerID))
            playerGameObjects[playerID] = player;
    }

    public void UnregisterPlayer(ulong playerID)
    {
        if (playerGameObjects.ContainsKey(playerID))
            playerGameObjects.Remove(playerID);
    }
}
