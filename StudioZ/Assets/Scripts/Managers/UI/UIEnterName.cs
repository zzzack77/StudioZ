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
                //FocusButton(Index[currentFocusing]);
            }
        }

        // If controller isnt active (mouse is in use) dont check for controller inputs
        if (!controllerActive)
            return;

    }
    private void RemoveControllerFocus()
    {
        //backButton.RemoveFromClassList("LevelButtonsFocus");
        //enterButton.RemoveFromClassList("LevelButtonsFocus");
    }
    private void BackButtonPress()
    {
        Debug.Log("Back Button Pressed");
    }
    private void EnterButtonPress()
    {
        PlayerDataManager.Instance.SetPlayerName(textInput.value);
        yourNameLabel.text = "Your Name: " + textInput.value.ToString();
        Debug.Log("Enter Button Pressed with name: " + textInput.value);
    }
}
