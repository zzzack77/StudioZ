using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UILevelManager : MonoBehaviour
{
    [SerializeField] private GameObject LevelUI;
    private VisualElement root;

    private List<Focusable> focusables = new List<Focusable>();
    private int currentIndex = 0;

    private const string BackButtonName = "BackButton";
    private const string LeaderboardButtonName = "LeaderboardButton";

    [SerializeField] private float[] BTDTime;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        //HookUpBackButton();
        //HookUpLeaderboardButton();
        HookUpLevelButtons();

        BuildNavigationList();
        FocusInitial();
    }

    // Back Button
    private void HookUpBackButton()
    {
        Button back = root.Q<Button>(BackButtonName);
        if (back != null)
            back.clicked += () => Debug.Log("Back button clicked!");

        back.focusable = true;
    }

    // Leaderboard Button
    private void HookUpLeaderboardButton()
    {
        Button leaderboard = root.Q<Button>(LeaderboardButtonName);
        if (leaderboard != null)
            leaderboard.clicked += () => Debug.Log("Leaderboard button clicked!");

        //leaderboard.focusable = true;
    }

    // Level Buttons 1,2...12
    private void HookUpLevelButtons()
    {
        for (int i = 1; i <= 12; i++)
        {
            if (i != 1 && PlayerDataManager.Instance.GetSingleLevelTime(Mathf.Max(i - 1, 1)) <= 0)
                return;

            SetPlayerBestTime(i);
            SetLevelText(i);

            string buttonName = i.ToString();

            Button levelButton = root.Q<Button>(buttonName);
            if (levelButton != null)
            {
                int capturedNumber = i;
                levelButton.clicked += () => LoadLevel(capturedNumber);
            }
        }
        if (PlayerDataManager.Instance.GetSingleLevelTime(12) > 0)
        {
            ShowBTD();
        }
    }

    // Set level ONLY if player has unlocked the level
    private void SetLevelText(int level)
    {
        string levelString = level.ToString() + "Level";
        Label levelLabel = root.Q<Label>(levelString);
        if (levelLabel != null)
        {
            levelLabel.text = $"Level {level}";
        }
    }

    // Set players best time ONLY if level is unlocked
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

    // BTD = Beat the Dev, will show ONLY if the player has completed all levels avaliable
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
                    if (PlayerDataManager.Instance.GetSingleLevelTime(i) < timeTobeat)
                        boolLabel.text = "✓";
                    else
                        boolLabel.text = "✗";
                }
            }
        }
    }

    // Loads level pressed
    private void LoadLevel(int levelNumber)
    {
        Debug.Log($"Loading Level {levelNumber}");
        GameManager.Instance.RequestLoadLevel(levelNumber);
    }

    // Code for controller navigation
    private void BuildNavigationList()
    {
        focusables.Clear();

        // Add Back button
        var back = root.Q<Button>(BackButtonName);
        if (back != null)
        {
            back.focusable = true;
            focusables.Add(back);
        }

        // Add Leaderboard button
        var leaderboard = root.Q<Button>(LeaderboardButtonName);
        if (leaderboard != null)
        {
            leaderboard.focusable = true;
            focusables.Add(leaderboard);
        }

        // Add level buttons 1–12
        for (int i = 1; i <= 12; i++)
        {
            var button = root.Q<Button>(i.ToString());
            if (button != null)
            {
                button.focusable = true;
                focusables.Add(button);
            }
        }
        BuildNavRules();
    }

    private void FocusInitial()
    {
        if (focusables.Count > 0)
        {
            focusables[0].Focus();
            currentIndex = 0;
        }
    }
    private struct NavRule
    {
        public int left, right, up, down;

        public NavRule(int left, int right, int up, int down)
        {
            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
        }
    }
    private Dictionary<int, NavRule> nav = new Dictionary<int, NavRule>();
    private void BuildNavRules()
    {
        nav.Clear();

        // index : left, right, up, down
        nav[0] = new NavRule(-1, 1, -1, 2);
        nav[1] = new NavRule(0, 2, -1, 4);
        nav[2] = new NavRule(0, 3, 0, 6);
        nav[3] = new NavRule(2, 4, 0, 7);
        nav[4] = new NavRule(3, 5, 1, 8);

        // All remaining buttons follow a simple grid: 3 columns
        for (int i = 5; i < focusables.Count; i++)
        {
            nav[i] = new NavRule(
                i - 1,
                i + 1,
                i - 4,
                i + 4
            );
        }
    }
    private void OnNullSwipe()
    {
        Debug.Log("Null swipe (no valid button) - play fail sound later.");
    }
    private void TryMove(int targetIndex)
    {
        if (!nav.ContainsKey(currentIndex))
        {
            OnNullSwipe();
            return;
        }

        if (targetIndex < 0 || targetIndex >= focusables.Count)
        {
            OnNullSwipe();
            return;
        }

        MoveFocus(targetIndex - currentIndex);
    }

    private void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        var r = nav[currentIndex];

        if (gamepad.dpad.left.wasPressedThisFrame || gamepad.leftStick.left.wasPressedThisFrame)
            TryMove(r.left);

        if (gamepad.dpad.right.wasPressedThisFrame || gamepad.leftStick.right.wasPressedThisFrame)
            TryMove(r.right);

        if (gamepad.dpad.up.wasPressedThisFrame || gamepad.leftStick.up.wasPressedThisFrame)
            TryMove(r.up);

        if (gamepad.dpad.down.wasPressedThisFrame || gamepad.leftStick.down.wasPressedThisFrame)
            TryMove(r.down);

        if (gamepad.buttonSouth.wasPressedThisFrame)
            ActivateCurrent();
    }


    private void MoveFocus(int delta)
    {
        int newIndex = currentIndex + delta;
        if (newIndex < 0 || newIndex >= focusables.Count)
            return;

        // Remove focus class from old
        if (focusables[currentIndex] is VisualElement oldVe)
            oldVe.RemoveFromClassList("LevelButtonsFocus");

        currentIndex = newIndex;

        // Apply focus
        focusables[currentIndex].Focus();

        // Add class to new
        if (focusables[currentIndex] is VisualElement newVe)
            newVe.AddToClassList("LevelButtonsFocus");
    }

    private void ActivateCurrent()
    {
        if (focusables[currentIndex] is Button button)
        {
            button.SendEvent(new ClickEvent());
        }
    }
}
