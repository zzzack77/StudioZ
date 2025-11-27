using Unity.Netcode;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    [Header("Level Prefabs (Order Matters)")]
    public GameObject[] levelPrefabs;

    [Header("Breaker Level Settings")]
    public bool[] isBreakerLevel;

    private GameObject currentLevelInstance;
    private int currentLevelIndex = -1;

    public int CurrentLevelIndex => currentLevelIndex;

    public void LoadLevel(int index)
    {
        // ------------------------------------------------------
        // VALIDATION
        // ------------------------------------------------------
        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError("LevelManager: Invalid level index!");
            return;
        }

        if (index == currentLevelIndex)
            return;

        UnloadCurrentLevel();

        // ------------------------------------------------------
        // SINGLE-PLAYER MODE (NO NETWORKING)
        // ------------------------------------------------------
        if (!GameMode.IsMultiplayer)
        {
            currentLevelInstance = Instantiate(levelPrefabs[index]);
            currentLevelIndex = index;

            Debug.Log($"[LevelManager] (SP) Loaded Level {index}");
            return;
        }

        // ------------------------------------------------------
        // MULTIPLAYER MODE (SERVER LOADS)
        // ------------------------------------------------------
        if (!IsServer)
            return; // clients do nothing

        GameObject level = Instantiate(levelPrefabs[index]);

        // Every root-level object needs a NetworkObject
        var netObj = level.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        else
        {
            Debug.LogError("Level prefab needs a NetworkObject root for multiplayer!");
        }

        currentLevelInstance = level;
        currentLevelIndex = index;

        Debug.Log($"[LevelManager] (MP) Loaded Level {index}");
    }

    public void UnloadCurrentLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }

        currentLevelIndex = -1;
    }

    public void ReloadLevel()
    {
        if (currentLevelIndex != -1)
            LoadLevel(currentLevelIndex);
    }
}
