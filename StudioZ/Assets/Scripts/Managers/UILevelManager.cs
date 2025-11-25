using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UILevelManager : MonoBehaviour
{
    [SerializeField] private GameObject LevelUI;
    private VisualElement root;

    private const string BackButtonName = "Back";
    private const string LeaderboardButtonName = "LeaderboardButton";

    [SerializeField] private float[] BTDTime;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        HookUpBackButton();
        HookUpLeaderboardButton();
        HookUpLevelButtons();
    }

    // ------------------------------------------------------
    // BACK BUTTON
    // ------------------------------------------------------
    private void HookUpBackButton()
    {
        Button back = root.Q<Button>(BackButtonName);
        if (back != null)
            back.clicked += () => Debug.Log("Back button clicked!");
    }

    // ------------------------------------------------------
    // LEADERBOARD BUTTON
    // ------------------------------------------------------
    private void HookUpLeaderboardButton()
    {
        Button leaderboard = root.Q<Button>(LeaderboardButtonName);
        if (leaderboard != null)
            leaderboard.clicked += () => Debug.Log("Leaderboard button clicked!");
    }

    // ------------------------------------------------------
    // LEVEL BUTTONS (1..12)
    // ------------------------------------------------------
    private void HookUpLevelButtons()
    {
        // Iterate button names "1" to "12"
        for (int i = 1; i <= 12; i++)
        {
            if (i != 1 && PlayerDataManager.Instance.GetSingleLevelTime(Mathf.Max(i - 1, 1)) <= 0) return;
            else
            {
                SetPlayerBestTime(i);
                SetLevelText(i);
            }
            Debug.Log($"{i} passed and is a button");

            string buttonName = i.ToString();

            Button levelButton = root.Q<Button>(buttonName);
            if (levelButton != null)
            {
                int capturedNumber = i; // Capture for closure
                levelButton.clicked += () => LoadLevel(capturedNumber);
            }
            else
            {
                Debug.LogWarning($"Level button '{buttonName}' not found in UI.");
            }
        }
        if (PlayerDataManager.Instance.GetSingleLevelTime(12) > 0){
            ShowBTD();
        }
    }
    private void SetLevelText(int level)
    {
        string levelString = level.ToString() + "Level";
        Label levelLabel = root.Q<Label>(levelString);
        if (levelLabel != null)
        {
            levelLabel.text = $"Level {level}";
        }
    }
    private void SetPlayerBestTime(int level)
    {
        string timeString = level.ToString() + "Time";
        Label timeLabel = root.Q<Label>(timeString);
        float seconds = PlayerDataManager.Instance.GetSingleLevelTime(level);

        if (seconds > 0 && timeLabel != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            timeLabel.text = $"Time : {time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds / 10:D2}";
        }
    }
    private void ShowBTD()
    {
        for (int i = 0; i <= 12; i++)
        {
            float timeTobeat = BTDTime[i];
            string BTDString = i.ToString() + "BTD";
            Label BTDLabel = root.Q<Label>(BTDString);
            string boolString = i.ToString() + "Bool";
            Label boolLabel = root.Q<Label>(boolString);

            if (BTDLabel != null && timeTobeat != 0)
            {
                TimeSpan time = TimeSpan.FromSeconds(timeTobeat);
                BTDLabel.text = $"Beat the Devs : {time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds / 10:D2}";
                if (boolLabel != null)
                {
                    if (PlayerDataManager.Instance.GetSingleLevelTime(i) < timeTobeat) boolLabel.text = "✓";
                    else boolLabel.text = "✗";
                }
            }
        }
    }

    // ------------------------------------------------------
    // LOAD LEVEL BY NUMBER
    // ------------------------------------------------------
    private void LoadLevel(int levelNumber)
    {
        Debug.Log($"Loading Level {levelNumber}");

        GameManager.Instance.RequestLoadLevel(levelNumber);
    }
}
