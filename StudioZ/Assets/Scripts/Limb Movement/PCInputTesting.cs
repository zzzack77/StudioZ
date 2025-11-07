using UnityEngine;
using UnityEngine.InputSystem;

public class PCInputTesting : MonoBehaviour
{
    // Input action asset (drag this in from controls folder)
    [SerializeField] private InputActionAsset inputActions;

    // Seting up the Input Actions for moving and gripping
    private InputAction moveLAction;
    private InputAction moveRAction;

    private InputAction gripLAction;
    private InputAction gripRAction;

    // Variable for our input to be stored in
    private Vector2 moveL;
    private Vector2 moveR;

    private float gripLValue;
    private float gripRValue;

    [SerializeField] private Rigidbody R_Anchor;
    [SerializeField] private Rigidbody L_Anchor;
    // LS Left Shoulder, RS Right Shoulder
    [SerializeField] private Transform LS;
    [SerializeField] private Transform RS;
    private Transform currentAnchor;

    [SerializeField] private HingeJoint2D hinge;


    

    // Bool to check if the mouse is dragging the limb
    private bool isDragging = false;

    private float armDistance;
    // The arm's and legs max reach
    //[SerializeField] private float maxDistance;

    // The amount of force applied to rigidbody when moving limbs
    [SerializeField] private float variableForce;

    // For furture use change trigger deadzone so slight pressure doesn't activate grip
    // Higher deadzone means more pressure is needed
    private float triggerDeadZone = 0.01f;

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
    void Update()
    {
        ReadControls();
    }
    
    void ReadControls()
    {
        // Reading the value from input (either PC or controller)
        moveL = moveLAction.ReadValue<Vector2>();
        moveR = moveRAction.ReadValue<Vector2>();

        // The mouse buttons work as floats (not pressed - 0, pressed - 1)
        gripLValue = gripLAction.ReadValue<float>();
        gripRValue = gripRAction.ReadValue<float>();


        if (moveL != Vector2.zero)
        {
            R_Anchor.AddForce(moveL * variableForce);
        }
        if (moveR != Vector2.zero)
        {
            L_Anchor.AddForce(moveR * variableForce);
        }
        if (gripRValue > triggerDeadZone) { R_Anchor.isKinematic = true; }
        if (gripRValue <= triggerDeadZone) { R_Anchor.isKinematic = false; }
        if (gripLValue > triggerDeadZone) { L_Anchor.isKinematic = true; }
        if (gripLValue <= triggerDeadZone) { L_Anchor.isKinematic = false; }
    }
}

