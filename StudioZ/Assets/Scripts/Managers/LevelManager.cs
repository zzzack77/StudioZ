using Unity.Netcode;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    [Header("Level Prefabs (Order Matters)")]
    public GameObject[] levelPrefabs;

    [Header("Breaker Level Settings")]
    public bool[] isBreakerLevel;   // Mark which levels contain HoldBreakers

    private GameObject currentLevelInstance;
    private int currentLevelIndex = -1;

    public int CurrentLevelIndex => currentLevelIndex;

    public void LoadLevel(int index)
    {
        if (!IsServer)
        {
            Debug.LogError("LevelManager.LoadLevel MUST be called on the SERVER.");
            return;
        }

        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError("LevelManager: Invalid level index!");
            return;
        }

        // Ignore duplicate load request
        if (index == currentLevelIndex)
            return;

        // Unload old level
        UnloadCurrentLevel();

        // Instantiate new level (server-side)
        currentLevelInstance = Instantiate(levelPrefabs[index]);
        currentLevelIndex = index;

        Debug.Log($"[LevelManager] Loaded Level: {index}");

        //
        // --- NEW LOGIC: Spawn holds if this is a breaker level ---
        //

        if (isBreakerLevel.Length > index && isBreakerLevel[index])
        {
            SpawnBreakerHolds(currentLevelInstance);
        }
    }

    private void SpawnBreakerHolds(GameObject levelRoot)
    {
        foreach (var hold in levelRoot.GetComponentsInChildren<HoldBreaker>(true))
        {
            var netObj = hold.GetComponent<NetworkObject>();

            if (netObj != null && !netObj.IsSpawned)
            {
                netObj.Spawn(true);
            }
            else
            {
                Debug.LogWarning($"HoldBreaker {hold.name} has NO NetworkObject! Add one.");
            }
        }

        Debug.Log("[LevelManager] Spawned all HoldBreakers in this level.");
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
