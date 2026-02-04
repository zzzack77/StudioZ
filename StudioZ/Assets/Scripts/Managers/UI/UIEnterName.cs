using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIEnterName : MonoBehaviour
{
    private VisualElement root;
    private bool controllerActive = false;
    private bool mouseActive = false;

    private Label yourNameLabel;
    private Button backButton;
    private TextField textInput;
    private Button enterButton;

    [SerializeField] private int currentFocusing = 1;

    private readonly string[] bannedNames =
    {
        "nigger",
        "Nigger",
        "NIGGER",
        "n1gger",

        "Fucker",
        "fuck",
        "FUCK",
        "SHIT",
        "shit"
    };

    private readonly string[] randomNames =
    {
        "BigMan",
        "OoglyBoogly",
        "BingBong",
        "BonkerConker",
        "SUMB",
        "WaffleStomper",
        "NaughtyGeezer",
        "Bob",
        "Dave",
        "Dogg",
        "SillyGoose",
        "CoffeeGoneCold",
        "WetBlanket",
        "RatBag",
        "MrIDidntEnterAName",
    };

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        yourNameLabel = root.Q<Label>("YourName");
        if (PlayerDataManager.Instance != null)
        {
            if (PlayerDataManager.Instance.GetPlayerName() != "")
            {
                yourNameLabel.text = "Your Name: " + PlayerDataManager.Instance.GetPlayerName();
            }
            else
            {
                yourNameLabel.text = "Your Name";
            }
        }
        
        backButton = root.Q<Button>("BackButton");
        textInput = root.Q<TextField>("TextInput");
        enterButton = root.Q<Button>("EnterButton");

        backButton.clicked += BackButtonPress;
        enterButton.clicked += EnterButtonPress;
    }

    // Update is called once per frame
    void Update()
    {
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
        Debug.Log("Back Button Pressed");
    }
    private void EnterButtonPress()
    {
        string inputName = textInput.value.Trim();

        // If empty, pick a random name
        if (string.IsNullOrEmpty(inputName))
        {
            inputName = randomNames[UnityEngine.Random.Range(0, randomNames.Length)];
        }

        // Check banned names (case-insensitive)
        foreach (string banned in bannedNames)
        {
            if (inputName.Equals(banned, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Banned name entered, assigning random name");
                inputName = randomNames[UnityEngine.Random.Range(0, randomNames.Length)];
                break;
            }
        }

        PlayerDataManager.Instance.SetPlayerName(inputName);
        yourNameLabel.text = "Your Name: " + inputName;
        Debug.Log("Enter Button Pressed with name: " + inputName);
    }
}
