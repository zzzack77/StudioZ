using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProvider : MonoBehaviour, IPlayerInput
{
    public float GripDeadZone { get; }
    public Vector2 StickL { get; private set; }
    public Vector2 StickR { get; private set; }

    public float TriggerLValue { get; private set; }
    public float TriggerRValue { get; private set; }

    [SerializeField] private float triggerDeadZone = 0.01f;
    public float TriggerDeadZone
    {
        get => triggerDeadZone;
        set
        {
            // The amount the player has to press the triggers before it detects input
            // Clamped at 0.01f to 0.99f to ensure this value cant be broken
            triggerDeadZone = Mathf.Clamp(value, 0.01f, 0.99f);
        }
    }

    [SerializeField] private float gripDeadZone = 0.1f;
    public float GripDeadZone
    {
        get => gripDeadZone;
        set
        {

        }
    }

    public bool VibrationEnabled
    {
        get => vibrationEnabled;
        set => vibrationEnabled = value;
    }

    

    [SerializeField] private bool vibrationEnabled = true;
    [SerializeField] private float vibrationDuration = 0.05f;
    [SerializeField] private float vibrationStrengthLowFrequency = 0.05f;
    [SerializeField] private float vibrationStrengthHighFrequency = 0.1f;


    // Input action asset (drag this in from controls folder)
    [SerializeField] private InputActionAsset inputActions;

    // Seting up the Input Actions 
    // Moving left and right arms
    private InputAction stickLAction;
    private InputAction stickRAction;

    // Gripping
    private InputAction triggerLAction;
    private InputAction triggerRAction;

    // Crimp grip
    private InputAction bumperLAction;
    private InputAction bumperRAction;

    // Action buttons
    private InputAction buttonSouthAction;
    private InputAction buttonEastAction;
    private InputAction buttonNorthAction;
    private InputAction buttonWestAction;

    // Menu Buttons
    private InputAction buttonStartAction;
    private InputAction buttonBackAction;

    // Stick Press
    private InputAction stickPressLAction;
    private InputAction stickPressRAction;

    // DPad
    private InputAction dPadUpAction;
    private InputAction dPadDownAction;
    private InputAction dPadLeftAction;
    private InputAction dPadRightAction;
    private bool vibrationEnabled1;

    private void OnEnable()
    {
        inputActions.FindActionMap("PlayerControls").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("PlayerControls").Disable();
    }

    private void Awake()
    {
        // On Awake find the action map "PlayerControls" and enable it
        inputActions.FindActionMap("PlayerControls").Enable();

        // Set the input actions from the PlayerControls action map
        // Moving Sticks
        stickLAction = InputSystem.actions.FindAction("StickL");
        stickRAction = InputSystem.actions.FindAction("StickR");

        // Gripping
        triggerLAction = InputSystem.actions.FindAction("TriggerL");
        triggerRAction = InputSystem.actions.FindAction("TriggerR");

        // Crimp Input
        bumperLAction = InputSystem.actions.FindAction("BumperL");
        bumperRAction = InputSystem.actions.FindAction("BumperR");

        // Action Buttons
        buttonSouthAction = InputSystem.actions.FindAction("ButtonSouth");
        buttonEastAction = InputSystem.actions.FindAction("ButtonEast");
        buttonNorthAction = InputSystem.actions.FindAction("ButtonNorth");
        buttonWestAction = InputSystem.actions.FindAction("ButtonWest");

        // Menu Buttons
        buttonStartAction = InputSystem.actions.FindAction("ButtonStart");
        buttonBackAction = InputSystem.actions.FindAction("ButtonBack");

        // Stick Press
        stickPressLAction = InputSystem.actions.FindAction("StickPressL");
        stickPressRAction = InputSystem.actions.FindAction("StickPressR");

        // DPad
        dPadUpAction = InputSystem.actions.FindAction("DPadUp");
        dPadDownAction = InputSystem.actions.FindAction("DPadDown");
        dPadLeftAction = InputSystem.actions.FindAction("DPadLeft");
        dPadRightAction = InputSystem.actions.FindAction("DPadRight");

}

    private void Update()
    {
        // Reading the value from input (either PC or controller)
        StickL = stickLAction.ReadValue<Vector2>();
        StickR = stickRAction.ReadValue<Vector2>();
    }

    // Functions for reading input 
    // Triggers
    public bool TriggerLPressed()
    {
        // Checks if the amount of grip input on the mouse click or trigger is above the deadzone
        return triggerLAction.ReadValue<float>() > triggerDeadZone;
    }

    public bool TriggerRPressed()
    {
        return triggerRAction.ReadValue<float>() > triggerDeadZone;
    }

    // Shoulder Buttons
    public bool BumperLPressed()
    {
        return bumperLAction.ReadValue<float>() == 1;
    }

    public bool BumperRPressed()
    {
        return bumperRAction.ReadValue<float>() == 1;
    }

    // Action Buttons
    public bool ButtonSouthPressed()
    {
        return buttonSouthAction.ReadValue<float>() == 1;
    }

    public bool ButtonEastPressed()
    {
        return buttonEastAction.ReadValue<float>() == 1;
    }

    public bool ButtonNorthPressed()
    {
        return buttonNorthAction.ReadValue<float>() == 1;
    }

    public bool ButtonWestPressed()
    {
        return buttonWestAction.ReadValue<float>() == 1;
    }

    // Menu Buttons
    public bool ButtonStartPressed()
    {
        return buttonStartAction.ReadValue<float>() == 1;
    }

    public bool ButtonBackPressed()
    {
        return buttonBackAction.ReadValue<float>() == 1;
    }

    // Stick Presses
    public bool StickLPressed()
    {
        return stickPressLAction.ReadValue<float>() == 1;
    }

    public bool StickRPressed()
    {
        return stickPressRAction.ReadValue<float>() == 1;
    }

    // DPad Presses
    public bool DPadUpPressed()
    {
        return dPadUpAction.ReadValue<float>() == 1;
    }

    public bool DPadDownPressed()
    {
        return dPadDownAction.ReadValue<float>() == 1;
    }

    public bool DPadLeftPressed()
    {
        return dPadLeftAction.ReadValue<float>() == 1;
    }

    public bool DPadRightPressed()
    {
        return dPadRightAction.ReadValue<float>() == 1;
    }

    // Vibration Functions
    public IEnumerator ActivateVibrationCoroutine()
    {
        if (Gamepad.current == null)
            yield break; // Exits the Coroutine if no controller is connected 

        if (vibrationEnabled)
        {
            Gamepad.current.SetMotorSpeeds(vibrationStrengthLowFrequency, vibrationStrengthHighFrequency);
            yield return new WaitForSeconds(vibrationDuration);
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }

    public void StopVibration()
    {
        if (Gamepad.current == null)
            return; // Exits if no controller is connected 

        if (vibrationEnabled)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }

    public void ActivateVibration()
    {
        StartCoroutine(ActivateVibrationCoroutine());
    }
}
