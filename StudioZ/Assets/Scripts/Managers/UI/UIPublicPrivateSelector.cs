using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIPublicPrivateSelector : MonoBehaviour
{
    [SerializeField] GameObject PublicLobbyUI;
    [SerializeField] GameObject MainMenuUIGameObject;

    private VisualElement root;

    private Button backButton;
    private Button publicButton;
    private Button privateButton;

    private bool usingController = false;

    private int currentFocusing = 1;
    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Assign buttons
        backButton = root.Q<Button>("0");
        publicButton = root.Q<Button>("1");
        privateButton = root.Q<Button>("2");

        // Remove old callbacks
        backButton.clicked -= OnBackButtonPress;
        publicButton.clicked -= OnPublicButtonPress;
        privateButton.clicked -= OnPrivateButtonPress;

        // Button click events
        backButton.clicked += OnBackButtonPress;
        publicButton.clicked += OnPublicButtonPress;
        privateButton.clicked += OnPrivateButtonPress;

        // Clear highlight classes (used for a bug fix where highlighting would stay after a re-enable
        backButton.RemoveFromClassList("LevelButtonsFocus");
        publicButton.RemoveFromClassList("SingleMultiFocus");
        privateButton.RemoveFromClassList("SingleMultiFocus");

        // Reset focus state
        usingController = Gamepad.current != null;
        if (usingController)
        {
            currentFocusing = 1;

            publicButton.Focus();
            publicButton.AddToClassList("SingleMultiFocus");
        }
        else
        {
            root.Focus();
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        var pad = Gamepad.current;
        if (pad == null) return;

        // Reads joystick input and dpad input
        bool left = pad.dpad.left.wasPressedThisFrame || pad.leftStick.left.wasPressedThisFrame;
        bool right = pad.dpad.right.wasPressedThisFrame || pad.leftStick.right.wasPressedThisFrame;
        bool up = pad.dpad.up.wasPressedThisFrame || pad.leftStick.up.wasPressedThisFrame;
        bool down = pad.dpad.down.wasPressedThisFrame || pad.leftStick.down.wasPressedThisFrame;

        bool anyMoveThisFrame = left || right || up || down;

        if (anyMoveThisFrame)
        {
            if (!usingController)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                usingController = true;
                currentFocusing = 1;

                publicButton.AddToClassList("SingleMultiFocus");
                publicButton.Focus();
            }

            // LEFT
            if (left)
            {
                if (currentFocusing == 2)
                {
                    currentFocusing = 1;
                    privateButton.RemoveFromClassList("SingleMultiFocus");
                    publicButton.AddToClassList("SingleMultiFocus");
                    publicButton.Focus();
                }
            }

            // RIGHT
            if (right)
            {
                if (currentFocusing == 1)
                {
                    currentFocusing = 2;
                    publicButton.RemoveFromClassList("SingleMultiFocus");
                    privateButton.AddToClassList("SingleMultiFocus");
                    privateButton.Focus();
                }
            }

            // UP
            if (up && currentFocusing != 0)
            {
                if (currentFocusing == 1) publicButton.RemoveFromClassList("SingleMultiFocus");
                else if (currentFocusing == 2) privateButton.RemoveFromClassList("SingleMultiFocus");

                currentFocusing = 0;
                backButton.AddToClassList("LevelButtonsFocus");
                backButton.Focus();
            }

            // DOWN
            if (down && currentFocusing == 0)
            {
                currentFocusing = 1;
                backButton.RemoveFromClassList("LevelButtonsFocus");
                publicButton.AddToClassList("SingleMultiFocus");
                publicButton.Focus();
            }
        }
        else if (!usingController)
        {
            backButton.RemoveFromClassList("LevelButtonsFocus");
            publicButton.RemoveFromClassList("SingleMultiFocus");
            privateButton.RemoveFromClassList("SingleMultiFocus");
            root.Focus();
        }

        // Button pressed (button used is "A"
        if (usingController && pad.buttonSouth.wasPressedThisFrame)
        {
            ActivateFocusedButton();
        }
        if (usingController && pad.buttonEast.wasPressedThisFrame)
        {
            OnBackButtonPress();
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

    private void OnPublicButtonPress()
    {
        PublicLobbyUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void OnPrivateButtonPress()
    {
        Debug.Log("Multiplayer pressed");
    }
}
