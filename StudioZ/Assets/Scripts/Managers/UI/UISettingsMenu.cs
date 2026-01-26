using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UISettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    public IPlayerInput input;
    public GameObject playeRef;
    private bool controllerActive = false;
    private bool mouseActive = false;
    private VisualElement root;

    private Button backButton;
    private Button deleteButton;

    private Toggle Toggle1;
    private Toggle Toggle2;
    private Toggle Toggle3;

    private Slider Slider1;
    private Slider Slider2;
    private Slider Slider3;
    private float slider1Value = 50;
    private float slider2Value = 50;
    private float slider3Value = 50;

    private VisualElement[] Index = new VisualElement[8];

    [SerializeField] private int currentFocusing = 1;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        backButton = root.Q<Button>("BackButton");
        deleteButton = root.Q<Button>("DeleteSavedDataButton");

        backButton.clicked += BackButtonPress;
        deleteButton.clicked += DeleteSavedDataPress;


        Toggle1 = root.Q<Toggle>("ControllerVibrationToggle");
        Toggle2 = root.Q<Toggle>("InvertControllsToggle");
        Toggle3 = root.Q<Toggle>("FullscreenToggle");

        Toggle1.RegisterValueChangedCallback(Toggle1Changed);
        Toggle2.RegisterValueChangedCallback(Toggle2Changed);
        Toggle3.RegisterValueChangedCallback(Toggle3Changed);

        Slider1 = root.Q<Slider>("VolumeSlider");
        Slider2 = root.Q<Slider>("MusicSlider");
        Slider3 = root.Q<Slider>("SoundFXSlider");

        for (int i = 1; i < 8; i++)
        {
            Debug.Log(i);
            Index[i] = root.Q<VisualElement>("Index" + i.ToString());
        }
        input = playeRef.GetComponent<IPlayerInput>();

        
    }

    private void Update()
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
                FocusButton(Index[currentFocusing]);
            }
        }

        // If controller isnt active (mouse is in use) dont check for controller inputs
        if (!controllerActive)
            return;

        // --------- Button Presses ---------

        // Pressing south button, used for buttons and toggles
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            // Back Button
            if (currentFocusing == 0)
            {
                BackButtonPress();
            }
            // Toggles
            if (currentFocusing == 1)
            {
                Toggle1.value = !Toggle1.value;
                input.VibrationEnabled = Toggle1.value;
            }
            if (currentFocusing == 2)
            {
                Toggle2.value = !Toggle2.value;
            }
            if (currentFocusing == 3)
            {
                Toggle3.value = !Toggle3.value;
            }
            // Delete Saved Data Button
            if (currentFocusing == 7)
            {
                DeleteSavedDataPress();
            }
        }

        // holding down south button, used for sliders
        if (gamepad.buttonSouth.isPressed)
        {
            if (currentFocusing == 4)
            {
                if (input.DPadLeftPressed() || input.StickL.x <= -0.05f)
                {
                    slider1Value = Mathf.Clamp(slider1Value - 15f * Time.deltaTime, 0, 100);
                    Slider1.value = slider1Value;
                }
                if (input.DPadRightPressed() || input.StickL.x >= 0.05f)
                {
                    slider1Value = Mathf.Clamp(slider1Value + 15f * Time.deltaTime, 0, 100);
                    Slider1.value = slider1Value;
                }
            }
            if (currentFocusing == 5)
            {
                if (input.DPadLeftPressed() || input.StickL.x <= -0.05f)
                {
                    slider2Value = Mathf.Clamp(slider2Value - 15f * Time.deltaTime, 0, 100);
                    Slider2.value = slider2Value;
                }
                if (input.DPadRightPressed() || input.StickL.x >= 0.05f)
                {
                    slider2Value = Mathf.Clamp(slider2Value + 15f * Time.deltaTime, 0, 100);
                    Slider2.value = slider2Value;
                }
            }
            if (currentFocusing == 6)
            {
                if (input.DPadLeftPressed() || input.StickL.x <= -0.05f)
                {
                    slider3Value = Mathf.Clamp(slider3Value - 15f * Time.deltaTime, 0, 100);
                    Slider3.value = slider3Value;
                }
                if (input.DPadRightPressed() || input.StickL.x >= 0.05f)
                {
                    slider3Value = Mathf.Clamp(slider3Value + 15f * Time.deltaTime, 0, 100);
                    Slider3.value = slider3Value;
                }
            }   
        }

        // --------- Controller Navigation ---------

        // Left press, goes to back button
        if ((gamepad.dpad.left.wasPressedThisFrame || gamepad.leftStick.left.wasPressedThisFrame) && !gamepad.buttonSouth.isPressed)
        {
            RemoveControllerFocus();
            backButton.AddToClassList("LevelButtonsFocus");
            currentFocusing = 0;
        }
        // Right press, goes to first toggle
        if ((gamepad.dpad.right.wasPressedThisFrame || gamepad.leftStick.right.wasPressedThisFrame) && currentFocusing == 0)
        {
            FocusButton(Index[1]);
            currentFocusing = 1;
        }
        // Up and Down presses, goes through the settings options
        if (gamepad.dpad.up.wasPressedThisFrame || gamepad.leftStick.up.wasPressedThisFrame)
        {
            if (currentFocusing > 1)
            {
                currentFocusing--;
                FocusButton(Index[currentFocusing]);
            }
        }
        if (gamepad.dpad.down.wasPressedThisFrame || gamepad.leftStick.down.wasPressedThisFrame)
        {
            if (currentFocusing < 7)
            {
                currentFocusing++;
                FocusButton(Index[currentFocusing]);
                if (currentFocusing == 7)
                {
                    deleteButton.AddToClassList("LevelButtonsFocus");
                }
            }
        }
    }
    private void FocusButton(VisualElement name)
    {
        RemoveControllerFocus();

        name.AddToClassList("SettingsFocused");
    }
    private void RemoveControllerFocus()
    {
        backButton.RemoveFromClassList("LevelButtonsFocus");

        for (int i = 1; i < 8; i++)
        {
            Debug.Log(i);
            Index[i].RemoveFromClassList("SettingsFocused");
        }

        deleteButton.RemoveFromClassList("LevelButtonsFocus");
    }
    private void BackButtonPress()
    {
        mainMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
    private void Toggle1Changed(ChangeEvent<bool> evt)
    {
        input.VibrationEnabled = evt.newValue;
    }
    private void Toggle2Changed(ChangeEvent<bool> evt)
    {
        Debug.Log("Invert Controls Toggle Changed to: " + evt.newValue);
    }
    private void Toggle3Changed(ChangeEvent<bool> evt)
    {
        Debug.Log("Fullscreen Toggle Changed to: " + evt.newValue);
    }
    private void DeleteSavedDataPress()
    {
        Debug.Log("Delete Saved Data Button Pressed");
    }
}
