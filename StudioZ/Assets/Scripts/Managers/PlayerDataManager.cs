using System.IO;
using UnityEngine;

// Class to store player information
[System.Serializable]
public class PlayerData
{
    public string PlayerName;
    public int PlayerScore;
    public float[] BestLevelTimes = new float[8];
}

// Manager to handle saving and loading player data
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;
    private string filePath;
    private PlayerData cachedData;

    void Awake()
    {
        // Singleton setup
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        Debug.Log("Save Path: " + filePath);

        LoadOrCreate();
    }

    // Load existing data or create new file
    private void LoadOrCreate()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            cachedData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.Log("No save found, creating new data.");
            cachedData = new PlayerData();
            Save();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(cachedData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Saved PlayerData.");
    }

    // ------------ Getters and Setters ------------

    public string GetPlayerName() => cachedData.PlayerName;

    public void SetPlayerName(string name)
    {
        cachedData.PlayerName = name;
        Save();
    }

    public float[] GetBestLevelTimes() => cachedData.BestLevelTimes;

    public float GetSingleLevelTime(int level)
    {
        if (level < 0 || level >= cachedData.BestLevelTimes.Length)
            return -1f;

        return cachedData.BestLevelTimes[level];
    }

    public void SetSingleLevelTime(int level, float time)
    {
        // Expand if needed
        if (level >= cachedData.BestLevelTimes.Length)
        {
            float[] newArray = new float[level + 1];
            cachedData.BestLevelTimes.CopyTo(newArray, 0);
            cachedData.BestLevelTimes = newArray;
        }

        cachedData.BestLevelTimes[level] = time;
        Save();
    }
}
