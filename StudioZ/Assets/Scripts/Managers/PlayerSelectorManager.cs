using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] GameObject SinglePlayerUIGameobject;
    [SerializeField] GameObject MainMenuUIGameObject;

    private VisualElement root;

    private Button backButton;
    private Button singleButton;
    private Button multiButton;

    private bool usingController = false;

    private int currentFocusing = 1;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Re-query all buttons to avoid stale references
        backButton = root.Q<Button>("0");
        singleButton = root.Q<Button>("1");
        multiButton = root.Q<Button>("2");

        // -------- FIX: remove old callbacks --------
        backButton.clicked -= OnBackButtonPress;
        singleButton.clicked -= OnSinglePlayerButtonPress;
        multiButton.clicked -= OnMultiPlayerButtonPress;

        // -------- FIX: add fresh callbacks --------
        backButton.clicked += OnBackButtonPress;
        singleButton.clicked += OnSinglePlayerButtonPress;
        multiButton.clicked += OnMultiPlayerButtonPress;

        // Clear highlight classes (important when re-opening)
        backButton.RemoveFromClassList("LevelButtonsFocus");
        singleButton.RemoveFromClassList("SingleMultiFocus");
        multiButton.RemoveFromClassList("SingleMultiFocus");

        // Reset focus state
        usingController = Gamepad.current != null;
        currentFocusing = 1;

        singleButton.Focus();
        singleButton.AddToClassList("SingleMultiFocus");
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        var pad = Gamepad.current;
        if (pad == null) return;

        // --- NEW directional "pressed this frame" logic ---
        bool left = pad.dpad.left.wasPressedThisFrame || pad.leftStick.left.wasPressedThisFrame;
        bool right = pad.dpad.right.wasPressedThisFrame || pad.leftStick.right.wasPressedThisFrame;
        bool up = pad.dpad.up.wasPressedThisFrame || pad.leftStick.up.wasPressedThisFrame;
        bool down = pad.dpad.down.wasPressedThisFrame || pad.leftStick.down.wasPressedThisFrame;

        // Detect ANY directional input once
        bool anyMoveThisFrame = left || right || up || down;

        if (anyMoveThisFrame)
        {
            if (!usingController)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                usingController = true;
                currentFocusing = 1;

                singleButton.AddToClassList("SingleMultiFocus");
                singleButton.Focus();
            }

            // LEFT
            if (left)
            {
                if (currentFocusing == 2)
                {
                    currentFocusing = 1;
                    multiButton.RemoveFromClassList("SingleMultiFocus");
                    singleButton.AddToClassList("SingleMultiFocus");
                    singleButton.Focus();
                }
            }

            // RIGHT
            if (right)
            {
                if (currentFocusing == 1)
                {
                    currentFocusing = 2;
                    singleButton.RemoveFromClassList("SingleMultiFocus");
                    multiButton.AddToClassList("SingleMultiFocus");
                    multiButton.Focus();
                }
            }

            // UP
            if (up && currentFocusing != 0)
            {
                if (currentFocusing == 1) singleButton.RemoveFromClassList("SingleMultiFocus");
                else if (currentFocusing == 2) multiButton.RemoveFromClassList("SingleMultiFocus");

                currentFocusing = 0;
                backButton.AddToClassList("LevelButtonsFocus");
                backButton.Focus();
            }

            // DOWN
            if (down && currentFocusing == 0)
            {
                currentFocusing = 1;
                backButton.RemoveFromClassList("LevelButtonsFocus");
                singleButton.AddToClassList("SingleMultiFocus");
                singleButton.Focus();
            }
        }
        else if (!usingController)
        {
            // original logic unchanged
            backButton.RemoveFromClassList("LevelButtonsFocus");
            singleButton.RemoveFromClassList("SingleMultiFocus");
            multiButton.RemoveFromClassList("SingleMultiFocus");
            root.Focus();
        }

        // Submit
        if (usingController && pad.buttonSouth.wasPressedThisFrame)
        {
            ActivateFocusedButton();
        }

        // Mouse movement switches out of controller mode
        if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            usingController = false;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            usingController = false;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }


    private void ActivateFocusedButton()
    {
        var focused = root.focusController.focusedElement as Button;
        focused?.SendEvent(new ClickEvent());
    }

    private void OnBackButtonPress()
    {
        MainMenuUIGameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void OnSinglePlayerButtonPress()
    {
        GameMode.IsMultiplayer = false;
        SinglePlayerUIGameobject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void OnMultiPlayerButtonPress()
    {
        GameMode.IsMultiplayer = true;
        Debug.Log("Multiplayer pressed");
    }
}
