using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProvider : MonoBehaviour, IPlayerInput
{
    public Vector2 MoveL { get; private set; }
    public Vector2 MoveR { get; private set; }

    public float GripLValue { get; private set; }
    public float GripRValue { get; private set; }

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

    // Seting up the Input Actions for moving and gripping
    private InputAction moveLAction;
    private InputAction moveRAction;

    private InputAction gripLAction;
    private InputAction gripRAction;

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

    public bool GripLNotPressed()
    {
        // Checks if the amount of grip input on the mouse click or trigger is below the deadzone
        return GripLValue <= gripDeadZone;
    }

    public bool GripRNotPressed()
    {
        return GripRValue <= gripDeadZone;
    }

    public bool JugInput()
    {
        return false;
    }

    public bool PocketInput()
    {
        return false;
    }

    public bool CrimpInput()
    {
        return false;
    }
}
