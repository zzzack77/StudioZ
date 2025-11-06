using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickTesting : MonoBehaviour
{
    // The rigidbodies attached to the right and left hand
    [SerializeField] private Rigidbody L_AnchorRB;
    [SerializeField] private Rigidbody R_AnchorRB;

    // LS Left Shoulder, RS Right Shoulder
    [SerializeField] private Transform LS;
    [SerializeField] private Transform RS;
    private Transform currentAnchor;


    // -------- Mouse Movement Variables --------

    // Stores the raycast hit data and mouse vector3 position
    private RaycastHit hit;
    private Vector3 mousePos;

    // Bool to check if the mouse is dragging the limb
    private bool isDragging = false;

    // -------- Controller Movement Variables --------

    // Joystick values
    private Vector2 leftStick;
    private Vector2 rightStick;

    // Triggers and shoulders values
    private float leftTrigger;
    private float rightTrigger;
    private float leftShoulder;
    private float rightShoulder;

    // -------- Movement Settings --------

    // The amount of force applied to rigidbody when moving limbs
    [SerializeField] private float handAcceleration;
    [SerializeField] private float handMaxVelocity;

    // For furture use change trigger deadzone so slight pressure doesn't activate grip
    // Higher deadzone means more pressure is needed
    private float triggerDeadZone = 0.01f;

    // Bools to check if the hands are on a hold
    // Used by GripHolds.cs
    public bool isLeftHolding;
    public bool isRightHolding;
    void Update()
    {
        InitializeGamePad();
        MovingLimbsWMouse();
    }
    private void FixedUpdate()
    {
        ControllerMovement();
    }
    void MovingLimbsWMouse()
    {
        // Raycast from mouse to see if the player has clicked anything
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                isDragging = true;
                // Determine which anchor to use based on name
                currentAnchor = (hit.collider.name == "LA_Anchor") ?
                    LS : (hit.collider.name == "RA_Anchor") ?
                    RS : null;
            }
        }

        // If raycast is successfull set the hit transform position to the mouse location
        if (Input.GetMouseButton(0) && isDragging)
        {
            // Get the mouse position in world space
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector3 direction = mousePos - hit.transform.position;
            if (direction.magnitude > 0.3f)
            {
                //hit.rigidbody.forc
                hit.rigidbody.AddForce(direction.normalized * handAcceleration);
            }
            else
            {
                hit.rigidbody.AddForce(direction.normalized * handAcceleration * direction.magnitude / 2);
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            currentAnchor = null;
        }
    }
    void InitializeGamePad()
    {
        // Get the current gamepad
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.Log("No controller connected.");
            return;
        }

        // Read joystick values
        leftStick = gamepad.leftStick.ReadValue();
        rightStick = gamepad.rightStick.ReadValue();

        // Read trigger and shoulder values
        leftTrigger = gamepad.leftTrigger.ReadValue();
        rightTrigger = gamepad.rightTrigger.ReadValue();
        leftShoulder = gamepad.leftShoulder.ReadValue();

        // Debug button presses
        if (gamepad.buttonSouth.wasPressedThisFrame) Debug.Log("A / Cross pressed");
        if (gamepad.buttonEast.wasPressedThisFrame) Debug.Log("B / Circle pressed");
        if (gamepad.buttonNorth.wasPressedThisFrame) Debug.Log("Y / Triangle pressed");
        if (gamepad.buttonWest.wasPressedThisFrame) Debug.Log("X / Square pressed");

        //Debug.Log($"LeftStick: {leftStick}, RightStick: {rightStick}, LT: {leftTrigger:F2}, RT: {rightTrigger:F2}, LS: {leftShoulder:F2}");

        // Grip logic for left and right hands
        if (leftTrigger > triggerDeadZone && isLeftHolding) { L_AnchorRB.isKinematic = true; }
        if (leftTrigger <= triggerDeadZone) { L_AnchorRB.isKinematic = false; }
        if (rightTrigger > triggerDeadZone && isRightHolding) { R_AnchorRB.isKinematic = true; }
        if (rightTrigger <= triggerDeadZone) { R_AnchorRB.isKinematic = false; }
    }
    
    private void ControllerMovement()
    {
        if (leftStick != Vector2.zero)
        {
            L_AnchorRB.AddForce(leftStick * handAcceleration, ForceMode.Force);

            Debug.Log(L_AnchorRB.linearVelocity.magnitude);
            if (L_AnchorRB.linearVelocity.magnitude > handMaxVelocity)
            {
                L_AnchorRB.linearVelocity = L_AnchorRB.linearVelocity.normalized * handMaxVelocity;
            }
        }
        if (rightStick != Vector2.zero)
        {
            R_AnchorRB.AddForce(rightStick * handAcceleration);
            if (R_AnchorRB.linearVelocity.magnitude > handMaxVelocity)
            {
                R_AnchorRB.linearVelocity = R_AnchorRB.linearVelocity.normalized * handMaxVelocity;
            }
        }
    }
}

