using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProvider : MonoBehaviour, IPlayerInput
{
    public Vector2 MoveL { get; private set; }
    public Vector2 MoveR { get; private set; }

    public float GripLValue { get; private set; }
    public float GripRValue { get; private set; }

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
        inputActions.FindActionMap("PlayerControls").Enable();

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

    //public bool JugInput()
    //{
        
    //    return 
    //}
}
