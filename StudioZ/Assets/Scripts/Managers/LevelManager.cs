using Unity.Netcode;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    [Header("Level Prefabs (Order Matters)")]
    public GameObject[] levelPrefabs;

    private GameObject currentLevelInstance;
    private int currentLevelIndex = -1;
    public int CurrentLevelIndex => currentLevelIndex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) LoadLevel(1);
        if (Input.GetKeyDown(KeyCode.F2)) LoadLevel(2);
        if (Input.GetKeyDown(KeyCode.F3)) LoadLevel(3);
        if (Input.GetKeyDown(KeyCode.F4)) LoadLevel(4);
        if (Input.GetKeyDown(KeyCode.F5)) LoadLevel(5);
    }
    // Loads a level prefab by index, unloaded the previous level if needed.
    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError("LevelManager: Invalid level index!");
            return;
        }

        // If same level requested, ignore
        if (index == currentLevelIndex)
            return;
        
        // Unload existing level
        UnloadCurrentLevel();

        // Instantiate new level
        currentLevelInstance = Instantiate(levelPrefabs[index]);
        currentLevelIndex = index;

        Debug.Log("Loaded Level: " + index);
    }

    // Destroys the currently active level.
    public void UnloadCurrentLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
            Debug.Log("Unloaded current level.");
        }

        currentLevelIndex = -1;
    }

    // Reloads the currently active level.
    public void ReloadLevel()
    {
        if (currentLevelIndex != -1)
            LoadLevel(currentLevelIndex);
    }
}
