using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    private PlayerDataManager playerDataManager;
    private LevelManager levelManager;

    public List<GameObject> playerGameObjects = new List<GameObject>();

    // A list for the tracked targets in the Cinemachine Target Group
    public List<GameObject> trackedTargets = new List<GameObject>();

    [SerializeField] private bool setLevel;
    [SerializeField] private int level;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);

        playerDataManager = GetComponent<PlayerDataManager>();
        levelManager = GetComponent<LevelManager>();
    }
    private void Update()
    {
        if (setLevel)
        {
            setLevel = false;
            LoadLevel(level);
        }
    }
    public void setLevelTime(float time)
    {
        if (playerDataManager.GetSingleLevelTime(levelManager.CurrentLevelIndex) < time)
        {
            playerDataManager.SetSingleLevelTime(levelManager.CurrentLevelIndex, time);
        }
    }
    public void LoadLevel(int index) 
    {
        foreach (GameObject gameObject in playerGameObjects)
        {
            NetworkPlayerMovement networkPlayerMovement = gameObject.GetComponent<NetworkPlayerMovement>();
            if (networkPlayerMovement != null)
            {
                networkPlayerMovement.currentCheckpoint = Vector2.zero;
                networkPlayerMovement.SpawnPlayer();
            }
        }
        levelManager.LoadLevel(index); 
    }
    public void Reloadlevel() { levelManager.ReloadLevel(); }

    public void RegisterPlayer(GameObject player)
    {
        if (!playerGameObjects.Contains(player))
        {
            playerGameObjects.Add(player);
        }
    }

    public void UnregisterPlayer(GameObject player)
    {
        if (playerGameObjects.Contains(player))
        {
            playerGameObjects.Remove(player);
        }
    }
}
