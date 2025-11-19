using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProvider : MonoBehaviour, IPlayerInput
{
    public Vector2 MoveL { get; private set; }
    public Vector2 MoveR { get; private set; }

    public float GripLValue { get; private set; }
    public float GripRValue { get; private set; }

    public float CrimpLValue { get; private set; }
    public float CrimpRValue { get; private set; }

    [SerializeField] private float gripDeadZone = 0.01f;
    public float GripDeadZone
    {
        get => gripDeadZone;
        set
        {
            // The amount the player has to press the triggers before it detects input
            // Clamped at 0.01f to 0.99f to ensure this value cant be broken
            gripDeadZone = Mathf.Clamp(value, 0.01f, 0.99f);
        }
    }

    // Input action asset (drag this in from controls folder)
    [SerializeField] private InputActionAsset inputActions;

    // Seting up the Input Actions 
    // Moving left and right arms
    private InputAction moveLAction;
    private InputAction moveRAction;

    // Gripping
    private InputAction gripLAction;
    private InputAction gripRAction;

    // Crimp grip
    private InputAction crimpLAction;
    private InputAction crimpRAction;

    // Action buttons
    private InputAction buttonSouthAction;
    private InputAction buttonEastAction;
    private InputAction buttonNorthAction;
    private InputAction buttonWestAction;

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
        moveLAction = InputSystem.actions.FindAction("MoveL");
        moveRAction = InputSystem.actions.FindAction("MoveR");

        gripLAction = InputSystem.actions.FindAction("GripL");
        gripRAction = InputSystem.actions.FindAction("GripR");

        crimpLAction = InputSystem.actions.FindAction("CrimpL");
        crimpRAction = InputSystem.actions.FindAction("CrimpR");

        buttonSouthAction = InputSystem.actions.FindAction("ButtonSouth");
        buttonEastAction = InputSystem.actions.FindAction("ButtonEast");
        buttonNorthAction = InputSystem.actions.FindAction("ButtonNorth");
        buttonWestAction = InputSystem.actions.FindAction("ButtonWest");

    }

    private void Update()
    {
        // Reading the value from input (either PC or controller)
        MoveL = moveLAction.ReadValue<Vector2>();
        MoveR = moveRAction.ReadValue<Vector2>();

        // The mouse buttons work as floats (not pressed - 0, pressed - 1)
        GripLValue = gripLAction.ReadValue<float>();
        GripRValue = gripRAction.ReadValue<float>();
    }

    public bool GripLPressed()
    {
        // Checks if the amount of grip input on the mouse click or trigger is above the deadzone
        return GripLValue > gripDeadZone;
    }

    public bool GripRPressed()
    {
        return GripRValue > gripDeadZone;
    }

    
    public bool CrimpLPressed()
    {
        return crimpLAction.ReadValue<float>() == 1;
    }

    public bool CrimpRPressed()
    {
        return crimpRAction.ReadValue<float>() == 1;
    }

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
}
