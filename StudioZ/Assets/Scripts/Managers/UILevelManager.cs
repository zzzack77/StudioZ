using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UILevelManager : MonoBehaviour
{
    private VisualElement root;

    private const string BackButtonName = "Back";
    private const string LeaderboardButtonName = "LeaderboardButton";

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
