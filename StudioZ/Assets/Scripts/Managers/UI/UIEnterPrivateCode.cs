using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIEnterPrivateCode : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuUIGameObject;
    [SerializeField] private GameObject PrivateLobyGameObject;
    private VisualElement root;
    private bool controllerActive = false;
    private bool mouseActive = false;

    
    private Button backButton;
    private TextField textInput;
    private Button enterButton;

    [SerializeField] private int currentFocusing = 1;

    

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        backButton = root.Q<Button>("BackButton");
        textInput = root.Q<TextField>("TextInput");
        enterButton = root.Q<Button>("EnterButton");

        backButton.clicked += BackButtonPress;
        enterButton.clicked += EnterButtonPress;
    }

    // Update is called once per frame
    void Update()
    {
        return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackButtonPress();
        }
        if (Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            EnterButtonPress();
        }
        // Checks for mouse movement, if true turn of controller movement and switch to mouse
        if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f && !mouseActive)
        {
            mouseActive = true;
            controllerActive = false;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            RemoveControllerFocus();
        }

        // Gamepad controller 
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
                mouseActive = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                currentFocusing = 1;
                enterButton.AddToClassList("LevelButtonsFocus");
            }
        }

        // If controller isnt active (mouse is in use) dont check for controller inputs
        if (!controllerActive)
            return;

        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            // Activate the focused button
            if (currentFocusing == 0)
            {
                BackButtonPress();
            }
            else if (currentFocusing == 1)
            {
                EnterButtonPress();
            }
        }
        if ((gamepad.dpad.left.wasPressedThisFrame || gamepad.leftStick.left.wasPressedThisFrame) && !gamepad.buttonSouth.isPressed)
        {
            if (currentFocusing == 1)
            {
                backButton.AddToClassList("LevelButtonsFocus");
                enterButton.RemoveFromClassList("LevelButtonsFocus");
                currentFocusing = 0;
            }
        }
        if ((gamepad.dpad.right.wasPressedThisFrame || gamepad.leftStick.right.wasPressedThisFrame) && !gamepad.buttonSouth.isPressed)
        {
            if (currentFocusing == 0)
            {
                enterButton.AddToClassList("LevelButtonsFocus");
                backButton.RemoveFromClassList("LevelButtonsFocus");
                currentFocusing = 1;
            }
        }
    }
    private void RemoveControllerFocus()
    {
        backButton.RemoveFromClassList("LevelButtonsFocus");
        enterButton.RemoveFromClassList("LevelButtonsFocus");
    }
    private void BackButtonPress()
    {
        MainMenuUIGameObject.SetActive(true);
        this.gameObject.SetActive(false);
        Debug.Log("Back Button Pressed");
    }
    private void EnterButtonPress()
    {
        string inputCode = textInput.value.Trim();

        SimpleMatchmaking.Instance.JoinPrivateLobbyWithCode(inputCode);

        PrivateLobyGameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
