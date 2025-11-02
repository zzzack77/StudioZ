using System.IO;
using UnityEngine;

// Class to store player information
[System.Serializable]
public class PlayerData
{
    public string PlayerName;
    public int PlayerScore;
    public bool[] BoolArray = new bool[8];
}

// Manager to handle saving and loading player data
public class PlayerDataManager : MonoBehaviour
{
    private string filePath;

    void Awake()
    {
        // Set the file path where the player data will be stored
        filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");

        // Debug log to check the file path
        Debug.Log("Persistent Data Path: " + filePath);
    }
    private void Start()
    {
        Debug.Log(GetPlayerName());
        Debug.Log(GetPlayerScore());
    }

    // Save player data to a JSON file
    public void SavePlayerData(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("Error: Player data is null!");
            return;
        }

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("Error: filePath is null or empty!");
            return;
        }

        // Convert the PlayerData object to JSON
        string json = JsonUtility.ToJson(playerData, true);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Serialization failed: JSON is empty.");
            return;
        }

        // Write the JSON to the file
        File.WriteAllText(filePath, json);
        //Debug.Log("Player data saved successfully at: " + filePath);
    }

    // Load player data from a JSON file
    public PlayerData LoadPlayerData()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found! Returning new player data.");
            return new PlayerData();
        }

        string json = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Error: Loaded JSON is empty.");
            return new PlayerData();
        }

        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
        return playerData;
    }
    public string GetPlayerName()
    {
        if (GetPlayerDataManager())
        {
            PlayerData player = LoadPlayerData();
            return player.PlayerName;
        }
        return null;
    }
    public void SetPlayerName(string name)
    {
        if (GetPlayerDataManager())
        {
            PlayerData player = LoadPlayerData();
            player.PlayerName = name;
            SavePlayerData(player);
        }
    }
    public int GetPlayerScore()
    {
        if (GetPlayerDataManager())
        {
            PlayerData player = LoadPlayerData();
            return player.PlayerScore;
        }
        return 0;
    }
    public void SetPlayerScore(int score)
    {
        if (GetPlayerDataManager())
        {
            PlayerData player = LoadPlayerData();
            player.PlayerScore = score;
            SavePlayerData(player);
        }
    }
    //public bool[] GetPlayerUnlockedPets()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.unlockedPets;
    //    }
    //    return null;
    //}
    //public void SetPlayerUnlockedPets(bool[] unlockedP)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.unlockedPets = unlockedP;
    //        SavePlayerData(player);
    //    }
    //}
    //public string GetPlayerPet()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.currentPet;
    //    }
    //    return null;
    //}
    //public void SetPlayerPet(string pet)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.currentPet = pet;
    //        SavePlayerData(player);
    //    }
    //}
    //public int GetPlayerLevel()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.currentLevel;
    //    }
    //    return 0;
    //}
    //public void SetPlayerLevel(int level)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.currentLevel = level;
    //        SavePlayerData(player);
    //    }
    //}
    //public int GetPlayerXP()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.xp;
    //    }
    //    return 0;
    //}
    //public void SetPlayerXP(int xp)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.xp = xp;
    //        SavePlayerData(player);
    //    }
    //}
    //public float GetTotalTimePlayed()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.totalTimePlayed;
    //    }
    //    return 0f;
    //}

    //public void SetTotalTimePlayed(float time)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.totalTimePlayed = time;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumQuestionsAnswered()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numQuestionsAnswered;
    //    }
    //    return 0;
    //}

    //public void SetNumQuestionsAnswered(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numQuestionsAnswered = num;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumQuestionsAnsweredCorrect()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numQuestionsAnsweredCorrect;
    //    }
    //    return 0;
    //}

    //public void SetNumQuestionsAnsweredCorrect(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numQuestionsAnsweredCorrect = num;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumAdditionPicked()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numAdditionPicked;
    //    }
    //    return 0;
    //}

    //public void SetNumAdditionPicked(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numAdditionPicked = num;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumSubtractionPicked()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numSubtractionPicked;
    //    }
    //    return 0;
    //}

    //public void SetNumSubtractionPicked(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numSubtractionPicked = num;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumMultiplicationPicked()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numMultiplicationPicked;
    //    }
    //    return 0;
    //}

    //public void SetNumMultiplicationPicked(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numMultiplicationPicked = num;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumDivisionPicked()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numDivisionPicked;
    //    }
    //    return 0;
    //}

    //public void SetNumDivisionPicked(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numDivisionPicked = num;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumEasyPicked()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numEasyPicked;
    //    }
    //    return 0;
    //}

    //public void SetNumEasyPicked(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numEasyPicked = num;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumMediumPicked()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numMediumPicked;
    //    }
    //    return 0;
    //}

    //public void SetNumMediumPicked(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numMediumPicked = num;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumHardPicked()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numHardPicked;
    //    }
    //    return 0;
    //}

    //public void SetNumHardPicked(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numHardPicked = num;
    //        SavePlayerData(player);
    //    }
    //}

    //public int GetNumVeryHardPicked()
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        return player.numVeryHardPicked;
    //    }
    //    return 0;
    //}

    //public void SetNumVeryHardPicked(int num)
    //{
    //    if (GetPlayerDataManager())
    //    {
    //        PlayerData player = LoadPlayerData();
    //        player.numVeryHardPicked = num;
    //        SavePlayerData(player);
    //    }
    //}

    public bool GetPlayerDataManager()
    {
        // Find an existing PlayerDataManager in the scene
        PlayerDataManager manager = FindFirstObjectByType<PlayerDataManager>();

        if (manager == null)
        {
            Debug.LogError("PlayerDataManager not found in the scene!");
            return false;
        }
        return true;
    }
}
