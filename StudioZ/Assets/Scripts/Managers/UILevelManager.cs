using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UILevelManager : MonoBehaviour
{
    [SerializeField] private GameObject LevelUI;
    private VisualElement root;

    private List<Focusable> focusables = new List<Focusable>();
    private int currentIndex = 2;

    private const string BackButtonName = "BackButton";
    private const string LeaderboardButtonName = "LeaderboardButton";

    [SerializeField] private float[] BTDTime;

    private bool controllerActive = true;
    private Vector2 lastMousePos;
    private float mouseMoveThreshold = 0.5f; // pixels (tiny movement switches control)


    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        root.RegisterCallback<MouseMoveEvent>(OnMouseMoved);
    
    //HookUpBackButton();
    //HookUpLeaderboardButton();
        HookUpLevelButtons();

        BuildNavigationList();
        FocusInitial();
    }
    private void FocusInitial()
    {
        TryMove(2);
    }
    private void OnMouseMoved(MouseMoveEvent evt)
    {
        Vector2 pos = evt.mousePosition;

        if ((pos - lastMousePos).sqrMagnitude > mouseMoveThreshold * mouseMoveThreshold)
        {
            if (controllerActive)
            {
                controllerActive = false;
                ShowMouse();
                RemoveControllerFocus();
            }
        }

        lastMousePos = pos;
    }
    private void RemoveControllerFocus()
    {
        if (currentIndex >= 0 && currentIndex < focusables.Count)
        {
            if (focusables[currentIndex] is VisualElement ve)
                ve.RemoveFromClassList("LevelButtonsFocus");
        }

        // Stop UI Toolkit from keeping the element focused
        root.Focus(); // moves focus off all focusables safely
    }
    private void HideMouse()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
    

    private void ShowMouse()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
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
        nav[1] = new NavRule(0, 2, -1, 5);
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
        if (Input.GetMouseButtonDown(0)) ShowMouse();
        if (Input.GetMouseButtonDown(1)) ShowMouse();
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // detect controller movement
        if (gamepad.leftStick.ReadValue().sqrMagnitude > 0.1f ||
            gamepad.dpad.ReadValue() != Vector2.zero ||
            gamepad.buttonSouth.wasPressedThisFrame)
        {
            if (!controllerActive)
            {
                controllerActive = true;
                HideMouse();
            }
        }

        // controller inactive? mouse is in control → do not run navigation
        if (!controllerActive)
            return;

        // -----------------------------
        // Controller NAVIGATION LOGIC
        // -----------------------------
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
        if (!controllerActive)
            return;

        int newIndex = currentIndex + delta;
        if (newIndex < 0 || newIndex >= focusables.Count)
            return;

        if (focusables[currentIndex] is VisualElement oldVe)
            oldVe.RemoveFromClassList("LevelButtonsFocus");

        currentIndex = newIndex;

        focusables[currentIndex].Focus();

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
