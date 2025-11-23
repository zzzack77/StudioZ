using Unity.Netcode;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    //if (Input.GetKeyDown(KeyCode.F1)) LoadLevel(1);
    //    if (Input.GetKeyDown(KeyCode.F2)) LoadLevel(2);
    //    if (Input.GetKeyDown(KeyCode.F3)) LoadLevel(3);
    //    if (Input.GetKeyDown(KeyCode.F4)) LoadLevel(4);
    //    if (Input.GetKeyDown(KeyCode.F5)) LoadLevel(5);
    [Header("Level Prefabs (Order Matters)")]
    public GameObject[] levelPrefabs;

    private GameObject currentLevelInstance;
    private int currentLevelIndex = -1;

    public int CurrentLevelIndex => currentLevelIndex;

    // No Update() input allowed — that caused desync!
    // Level loading MUST be requested via GameManager RPC.

    /// <summary>
    /// Loads a level prefab by index on this client/server instance.
    /// Must only be called by GameManager via ServerRpc or ClientRpc.
    /// </summary>
    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError("LevelManager: Invalid level index!");
            return;
        }

        // Ignore duplicate load request
        if (index == currentLevelIndex)
            return;

        // Unload previous level
        UnloadCurrentLevel();

        // Instantiate the new level
        currentLevelInstance = Instantiate(levelPrefabs[index]);
        currentLevelIndex = index;

        Debug.Log($"[LevelManager] Loaded Level: {index}");
    }

    /// <summary>
    /// Destroys the currently active level instance.
    /// </summary>
    public void UnloadCurrentLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }

        currentLevelIndex = -1;
    }

    /// <summary>
    /// Reloads the currently active level if one exists.
    /// </summary>
    public void ReloadLevel()
    {
        if (currentLevelIndex != -1)
            LoadLevel(currentLevelIndex);
    }
}
