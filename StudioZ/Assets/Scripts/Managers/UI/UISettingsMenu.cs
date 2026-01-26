using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UISettingsMenu : MonoBehaviour
{
    private bool controllerActive = false;
    private bool mouseActive = false;
    private VisualElement root;

    private Button backButton;
    private Button deleteButton;

    private Toggle Toggle1;
    private Toggle Toggle2;
    private Toggle toggle3;

    private Slider Slider1;
    private Slider Slider2;
    private Slider Slider3;
    private int slider1Value = 50;
    private int slider2Value = 50;
    private int slider3Value = 50;

    private VisualElement[] Index = new VisualElement[8];

    [SerializeField] private int currentFocusing = 1;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        backButton = root.Q<Button>("BackButton");
        deleteButton = root.Q<Button>("DeleteSavedDataButton");

        Toggle1 = root.Q<Toggle>("ControllerVibrationToggle");
        Toggle2 = root.Q<Toggle>("InvertControllsToggle");
        toggle3 = root.Q<Toggle>("FullscreenToggle");

        Slider1 = root.Q<Slider>("VolumeSlider");
        Slider2 = root.Q<Slider>("MusicSlider");
        Slider3 = root.Q<Slider>("SoundFXSlider");

        for (int i = 1; i < 8; i++)
        {
            Debug.Log(i);
            Index[i] = root.Q<VisualElement>("Index" + i.ToString());
        }
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
                FocusButton(Index[1]);
            }
        }

        // If controller isnt active (mouse is in use) dont check for controller inputs
        if (!controllerActive)
            return;

        // Controller movement
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            if (currentFocusing == 1)
            {
                Toggle1.value = !Toggle1.value;
            }
            if (currentFocusing == 2)
            {
                Toggle2.value = !Toggle2.value;
            }
            if (currentFocusing == 3)
            {
                toggle3.value = !toggle3.value;
            }
            
        }
        if (gamepad.buttonSouth.isPressed)
        {
            if (currentFocusing == 4)
            {
                if (gamepad.dpad.left.wasPressedThisFrame || gamepad.leftStick.left.wasPressedThisFrame)
                {
                    slider1Value = Mathf.Clamp(slider1Value - 5, 0, 100);
                    Slider1.value = slider1Value;
                }
                if (gamepad.dpad.right.wasPressedThisFrame || gamepad.leftStick.right.wasPressedThisFrame)
                {
                    slider1Value = Mathf.Clamp(slider1Value + 5, 0, 100);
                    Slider1.value = slider1Value;
                }
            }
        }

        if ((gamepad.dpad.left.wasPressedThisFrame || gamepad.leftStick.left.wasPressedThisFrame) && !gamepad.buttonSouth.isPressed)
        {
            RemoveControllerFocus();
            backButton.AddToClassList("LevelButtonsFocus");
            currentFocusing = 0;
        }
        if ((gamepad.dpad.right.wasPressedThisFrame || gamepad.leftStick.right.wasPressedThisFrame) && currentFocusing == 0)
        {
            FocusButton(Index[1]);
            currentFocusing = 1;
        }
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
}
